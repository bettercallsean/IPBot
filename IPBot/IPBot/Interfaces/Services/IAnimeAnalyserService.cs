namespace IPBot.Interfaces.Services;

public interface IAnimeAnalyserService
{
    Task CheckMessageForAnimeAsync(SocketMessage message);
}