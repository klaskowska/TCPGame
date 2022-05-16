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

    public async void Run()
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

            // Send ids to the players and start the game.
            var startActions = new List<Task>();
            players.ForEach(player => startActions.Add(StartGame(server, player)));
            await Task.WhenAll(startActions);

            // Get the numbers.
            var getNumbers = new List<Task>();
            players.ForEach(player => getNumbers.Add(GetNumber(server, player)));
            await Task.WhenAll(getNumbers);
            
            // Send the result.
            Result result = new (Resolve(players[0], players[1]));
            var sendResults = new List<Task>();
            players.ForEach(player => sendResults.Add(SendResult(server, player, result)));
            await Task.WhenAll(sendResults);

        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: " + e.Message);
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine("Error in receiving a number: " + e.Message);
        }
        finally
        {
            if (server is not null)
                server.Stop();
        }
        
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