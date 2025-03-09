using scrapper.Models;

namespace scrapper.Services
{
    public interface IAIService
    {
        public Task<OpenJobs> GetData(string url, PAGE page);
    }
}
