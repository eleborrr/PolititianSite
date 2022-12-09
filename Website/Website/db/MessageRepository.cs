using System.Data.SqlClient;
using Political.Models;
using Political.Controllers;

namespace Political;

public class MessageRepository
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

    public Message? GetElem(int id) 
    {
        return _messages.Find(acc => acc.Id == id);
    }

    public void Insert(Message comment) // создание объекта
    {
        var queryString = $"INSERT INTO Messages (AuthorId, DebateId, Content, Date) VALUES (\'{comment.AuthorId}\', \'{comment.DebateId}\'" +
                          $", N\'{comment.Content}\', \'{comment.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")}\')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        Update();
    }

    void Update() 
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
                    DateTime date = reader.GetDateTime(4);
                    _messages.Add(new Message(id, authorId, newsId, content, date));
                }
            }

            reader.Close();
        }
    }
}