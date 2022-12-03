using System.Data.SqlClient;
using Political.Models;

namespace Political;

public class AccountRepository
{
    private List<Account> _accounts = new List<Account>();
    private List<AccountToAccount> _subs = new List<AccountToAccount>();
    private string connectionString;
    
    public AccountRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Update();
        SubsUpdate();
    }

    public IEnumerable<Account> GetElemList() // получение всех объектов
    {
        return _accounts;
    }

    public void Replace(Account acc)
    {
        var queryString = $"UPDATE Accounts Set Name = \'{acc.Name}\', Surname = \'{acc.Surname}\', Password = \'{acc.Password}\'," +
                          $"About = \'{acc.About}\', Organization = \'{acc.Organization}\', Email = \'{acc.Email}\' WHERE Id = \'{acc.Id}\'";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        Update();
    }

    public Account? GetElem(int id) // получение одного объекта по id
    {
        return _accounts.Find(acc => acc.Id == id);
    }

    public IEnumerable<Account> GetElemList(string? name, string? surname)
    {
        var result = _accounts;
        if (name != "")
            _accounts = _accounts.Where(acc => acc.Name == name).ToList();
        if (surname != "")
            _accounts = _accounts.Where(acc => acc.Surname == surname).ToList();
        return _accounts;
    }

    public void Unsubscribe(int subscriber, int reciever)
    {
        var acc_to_acc = new AccountToAccount(subscriber, reciever);
        if (GetSubscription(subscriber, reciever) is null)
            throw new Exception("Already unsubscribed");
        var queryString = $"DELETE FROM AccountToAccount WHERE subscriber_id = \'{acc_to_acc.SubscriberId}\' and reciever_id = \'{acc_to_acc.RecieverId}\'";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        SubsUpdate();
    }

    public void Subscribe(int subscriber, int reciever)
    {
        var acc_to_acc = new AccountToAccount(subscriber, reciever);
        if (GetSubscription(subscriber, reciever) is not null)
            throw new Exception("Already subscribed");
        var queryString = $"INSERT INTO AccountToAccount (subscriber_id, reciever_id) VALUES (\'{acc_to_acc.SubscriberId}\', \'{acc_to_acc.RecieverId}\')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        SubsUpdate();
    }

    public AccountToAccount? GetSubscription(int subscriber, int reciever)
    {
        return _subs.Find(s => s.SubscriberId == subscriber && s.RecieverId == reciever);
    }

    public IEnumerable<AccountToAccount> GetSubscribers()
    {
        return _subs;
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

    void SubsUpdate()
    {
        string sqlExpression = "SELECT * FROM AccountToAccount";
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
                    int subscriber_id = reader.GetInt32(1);
                    int reciever_id = reader.GetInt32(2);
                    _subs.Add(new AccountToAccount(subscriber_id, reciever_id));
                }
            }
        }
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
}