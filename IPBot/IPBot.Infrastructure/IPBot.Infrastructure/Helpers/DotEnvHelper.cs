namespace IPBot.Infrastructure.Helpers;

public static class DotEnvHelper
{
    private static readonly IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();

    public static IDictionary<string, string> EnvironmentVariables => _environmentVariables;

    public static void Load(string filePath)
    {
        var envDirectory = Path.Combine(AppContext.BaseDirectory, filePath);
        if (!File.Exists(envDirectory))
            return;

        foreach (var line in File.ReadAllLines(envDirectory))
        {
            var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            _environmentVariables.Add(parts[0], parts[1]);
        }
    }
}
