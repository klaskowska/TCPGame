using System.Net.Sockets;
using Common;

namespace Client.Execution;
public class ClientRound
{
    private const string START_STR = "START";

    private const string IP_ADDRESS_STR = "127.0.0.1";
    private const Int32 PORT = 12222;

    private Player? player;

    public void Run()
    {
        TcpClient? client = null;
        NetworkStream? clientStream = null;
        
        try
        {
            // Connect to the server.
            client = new TcpClient(IP_ADDRESS_STR, PORT);
            clientStream = client.GetStream();
            var tcpHandler = new TCPHandler(clientStream);


            ReceiveStartMessage(tcpHandler);

            SendNumber(tcpHandler);

            ReceiveResult(tcpHandler);

        }
        catch (ArgumentNullException e)
        {
            Console.Error.WriteLine("ArgumentNullException: ", e.Message);
        }
        catch (NullReferenceException e)
        {
            Console.Error.WriteLine("NullReferenceException: " + e);
        }
        catch (SocketException e)
        {
            Console.Error.WriteLine("SocketException: ", e.Message);
        }
        catch (StartMessageNotReceivedException e)
        {
            Console.Error.WriteLine(e.Message);
        }
        finally
        {
            if (clientStream is not null)
                clientStream.Close();
            if (client is not null)
                client.Close();
        }
    }

    private void ReceiveStartMessage(TCPHandler tcpHandler)
    {
        var startMessage = tcpHandler.ReceiveStructure<StartMessage>();

        player = new (startMessage.PlayerId);

        if (String.Compare(START_STR, startMessage.Message) != 0)
            throw new StartMessageNotReceivedException("Haven't received START message.");
    }

    private void SendNumber(TCPHandler tcpHandler)
    {
        if (player is null)
            throw new NullReferenceException();

        player.DrawNumber();
        tcpHandler.SendStructure<GameNumber>(new(player.GameNumber));
    }

    private void ReceiveResult(TCPHandler tcpHandler)
    {
        if (player is null)
            throw new NullReferenceException();

        var result = tcpHandler.ReceiveStructure<Result>();

        if (result.PlayerId == player.Id)
            Console.WriteLine("I am the winner!");
        else
            Console.WriteLine("I lost :(");
    }

    private int a()
    {
        return 1;
    }
    
}



public class StartMessageNotReceivedException : Exception
{
    public StartMessageNotReceivedException(string message) : base(message){}
}