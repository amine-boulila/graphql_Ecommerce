namespace GraphQLApi.Middleware;

public class ClientCredentialMiddleware
{
    private const string ClientIdHeader = "X-Client-Id";
    private const string ClientSecretHeader = "X-Client-Secret";

    private readonly RequestDelegate _next;
    private readonly string _expectedClientId;
    private readonly string _expectedClientSecret;

    public ClientCredentialMiddleware(
        RequestDelegate next,
        string expectedClientId,
        string expectedClientSecret)
    {
        _next = next;
        _expectedClientId = expectedClientId;
        _expectedClientSecret = expectedClientSecret;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/graphql"))
        {
            await _next(context);
            return;
        }

        if (HttpMethods.IsGet(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var clientId = context.Request.Headers[ClientIdHeader].FirstOrDefault();
        var clientSecret = context.Request.Headers[ClientSecretHeader].FirstOrDefault();

        if (clientId == _expectedClientId && clientSecret == _expectedClientSecret)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync("""
            {
              "error": "Unauthorized",
              "message": "Invalid or missing GraphQL client credentials."
            }
            """);
    }
}

