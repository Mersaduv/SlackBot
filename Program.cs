using SlackBot.Handlers;
using SlackBot.Models;
using SlackNet.AspNetCore;
using SlackNet.Events;

var builder = WebApplication.CreateBuilder(args);

var slackSettings = builder.Configuration.GetSection("Slack").Get<SlackSettings>();
builder.Services.AddHttpClient();
builder.Services.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(slackSettings.ApiToken) // This gets used by the API client
        .UseAppLevelToken(slackSettings.AppLevelToken) // (Optional) used for socket mode

    // Register your Slack handlers here
    .RegisterEventHandler<MessageEvent, PingHandler>()
      .RegisterSlashCommandHandler<BotCommandHandler>(BotCommandHandler.SlashCommand)
);

var app = builder.Build();

// This sets up the SlackNet endpoints for handling requests from Slack
// By default the endpoints are /slack/event, /slack/action, /slack/options, and /slack/command,
// but the 'slack' prefix can be changed using MapToPrefix.
app.UseSlackNet(c => c
    // The signing secret ensures that SlackNet only handles requests from Slack 
    .UseSigningSecret(slackSettings!.SigningSecret)

    // You can enable socket mode for testing without having to make your web app publicly accessible
    .UseSocketMode(true)
);

app.MapGet("/", () => "Hello, Slack!");

app.Run("http://localhost:5000");

