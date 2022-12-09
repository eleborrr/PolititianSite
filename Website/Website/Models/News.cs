using Political.Interfaces;

namespace Political.Models;

public class News: IModel
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public int AuthorId { get; set; }
    
    public DateTime Date { get; set; }
    
    public News(int id, string title, string content, int authorId, DateTime date)
    {
        Id = id;
        Title = title;
        Content = content;
        AuthorId = authorId;
        Date = date;
    }
    
    public News(string title, string content, int authorId, DateTime date) 
    {
        Title = title;
        Content = content;
        AuthorId = authorId;
        Date = date;
    }
    
    
}