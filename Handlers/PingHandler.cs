using System.Net.Http.Headers;
using System.Text;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SlackBot.Handlers;
class PingHandler : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slack;
    private readonly ILogger _log;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private static Guid respondedMessageIds;
    public PingHandler(ISlackApiClient slack, ILogger<PingHandler> log, HttpClient httpClient, IConfiguration configuration)
    {
        _slack = slack;
        _log = log;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        var zeroId = Guid.Empty;
        var messageKey = slackEvent.ClientMsgId;
        if (slackEvent.Text.ToLower() == "start")
        {
            await SendWelcomeMessage(slackEvent.User, slackEvent.Channel).ConfigureAwait(false);
        }
        else if (CheckIfBadWords(slackEvent.Text))
        {
            await SendWarningMessage(slackEvent.Channel, slackEvent.Ts).ConfigureAwait(false);
        }
        else
        {
            if (slackEvent.Text.Contains("ping"))
            {
                await _slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                }).ConfigureAwait(false);
            }
        }


    }

    private async Task SendWelcomeMessage(string userId, string channel)
    {
        await _slack.Chat.PostMessage(new Message
        {
            Text = $"Welcome to the channel, <@{userId}>!\n\nGet started by completing the tasks!",
            Channel = userId
        }).ConfigureAwait(false);

        await _slack.Chat.PostMessage(new Message
        {
            Text = $"Welcome to the channel, user!\n\nGet started by completing the tasks!",
            Channel = channel
        }).ConfigureAwait(false);
    }

    private async Task SendWarningMessage(string channel, string threadTs)
    {
        await _slack.Chat.PostMessage(new Message
        {
            Text = "THAT IS A BAD WORD!",
            Channel = channel,
            ThreadTs = threadTs
        }).ConfigureAwait(false);
    }

    private bool CheckIfBadWords(string message)
    {
        var badWords = new List<string> { "hmm", "no", "tim" };
        var msgLower = message.ToLower();

        return badWords.Any(word => msgLower.Contains(word));
    }
}