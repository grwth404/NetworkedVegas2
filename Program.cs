using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

var app = new CommandApp<NetworkedVegas2Command>();
return await app.RunAsync(args);

internal sealed class NetworkedVegas2Command : AsyncCommand<NetworkedVegas2Command.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("IP address of server to proxy to.")]
        [CommandArgument(0, "ipAddress")]
        public string? IPAddress { get; init; }

        [Description("Port to proxy. Defaults to 45000.")]
        [CommandOption("-p|--port")]
        public int Port { get; init; } = 45000;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("NetworkedVegas2 starting up...");

        var serverAddress = new IPEndPoint(IPAddress.Parse(settings.IPAddress!), settings.Port);
        using var proxiedServer = new Socket(serverAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        var interceptAddress = new IPEndPoint(IPAddress.Any, settings.Port);
        using var clientIntercept = new Socket(interceptAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        clientIntercept.Bind(interceptAddress);

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) =>
        {
            cts.Cancel();
        };

        try
        {
            var buffer = new byte[512];
            while (cts.IsCancellationRequested)
            {

                AnsiConsole.MarkupLine($"[grey]Waiting for client to search for games...[/]");
                var broadcastIntercept = await clientIntercept.ReceiveFromAsync(buffer, SocketFlags.None, interceptAddress, cts.Token);
                AnsiConsole.MarkupLine($"[green]Received client game search request![/]");
                AnsiConsole.MarkupLine($"[grey]Forwarding to remote server...[/]");
                await proxiedServer.SendToAsync(buffer.AsMemory()[..broadcastIntercept.ReceivedBytes], SocketFlags.None, serverAddress, cts.Token);
                var serverResponse = await proxiedServer.ReceiveFromAsync(buffer, SocketFlags.None, serverAddress, cts.Token);
                AnsiConsole.MarkupLine($"[green]Received server game search response![/]");
                AnsiConsole.MarkupLine($"[grey]Forwarding to client...[/]");
                await clientIntercept.SendToAsync(buffer.AsMemory()[..serverResponse.ReceivedBytes], SocketFlags.None, broadcastIntercept.RemoteEndPoint, cts.Token);
                AnsiConsole.MarkupLine($"[green]Forward to client! Enjoy your game![/]");
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            AnsiConsole.MarkupLine($"[red]An error has occurred! {ex.Message}[/]");
            AnsiConsole.MarkupLine($"[red]{ex.StackTrace}[/]");
            return -1;
        }

        return 0;
    }
}