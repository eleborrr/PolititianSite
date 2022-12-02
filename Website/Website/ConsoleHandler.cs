namespace Political;

public class ConsoleHandler
{
    public void Handle(HttpServer server)
    {
        var command = Console.ReadLine();
        switch (command)
        {
            case "stop":
                server.StopServer("Server stopped");
                break;
            case "exit":
                Console.WriteLine("Exit? y/n");
                var inpt = Console.ReadLine();
                if (inpt == "y")
                {
                    server.StopServer("see ya");
                }
                break;
            case "start":
                server.StartServer();
                break;
            case "throw":
                server.StopServer("Test exception");
                break;
            // case "ping":
            // {
            //     if (server.listener.IsListening)
            //         Console.WriteLine("connected");
            //     break;
            // }
            default:
                Console.WriteLine("unknown operation");
                break;
        }
    }
}