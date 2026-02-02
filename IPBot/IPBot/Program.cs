using IPBot;

CreateHostBuilder().Build().Run();

static IHostBuilder CreateHostBuilder() =>
    Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webHost =>
    {
        webHost.UseStartup<Startup>();
    });
