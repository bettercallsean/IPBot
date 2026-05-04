using IPBot.Common.Dtos;

namespace IPBot.Interfaces.Services;

public interface IHatefulContentAnalyserService
{
    Task CheckMessageForHatefulContentAsync(SocketMessage message);
}