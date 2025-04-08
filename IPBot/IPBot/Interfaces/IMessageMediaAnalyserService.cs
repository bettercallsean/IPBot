namespace IPBot.Interfaces;

public interface IMessageMediaAnalyserService
{
    Task<List<string>> GetContentUrlsAsync(SocketMessage message);
}