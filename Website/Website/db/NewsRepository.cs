using System.Data.SqlClient;
using googleHW.Controllers;

namespace googleHW;

public class NewsRepository : IRepository<Models.News>
{
    private List<Models.News> _news = new List<Models.News>();
    private string _connectionString;

    public NewsRepository(string connectionString)
    {
        _connectionString = connectionString;
        Update();
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.News> GetElemList()
    {
        return _news;
    }

    public Models.News GetElem(int id)
    {
        throw new NotImplementedException();
    }

    public void Create(Models.News item)
    {
        throw new NotImplementedException();
    }

    public void Update(Models.News item)
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
        string sqlExpression = "SELECT * FROM News";
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
                    string title = reader.GetString(1);
                    string content = reader.GetString(2);
                    int authorId = reader.GetInt32(3);
                    _news.Add(new Models.News(id, title, content, authorId));
                }
            }

            reader.Close();
        }
    }
}