using ImageMagick;

namespace IPBot.AnimeAnalyser.Helpers;

public static class ImageCompressorHelper
{
    public static async Task<Stream> CompressImageFromUrlAsync(string imageUrl)
    {
        using var httpClient = new HttpClient();

        var imageStream = await httpClient.GetStreamAsync(imageUrl);

        using var image = new MagickImage(imageStream);

        image.Format = image.Format;
        image.Resize(800, 800);
        image.Quality = 35;

        return new MemoryStream(image.ToByteArray());
    }
}