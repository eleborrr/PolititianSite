using System.Data.SqlClient;
using Political.Models;

namespace Political;

public class AccountRepository: IRepository<Account>
{
    private List<Account> _accounts = new List<Account>();
    private string connectionString;
    
    public AccountRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Update();
    }

    public IEnumerable<Account> GetElemList() // получение всех объектов
    {
        return _accounts;
    }

    public Account? GetElem(int id) // получение одного объекта по id
    {
        return _accounts.Find(acc => acc.Id == id);
    }

    public void Create(Account item)
    {
        throw new NotImplementedException();
    }

    public void Update(Account item)
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

    public Account? GetElem(string email, string password) // получение одного объекта по id
    {
        return _accounts.Find(acc => acc.Email == email && acc.Password == password);
    }

    public void Insert(Account acc) // создание объекта
    {
        var queryString = $"INSERT INTO Accounts (Name, Surname, Password, About, Organization, Email) VALUES (\'{acc.Name}\', \'{acc.Surname}\', " +
                          $"\'{acc.Password}\', \'{acc.About}\', \'{acc.Organization}\', \'{acc.Email}\')";
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
        string sqlExpression = "SELECT * FROM Accounts";
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
                    string name = reader.GetString(1);
                    string surname = reader.GetString(2);
                    string password = reader.GetString(3);
                    string about = reader.GetString(4);
                    string organization = reader.GetString(5);
                    string email = reader.GetString(6);
                    _accounts.Add(new Account(id, name, surname, password, about, organization, email));
                }
            }

            reader.Close();
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}