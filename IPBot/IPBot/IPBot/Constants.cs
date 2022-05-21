namespace IPBot;

public static class Constants
{
    public const int MinecraftServerPort = 25565;
    public const int ZomboidServerPort = 16261;
    public static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string ConfigDirectory = BaseDirectory + "Configs";
    public static readonly string ScriptsDirectory = BaseDirectory + "Scripts";
    public const string ServerOfflineString = "The server is currently offline :(";
    public const string SeverOnlineString = "The server is online!";
}
