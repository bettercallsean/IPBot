namespace IPBot.Interfaces;

public interface IAnimeAnalyserService
{
    Task CheckMessageForAnimeAsync(SocketMessage message);
}