namespace IPBot;

public static class Program
{
    public static void Main() => CreateHostBuilder().Build().Run();

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webHost =>
        {
            webHost.UseStartup<Startup>();
        });
}
