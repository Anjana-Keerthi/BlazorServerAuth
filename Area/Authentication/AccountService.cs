namespace ServerBlazorAuth.Authentication;

public class AccountService
{
    private List<Account> _users;

    public AccountService()
    {
        _users = new List<Account>
        {
            new Account{UserName = "admin", Password = "admin", Role="Admin"},
            new Account{UserName = "user", Password = "user", Role="User"},
        };
    }

    public Account? GetUser(string name, string pwd)
    {
        return _users.FirstOrDefault(x=>x.UserName.ToLower() == name.ToLower() && x.Password.ToLower() == pwd.ToLower());   
    }
}
