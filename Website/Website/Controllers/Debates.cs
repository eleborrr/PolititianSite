using System.Text;
using googleHW.Attributes;
using Scriban;

namespace googleHW.Controllers;

[HttpController("debates")]
public class Debates
{
    private string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PolititianDB;Integrated Security=True;";
    
    
    [HttpGET("")]
    // public List<Models.News> GetNews()
    public byte[] GetDebates()
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/Debates.html");
        var template = Template.Parse(data);
        var debates = new DebatesRepository(connectionString).GetElemList().ToList();
        var htmlPage = template.Render(new { debates = debates });
        return  Encoding.UTF8.GetBytes(htmlPage);
    }
    
    [HttpGET(@"^[0-9]+$")] // $"#^[0-9]+$#"
    public byte[] GetDebateById(int id)
    {
        var data = File.ReadAllText(Directory.GetCurrentDirectory() + "/Views/SingleDebate.html");
        var template = Template.Parse(data);
        var debate = new DebatesRepository(connectionString).GetElem(id);
        var htmlPage = template.Render(new { debate = debate });
        return Encoding.UTF8.GetBytes(htmlPage);
    }

    // [HttpPOST(@"^[0-9]+$")]
    // public byte[] SendComment()
    // {
    //     
    // }
}