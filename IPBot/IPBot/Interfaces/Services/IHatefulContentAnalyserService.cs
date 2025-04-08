namespace IPBot.Interfaces.Services;

public interface IHatefulContentAnalyserService
{
    Task CheckMessageForHatefulContentAsync(SocketMessage message);
}