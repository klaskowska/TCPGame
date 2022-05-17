using System.Net.Sockets;
using Common;

public class ConnectedPlayer
{
    public int Id { get; }

    public TCPHandler TcpHandler { get; }

    public int GameNumber { get; set; }

    public ConnectedPlayer(int id, TcpClient tcpClient)
    {
        Id = id;
        TcpHandler = new (tcpClient.GetStream());
    }

}