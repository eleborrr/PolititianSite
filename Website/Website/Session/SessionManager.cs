using System.Net;
using Guid = System.Guid;

namespace Political;

using Microsoft.Extensions.Caching.Memory;

public static class SessionManager
{
    private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public static void Logout(Guid id)
    {
        _cache.Remove(id);
    }

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
        if (listener.Request.Cookies["SessionId"] is null)
            return false;
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
        if (listener.Request.Cookies["SessionId"] is null)
            return null;
        var expectedValue = listener.Request.Cookies["SessionId"].Value;  // ломается при news без сессии
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
    
    public static void CreateSession(HttpListenerContext listener, AccountRepository rep, string email, string password, string _remember_me)  // сделать валидацию, в логине при неполных данных эксепшн
    {
        bool remember_me = _remember_me == "on" ? true : false;
        var guid = Guid.NewGuid();
        var account = rep.GetElem(email, password);
        var session = new Session(guid, account.Id, DateTime.Now); // обработка что акка нет
        SessionManager.CreateSession(guid, () => session);  // точно ли такой ключ??
        listener.Response.Cookies.Add(new Cookie("SessionId",$"{session.Id}")
        {
            Expires = remember_me?DateTime.Now.AddYears(1):DateTime.Now.AddDays(1),
            Path = "/",
        });  
    }
}

public class Session
{
    public Guid Id { get; set; }
    
    public int AccountId { get; set; }
    
    public DateTime CreateDateTime { get; set; }

    public Session(Guid id, int accountId, DateTime date)
    {
        Id = id;
        AccountId = accountId;
        CreateDateTime = date;
    }
}