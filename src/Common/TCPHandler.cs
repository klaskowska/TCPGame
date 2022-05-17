using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Common;
public class TCPHandler
{
    private const int BUFFER_SIZE = 1024;
    private NetworkStream _stream;

    public TCPHandler(NetworkStream stream)
    {
        _stream = stream;
    } 


    public void SendStructure<T>(T structure)
    {
        string structureString = JsonSerializer.Serialize(structure);
        byte[] structureBytes = Encoding.Default.GetBytes(structureString);

        _stream.Write(structureBytes, 0, structureBytes.Length);
    } 
    public T? ReceiveStructure<T>()
    {
        var structureBytes = new Byte[BUFFER_SIZE];
        var structureString = String.Empty;
        var byteCounter = _stream.Read(structureBytes, 0, structureBytes.Length);
        structureString = System.Text.Encoding.ASCII.GetString(structureBytes, 0, byteCounter);
        
        return JsonSerializer.Deserialize<T>(structureString);
    }

}