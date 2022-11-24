using System.Data.SqlClient;
using System.Net;
using System.Text;
using googleHW.Attributes;
using googleHW.Models;
using Scriban;

namespace googleHW.Controllers;

[HttpController("news")]
public class News
{
    // поправить бд с новостями

    private string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("")]
    // public List<Models.News> GetNews()
    public byte[] GetNews()
    {
        var template = getTemplate("/Views/News.html");
        var news = new NewsRepository(connectionString).GetElemList().ToList();
        var htmlPage = template.Render(new { news = news });
        return  Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")] // $"#^[0-9]+$#"
    public byte[] GetNewsById(int id)
    {
        var template = getTemplate("/Views/SingleNews.html");
        var news = new NewsRepository(connectionString).GetElem(id);
        var comments = new CommentRepository(connectionString).GetElemList().Where(c => c.NewsId == id).ToList();
        var htmlPage = template.Render(new { news= news, comments = comments, author = "TODO AUTHOR NAME" });
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST(@"^[0-9]+$")]
    public void PostComment(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        
        var newsId = int.Parse(listener.Request.RawUrl.Split("/").Last());

        var rep = new CommentRepository(connectionString);
        rep.Insert(new Comment(3, newsId, bodyParam, 0, 0, DateTime.Today));  // authorId через куки, // news id как то через листенер
        
        listener.Response.Redirect("http://localhost:7700/news/" + newsId);
        listener.Response.Close();
        
        // return GetNewsById(newsId);
    }

    private Template? getTemplate(string path)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + path);
        var template = Template.Parse(data);
        return template;
    }
}