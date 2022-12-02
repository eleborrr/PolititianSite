using System.Data.SqlClient;
using System.Net;
using System.Text;
using Political.Models;
using Political.Attributes;
using Scriban;

namespace Political.Controllers;

[HttpController("news")]
public class News
{
    // поправить бд с новостями

    private string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("")]
    public byte[] GetNews(HttpListenerContext listener)
    {
        var isAuthorized = SessionManager.IfAuthorizedGetSession(listener);
        var template = getTemplate("/Views/News.html");
        var news = new NewsRepository(connectionString).GetElemList().ToList();
        var htmlPage = template.Render(new { news = news , isAuthorized = isAuthorized });

        return  Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")] // $"#^[0-9]+$#"
    public byte[] GetNewsById(HttpListenerContext listener) 
    {
        int id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());
        
        bool isAuthorized = SessionManager.IfAuthorized(listener);
        var template = getTemplate("/Views/SingleNews.html");
        var news = new NewsRepository(connectionString).GetElem(id);
        var comments = new CommentRepository(connectionString).GetElemList().Where(c => c.NewsId == id).Distinct().ToList();
        var commentsAuthorsId = comments.Select(c => c.AuthorId);
        var authors = new AccountRepository(connectionString).GetElemList()
            .Where(a => commentsAuthorsId.Contains(a.Id)).ToDictionary(key => key.Id, val => val.Name);
        var htmlPage = template.Render(new { news= news, comments = comments, authors = authors, isAuthorized = isAuthorized});
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST(@"^[0-9]+$")]
    public byte[] PostComment(HttpListenerContext listener)  // + check if Authorized
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if ((session is null))
            return HttpServer.ReturnError404(listener.Response);
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);
        
        var newsId = int.Parse(listener.Request.RawUrl.Split("/").Last());

        var rep = new CommentRepository(connectionString);
        rep.Insert(new Comment(session.AccountId, newsId, bodyParam, 0, 0, DateTime.Today)); 
        
        // listener.Response.Redirect("http://localhost:7700/news/" + newsId);
        // listener.Response.Close();
        
        return GetNewsById(listener);
    }

    [HttpGET("create")]
    public byte[] GetCreationPage(HttpListenerContext listener)  // + check if Authorized
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if (!(session is null))
            return HttpServer.ReturnError404(listener.Response);
        var template = getTemplate("/Views/CreateNews.html");
        var htmlPage = template.Render();
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST("create")]
    public byte[] CreateNews(HttpListenerContext listener)  // + check if Authorized
    {
        var session = SessionManager.IfAuthorizedGetSession(listener);
        if (!(session is null))
            return HttpServer.ReturnError404(listener.Response);
        var template = getTemplate("/Views/CreateNews.html");
        var htmlPage = template.Render();
        
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var title = parsed["title"];
        var content = parsed["content"];

        var rep = new NewsRepository(connectionString);
        
        rep.Insert(new Models.News(title, content, session.AccountId)); // AuthorId через сессию
        var newsId = rep.GetElemList().Last().Id;
        listener.Response.Redirect("/news/" + newsId);
        return GetNewsById(listener);
        // return Encoding.UTF8.GetBytes(htmlPage);
    }

    private Template? getTemplate(string path)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + path);
        var template = Template.Parse(data);
        return template;
    }

    
}