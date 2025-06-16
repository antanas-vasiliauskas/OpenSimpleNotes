using Microsoft.AspNetCore.Http;

namespace OSN.Infrastructure.Interfaces;

public interface IHttpContentResolver
{
    public Task<T> GetPayloadAsync<T>(HttpRequest request);
}