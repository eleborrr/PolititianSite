using googleHW.Interfaces;

namespace googleHW.Models;

public class Debate: IModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Password { get; set; }
    
    public Debate(int id, string name, string password)
    {
        Id = id;
        Name = name;
        Password = password;
    }
    
    
}