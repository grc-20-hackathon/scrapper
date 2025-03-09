using scrapper.Models;
using HtmlAgilityPack;
using System.Buffers.Text;
using System.Text;
using scrapper.Models.Dto;

namespace scrapper.Services
{
    public class JobParserService : IJobParserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobParserService> _logger;
        private readonly IAIService _claudeService;
        private readonly INeo4jService _neo4jService;
        private List<ProjectView> ListError = new List<ProjectView>();

        public JobParserService(HttpClient httpClient, ILogger<JobParserService> logger, IAIService claudeService, INeo4jService neo4jService)
        {
            _httpClient = httpClient;
            _logger = logger;
            // Setting User-Agent to bypass blocks
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _claudeService = claudeService;
            _neo4jService = neo4jService;
        }

        public async Task<List<JobOpeningDto>> ScrapingProjectAsync(string[] urls)
        {
            var projectList = new List<ProjectView>();
            foreach (var url in urls)
            {
                Dictionary<string, List<OpenJobs>> jobOpen = new Dictionary<string, List<OpenJobs>>();
            }
            var sss = projectList
                .Where(x => x.ListOpenJobsInProject?.Count > 0).ToList();
            var cc = sss?.Select(x => x.ListOpenJobsInProject?.Count()).ToList().Sum();
            return null;
        }
        private async Task<Dictionary<string, List<OpenJobs>>> GetListOpenJobsInCryptojobs(string xPach, string baseUrl)
        {
            Dictionary<string, List<OpenJobs>> UrlDetails = new Dictionary<string, List<OpenJobs>>();
            try
            {
                int page = 1;
                while (true)
                {
                    var html = await _httpClient.GetStringAsync("https://www.cryptojobs.com/jobs?page=" + page + "&per_page=50");
                    if (!string.IsNullOrEmpty(html))
                    {
                        page++;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        var links = doc.DocumentNode.SelectNodes(xPach)?.Select(x => x?.ChildNodes?.Where(y => y?.Name == "h2" || y?.Name == "ul")?.Select(z => z?.ChildNodes));
                        if (links != null)
                        {
                            foreach (var item in links)
                            {
                                var model = new OpenJobs
                                {
                                    jobOpening = new Models.JobOpening
                                    {
                                        name = item.FirstOrDefault().FirstOrDefault().InnerText.Trim()
                                    },
                                    jobSourse = item.FirstOrDefault().FirstOrDefault().GetAttributes().FirstOrDefault(x => x.Name == "href").Value
                                };
                                var urlProject = item.Skip(1).Take(1).FirstOrDefault()[1].ChildNodes.FirstOrDefault(x => x.Name == "a").GetAttributes().FirstOrDefault(x => x.Name == "href").Value;
                                if (UrlDetails.ContainsKey(urlProject))
                                {
                                    UrlDetails[urlProject].Add(model);
                                }
                                else
                                {
                                    UrlDetails.Add(urlProject, new List<OpenJobs> { model });
                                }
                            }
                        }
                        else
                            return UrlDetails;
                    }
                    else
                        return UrlDetails;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public SelectorModel? GetXPach(string baseUrl)
        {
            var selectorModel = new SelectorModel();
            switch (baseUrl)
            {
                case "https://cryptojobslist.com":
                    selectorModel.ProjectListUrlXPath = "//*[@id=\"__next\"]/main/section[2]/ul/li[*]/a";
                    selectorModel.ProjctDescriptionXPath = "//*[@id=\"__next\"]/main/div[1]/div[2]/div/div[1]";
                    selectorModel.ProjctNameXPath = "//*[@id=\"__next\"]/main/div[1]/div[1]/h1/text()[1]";
                    selectorModel.ProjctWebsiteXPath = "//*[@id=\"__next\"]/main/div[1]/div[2]/aside/a[*]";
                    selectorModel.ProjctXXPath = "//*[@id=\"__next\"]/main/div[1]/div[2]/aside/a[*]";
                    selectorModel.ProjectImageAvatarXpach = "//*[@id=\"__next\"]/main/div/img";
                    selectorModel.OpenJopsXPath = "//*[@id=\"__next\"]/main/div/div[2]/div/div[2]/div[1]/div[*]/div/h3/a";
                    selectorModel.OpenJopsOverviewXpach = "//*[@id=\"__next\"]/main/div[3]/div[1]";
                    selectorModel.OpenJopsPropertyXpach = "//*[@id=\"__next\"]/main/div[3]/div[2]/div";
                    selectorModel.OpenJopsPostedDateXpach = "//*[@id=\"__next\"]/main/div[1]/div[1]/div[2]";
                    return selectorModel;
                case "https://cryptojobs.com":
                    selectorModel.ProjectListUrlXPath = "//*[@id=\"searchForm\"]/section[2]/div/div[*]/div/article/aside[1]/h2/a";
                    selectorModel.ProjctDescriptionXPath = "/html/body/div[1]/div/section[2]/div[1]/div/div[2]/div";
                    selectorModel.ProjctNameXPath = "/html/body/div[1]/div/div[2]/div[1]/div/article/aside[1]/div/h1";
                    selectorModel.ProjctWebsiteXPath = "/html/body/div[1]/div/div[2]/div[1]/div/article/aside[2]/ul[2]/li/a[*]";
                    selectorModel.ProjectImageAvatarXpach = "/html/body/div[1]/div/div[2]/div[1]/div/figure/span/img";
                    selectorModel.OpenJopsXPath = "//*[@id=\"searchForm\"]/section[2]/div/div[*]/article/aside[1]";
                    selectorModel.OpenJopsOverviewXpach = "/html/body/div[1]/div/section[3]/div/div[2]/div/div[1]";
                    selectorModel.OpenJopsPropertyXpach = "/html/body/div[1]/div/section[2]/div/div[1]/article";
                    return selectorModel;
                case "https://cryptocurrencyjobs.co":
                    selectorModel.ProjectListUrlXPath = "/html/body/main/div[4]/div/div/a[*]";
                    selectorModel.ProjctDescriptionXPath = "/html/body/main/section[1]/div[1]/div";
                    selectorModel.ProjctNameXPath = "/html/body/main/div/div/div[1]/h1";
                    selectorModel.ProjctWebsiteXPath = "/html/body/main/section[1]/div[*]/ul/li[*]/a";
                    selectorModel.ProjctXXPath = "/html/body/main/section[1]/div[*]/ul/li[*]/a";
                    selectorModel.ProjectImageAvatarXpach = "/html/body/main/div/div/div[1]/img";
                    selectorModel.OpenJopsXPath = "/html/body/main/section[1]/div[2]/div/div/div[*]/div[*]/div/h4/a";
                    selectorModel.OpenJopsOverviewXpach = "/html/body/main/div/div[2]/div/div[1]";
                    selectorModel.OpenJopsPropertyXpach = "/html/body/main/div/div[2]/div/div[2]";
                    return selectorModel;
                default:
                    return selectorModel;
            }
        }
        public async Task<string> GetListProjectUrl(string baseUrl)
        {
            switch (baseUrl)
            {
                case "https://cryptojobslist.com":
                    return System.IO.Path.Combine(baseUrl, "companies");
                case "https://cryptojobs.com":
                    return System.IO.Path.Combine(baseUrl, "companies");
                case "https://cryptocurrencyjobs.co":
                    return System.IO.Path.Combine(baseUrl, "startups");
                default:
                    return "";
            }
            ;
        }
        public async Task<List<string>> GetProjectSourse(string baseUrl, string link, SelectorModel selectorModel)
        {
            try
            {
                List<string> UrlDetails = new List<string>();
                int page = 1;
                string firstLink = "";
                while (true)
                {
                    var html = await _httpClient.GetStringAsync(link + "?page=" + page);
                    if (!string.IsNullOrEmpty(html))
                    {
                        page++;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var links = doc.DocumentNode.SelectNodes(selectorModel.ProjectListUrlXPath)?
                            .Select(item => item.GetAttributes().FirstOrDefault(x => x.Name == "href")?.Value)?
                            .Select(x => ValidateUrl(baseUrl, x))?
                            .ToList();

                        if (links != null && firstLink != links.First())
                        {
                            firstLink = links.First();
                            UrlDetails.AddRange(links);
                        }
                        else
                            return UrlDetails;
                    }
                    else
                        return UrlDetails;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public async Task<ProjectView> GetWebSiteAndName(ProjectView projectView, SelectorModel selectorModel, string baseUrl)
        {
            try
            {
                var doc = new HtmlDocument();
                projectView.Status = Models.TaskStatus.Processing;
                var response = await _httpClient.GetStringAsync(projectView.ProjectSourse);
                doc.LoadHtml(response);
                if (projectView.ProjectWebsite == null && selectorModel.ProjctWebsiteXPath != null)
                {
                    projectView.ProjectWebsite = doc.DocumentNode
                            .SelectNodes(selectorModel.ProjctWebsiteXPath)?
                            .Where(x => x?.Name == "a" && (x?.InnerText?.ToLowerInvariant()?.Contains("website")).Value)?
                            .Select(x => x?.GetAttributeValue("href", null))?
                            .FirstOrDefault()?.Split("?")?.FirstOrDefault();
                }
                if (projectView.ProjectName == null && selectorModel.ProjctNameXPath != null)
                {
                    projectView.ProjectName = doc.DocumentNode
                            .SelectSingleNode(selectorModel.ProjctNameXPath)?.InnerText?.Replace("\n", "")?.Trim();
                }
                projectView.Status = Models.TaskStatus.Completed;
            }
            catch (Exception ex)
            {
                projectView.Status = Models.TaskStatus.Failed;
                _logger.LogError(ex, ex.Message);
            }

            return projectView;
        }
        public async Task<ProjectView> GetProjectDetails(ProjectView projectView, SelectorModel selectorModel, string baseUrl)
        {
            try
            {
                projectView.Status = Models.TaskStatus.Processing;

                var doc = new HtmlDocument();
                var response = await _httpClient.GetStringAsync(projectView.ProjectSourse);
                doc.LoadHtml(response);
                if (string.IsNullOrEmpty(projectView.ProjectDescription) && selectorModel.ProjctDescriptionXPath != null)
                {
                    projectView.ProjectDescription = doc.DocumentNode
                                .SelectSingleNode(selectorModel.ProjctDescriptionXPath)?.InnerText?.Replace("\n", "")?.Trim();
                }
                if (string.IsNullOrEmpty(projectView.ProjectX) && selectorModel.ProjctXXPath != null)
                {
                    projectView.ProjectX = doc.DocumentNode
                            .SelectNodes(selectorModel.ProjctXXPath)?
                            .Where(x => x?.Name == "a" && (x?.InnerText?.ToLowerInvariant()?.Contains("twitter")).Value)?
                            .Select(x => x?.GetAttributeValue("href", null))?
                            .FirstOrDefault();
                }
                if (selectorModel.OpenJopsXPath != null && baseUrl != "https://cryptojobs.com")
                {
                    var links = doc.DocumentNode
                        .SelectNodes(selectorModel.OpenJopsXPath)?
                        .Select(x => new OpenJobs { jobSourse = ValidateUrl(baseUrl, x?.GetAttributeValue("href", null)), jobOpening = new Models.JobOpening { name = x?.InnerText?.Trim() } })?.ToList();
                    if (links != null)
                    {
                        links = await GetOpenJopDetails(links, selectorModel, baseUrl);
                        if (projectView.ListOpenJobsInProject == null)
                        {
                            projectView.ListOpenJobsInProject = links;
                        }
                        else
                        {
                            projectView.ListOpenJobsInProject.AddRange(links);
                        }
                    }
                }
                if (string.IsNullOrEmpty(projectView.ProjectAvatar) && selectorModel.ProjectImageAvatarXpach != null)
                {
                    projectView.ProjectAvatar = doc.DocumentNode
                            .SelectSingleNode(selectorModel.ProjectImageAvatarXpach)?.GetAttributes(new string[] { "src", "data-src" })?.FirstOrDefault(x => x != null)?.Value;
                }
                if (projectView.ProjectX != null)
                {
                    //projectView.ProjectAvatar = /*await GetImagesSourse(*/projectView.ProjectX + "/photo"/*)*/;
                    projectView.ProjectCover = /*await GetImagesSourse(*/projectView.ProjectX + "/header_photo"/*)*/;
                }
                projectView.Status = Models.TaskStatus.Completed;
            }
            catch (Exception ex)
            {
                projectView.Status = Models.TaskStatus.Failed;
                _logger.LogError(ex, ex.Message);
            }
            return projectView;
        }
        public async Task<List<OpenJobs>> GetOpenJopDetails(List<OpenJobs> jobOpening, SelectorModel selectorModel, string baseUrl)
        {
            List<OpenJobs> jobOpeningNew = new List<OpenJobs>();
            foreach (var item in jobOpening)
            {
                var doc = new HtmlDocument();
                var response = await _httpClient.GetStringAsync(item.jobSourse);
                doc.LoadHtml(response);

                StringBuilder sb = new StringBuilder();

                if (selectorModel.OpenJopsOverviewXpach != null)
                {
                    sb.AppendLine(doc.DocumentNode
                                .SelectSingleNode(selectorModel.OpenJopsOverviewXpach)?.InnerHtml?.Trim());

                }
                if (selectorModel.OpenJopsPropertyXpach != null)
                {
                    sb.AppendLine(doc.DocumentNode
                                .SelectSingleNode(selectorModel.OpenJopsPropertyXpach)?.InnerHtml?.Trim());
                }
                if (selectorModel.OpenJopsPostedDateXpach != null)
                {
                    sb.AppendLine(doc.DocumentNode
                                .SelectSingleNode(selectorModel.OpenJopsPostedDateXpach)?.InnerHtml?.Trim());
                }

                var data = item; // await _claudeService.GetData(sb.ToString(), PAGE.DETAILS);
                data.jobOpening.name = item.jobOpening.name;
                data.jobSourse = item.jobSourse;
                jobOpeningNew.Add(data);
            }
            return jobOpeningNew;
        }
        private string ValidateUrl(string baseUrl, string newUrl)
        {
            return (newUrl.Contains("http://") || newUrl.Contains("https://")) ? newUrl : baseUrl + newUrl;
        }
    }
}
