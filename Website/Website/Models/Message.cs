namespace Political.Models;

public class Message
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }

    public int DebateId { get; set; }
    
    public string Content { get; set; }

    public DateTime Date { get; set; }

    public Message(int id, int authorId, int debateId, string content, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        DebateId = debateId;
        Content = content;
        Date = date;
    }
    
    public Message(int authorId, int debateId, string content, DateTime date)
    {
        AuthorId = authorId;
        DebateId = debateId;
        Content = content;
        Date = date;
    }
}