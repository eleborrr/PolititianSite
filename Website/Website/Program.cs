using Political;


using (var server = new HttpServer())
{
    server.StartServer();
    var console = new ConsoleHandler();
    while (true)
    {
        console.Handle(server);
    }
}


