using System.Data.SqlClient;
using Political.Models;
using Political.Controllers;

namespace Political;

public class MessageRepository : IRepository<Message>
{
    private List<Message> _messages = new List<Message>();
    private string connectionString;
    
    public MessageRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Update();
    }

    public IEnumerable<Message> GetElemList() 
    {
        return _messages;
    }

    public Message? GetElem(int id) // получение одного объекта по id
    {
        return _messages.Find(acc => acc.Id == id);
    }

    public void Insert(Message comment) // создание объекта
    {
        var queryString = $"INSERT INTO Messages (AuthorId, DebateId, Content, Likes, Dislikes, Date) VALUES (\'{comment.AuthorId}\', \'{comment.DebateId}\'" +
                          $", \'{comment.Content}\', \'{comment.Likes}\', \'{comment.Dislikes}\', \'{comment.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")}\')";
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
        _messages = new List<Message>();
        string sqlExpression = "SELECT * FROM Messages";
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
                    _messages.Add(new Message(id, authorId, newsId, content, likes, dislikes, date));
                }
            }

            reader.Close();
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Create(Message item)
    {
        throw new NotImplementedException();
    }

    public void Update(Message item)
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