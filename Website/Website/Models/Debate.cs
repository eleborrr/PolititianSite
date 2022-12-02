using Political.Interfaces;

namespace Political.Models;

public class Debate: IModel
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public int Likes { get; set; }
    
    public int Dislikes { get; set; }
    
    public DateTime Date { get; set; }
    
    public Debate(int id, int authorId, string title, string content, int likes, int dislikes, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        Title = title;
        Content = content;
        Likes = likes;
        Dislikes = dislikes;
        Date = date;
    }
}