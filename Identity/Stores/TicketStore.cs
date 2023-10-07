using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace TestIdentity.Identity.Stores
{
    public class TicketStore : ITicketStore, ICustomSessionStore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDictionary<string, List<string>> _tickets = new Dictionary<string, List<string>>();

        public TicketStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task RemoveAsync(string key)
        {
            var ticket = _memoryCache.Get<AuthenticationTicket>(key);
            if (ticket != null)
            {
                var username = ticket?.Principal?.Identity?.Name?.ToLower();
                if (_tickets.TryGetValue(username!, out var tickets))
                {
                    tickets.Remove(key);
                }
            }

            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _memoryCache.Remove(key);
            _memoryCache.Set(key, ticket);
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var ticket = _memoryCache.Get<AuthenticationTicket>(key);
            return Task.FromResult(ticket);
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = ticket.Properties.GetString("SessionId") ?? Guid.NewGuid().ToString();
            var username = ticket.Principal?.Identity?.Name?.ToLower();
            if(_tickets.TryGetValue(username!, out var tickets))
            {
                tickets.Add(key);
            } else
            {
                tickets = new();
                tickets.Add(key);
                _tickets[username!] = tickets;
            }

            ticket.Principal.AddIdentity(new ClaimsIdentity(new List<Claim>()
            {
                new("SID", key)
            }));
            ticket.Properties.SetString("SessionId", key);
            _memoryCache.Set(key, ticket);
            return Task.FromResult(key);
        }

        public IEnumerable<AuthenticationTicket> GetSessions(string username)
        {
            var sessions = new List<AuthenticationTicket>();
            if(_tickets.TryGetValue(username.ToLower(), out var tickets))
            {
                foreach(var ticket in tickets)
                {
                    var session = _memoryCache.Get<AuthenticationTicket>(ticket);
                    sessions.Add(session!);
                }
            }
            return sessions;
        }

        public Task RemoveAllAsync(string username)
        {
            foreach(var session in GetSessions(username.ToLower()))
            {
                var key = session.Properties.GetString("SessionId");
                RemoveAsync(key);
            }

            _tickets.Remove(username.ToLower());
            return Task.CompletedTask;
        }
    }
}
