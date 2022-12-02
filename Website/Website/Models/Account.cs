using Political.Interfaces;

namespace Political.Models;

public class Account: IModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }

    public string Password { get; set; }
    
    public string About { get; set; }
    
    public string Organization { get; set; }
    
    public string Email { get; set; }

    public Account(int id, string name, string surname, string password, string about, string organization, string email)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Password = password;
        About = about;
        Organization = organization;
        Email = email;
    }
    
    public Account(string name, string surname, string password, string about, string organization, string email)
    {
        Name = name;
        Surname = surname;
        Password = password;
        About = about;
        Organization = organization;
        Email = email;
    }
    
    
}