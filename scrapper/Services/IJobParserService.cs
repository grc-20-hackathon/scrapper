using System.Collections.Generic;
using System.Threading.Tasks;
using scrapper.Models.Dto;


namespace scrapper.Services
{
    public interface IJobParserService
    {
        Task<List<JobOpeningDto>> ScrapingProjectAsync(string[] url);
    }
}
