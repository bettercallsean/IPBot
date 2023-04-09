using IPBot.Shared.Dtos;

namespace IPBot.Helpers;

internal static class ServerInfoHelper
{
    public static string GetServerStatus(ServerInfoDto serverInfo)
    {
        if (serverInfo is null || !serverInfo.Online) return BotConstants.ServerOfflineString;

        var serverStatus = serverInfo.PlayerNames.Count == 0
            ? PlayerCountStatus(serverInfo.PlayerCount)
            : PlayerCountStatus(serverInfo.PlayerNames);

        return serverStatus;
    }
    
    private static string PlayerCountStatus(IEnumerable<string> players)
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

    private static string PlayerCountStatus(int playerCount)
    {
        return playerCount switch
        {
            0 => $"{BotConstants.SeverOnlineString} No one is currently playing :)",
            1 => $"{BotConstants.SeverOnlineString} One person is playing :)",
            _ => $"{BotConstants.SeverOnlineString} {playerCount} people are playing :)",
        };
    }
}
