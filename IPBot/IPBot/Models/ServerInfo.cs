namespace IPBot.Models
{
    internal record ServerInfo
    {
        public bool Online { get; }
        public string Map { get; }
        public int PlayerCount { get; }
        public List<string> PlayerNames { get; }
    }
}