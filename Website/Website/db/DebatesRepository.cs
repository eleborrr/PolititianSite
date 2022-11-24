using System.Data.SqlClient;
using googleHW.Controllers;
using googleHW.Models;

namespace googleHW;

public class DebatesRepository : IRepository<Debate>
{
    private List<Debate> _debates = new List<Debate>();
    private string _connectionString;

    public DebatesRepository(string connectionString)
    {
        _connectionString = connectionString;
        Update();
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Debate> GetElemList()
    {
        return _debates;
    }

    public Debate GetElem(int id)
    {
        return _debates.Where(debate => debate.Id == id).FirstOrDefault();
    }

    public void Create(Debate item)
    {
        throw new NotImplementedException();
    }

    public void Update(Debate item)
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
    
    void Update() // обновление объекта
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
                    int likes = reader.GetInt32(2);
                    int dislikes = reader.GetInt32(3);
                    DateTime date = reader.GetDateTime(4);
                    string content = reader.GetString(5);
                    string title = reader.GetString(6);
                    _debates.Add(new Debate(id,authorId, title, content, likes, dislikes, date));
                }
            }
            reader.Close();
        }
    }
}