using System.Net.Sockets;

while (true)
{
    Console.Write("> ");

    // Connect to the server
    var client = new TcpClient("localhost", 5200);
    var stream = client.GetStream();

    // Get the current prompt from the user
    var prompt = Console.ReadLine()!;
    if (prompt.Equals("exit"))
        break;

    // Send the message to the server
    var buffer = System.Text.Encoding.UTF8.GetBytes(prompt);
    stream.Write(buffer, 0, buffer.Length);

    // Receive the response from the server
    var received = new byte[1024];
    stream.Read(received, 0, received.Length);
    Console.WriteLine(System.Text.Encoding.UTF8.GetString(received));

    // Close the connection
    client.Close();
}