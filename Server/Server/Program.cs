using Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "frontend"}
    );
var app = builder.Build();

var userDataController = UserDataController.GetInstance();
var securityDataProvider = SecurityDataProvider.GetInstance();
var etfs = securityDataProvider.GetAllEtf();

var isinToTicker = securityDataProvider.GetIsinToTickerDictionary();
var isinToPrice = securityDataProvider.GetIsinToPriceDictionary();

//var dm = DiversificationModel.Create("user");

//var total1 = dm.CountriesToPart.Sum(p => p.Value);
//var total2 = dm.IndustryToPart.Sum(p => p.Value);
//var total3 = dm.SectorToPart.Sum(p => p.Value);
//var total4 = dm.IsinToPart.Sum(p => p.Value);

try
{
    userDataController.RegisterNewUser("user", "user"); 
}
catch(Exception e)
{
    Console.WriteLine("��������" + e.Message);
};

//List<string> availableETFs = new() { "FXCN", "FXDM", "FXES", "FXIM" };
List<KeyValuePair<string, string>> availableIsinTickerPairs = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("0001", "FXCN"),
    new KeyValuePair<string, string>("0002", "FXDM"),
    new KeyValuePair<string, string>("0003", "FXES"),
    new KeyValuePair<string, string>("0004", "FXIM"),
};

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<TockenMiddleware>();

app.Map("/authorization", async(context) =>
{
    var response = context.Response;
    if (context.Request.HasJsonContentType())
    {
        UserLoginData? userLoginData = await context.Request.ReadFromJsonAsync<UserLoginData>();
        Authorisation authorisation = Authorisation.GetInstace();

        if (userLoginData == null)
        {
            response.StatusCode = 404;
            await response.WriteAsync("userLoginData was null");

            return;
        }

        string tocken = authorisation.AuthorizeUserAndGetTocken(userLoginData);

        response.Cookies.Append("tocken", tocken);
        response.Cookies.Append("login", userLoginData.login);

        response.StatusCode = 200;

        var userData = authorisation.GetUserDataByLogin(userLoginData.login);
        await response.WriteAsync(
            JsonConvert.SerializeObject(
                InitialData.Create(
                    userData,
                    etfs
                    )
                )
            );
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsync("Invalid userLoginData");
    }
});

app.Map("/registration", async (context) =>
{
    var response = context.Response;
    if (context.Request.HasJsonContentType())
    {
        UserRegData? userRegData = await context.Request.ReadFromJsonAsync<UserRegData>();
        Authorisation authorisation = Authorisation.GetInstace();

        if(userRegData == null)
        {
            response.StatusCode = 404;
            await response.WriteAsync("userRegData was null");

            return;
        }

        string tocken = authorisation.RegisterNewUser(userRegData);

        response.StatusCode = 200;
        response.Cookies.Append("tocken", tocken);
        response.Cookies.Append("login", userRegData.login);

        var userData = authorisation.GetUserDataByLogin(userRegData.login);

        await response.WriteAsync(
            JsonConvert.SerializeObject(
                InitialData.Create(
                    userData,
                    etfs
                    )
                )
            );
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsync("Invalid userRegData");
    }
});

app.Map("/saveinvestmentportfolio", async (context) =>
{
    if (context.Request.HasJsonContentType())
    {
        var login = context.Request.Cookies["login"];

        var stream = context.Request.Body;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEndAsync().Result;
        List<SecurityData>? portfolio = JsonConvert.DeserializeObject<List<SecurityData>>(str);

        if (portfolio == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("portfolio was null");

            return;
        }

        userDataController.SavePortfolio(login, portfolio);

        context.Response.StatusCode = 200; 
        await context.Response.WriteAsync("ok. save it.");
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("invalid data");
    }
});

app.Map("/getinvestmentportfolio", async (context) =>
{
    string login = context.Request.Cookies["login"] ?? throw new ArgumentNullException("� ����� �� ���� ������");

    var portfolio = userDataController.GetUserPortfolioByUserLogin(login);
    var portfolioDTOs = portfolio.Select(sec =>
    {
        var etf = securityDataProvider.GetEtfByIsin(sec.Isin);
        return new PortfolioDTO(sec.Isin, etf.Ticker, etf.Price, sec.Amount);
    });

    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(portfolioDTOs.ToList()));
});

app.Map("/getisintotickerdictionary", async (context) =>
{
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(isinToTicker.ToArray()));
});

app.Map("/getavailableetfs", async (context) =>
{
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(etfs));
});

app.Map("/getisintopricedictionary", async (context) =>
{
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(isinToPrice.ToArray()));
});

app.Map("/diversificationmodel", async (context) =>
{
    var login = context.Request.Cookies["login"];

    var dm = DiversificationModel.Create(login);

    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(dm));
});

app.Map("/getfullcomposition", async (context) =>
{
    var login = context.Request.Cookies["login"];

    context.Response.StatusCode = 404;
    if (login == null)
    {
        await context.Response.WriteAsync("login was null");
        return;
    }

    var fcm = FullCompositionModel.Create(login);
    context.Response.StatusCode = 200; 
    await context.Response.WriteAsync(
     JsonConvert.SerializeObject(fcm));
});

app.Map("/sendreport", async (context) =>
{
    if (context.Request.HasJsonContentType())
    {
        var login = context.Request.Cookies["login"];

        var stream = context.Request.Body;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEndAsync().Result;
        var objects = JsonConvert.DeserializeObject<object[]>(str);
        string? isin = (string?)objects[0];
        string? reportText = (string?)objects[1];

        userDataController.SaveReport(login ?? throw new ArgumentNullException(),
            isin ?? throw new ArgumentNullException(),
            reportText ?? throw new ArgumentNullException());

        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("ok. save it.");
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("invalid data");
    }
});

app.Map("/getreports", async (context) =>
{
    var login = context.Request.Cookies["login"];

    context.Response.StatusCode = 404;
    if (login == null)
    {
        await context.Response.WriteAsync("login was null");
        return;
    }

    var reports = userDataController.GetReports();

    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(
        JsonConvert.SerializeObject(reports));
});

app.Run();


public record PortfolioDTO(string Isin, string Ticker, double Price, int Amount);
public record UserLoginData(string login, string password);
public record UserRegData(string login, string password);