using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using RaftLabs.Users.Core.Interfaces;
using RaftLabs.Users.Application;
using RaftLabs.Users.Infrastructure.Options;
using RaftLabs.Users.Infrastructure.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;

        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        config.AddEnvironmentVariables(); // Optional: support ENV vars
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ApiOptions>(context.Configuration.GetSection(nameof(ApiOptions)));

        services.AddHttpClient<ExternalUserApiClient>()
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));

        services.AddTransient<IUserService, UserService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

// Use the service
var userService = host.Services.GetRequiredService<IUserService>();
var users = await userService.GetAllUsersAsync();

foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName} - {user.Email}");
}
