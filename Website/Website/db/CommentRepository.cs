using System.Data.SqlClient;
using Political.Models;
using Political.Controllers;

namespace Political;

public class CommentRepository : IRepository<Comment>
{
    private List<Comment> _comments = new List<Comment>();
    private string connectionString;
    
    public CommentRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Update();
    }

    public IEnumerable<Comment> GetElemList() 
    {
        return _comments;
    }

    public Comment? GetElem(int id) // получение одного объекта по id
    {
        return _comments.Find(acc => acc.Id == id);
    }

    public void Insert(Comment comment) // создание объекта
    {
        var queryString = $"INSERT INTO Comments (AuthorId, NewsId, Content, Likes, Dislikes, Date) VALUES (\'{comment.AuthorId}\', \'{comment.NewsId}\'" +
                          $", N\'{comment.Content}\', \'{comment.Likes}\', \'{comment.Dislikes}\', \'{comment.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")}\')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        Update();
    }

    void Update() // обновление объекта
    {
        _comments = new List<Comment>();
        string sqlExpression = "SELECT * FROM Comments";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int authorId = reader.GetInt32(1);
                    int newsId = reader.GetInt32(2);
                    string content = reader.GetString(3);
                    int likes = reader.GetInt32(4);
                    int dislikes = reader.GetInt32(5);
                    DateTime date = reader.GetDateTime(6);
                    _comments.Add(new Comment(id, authorId, newsId, content, likes, dislikes, date));
                }
            }

            reader.Close();
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Create(Comment item)
    {
        throw new NotImplementedException();
    }

    public void Update(Comment item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        throw new NotImplementedException();
    }
}