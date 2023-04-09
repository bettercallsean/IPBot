using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IPBot.Infrastructure.Helpers;

public static class PythonScriptHelper
{
    private static readonly string ScriptsDirectory = AppContext.BaseDirectory + "Scripts";
    
    public static async Task<string> RunPythonScriptAsync(string fileName, string arguments = "")
    {
        var fullFilePath = Path.Combine(ScriptsDirectory, fileName);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "python" : "python3",
            Arguments = string.IsNullOrWhiteSpace(arguments) ? fullFilePath : $"{fullFilePath} {arguments}",
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