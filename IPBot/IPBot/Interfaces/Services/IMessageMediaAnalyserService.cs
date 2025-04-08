namespace IPBot.Interfaces.Services;

public interface IMessageMediaAnalyserService
{
    Task<List<string>> GetContentUrlsAsync(SocketMessage message);
}