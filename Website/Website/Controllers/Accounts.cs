using System.Data.SqlClient;
using System.Net;
using System.Text;
using Political.Attributes;
using Political.Models;
using Scriban;


namespace Political.Controllers;

[HttpController("accounts")]
public class Accounts
{
    string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("list")]
    public List<Account> getAccounts()
    {
        var rep = new AccountRepository(connectionString);
        return rep.GetElemList().ToList();
    }
    
    [HttpGET(@"^[0-9]+$")]
    public byte[] GetAccountById(HttpListenerContext listener)
    {
        int id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Account.html");
        var template = Template.Parse(data);
        var account = new AccountRepository(connectionString).GetElem(id);
        var htmlPage = template.Render(new { account = account });
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST("register")]
    public void SaveAccount(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var name = parsed["name"];
        var surname = parsed["surname"];
        var email = parsed["email"];

        var about = parsed["about"];
        var organization = parsed["organization"];
        var password = parsed["password"];
        
        var rep = new AccountRepository(connectionString);
        
        var acc = rep.GetElem(email, password);
        
        if (acc is null)
        {
            rep.Insert(new Account(name, surname, password, about, organization, email));
            CreateSession(listener, rep, email, password);
        }
    }
    

    [HttpGET("register")]
    public byte[] Register()
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/RegisterBrandNew.html");
        var template = Template.Parse(data);
        var htmlPage = template.Render();
        
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET("login")]
    public byte[] Login(HttpListenerContext listener)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Login.html");
        var template = Template.Parse(data);
        var htmlPage = template.Render();
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST("login")]
    public byte[] LoginAccount(HttpListenerContext listener)  // + remember me
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var email = parsed["email"];
        var password = parsed["password"];

        var rep = new AccountRepository(connectionString);
        
        var acc = rep.GetElem(email, password); // if acc == null LOGIN ERROR
        CreateSession(listener, rep, email, password);
        listener.Response.Redirect("/news");
        // listener.Response.RedirectLocation = "/news";
        return new News().GetNews(listener);
    }

    private void CreateSession(HttpListenerContext listener, AccountRepository rep, string email, string password)  // сделать валидацию, в логине при неполных данных эксепшн
    {
        var guid = Guid.NewGuid();
        var account = rep.GetElem(email, password);
        var session = new Session(guid, account.Id, account.Name, DateTime.Now);
        SessionManager.CreateSession(guid, () => session);  // точно ли такой ключ??
        listener.Response.AddHeader("Set-Cookie", $"SessionId={session.Id} ; path=/");
    }

    // private void TryDeleteCookie(HttpListenerContext listener)
    // {
    //     listener.Request.Cookies.Remove();
    // }
}