using System.Data.SqlClient;
using Political.Controllers;
using Political.Models;

namespace Political;

public class DebatesRepository
{
    private List<Debate> _debates = new List<Debate>();
    private string _connectionString;

    public DebatesRepository(string connectionString)
    {
        _connectionString = connectionString;
        Update();
    }

    public IEnumerable<Debate> GetElemList()
    {
        return _debates;
    }

    public Debate GetElem(int id)
    {
        return _debates.Where(debate => debate.Id == id).FirstOrDefault();
    }

    public void Insert(Debate debate) 
    {
        var queryString = $"INSERT INTO Debates (AuthorId, CreationDate, Content, Title) VALUES (\'{debate.AuthorId}\', " +
                          $"\'{debate.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")}\', N\'{debate.Content}\',  N\'{debate.Title}\')";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        Update();
    }
    
    void Update() 
    {
        string sqlExpression = "SELECT * FROM Debates";
        using (SqlConnection connection = new SqlConnection(_connectionString))
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
                    
                    DateTime date = reader.GetDateTime(2);
                    string content = reader.GetString(3);
                    string title = reader.GetString(4);
                    _debates.Add(new Debate(id,authorId, title, content, date));
                }
            }
            reader.Close();
        }
    }
}