using System.Net;
using System.Text;

namespace OSM.Web.Middleware;

public class BasicAuthMiddleware : IMiddleware
{
    private readonly string _userName ;
    private readonly string _password ;
    
    public BasicAuthMiddleware(IConfiguration configuration)
    {
        _userName = configuration.GetValue<string>("auth:user") ?? string.Empty;
        _password = configuration.GetValue<string>("auth:password") ?? string.Empty;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //Only do the secondary auth if the user is already authenticated
        if (context.User.Identity is {IsAuthenticated: false})
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                // Split username and password
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                // Check if login is correct
                if (IsAuthorized(username, password))
                {                   
                    
                    await next.Invoke(context);
                    return;
                }
            }

            // Return authentication type (causes browser to show login dialog)
            context.Response.Headers["WWW-Authenticate"] = "Basic";

            // Return unauthorized
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            await next.Invoke(context);
        }
    }
    
    //If you have a db another source you want to check then you can do that here
    private bool IsAuthorized(string username, string password) =>
        _userName == username && _password == password;
}