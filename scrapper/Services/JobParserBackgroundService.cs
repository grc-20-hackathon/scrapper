using AngleSharp.Dom;
using Newtonsoft.Json;
using scrapper.Models.Dto;


namespace scrapper.Services
{
    public class JobParserBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<JobParserBackgroundService> _logger;
        private readonly List<string> _jobSites = new List<string>();

        public JobParserBackgroundService(IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<JobParserBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            //TODO Replace with value from the database
            _jobSites = configuration.GetSection("Site").Get<List<string>>()
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
                        var neo4jService = scope.ServiceProvider.GetRequiredService<INeo4jService>();
                        foreach (var url in _jobSites)
                        {
                            try
                            {
                                var vacancies = new List<JobOpeningDto>();
                                await jobParserService.ScrapingProjectAsync(_jobSites.ToArray());
                                _logger.LogInformation($"Scraped jobs from {url}: {vacancies?.Count}");
                                // Saving jobs to Neo4j
                                if (vacancies.Any())
                                {
                                    //await neo4jService.SaveDataModelsAsync(vacancies.ToArray());
                                    _logger.LogInformation($"Saved jobs to Neo4j: {vacancies.Count} for site {url} ");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error occurred while scraping {url}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in job scraping background service");
                }

                // Waiting 2 hours before the next cycle
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}
