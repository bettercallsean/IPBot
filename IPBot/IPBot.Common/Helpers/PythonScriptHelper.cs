using System.Diagnostics;

namespace IPBot.Common.Helpers;

public static class PythonScriptHelper
{
    private static readonly string ScriptsDirectory = AppContext.BaseDirectory + "Scripts";

    public static async Task<string> RunPythonScriptAsync(string fileName, string arguments = "")
    {
        var fullFilePath = Path.Combine(ScriptsDirectory, fileName);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = fullFilePath,
            Arguments = string.IsNullOrWhiteSpace(arguments) ? string.Empty : arguments,
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