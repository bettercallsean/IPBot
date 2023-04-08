namespace IPBot.Interfaces;

public interface ITenorApiHelper
{
    Task<string> GetDirectTenorGifUrlAsync(string tenorUrl);
}