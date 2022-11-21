using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using googleHW.Attributes;

namespace googleHW
{
    public class HttpServer
    {
        HttpListener listener;
        private ServerSettings _serverSetting;
        private FileInspector _inspector = new FileInspector();
        string PATH = Directory.GetCurrentDirectory() + "/site";

        public HttpServer()
        {
            var path = Directory.GetParent(
                Directory.GetParent(Directory.GetParent(Directory.GetParent(PATH).FullName).FullName).FullName).FullName;
            Directory.SetCurrentDirectory(path);
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
        }

        public void StartServer()
        {
            // _serverSetting = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllBytes("./settings.json"));
            _serverSetting = new ServerSettings();
            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://localhost:{_serverSetting.Port}/");
            // PATH = _serverSetting.Path;
            listener.Start();
            Console.WriteLine("Server started");
            Work();
        }
        
        public void StopServer(string message)
        {
            listener.Stop();
            Console.WriteLine(message);
            Console.Read();
        }
        
        private object[] GetQuery(HttpListenerContext listener, MethodInfo method)
        {
            string[] strParams = listener.Request.Url
                .Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();
            if (listener.Request.HttpMethod == "GET")
            {
                
                return method.GetParameters()
                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                    .ToArray();
            }
            return new object[] {listener};
        }


        private void Work()
        {
            while (listener.IsListening)
            {
                Listen();
                var command = Console.ReadLine();
                switch (command)
                {
                    case "stop":
                        StopServer("Server stopped");
                        break;
                    case "exit":
                        Console.WriteLine("Exit? y/n");
                        var inpt = Console.ReadLine();
                        if (inpt == "y")
                        {
                            StopServer("see ya");
                        }

                        break;
                    case "start":
                        StartServer();
                        break;
                    case "throw":
                        StopServer("Test exception");
                        break;
                    case "ping":
                    {
                        if (listener.IsListening)
                            Console.WriteLine("connected");
                        break;
                    }
                    default:
                        Console.WriteLine("unknown operation");
                        break;
                }
            }
        }


        private async Task Listen() {
            
            while (listener.IsListening)
            {
                byte[]? buffer = new byte[] { };
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                buffer = GetResponeResult(_inspector.getFile(request.RawUrl, PATH), context);

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                response.Close();
            }
        }

        private byte[] GetResponeResult(byte[] buffer, HttpListenerContext context)
        {
            if (Directory.Exists(PATH))
            {
                if (buffer == null)
                {
                    buffer = MethodHandler(context, context.Response);
                    if (buffer == null)
                        buffer = ReturnError404(context.Response);
                }
                else
                {
                    var _type = _inspector.getContentType(context.Request.RawUrl);
                    context.Response.Headers.Set("Content-Type", _type);
                }
            }
            else
            {
                    
                string err = $"Directory '{PATH}' not found";
                buffer = Encoding.UTF8.GetBytes(err);
            }

            return buffer;
        }

        private byte[] ReturnError404(HttpListenerResponse response)
        {
            response.Headers.Set("Content-Type", "text/plain");
            response.StatusCode = (int)HttpStatusCode.NotFound;
            string err = "404 - not found";
            return Encoding.UTF8.GetBytes(err);   
        }

        private byte[]? MethodHandler(HttpListenerContext _httpContext, HttpListenerResponse response)
        {
            
            if (_httpContext.Request.Url.Segments.Length < 2) return null;

            if (_httpContext.Request.Url.ToString().Contains("news"))
            {
                var a = 2;
            }

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");
            

            var assembly = Assembly.GetExecutingAssembly();

            // походу здесь проверку на наличие названия Controller в атрибуте
            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return null;
            

            var methods = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"));
            
            var method = methods.FirstOrDefault();


            if (method == null) return null;
            
            var queryParams = GetQuery(_httpContext, method);
            
            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);
            response.ContentType = "text/html";
            byte[] buffer = (byte[])ret; // норм или нет..?
            // byte[] buffer = Encoding.UTF8.GetBytes(ret);
            // byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            return buffer;
        }
    }
}
