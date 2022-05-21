using System.Diagnostics;

namespace IPBot.Helpers;

public static class PythonScriptHelper
{
    public static async Task<string> RunPythonScriptAsync(string fileName, string arguments = "")
    {
        var fullFilePath = Path.Combine(Constants.ScriptsDirectory, fileName);
        
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = arguments == string.Empty ? fullFilePath : $"{fullFilePath} {arguments}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });

        if (process == null)
        {
            return string.Empty;
        }

        var result = await process.StandardOutput.ReadToEndAsync();
        return result.Trim();
    }
}