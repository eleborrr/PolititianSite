using System.Data.SqlClient;
using Political.Models;
using Political.Controllers;

namespace Political;

public class CommentRepository
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
        var queryString = $"INSERT INTO Comments (AuthorId, NewsId, Content, Date) VALUES (\'{comment.AuthorId}\', \'{comment.NewsId}\'" +
                          $", N\'{comment.Content}\', \'{comment.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")}\')";
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
                    DateTime date = reader.GetDateTime(4);
                    _comments.Add(new Comment(id, authorId, newsId, content, date));
                }
            }

            reader.Close();
        }
    }
}