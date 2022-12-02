using System.Runtime.CompilerServices;

namespace Political.Services;

public interface IAuthenticationService
{
    public void Login();
    public void Register();
}