using System.Net;
using System.Text;
using Political.Attributes;
using Scriban;

namespace Political.Controllers;

[HttpController("")]
public class Main
{
    [HttpGET("")]
    public byte[] MainPage(HttpListenerContext listener)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/MainPage.html");
        var template = Template.Parse(data);
        var is_authorized = SessionManager.IfAuthorized(listener);
        string htmlPage;
        htmlPage = template.Render();
        return Encoding.UTF8.GetBytes(htmlPage);
    }
}