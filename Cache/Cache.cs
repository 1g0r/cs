public T GetFromCache<T>(string key, Func<T> valueFactory) 
{
    var newValue = new Lazy<T>(valueFactory);
    // the line below returns existing item or adds the new value if it doesn't exist
    var value = (Lazy<T>)cache.AddOrGetExisting(key, newValue, MemoryCache.InfiniteExpiration);
    return (value ?? newValue).Value; // Lazy<T> handles the locking itself
}

//Fast insert and synchronous long get
