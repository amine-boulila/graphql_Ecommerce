using GraphQLApi.Data;
using GraphQLApi.GraphQL;
using GraphQLApi.Middleware;
using GraphQLApi.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load local environment variables from .env when present.
Env.Load();

// Read MongoDB settings from environment variables first, then fall back to appsettings.
var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")
    ?? builder.Configuration["MongoDb:ConnectionString"]
    ?? "mongodb://localhost:27017";

var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE")
    ?? builder.Configuration["MongoDb:DatabaseName"]
    ?? "ecommerce";

// Read the client credentials expected by the custom GraphQL middleware.
var clientId = Environment.GetEnvironmentVariable("GRAPHQL_CLIENT_ID")
    ?? builder.Configuration["Security:ClientId"]
    ?? "graphql-client";

var clientSecret = Environment.GetEnvironmentVariable("GRAPHQL_CLIENT_SECRET")
    ?? builder.Configuration["Security:ClientSecret"]
    ?? "graphql-secret";

// Create one MongoDB context instance and share it through dependency injection.
var mongoContext = new MongoDbContext(connectionString, databaseName);

// Register the GraphQL root types and enable in-memory pub/sub for subscriptions.
builder.Services
    .AddSingleton(mongoContext)
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

var app = builder.Build();

// Seed demo data on startup so the API can be tested immediately.
await MongoDbSeedService.SeedDatabaseAsync(mongoContext);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Subscriptions use WebSockets, then requests pass through HTTPS redirection
// and the custom credential middleware before reaching the GraphQL endpoint.
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseMiddleware<ClientCredentialMiddleware>(clientId, clientSecret);

// Expose the GraphQL API at /graphql.
app.MapGraphQL("/graphql");

app.Run();
