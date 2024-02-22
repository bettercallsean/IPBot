using IPBot.API.Domain.Enums.ValveServerQueries;

namespace IPBot.API;

public record SteamServerInfo
{
    public byte Header { get; set; }
    public byte Protocol { get; set; }
    public string Name { get; set; }
    public string Map { get; set; }
    public string Folder { get; set; }
    public string Game { get; set; }
    public short ID { get; set; }
    public byte PlayerCount { get; set; }
    public byte MaxPlayers { get; set; }
    public List<string> PlayerNames { get; set; }
    public byte Bots { get; set; }
    public ServerTypeFlags ServerType { get; set; }
    public EnvironmentFlags Environment { get; set; }
    public VisibilityFlags Visibility { get; set; }
    public VACFlags VAC { get; set; }
    public string Version { get; set; }
    public ExtraDataFlags ExtraDataFlag { get; set; }

    #region Extra Data Flag Members
    public ulong GameID { get; set; }
    public ulong SteamID { get; set; }
    public string Keywords { get; set; }
    public short Port { get; set; }
    #endregion

    #region Custom Data
    public string IP { get; set; }
    public bool Online { get; set; }
    #endregion
}
