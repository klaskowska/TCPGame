using System.Net;
using System.Net.Sockets;
using Common;

namespace Server.Execution;

public class ServerRound
{
    private const string START_STR = "START";
    private const Int32 PORT = 12222;
    private const string IP_ADDRESS_STR = "127.0.0.1";

    private const int BUFFER_SIZE = 256;

    public void Run()
    {
        TcpListener? server = null;

        var serverAddress = IPAddress.Parse(IP_ADDRESS_STR);

        try
        {
            // Run server.
            server = new TcpListener(serverAddress, PORT);
            server.Start();
            

            // Get connection from two clients.
            var players = new List<ConnectedPlayer>()
            {
                new (0, server.AcceptTcpClient()),
                new (1, server.AcceptTcpClient())
            };


            // Run the game.
            RunForAllAsync(server, players, StartGame);

            RunForAllAsync(server, players, GetNumber);
            
            var result = new Result(Resolve(players[0], players[1]));
            RunForAllAsync(server, players, result, SendResult);

        }
        catch (SocketException e)
        {
            Console.Error.WriteLine("SocketException: " + e.Message);
        }
        catch (NullReferenceException e)
        {
            Console.Error.WriteLine("Error in receiving a number: " + e.Message);
        }
        finally
        {
            if (server is not null)
                server.Stop();
        }
        
    }

    private async void RunForAllAsync(TcpListener server, List<ConnectedPlayer> players, Func<TcpListener, ConnectedPlayer, Task> func)
    {
        var tasks = new List<Task>();
        players.ForEach(player => tasks.Add(func(server, player)));
        await Task.WhenAll(tasks);
    }

    private async void RunForAllAsync(TcpListener server, List<ConnectedPlayer> players, Result result, Func<TcpListener, ConnectedPlayer, Result, Task> func)
    {
        var tasks = new List<Task>();
        players.ForEach(player => tasks.Add(func(server, player, result)));
        await Task.WhenAll(tasks);
    }

    private Task StartGame(TcpListener server, ConnectedPlayer player)
    {
        player.TcpHandler.SendStructure<StartMessage>(new(player.Id, "START"));

        return Task.CompletedTask;
    }

    private Task GetNumber(TcpListener server, ConnectedPlayer player)
    {
        var gameNumber = player.TcpHandler.ReceiveStructure<GameNumber>();
        player.GameNumber = gameNumber.Number;
        return Task.CompletedTask;
    }

    private Task SendResult(TcpListener server, ConnectedPlayer player, Result result)
    {
        player.TcpHandler.SendStructure<Result>(result);
        return Task.CompletedTask;
    }

    private int Resolve(ConnectedPlayer p0, ConnectedPlayer p1)
    {
        return p0.GameNumber > p1.GameNumber ? p0.Id : p1.Id;
    }

}