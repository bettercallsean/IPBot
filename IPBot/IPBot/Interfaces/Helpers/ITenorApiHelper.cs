namespace IPBot.Interfaces.Helpers;

public interface ITenorApiHelper
{
    Task<string> GetDirectTenorGifUrlAsync(string tenorUrl);
}