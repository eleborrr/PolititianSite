namespace Political.Controllers;

public class Comment
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }

    public int NewsId { get; set; }
    
    public string Content { get; set; }
    
    public int Likes { get; set; }
    
    public int Dislikes { get; set; }
    
    public DateTime Date { get; set; }

    public Comment(int id, int authorId, int newsId, string content, int likes, int dislikes, DateTime date)
    {
        Id = id;
        AuthorId = authorId;
        NewsId = newsId;
        Content = content;
        Likes = likes;
        Dislikes = dislikes;
        Date = date;
    }
    
    public Comment(int authorId, int newsId, string content, int likes, int dislikes, DateTime date)
    {
        AuthorId = authorId;
        NewsId = newsId;
        Content = content;
        Likes = likes;
        Dislikes = dislikes;
        Date = date;
    }
}