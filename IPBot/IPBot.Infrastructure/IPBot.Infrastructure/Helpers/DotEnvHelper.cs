namespace IPBot.Infrastructure.Helpers;

public static class DotEnvHelper
{
    private static readonly IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();

    public static IDictionary<string, string> EnvironmentVariables => _environmentVariables;

    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            _environmentVariables.Add(parts[0], parts[1]);
        }
    }
}
