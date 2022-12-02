using System.Net;
using Guid = System.Guid;

namespace Political;

using Microsoft.Extensions.Caching.Memory;

public static class SessionManager
{
    private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());    

    public static Session CreateSession(Guid id, Func<Session> createItem)
    {
        Session cacheEntry;
        if (!_cache.TryGetValue(id, out cacheEntry)) // Ищем ключ в кэше.
        {
            // Ключ отсутствует в кэше, поэтому получаем данные.
            cacheEntry = createItem();
            
            // Сохраняем данные в кэше. 
            _cache.Set(id, cacheEntry, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));
        }
        return cacheEntry;
    }

    public static bool IfAuthorized(HttpListenerContext listener)
    {
        var cookie = listener.Request.Cookies["SessionId"].Value;
        if (cookie is null)
            return false;
        Guid id = new Guid(cookie);
        var test = IfAuthorizedGetSession(listener);
        var test2 = GetSessionInfo(id);
        return _cache.TryGetValue(id, out _);
    }

    public static Session? IfAuthorizedGetSession(HttpListenerContext listener)
    {
        var expectedValue = listener.Request.Cookies["SessionId"].Value;
        if (expectedValue is null)
            return null;
        var key2 = new Guid(expectedValue);
        Session session = GetSessionInfo(key2);
        // _cache.TryGetValue(key, out session);
        if (session is null)
            return null;
        if (expectedValue == session.Id.ToString())
            return session;
        return null;
    }
    

    public static Session? GetSessionInfo(Guid id)
    {
        return _cache.TryGetValue(id, out Session session)? session: null;
    }
}

public class Session
{
    public Guid Id { get; set; }
    
    public int AccountId { get; set; }
    
    public string Email { get; set; }
    
    public DateTime CreateDateTime { get; set; }

    public Session(Guid id, int accountId, string email, DateTime date)
    {
        Id = id;
        AccountId = accountId;
        Email = email;
        CreateDateTime = date;
    }
}