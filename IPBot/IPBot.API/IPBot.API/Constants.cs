namespace IPBot.API;

public class Constants
{
    public static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string ConfigDirectory = BaseDirectory + "Configs";
    public static readonly string ScriptsDirectory = BaseDirectory + "Scripts";
    public const int MinecraftServerPort = 25565;
    public const int ZomboidServerPort = 16261;
    public const string SteamServerCode = "steam";
    public const string MinecraftServerCode = "mc";
}