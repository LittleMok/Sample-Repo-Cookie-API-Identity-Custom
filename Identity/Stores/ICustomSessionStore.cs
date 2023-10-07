using Microsoft.AspNetCore.Authentication;

namespace TestIdentity.Identity.Stores
{
    public interface ICustomSessionStore
    {
        IEnumerable<AuthenticationTicket> GetSessions(string username);
    }
}
