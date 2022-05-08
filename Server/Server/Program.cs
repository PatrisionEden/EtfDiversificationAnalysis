using Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "frontend"});
var app = builder.Build();
var userDataController = UserDataController.GetInstance();

try
{
    userDataController.RegisterNewUser("user", "user"); 
    userDataController.SavePortfolio("user", new()
    {
        new Security("0001", 1),
        new Security("0002", 2),
        new Security("0003", 3),
        new Security("0004", 4)
    });
}
catch(Exception e)
{
    Console.WriteLine("Лолецкий" + e.Message);
};

//List<string> availableETFs = new() { "FXCN", "FXDM", "FXES", "FXIM" };
List<KeyValuePair<string, string>> availableIsinTickerPairs = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("0001", "FXCN"),
    new KeyValuePair<string, string>("0002", "FXDM"),
    new KeyValuePair<string, string>("0003", "FXES"),
    new KeyValuePair<string, string>("0004", "FXIM"),
};

List<KeyValuePair<string, double>> isinToPrice = new List<KeyValuePair<string, double>>() {
    new KeyValuePair<string, double>("0001", 1000),
    new KeyValuePair<string, double>("0002", 333),
    new KeyValuePair<string, double>("0003", 111),
    new KeyValuePair<string, double>("0004", 37),
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
        await response.WriteAsync(
            JsonConvert.SerializeObject(
                new InitialData(
                    authorisation.GetUserDataByLogin(userLoginData.login),
                    isinToPrice, 
                    availableIsinTickerPairs
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
        await response.WriteAsync(
            JsonConvert.SerializeObject(
                new InitialData(
                    authorisation.GetUserDataByLogin(userRegData.login),
                    isinToPrice,
                    availableIsinTickerPairs
                    )
                )
            );
        //await response.WriteAsJsonAsync(new InitialData(userRegData.login, availableETFs));
        //await response.WriteAsJsonAsync(userRegData.login);
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
        Authorisation authorisation = Authorisation.GetInstace();
        var login = context.Request.Cookies["login"];

        var stream = context.Request.Body;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEndAsync().Result;
        List<Security>? portfolio = JsonConvert.DeserializeObject<List<Security>>(str);

        if (portfolio == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("portfolio was null");

            return;
        }

        authorisation.SaveProtfolio(login, portfolio);

        context.Response.StatusCode = 200; 
        await context.Response.WriteAsync("ok. save it.");
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("unauthorized");
    }
});

app.Run();

public record UserLoginData(string login, string password);
public record UserRegData(string login, string password);