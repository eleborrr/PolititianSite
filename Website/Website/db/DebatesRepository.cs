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
                          $"\'{debate.Date}\', N\'{debate.Content}\',  N\'{debate.Title}\')";
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
                    
                    DateTime date = reader.GetDateTime(4);
                    string content = reader.GetString(4);
                    string title = reader.GetString(5);
                    _debates.Add(new Debate(id,authorId, title, content, DateTime.Now));
                }
            }
            reader.Close();
        }
    }
}