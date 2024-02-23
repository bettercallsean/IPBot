namespace IPBot.API.Domain.Enums.ValveServerQueries;

[Flags]
public enum ExtraDataFlags : byte
{
    GameID = 0x01,
    SteamID = 0x10,
    Keywords = 0x20,
    Spectator = 0x40,
    Port = 0x80
}
