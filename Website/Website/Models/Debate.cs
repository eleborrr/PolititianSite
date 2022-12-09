using Political.Interfaces;

namespace Political.Models;

public class Debate: IModel
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }

    public DateTime Date { get; set; }
    
    public Debate(int id, int authorId, string title, string content, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        Title = title;
        Content = content;
        Date = date;
    }
    
    public Debate(int authorId, string title, string content, DateTime date)
    {
        AuthorId = authorId;
        Title = title;
        Content = content;
        Date = date;
    }
}