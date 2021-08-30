using System.Threading.Tasks;

namespace IPBot
{
    class MainClass
    {
        public static async Task Main(string[] args)
            => await Startup.RunAsync(args);
    }
}
