using scrapper.Models;
using HtmlAgilityPack;
using System.Buffers.Text;
using System.Text;
using scrapper.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace scrapper.Services
{
    public class JobParserService : IJobParserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobParserService> _logger;
        private readonly IAIService _claudeService;
        private readonly ApplicationDbContext _dbContext;
        private List<ProjectView> ListError = new List<ProjectView>();

        public JobParserService(HttpClient httpClient, ILogger<JobParserService> logger, IAIService claudeService, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            // Setting User-Agent to bypass blocks
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _claudeService = claudeService;
            _dbContext = dbContext;
        }

        public async Task ScrapingProjectAsync(Uri[] urls)
        {
            foreach (var url in urls)
            {
                Dictionary<Uri, List<OpenJobs>> jobOpen = new Dictionary<Uri, List<OpenJobs>>();

            
                SelectorModel? xpach = GetXPach(url);
                List<Uri> sourceUrl = await GetProjectSourse(url, await GetListProjectUrl(url), xpach);
      
                if (url.AbsoluteUri == new Uri("https://cryptojobs.com").AbsoluteUri)
                {
                    jobOpen = await GetListOpenJobsInCryptojobs(xpach.OpenJopsXPath, url);
                    foreach (var jobs in jobOpen.ToList())
                    {
                        jobOpen[jobs.Key] = await GetOpenJopDetails(jobs.Value, xpach, url);
                    }
                }
       
                foreach (var item in sourceUrl)
                {
                    ProjectView projectView = new ProjectView()
                    {
                        Status = Models.TaskStatus.Pending,
                        ProjectSourse = item.AbsoluteUri
                    };


                    if (url.AbsoluteUri == new Uri("https://cryptojobs.com").AbsoluteUri && jobOpen.ContainsKey(item))
                    {
                        projectView.ListOpenJobsInProject = jobOpen[item];
                    }
            
                    projectView = await GetWebSiteAndName(projectView, xpach, url.AbsoluteUri);

                    ProjectView? projectInList = null;
              
                    if (projectView.ProjectWebsite != null || !string.IsNullOrEmpty(projectView.ProjectName))
                    {
                        projectInList = _dbContext.projectView.
                            Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.skills).
                            Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.workFormats).
                            Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.jobOpening).
                            Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.role).AsEnumerable().
                            FirstOrDefault(x => x.ProjectWebsite?.ToString() == projectView.ProjectWebsite?.ToString() || x.ProjectName == projectView.ProjectName);
                    }

                    if (projectInList != null)
                    {
               
                        if (projectInList.ProjectWebsite == null)
                            projectInList.ProjectWebsite = projectView.ProjectWebsite;
                        if (projectInList.ProjectName == null)
                            projectInList.ProjectName = projectView.ProjectName;

                        if (projectView.ListOpenJobsInProject != null && projectView.ListOpenJobsInProject.Count() > 0)
                        {
                            if (projectInList.ListOpenJobsInProject == null)
                                projectInList.ListOpenJobsInProject = new List<OpenJobs>();
                            projectInList.ListOpenJobsInProject.AddRange(projectView.ListOpenJobsInProject);
                        }                    
             
                        projectInList.ProjectSourse = item.AbsoluteUri;
                        projectInList = await GetProjectDetails(projectInList, xpach, url.AbsoluteUri);
                        _dbContext.Entry<ProjectView>(projectInList).State = EntityState.Modified;

                    }
                    else
                    {
                        projectView = await GetProjectDetails(projectView, xpach, url.AbsoluteUri);
                        if (projectView.Status == Models.TaskStatus.Completed)
                        {
                            await _dbContext.projectView.AddAsync(projectView);
                        }
                        else
                            ListError.Add(projectView);
                    }
                    await _dbContext.SaveChangesAsync();
                }

            }
        }
        private async Task<Dictionary<Uri, List<OpenJobs>>> GetListOpenJobsInCryptojobs(string xPach, Uri baseUrl)
        {
            Dictionary<Uri, List<OpenJobs>> UrlDetails = new Dictionary<Uri, List<OpenJobs>>();
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
                                var urlProject = new Uri(item.Skip(1).Take(1).FirstOrDefault()[1].ChildNodes.FirstOrDefault(x => x.Name == "a").GetAttributes().FirstOrDefault(x => x.Name == "href").Value);
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
        public SelectorModel? GetXPach(Uri baseUrl)
        {
            var selectorModel = new SelectorModel();

            switch (baseUrl.AbsoluteUri)
            {
                case "https://cryptojobslist.com/":
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
                case "https://cryptojobs.com/":
                    selectorModel.ProjectListUrlXPath = "//*[@id=\"searchForm\"]/section[2]/div/div[*]/div/article/aside[1]/h2/a";
                    selectorModel.ProjctDescriptionXPath = "/html/body/div[1]/div/section[2]/div[1]/div/div[2]/div";
                    selectorModel.ProjctNameXPath = "/html/body/div[1]/div/div[2]/div[1]/div/article/aside[1]/div/h1";
                    selectorModel.ProjctWebsiteXPath = "/html/body/div[1]/div/div[2]/div[1]/div/article/aside[2]/ul[2]/li/a[*]";
                    selectorModel.ProjectImageAvatarXpach = "/html/body/div[1]/div/div[2]/div[1]/div/figure/span/img";
                    selectorModel.OpenJopsXPath = "//*[@id=\"searchForm\"]/section[2]/div/div[*]/article/aside[1]";
                    selectorModel.OpenJopsOverviewXpach = "/html/body/div[1]/div/section[3]/div/div[2]/div/div[1]";
                    selectorModel.OpenJopsPropertyXpach = "/html/body/div[1]/div/section[2]/div/div[1]/article";
                    return selectorModel;
                case "https://cryptocurrencyjobs.co/":
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
        public async Task<string> GetListProjectUrl(Uri baseUrl)
        {
            switch (baseUrl.AbsoluteUri)
            {
                case "https://cryptojobslist.com/":
                    return System.IO.Path.Combine(baseUrl.AbsoluteUri, "companies");
                case "https://cryptojobs.com/":
                    return System.IO.Path.Combine(baseUrl.AbsoluteUri, "companies");
                case "https://cryptocurrencyjobs.co/":
                    return System.IO.Path.Combine(baseUrl.AbsoluteUri, "startups");
                default:
                    return "";
            }
            ;
        }
        public async Task<List<Uri>> GetProjectSourse(Uri baseUrl, string link, SelectorModel selectorModel)
        {
            try
            {
                List<Uri> UrlDetails = new List<Uri>();
                int page = 1;
                Uri firstLink = null;
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
                            .Select(x => ValidateUrl(baseUrl.AbsoluteUri, x))?
                            .ToList();

                        if (links != null && firstLink?.AbsoluteUri != links.First().AbsoluteUri)
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
                    string? website = doc.DocumentNode
                            .SelectNodes(selectorModel.ProjctWebsiteXPath)?
                            .Where(x => x?.Name == "a" && (x?.InnerText?.ToLowerInvariant()?.Contains("website")).Value)?
                            .Select(x => x?.GetAttributeValue("href", null))?
                            .FirstOrDefault()?.Split("?")?.FirstOrDefault()?.Trim();
                    if (string.IsNullOrEmpty(website))
                        projectView.ProjectWebsite = null;
                    else
                        projectView.ProjectWebsite = new Uri(website);
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
                        .Select(x => new OpenJobs { jobSourse = ValidateUrl(baseUrl, x?.GetAttributeValue("href", null))?.AbsoluteUri, jobOpening = new Models.JobOpening { name = x?.InnerText?.Trim() } })?.ToList();
                    if (links != null)
                    {
                        links = await GetOpenJopDetails(links, selectorModel, new Uri(baseUrl));
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
        public async Task<List<OpenJobs>> GetOpenJopDetails(List<OpenJobs> jobOpening, SelectorModel selectorModel, Uri baseUrl)
        {
            List<OpenJobs> jobOpeningNew = new List<OpenJobs>();
            foreach (var item in jobOpening)
            {
                if (_dbContext.projectView.Include(x => x.ListOpenJobsInProject).Where(x => x.ListOpenJobsInProject.Select(y => y.jobSourse).Contains(item.jobSourse)).Count() > 0)
                    continue;
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

                var data = await _claudeService.GetData(sb.ToString(), PAGE.DETAILS);
                if (data != null)
                {
                    data.jobOpening.name = item.jobOpening.name;
                    data.jobSourse = item.jobSourse;
                    jobOpeningNew.Add(data);
                }
            }
            return jobOpeningNew;
        }
        private Uri? ValidateUrl(string baseUrl, string newUrl)
        {
            var url = (newUrl.Contains("http://") || newUrl.Contains("https://")) ? newUrl : baseUrl + newUrl;
            if (url != null)
                return new Uri(url);
            else
                return null;
        }
    }
}
