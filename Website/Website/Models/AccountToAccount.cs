namespace Political.Models;

public class AccountToAccount
{
    public int Id { get; set; }
    
    public int SubscriberId { get; set; }
    
    public int RecieverId { get; set; }

    public AccountToAccount(int subscriberId, int recieverId)
    {
        SubscriberId = subscriberId;
        RecieverId = recieverId;
    }
}