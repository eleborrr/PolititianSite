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
    //Get /news/ - spisok news
    //Get /news/{id} - news
    
    // поправить бд с новостями

    private string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("")]
    // public List<Models.News> GetNews()
    public byte[] GetNews()
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/News.html");
        var template = Template.Parse(data);
        var news = new NewsRepository(connectionString).GetElemList().ToList();
        var htmlPage = template.Render(new { news = news });
        return  Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")] // $"#^[0-9]+$#"
    public byte[] GetNewsById(int id)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/SingleNews2.html");
        var template = Template.Parse(data);
        var news = new NewsRepository(connectionString).GetElem(id);
        var htmlPage = template.Render(new { news = news });
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST("")]
    public void SaveAccount(HttpListenerContext listener)
    {
        // using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        // var bodyParam = sr.ReadToEnd();
        // var Params = bodyParam.Split("&");
        // var name = Params[0].Split("=")[1];
        // var password = Params[1].Split("=")[1];
        //
        // var rep = new AccountRepository(connectionString);
        // rep.Insert(new Account(0, name, password));
        //
        // listener.Response.Redirect(@"https://steamcommunity.com/login/home/");
    }
}