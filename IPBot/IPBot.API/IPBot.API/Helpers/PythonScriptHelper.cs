using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IPBot.API.Helpers;

public static class PythonScriptHelper
{
    public static async Task<string> RunPythonScriptAsync(string fileName, string arguments = "")
    {
        var fullFilePath = Path.Combine(Constants.ScriptsDirectory, fileName);
        var osIsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = osIsWindows ? "python" : fileName,
            Arguments = osIsWindows ? $"{fullFilePath} {arguments}" : arguments,
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