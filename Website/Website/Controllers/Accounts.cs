using System.Collections.Specialized;
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
    
    
    [HttpGET("")]
    public byte[] GetAccounts(HttpListenerContext listener)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Accounts.html");
        var template = Template.Parse(data);
        var rep = new AccountRepository(connectionString);
        var accounts = rep.GetElemList();
        var is_authorized = SessionManager.IfAuthorized(listener);
        string htmlPage;
        if (is_authorized)
            htmlPage = template.Render(new
            {
                accounts = accounts, is_authorized = is_authorized,
                id = SessionManager.IfAuthorizedGetSession(listener).AccountId
            });
        else
            htmlPage = template.Render(new { accounts = accounts, is_authorized = is_authorized });
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST("")]
    public byte[] SearchAccounts(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var name = parsed["name"];
        var surname = parsed["surname"];
        
        var rep = new AccountRepository(connectionString);
        var accounts = rep.GetElemList(name, surname);
        
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Accounts.html");
        var template = Template.Parse(data);
        var htmlPage = template.Render(new { accounts =accounts} );
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")]
    public byte[] GetAccountById(HttpListenerContext listener)
    {
        var isAuthorized = SessionManager.IfAuthorized(listener);
        int id;
        try{id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());}
        catch (Exception ex) {id = int.Parse(listener.Response.Headers["User-Id"]);}
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Account.html");
        var template = Template.Parse(data);

        var rep = new AccountRepository(connectionString);
        var session = SessionManager.IfAuthorizedGetSession(listener);
        var account = rep.GetElem(id);

        bool isSubscribed = false;
        if (session is not null)
            isSubscribed = rep.GetSubscription(session.AccountId, id) is not null ? true : false; // тут баг мб
        bool ownProfile = false;
        if (session is not null)
            ownProfile = session.AccountId == id;
        var htmlPage = template.Render(new { account = account, isAuthorized = isAuthorized, isSubscribed = isSubscribed, ownProfile = ownProfile});
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST(@"^[0-9]+$")]
    public byte[] PostHandle(HttpListenerContext listener)
    {
        var isAuthorized = SessionManager.IfAuthorized(listener);
        if(!isAuthorized)
            return HttpServer.ReturnError404(listener.Response);
        int id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());

        var rep = new AccountRepository(connectionString);
        var account = rep.GetElem(id);
        var session = SessionManager.IfAuthorizedGetSession(listener);
        
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);
        
        if(parsed["unfollow"] is not null)
            rep.Unsubscribe(session.AccountId, id);
        if(parsed["follow"] is not null)
            rep.Subscribe(session.AccountId, id);
        if (parsed["edit"] is not null)
        {
            listener.Response.Redirect("/accounts/edit");
            return EditAccountGet(listener);
        }
        listener.Response.AddHeader("User-Id", session.AccountId.ToString());
        return GetAccountById(listener);
    }

    [HttpPOST("register")]
    public byte[] SaveAccount(HttpListenerContext listener)
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
        
        var remember_me = parsed["remember-me"];

        
        var rep = new AccountRepository(connectionString);
        
        var acc = rep.GetElem(email, password);
        
        if (acc is null)
        {
            rep.Insert(new Account(name, surname, password, about, organization, email));
            CreateSession(listener, rep, email, password, remember_me);
        }

        acc = rep.GetElem(email, password);
        listener.Response.AddHeader("User-Id", acc.Id.ToString());
        listener.Response.Redirect("/accounts/" + acc.Id);
        return GetAccountById(listener);

    }

    [HttpGET("register")]
    public byte[] Register(HttpListenerContext listener)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Register.html");
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
        var remember_me = parsed["remember-me"];

        var rep = new AccountRepository(connectionString);
        
        var acc = rep.GetElem(email, password); // if acc == null LOGIN ERROR
        CreateSession(listener, rep, email, password, remember_me);
        listener.Response.Redirect("/news");
        // listener.Response.RedirectLocation = "/news";
        return new News().GetNews(listener);
    }
    
    [HttpGET("edit")]
    public byte[] EditAccountGet(HttpListenerContext listener)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/EditAccount.html");
        var template = Template.Parse(data);
        var session = SessionManager.IfAuthorizedGetSession(listener);
        var acc = new AccountRepository(connectionString).GetElem(session.AccountId);
        var htmlPage = template.Render(new {account = acc, edit = true});
        return Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpPOST("edit")]
    public byte[] EditAccountPost(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var parsed = System.Web.HttpUtility.ParseQueryString(bodyParam);

        var rep = new AccountRepository(connectionString);
        
        var session = SessionManager.IfAuthorizedGetSession(listener);
        var acc = rep.GetElem(session.AccountId);
        ReplaceInfo(acc, parsed);
        rep.Replace(acc);
        listener.Response.Redirect("/accounts/" + session.AccountId);
        listener.Response.AddHeader("User-Id", session.AccountId.ToString());
        return GetAccountById(listener);
    }

    private void ReplaceInfo(Account acc, NameValueCollection parsed)
    {
        acc.Name = parsed["name"];
        acc.Surname = parsed["surname"];
        acc.Email = parsed["email"];

        acc.About = parsed["about"];
        acc.Organization = parsed["organization"];
        acc.Password = parsed["password"];
    }
    

    private void CreateSession(HttpListenerContext listener, AccountRepository rep, string email, string password, string _remember_me)  // сделать валидацию, в логине при неполных данных эксепшн
    {
        bool remember_me = _remember_me == "on" ? true : false;
        var guid = Guid.NewGuid();
        var account = rep.GetElem(email, password);
        var session = new Session(guid, account.Id, DateTime.Now); // обработка что акка нет
        SessionManager.CreateSession(guid, () => session);  // точно ли такой ключ??
        listener.Response.Cookies.Add(new Cookie("SessionId",$"{session.Id}")
        {
            Expires = remember_me?DateTime.Now.AddYears(1):DateTime.Now.AddDays(1),
            Path = "/",
        });   // перенести в session manager
    }

    // private void TryDeleteCookie(HttpListenerContext listener)
    // {
    //     listener.Request.Cookies.Remove();
    // }
}