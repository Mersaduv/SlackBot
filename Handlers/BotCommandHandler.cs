using SlackNet;
using SlackNet.Interaction;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;
namespace SlackBot.Handlers;

public class BotCommandHandler : ISlashCommandHandler
{
    public const string SlashCommand = "/commandmsg";
    public List<string> Commands => new List<string> { "help", "weekly", "todo" };

    public Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        Console.WriteLine($"{command.UserName} used the {SlashCommand} slash command in the {command.ChannelName} channel");
        try
        {
            var tokens = command.Text.ToLower().Split(' ');
            var commandToken = tokens?.FirstOrDefault();
            var commandDetailsTokens = tokens?.Skip(1).ToArray();

            if (tokens == null || tokens.Any() == false || Commands.Contains(commandToken) == false)
            {
                // input string does not start with a valid command
                var response = new SlashCommandResponse
                {
                    Message = new Message
                    {
                        Text = $"No valid commands found in `{command.Text}`."
                    },
                    ResponseType = ResponseType.Ephemeral
                };
                return Task.FromResult(response);
            }

            switch (commandToken)
            {
                case "weekly":
                    return Weekly(commandDetailsTokens);
                case "todo":
                    return Todos(commandDetailsTokens);
                case "help":
                default:
                    return Help();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            var response = new SlashCommandResponse
            {
                Message = new Message
                {
                    Text = $"Encountered an error attempting to run your command."
                },
                ResponseType = ResponseType.Ephemeral
            };
            return Task.FromResult(response);
        }
    }

    private static Task<SlashCommandResponse> Weekly(string[] tokens)
    {
        var weeklyTable = $"handle weekly {string.Join(" ", tokens)}";
        var response = new SlashCommandResponse
        {
            Message = new Message
            {
                Text = weeklyTable
            },
            ResponseType = ResponseType.InChannel
        };

        return Task.FromResult(response);
    }

    private static Task<SlashCommandResponse> Todos(string[] tokens)
    {
        var todos = $"handle Todos {string.Join(" ", tokens)}";
        var response = new SlashCommandResponse
        {
            Message = new Message
            {
                Text = todos
            },
            ResponseType = ResponseType.InChannel
        };

        return Task.FromResult(response);
    }

    private static Task<SlashCommandResponse> Help()
    {
        var weeklyTable = $"handle /help";
        var response = new SlashCommandResponse
        {
            Message = new Message
            {
                Text = weeklyTable
            },
            ResponseType = ResponseType.InChannel
        };

        return Task.FromResult(response);
    }

}