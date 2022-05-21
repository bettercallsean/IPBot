using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IPBot.API.Helpers;

public static class PythonScriptHelper
{
    public static async Task<string> RunPythonScriptAsync(string fileName, string arguments = "")
    {
        File.WriteAllText("test.txt", "try");

        try
        {

            var fullFilePath = Path.Combine(Constants.ScriptsDirectory, fileName);
            File.WriteAllText("test.txt", "fullFilePath");

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "python" : "python3",
                Arguments = string.IsNullOrWhiteSpace(arguments) ? fullFilePath : $"{fullFilePath} {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            File.WriteAllText("test.txt", "process");

            if (process == null)
            {
                return string.Empty;
            }

            var result = await process.StandardOutput.ReadToEndAsync();
            return result.Trim();
        }
        catch (Exception ex)
        {
            File.WriteAllText("test.txt", ex.Message);

            return string.Empty;
        }
    }
}