namespace Server
{
    public class TockenMiddleware
    {
        readonly RequestDelegate next;
        //some

        private List<string> PathToWatch = new List<string>()
        {
            "/saveinvestmentportfolio",
            "/getinvestmentportfolio",
            "/pizzamozarera",
            "/rerarerarera"
        };
        public TockenMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if(!PathToWatch.Contains(context.Request.Path))
            {
                await next.Invoke(context);
                return;
            }

            if (!context.Request.Cookies.ContainsKey("tocken") || !context.Request.Cookies.ContainsKey("login"))
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("unauthorized");
                return;
            }

            Authorisation authorisation = Authorisation.GetInstace();
            var tocken = context.Request.Cookies["tocken"];
            var login = context.Request.Cookies["login"];

            if (tocken == null || login == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("tocken or login was null");
                return;
            }

            if (!authorisation.IsThisTockenForThatLoginValid(login, tocken))
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("tocken did not pass the authorization");
                return;
            }

            await next.Invoke(context);
            return;
        }
    }
}
