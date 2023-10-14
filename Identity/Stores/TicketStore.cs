using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace TestIdentity.Identity.Stores
{
    public class TicketStore : ITicketStore, ICustomSessionStore
    {
        private const string TicketPrefix = "AuthTicket";
        public const string SessionIdClaimType = "SID";

        private readonly IDistributedCache _cache;
        private readonly ILogger<TicketStore> _logger;

        public TicketStore(IDistributedCache cache, ILogger<TicketStore> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task RemoveAsync(string key)
        {
            _ = Task.Run(() =>
            {
                var serializedTicket = _cache.Get($"{TicketPrefix}_{key}");
                if (serializedTicket != null)
                {
                    var ticket = TicketSerializer.Default.Deserialize(serializedTicket);
                    var username = ticket?.Principal?.Identity?.Name?.ToLower();
                    
                    List<string> tickets;
                    var serializedTickets = _cache.GetString($"{username}_Tickets");
                    if (serializedTickets != null)
                    {
                        tickets = JsonConvert.DeserializeObject<List<string>>(serializedTickets) ?? new();
                        tickets?.Remove(key);
                        _cache.SetString($"{username}_Tickets", JsonConvert.SerializeObject(tickets));
                    }
                }
            });
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _cache.Remove($"{TicketPrefix}_{key}");
            var serializedTicket = TicketSerializer.Default.Serialize(ticket);
            
            _cache.Set($"{TicketPrefix}_{key}", serializedTicket);

            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            AuthenticationTicket? ticket = null;

            var serializedTicket = _cache.Get(key);
            if (serializedTicket != null)
            {
                ticket = TicketSerializer.Default.Deserialize(serializedTicket);
            }
            return Task.FromResult(ticket);
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = ticket.Principal.Claims.Where(x => x.Type == SessionIdClaimType).FirstOrDefault()?.Value;
            if(key  == null)
            {
                key = $"{TicketPrefix}_{Guid.NewGuid()}";
                ticket.Principal?.AddIdentity(new ClaimsIdentity(new List<Claim>()
                {
                    new(SessionIdClaimType, key)
                }));
            }
            var username = ticket.Principal?.Identity?.Name?.ToLower();

            List<string> tickets;
            var serializedTickets = _cache.GetString($"{username}_Tickets");
            if(serializedTickets != null)
            {
                tickets = JsonConvert.DeserializeObject<List<string>>(serializedTickets) ?? new();
            } else
            {
                tickets = new();
            }

            tickets.Add(key);

            ticket.Properties.SetString(SessionIdClaimType, key);
            var serializedTicket = TicketSerializer.Default.Serialize(ticket);
            _cache.Set(key, serializedTicket);
            _cache.SetString($"{username}_Tickets", JsonConvert.SerializeObject(tickets));

            return Task.FromResult(key);
        }

        public IEnumerable<AuthenticationTicket> GetSessions(string username)
        {
            var hostName = Dns.GetHostName();
            var myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            _logger.LogInformation("Retriving sessions for {username} from PC: {machine} - {IP}", username, hostName, myIP);
            var sessions = new List<AuthenticationTicket>();

            var serializedTickets = _cache.GetString($"{username.ToLower()}_Tickets");
            List<string> tickets;
            if (serializedTickets != null)
            {
                tickets = JsonConvert.DeserializeObject<List<string>>(serializedTickets) ?? new();
            } else
            {
                tickets = new();
            }

            foreach (var ticket in tickets)
            {
                _logger.LogInformation("Retriving ticket for id {SessionId}", ticket);
                var serializedSession = _cache.Get(ticket);

                if (serializedSession != null)
                {
                    var session = TicketSerializer.Default.Deserialize(serializedSession);
                    sessions.Add(session!);
                }
            }

            return sessions;
        }

        public Task RemoveAllAsync(string username)
        {
            foreach (var session in GetSessions(username.ToLower()))
            {
                var key = session.Properties.GetString("SessionId");
                RemoveAsync($"{TicketPrefix}_{key}");
            }

            _cache.Remove($"{username.ToLower()}_Tickets");
            return Task.CompletedTask;
        }
    }
}
