using System.Collections.Generic;
using System.Threading.Tasks;
using scrapper.Models.Dto;


namespace scrapper.Services
{
    public interface IJobParserService
    {
        Task ScrapingProjectAsync(Uri[] url);
    }
}
