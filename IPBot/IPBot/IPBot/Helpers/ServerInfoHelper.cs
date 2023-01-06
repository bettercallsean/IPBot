namespace IPBot.Helpers;

internal static class ServerInfoHelper
{
    private static readonly string ArkServerDataFile = Path.Combine(BotConstants.ConfigDirectory, "ark_server_data.json");
    
    public static string PlayerCountStatus(IEnumerable<string> players)
    {
        var playersList = players.ToList();
        var playerName = playersList.Count > 0
            ? playersList[Random.Shared.Next(playersList.Count)]
            : string.Empty;

        return playersList.Count switch
        {
            0 => $"{BotConstants.SeverOnlineString} No one is currently playing :)",
            1 => $"{BotConstants.SeverOnlineString} {playerName} is the only one playing :)",
            2 => $"{BotConstants.SeverOnlineString} {playerName} and one other are playing :)",
            _ => $"{BotConstants.SeverOnlineString} {playerName} and {playersList.Count - 1} others are playing :)"
        };
    }

    public static string PlayerCountStatus(int playerCount)
    {
        return playerCount switch
        {
            0 => $"{BotConstants.SeverOnlineString} No one is currently playing :)",
            1 => $"{BotConstants.SeverOnlineString} One person is playing :)",
            _ => $"{BotConstants.SeverOnlineString} {playerCount} people are playing :)",
        };
    }
    public static async Task<Dictionary<ushort, string>> LoadArkServerDataAsync()
    {
        if (!File.Exists(ArkServerDataFile))
        {
            await CreateArkDataFileAsync();
        }

        using var file = new StreamReader(ArkServerDataFile);
        return JsonConvert.DeserializeObject<Dictionary<ushort, string>>(await file.ReadToEndAsync());
    }

    public static async Task SaveArkServerDataAsync(Dictionary<ushort, string> servers)
    {
        await using var file = new StreamWriter(ArkServerDataFile);
        await file.WriteAsync(JsonConvert.SerializeObject(servers));
    }

    private static async Task CreateArkDataFileAsync()
    {
        var ports = Resources.Resources.ServerPorts.Split(Environment.NewLine).ToDictionary(ushort.Parse, _ => string.Empty);
        await SaveArkServerDataAsync(ports);
    }
}
