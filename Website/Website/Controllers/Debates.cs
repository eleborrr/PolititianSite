using System.Net;
using System.Text;
using Political.Attributes;
using Political.Models;

using Scriban;

namespace Political.Controllers;

[HttpController("debates")]
public class Debates
{
    private string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("")]
    public byte[] GetDebates(HttpListenerContext listener)
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        var isAuthorized = session is not null;
        var template = FileInspector.getTemplate("/Views/Debates.html");
        var debates = new DebatesRepository(connectionString).GetElemList().ToList();
        var htmlPage = template.Render(new { debates = debates , isAuthorized = isAuthorized });

        return  Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")] // $"#^[0-9]+$#"
    public byte[] GetDebateById(HttpListenerContext listener) 
    {
        int id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());
        
        bool isAuthorized = SessionManager.IfAuthorized(listener);
        var template = FileInspector.getTemplate("/Views/SingleDebate.html");
        var debate = new DebatesRepository(connectionString).GetElem(id);
        var messages = new MessageRepository(connectionString).GetElemList().Where(c => c.DebateId == id).Distinct().ToList();
        var messagesAuthorsId = messages.Select(c => c.AuthorId);
        var authors = new AccountRepository(connectionString).GetElemList()
            .Where(a => messagesAuthorsId.Contains(a.Id)).ToDictionary(key => key.Id, val => val.Name);
        var htmlPage = template.Render(new { debate = debate, messages = messages, authors = authors, isAuthorized = isAuthorized});
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST(@"^[0-9]+$")]
    public byte[] PostMessage(HttpListenerContext listener)
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if ((session is null))
            return HttpServer.ReturnError404(listener.Response);
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);
        
        var debateId = int.Parse(listener.Request.RawUrl.Split("/").Last());

        var rep = new MessageRepository(connectionString);
        rep.Insert(new Message(session.AccountId, debateId, bodyParam, 0, 0, DateTime.Today));
        return GetDebateById(listener);
    }

    [HttpGET("create")]
    public byte[] GetCreationPage(HttpListenerContext listener) 
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if ((session is null))
            return HttpServer.ReturnError404(listener.Response);
        var template = FileInspector.getTemplate("/Views/CreateNews.html");
        var htmlPage = template.Render();
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST("create")]
    public byte[] CreateDebate(HttpListenerContext listener)
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if ((session is null))
            return HttpServer.ReturnError404(listener.Response);
        var template = FileInspector.getTemplate("/Views/CreateNews.html");
        var htmlPage = template.Render();
        
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var author_id = session.AccountId;
        var content = parsed["content"];
        var title = parsed["title"];
        var rep = new DebatesRepository(connectionString);
        
        rep.Insert(new Models.Debate(author_id, title, content, 0, 0, DateTime.Now)); // AuthorId через сессию
        var debateId = rep.GetElemList().Last().Id;
        listener.Response.Redirect("/debates/" + debateId);
        return GetDebateById(listener);
    }
}