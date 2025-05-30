using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting TCP Server...");

TcpListener server = new TcpListener(IPAddress.Any, 7777);
server.Start();

Console.WriteLine("Waiting for clients...");
while (true)
{
    TcpClient client = await server.AcceptTcpClientAsync();
    _ = Task.Run(async () =>
    {
        using var stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
        string received = Encoding.UTF8.GetString(buffer, 0, byteCount);
        Console.WriteLine($"Received: {received}");

        string response = "Echo: " + received;
        byte[] sendBuffer = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
    });
}