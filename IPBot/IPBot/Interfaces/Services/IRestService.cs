namespace IPBot.Interfaces.Services;

public interface IRestService
{
    Task<T> GetAsync<T>(string url);
    Task<T> PostAsync<T>(string url, object dto);
    Task<T> PatchAsync<T>(string url, object dto);
    Task<T> PatchAsync<T>(string url);
    Task<T> PutAsync<T>(string url, object dto);
    Task<T> DeleteAsync<T>(string url);
}