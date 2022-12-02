using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Political.Attributes;

namespace Political
{
    public class HttpServer : IDisposable
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
            Listen();
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
        

        private async Task Listen() {
            
            while (listener.IsListening)
            {
                byte[]? buffer = new byte[] { };
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                try
                {
                    buffer = GetResponeResult(_inspector.getFile(request.RawUrl, PATH), context);
                    response.ContentLength64 = buffer.Length;
                    
                }
                catch (Exception ex)
                {
                    buffer = Encoding.ASCII.GetBytes(ex.Message);
                }
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

        public static byte[] ReturnError404(HttpListenerResponse response)
        {
            response.Headers.Set("Content-Type", "text/plain");
            response.StatusCode = (int)HttpStatusCode.NotFound;
            string err = "404 - not found";
            return Encoding.UTF8.GetBytes(err);   
        }

        private byte[]? MethodHandler(HttpListenerContext _httpContext, HttpListenerResponse response)
        {
            
            if (_httpContext.Request.Url.Segments.Length < 2) return null;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");
            
            var assembly = Assembly.GetExecutingAssembly();

            // походу здесь проверку на наличие названия Controller в атрибуте
            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return null;
            

            var methods = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"));

            var method = GetMethod(_httpContext, methods, controllerName);

            if (method == null) return null;
            
            //var queryParams = GetQuery(_httpContext, method);
            
            var ret = method.Invoke(Activator.CreateInstance(controller), new object[]{_httpContext});  // пока впихну чисто листенер, но вообще куериПарамс
            response.ContentType = "text/html";  // навернр не всегда должен быть text/html
            byte[] buffer = (byte[])ret; // норм или нет..?
            // byte[] buffer = Encoding.UTF8.GetBytes(ret);
            // byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            Console.WriteLine(_httpContext.Request.HttpMethod);
            return buffer;
        }

        private Type GetAttributeByRequest(HttpListenerContext context)
        {
            return context.Request.HttpMethod switch
            {
                "GET" => typeof(HttpGET),
                "POST" => typeof(HttpPOST),
                _ => throw new ArgumentException("Unknown http method" + context.Request.HttpMethod)
            };
        }

        private  MethodInfo? GetMethod(HttpListenerContext context,  IEnumerable<MethodInfo>? methods, string controllerName)
        {
            foreach (var method in methods)
            {
                var argValue = context.Request.RawUrl.Replace(controllerName, "").Split("/").LastOrDefault();
                var methodUriValue = GetUriValue(context, method);
                if ((Regex.IsMatch(argValue, methodUriValue) && methodUriValue != "") ||
                    (argValue == methodUriValue && methodUriValue == ""))
                    return method;
            }

            return null;
        }

        private string GetUriValue(HttpListenerContext context, MethodInfo method)  // берет значение uri, метод нужен для поиска нужного метода при одинаковых http но разных uri
        {
            var attr = GetAttributeByRequest(context);
            var methodAttrVal = method.GetCustomAttributes(attr).FirstOrDefault();
            var field = attr.GetField("MethodURI");
            var res = field.GetValue(methodAttrVal).ToString();
            return res;
        }

        public void Dispose()
        {
            ((IDisposable)listener).Dispose();
        }
    }
}
