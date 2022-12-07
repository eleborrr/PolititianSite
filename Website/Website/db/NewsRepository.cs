using System.Data.SqlClient;
using Political.Controllers;

namespace Political;

public class NewsRepository : IRepository<Models.News>
{
    private List<Models.News> _news = new List<Models.News>();
    private string connectionString;

    public NewsRepository(string connectionString)
    {
        this.connectionString = connectionString;
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
        return _news.Where(news => news.Id == id).FirstOrDefault();
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
    
    public void Insert(Models.News news) // создание объекта
    {
        var queryString = $"INSERT INTO News (Title, Content, AuthorID) VALUES (N\'{news.Title}\', N\'{news.Content}\',  \'{news.AuthorId}\')";
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
        string sqlExpression = "SELECT * FROM News";
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