namespace OrganicPortalBackend.Services.Cronos
{
    public class CronJob : CronJobProgram
    {
        private readonly ILogger<CronJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CronJob(IScheduleConfig<CronJob> config, ILogger<CronJob> logger, IServiceProvider serviceProvider) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob Start");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Checks if use developer mode.
                var webhost = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
#if DEBUG
                _logger.LogInformation("CronJob");
#else
                var service = scope.ServiceProvider.GetRequiredService<ISolanaCERT>();
                var res = service.CronSolana().Result;
#endif
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob Stop");
            return base.StopAsync(cancellationToken);
        }
    }
}
