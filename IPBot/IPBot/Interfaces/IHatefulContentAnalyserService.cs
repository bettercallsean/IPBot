namespace IPBot.Interfaces;

public interface IHatefulContentAnalyserService
{
    Task CheckMessageForHatefulContentAsync(SocketMessage message);
}