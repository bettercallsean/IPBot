namespace IPBot.Models
{
    internal record ServerInfo
    {
        public bool Online { get; } = false;
        public string Map { get; }
        public int PlayerCount { get; }
        public List<string> PlayerNames { get; }
    }
}