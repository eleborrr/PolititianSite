using System.Data.SqlClient;
using System.Net;
using System.Text;
using googleHW.Attributes;
using googleHW.Models;
using Scriban;


namespace googleHW.Controllers;

[HttpController("accounts")]
public class Accounts
{
    //Get /accounts/ - spisok accauntov
    //Get /accounts/{id} - account
    //POST /accounts - dobavl9t infu na server cherez body
    
    string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("list")]
    public List<Account> getAccounts()
    {
        var rep = new AccountRepository(connectionString);
        return rep.GetElemList().ToList();
    }
    
    [HttpGET(@"^[0-9]+$")]
    public byte[] GetAccountById(int id)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/SingleDebate.html");
        var template = Template.Parse(data);
        var debate = new AccountRepository(connectionString).GetElem(id);
        var htmlPage = template.Render(new { debate = debate });
        return Encoding.UTF8.GetBytes(htmlPage);
    }


    [HttpPOST("register")]
    public void SaveAccount(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var Params = bodyParam.Split("&");
        
        var name = Params[0].Split("=")[1];
        var surname = Params[1].Split("=")[1];
        var email = Params[2].Split("=")[1];

        var about = Params[3].Split("=")[1];
        var organization = Params[4].Split("=")[1];
        var password = Params[5].Split("=")[1];


        var rep = new AccountRepository(connectionString);
        
        
        var acc = rep.GetElem(email, password);
        // cookie.Value = new string[]{ "IsAuthorize = true", "Id = { rep.GetAccount(name, password).Id}" };
        if (acc is null)
        {
            rep.Insert(new Account(name, surname, password, about, organization, email));
            var guid = Guid.NewGuid();
            var account = rep.GetElem(email, password);
            var session = new Session(guid, account.Id, account.Name, DateTime.Now);
            SessionManager.CreateSession(guid, () => session);
            listener.Response.AddHeader("Set-Cookie", $"SessionId={session.Id}");
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
}