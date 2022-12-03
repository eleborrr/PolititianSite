namespace Political.Models;

public class Message
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }

    public int DebateId { get; set; }
    
    public string Content { get; set; }
    
    public int Likes { get; set; }
    
    public int Dislikes { get; set; }
    
    public DateTime Date { get; set; }

    public Message(int id, int authorId, int debateId, string content, int likes, int dislikes, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        DebateId = debateId;
        Content = content;
        Likes = likes;
        Dislikes = dislikes;
        Date = date;
    }
    
    public Message(int authorId, int debateId, string content, int likes, int dislikes, DateTime date)
    {
        AuthorId = authorId;
        DebateId = debateId;
        Content = content;
        Likes = likes;
        Dislikes = dislikes;
        Date = date;
    }
}