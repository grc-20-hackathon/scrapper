using AngleSharp.Dom;
using Newtonsoft.Json;
using scrapper.Models.Dto;


namespace scrapper.Services
{
    public class JobParserBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<JobParserBackgroundService> _logger;
        private readonly List<Uri> _jobSites = new List<Uri>();

        public JobParserBackgroundService(IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<JobParserBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            //TODO Replace with value from the database
            _jobSites = configuration.GetSection("Site").Get<List<Uri>>()
                ?? throw new InvalidOperationException("No sites for scraping in the settings");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting job scraping: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var jobParserService = scope.ServiceProvider.GetRequiredService<IJobParserService>();
                        try
                        {
                            await jobParserService.ScrapingProjectAsync(_jobSites.ToArray());
                            _logger.LogInformation($"Scraped jobs from sites: {string.Join(", ", _jobSites)} completed");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error occurred while scraping {string.Join(", ", _jobSites)}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in job scraping background service");
                }

     
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}
