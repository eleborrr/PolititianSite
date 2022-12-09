namespace Political.Models;

public class Comment
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }

    public int NewsId { get; set; }
    
    public string Content { get; set; }

    public DateTime Date { get; set; }

    public Comment(int id, int authorId, int newsId, string content, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        NewsId = newsId;
        Content = content;
        Date = date;
    }
    
    public Comment(int authorId, int newsId, string content, DateTime date)
    {
        AuthorId = authorId;
        NewsId = newsId;
        Content = content;
        Date = date;
    }
}