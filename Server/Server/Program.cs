using Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "frontend"});
var app = builder.Build();

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

app.Map("/about", () => "About Page");
app.Map("/contact", () => "Contacts Page");

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
        //await response.WriteAsync(JsonConvert.SerializeObject(new InitialData(userLoginData.login, availableETFs)));
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
    var response = context.Response;
    if (context.Request.Cookies.ContainsKey("tocken") && context.Request.Cookies.ContainsKey("login"))
    {
        Authorisation authorisation = Authorisation.GetInstace();
        var tocken = context.Request.Cookies["tocken"];
        var login = context.Request.Cookies["login"];

        if (tocken == null || login == null)
        {
            response.StatusCode = 404;
            await response.WriteAsync("tocken or login was null");

            return;
        }

        if (!authorisation.IsThisTockenForThatLoginValid(login, tocken))
        {
            response.StatusCode = 404;
            await response.WriteAsync("tocken did not pass the authorization");

            return;
        }

        if (context.Request.HasJsonContentType())
        {
            var stream = context.Request.Body;
            StreamReader streamReader = new StreamReader(stream);
            var str = streamReader.ReadToEndAsync().Result;
            List<Security>? portfolio = JsonConvert.DeserializeObject<List<Security>>(str);

            if (portfolio == null)
            {
                response.StatusCode = 404;
                await response.WriteAsync("portfolio was null");

                return;
            }

            authorisation.SaveProtfolio(login, portfolio);

            response.StatusCode = 200;
        }

        response.StatusCode = 200;
        await response.WriteAsJsonAsync(login);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsync("unauthorized");
    }
});

app.Run();



public record UserLoginData(string login, string password);
public record UserRegData(string login, string password);





//app.Map("/getinvestmentportfolio", async (context) =>
//{
//    var response = context.Response;
//    if (context.Request.Cookies.ContainsKey("tocken") && context.Request.Cookies.ContainsKey("login"))
//    {
//        Authorisation authorisation = Authorisation.GetInstace();
//        var tocken = context.Request.Cookies["tocken"];
//        var login = context.Request.Cookies["login"];

//        if (tocken == null || login == null)
//        {
//            response.StatusCode = 404;
//            await response.WriteAsync("tocken or login was null");

//            return;
//        }

//        if (!authorisation.IsThisTockenForThatLoginValid(login, tocken))
//        {
//            response.StatusCode = 404;
//            await response.WriteAsync("tocken did not pass the authorization");

//            return;
//        }

//        response.StatusCode = 200;
//        await response.WriteAsJsonAsync(login);
//    }
//    else
//    {
//        response.StatusCode = 404;
//        await response.WriteAsync("unauthorized");
//    }
//});