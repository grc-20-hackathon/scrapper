using scrapper.Models;
using scrapper.Services;
using System.Text.Json;

namespace scrapper.GraphQL
{
    public class Mutation
    {
        //public async Task<SelectorModel> CreateSelectorModelAsync([Service] INeo4jService neo4jService, SelectorModel selectorModel, string site)
        //{
        //    //await neo4jService.SaveSelectorModel(site, selectorModel);
        //    return selectorModel;
        //}
        //public Configuration AddSite(string url)
        //{
        //    var existingConfiguration = new Configuration
        //    {
        //        SettingsAi = new SettingsAi
        //        {
        //            Id = 1,
        //            AiModelId = 22,
        //            AiTypeId = 13,
        //            ApiKey = "mock-token",
        //            MaxTokens = 1000,
        //            Temperature = 0.7m,
        //            PromptForXPath = "mock-prompt-xpath",
        //            PromptForXPathData = "mock-prompt-xpath-data",
        //            AiModels = new List<AiModel>
        //                {
        //                    new AiModel { Id = 21, Name = "Deepseek" },
        //                    new AiModel { Id = 22, Name = "Chat GPT" },
        //                    new AiModel { Id = 23, Name = "Clode" },
        //                },
        //            AiTypes = new List<AiType>
        //                {
        //                    new AiType { Id = 11, Name = "O1" },
        //                    new AiType { Id = 12, Name = "Fast" },
        //                    new AiType { Id = 13, Name = "Slow" },
        //                },
        //        },
        //        SettingsSite = new SettingsSite
        //        {
        //            Id = 1,
        //            SiteId = 34,
        //            IsGenerateAutomatically = 0,
        //            TitleUrlXPath = new TitleUrlXPath
        //            {
        //                JobTitleXPath = "",
        //                JobDescriptionXPath = "",
        //                RequiredSkillsXPath = "",
        //                WorkFormatXPath = "",
        //                LocationXPath = "",
        //                SalaryXPath = "",
        //                PublicationDateXPath = ""
        //            },
        //            Sites = new List<Site>
        //                {
        //                    new Site { Id = 31, Name = "https://cryptojobslist.com" },
        //                    new Site { Id = 32, Name = "https://www.cryptojobs.com/jobs" },
        //                    new Site { Id = 33, Name = "https://cryptocurrencyjobs.com" },
        //                    new Site { Id = 34, Name = url },
        //                }
        //        }
        //    };

        //    // Add the new site to the existing configuration
        //    //existingConfiguration.SettingsSite.Sites.Add(newSite);

        //    // In a real application, you would update the configuration in the database here

        //    return existingConfiguration;
        //}
        //public Configuration UpdateSettingsAi(SettingsAi settingsAi)
        //{
        //    var existingConfiguration = new Configuration
        //    {
        //        SettingsAi = new SettingsAi
        //        {
        //            Id = 1,
        //            AiModelId = 22,
        //            AiTypeId = 13,
        //            ApiKey = "UpdateSettingsAi",
        //            MaxTokens = 1000,
        //            Temperature = 0.7m,
        //            PromptForXPath = "mock-prompt-xpath",
        //            PromptForXPathData = "mock-prompt-xpath-data",
        //            AiModels = new List<AiModel>
        //                {
        //                    new AiModel { Id = 21, Name = "Deepseek" },
        //                    new AiModel { Id = 22, Name = "Chat GPT" },
        //                    new AiModel { Id = 23, Name = "Clode" },
        //                },
        //            AiTypes = new List<AiType>
        //                {
        //                    new AiType { Id = 11, Name = "O1" },
        //                    new AiType { Id = 12, Name = "Fast" },
        //                    new AiType { Id = 13, Name = "Slow" },
        //                },
        //        },
        //        SettingsSite = new SettingsSite
        //        {
        //            Id = 1,
        //            SiteId = 34,
        //            IsGenerateAutomatically = 0,
        //            TitleUrlXPath = new TitleUrlXPath
        //            {
        //                JobTitleXPath = "",
        //                JobDescriptionXPath = "",
        //                RequiredSkillsXPath = "",
        //                WorkFormatXPath = "",
        //                LocationXPath = "",
        //                SalaryXPath = "",
        //                PublicationDateXPath = ""
        //            },
        //            Sites = new List<Site>
        //                {
        //                    new Site { Id = 31, Name = "https://cryptojobslist.com" },
        //                    new Site { Id = 32, Name = "https://www.cryptojobs.com/jobs" },
        //                    new Site { Id = 33, Name = "https://cryptocurrencyjobs.com" },
        //                }
        //        }
        //    };

        //    // Add the new site to the existing configuration
        //    //existingConfiguration.SettingsSite.Sites.Add(newSite);

        //    // In a real application, you would update the configuration in the database here

        //    return existingConfiguration;
        //}
        //public Configuration UpdateSettingsSite(SettingsSite settingsSite)
        //{
        //    var existingConfiguration = new Configuration
        //    {
        //        SettingsAi = new SettingsAi
        //        {
        //            Id = 1,
        //            AiModelId = 22,
        //            AiTypeId = 13,
        //            ApiKey = "UpdateSettingsSite",
        //            MaxTokens = 1000,
        //            Temperature = 0.7m,
        //            PromptForXPath = "mock-prompt-xpath",
        //            PromptForXPathData = "mock-prompt-xpath-data",
        //            AiModels = new List<AiModel>
        //                {
        //                    new AiModel { Id = 21, Name = "Deepseek" },
        //                    new AiModel { Id = 22, Name = "Chat GPT" },
        //                    new AiModel { Id = 23, Name = "Clode" },
        //                },
        //            AiTypes = new List<AiType>
        //                {
        //                    new AiType { Id = 11, Name = "O1" },
        //                    new AiType { Id = 12, Name = "Fast" },
        //                    new AiType { Id = 13, Name = "Slow" },
        //                },
        //        },
        //        SettingsSite = new SettingsSite
        //        {
        //            Id = 1,
        //            SiteId = 34,
        //            IsGenerateAutomatically = 0,
        //            TitleUrlXPath = new TitleUrlXPath
        //            {
        //                JobTitleXPath = "",
        //                JobDescriptionXPath = "",
        //                RequiredSkillsXPath = "",
        //                WorkFormatXPath = "",
        //                LocationXPath = "",
        //                SalaryXPath = "",
        //                PublicationDateXPath = ""
        //            },
        //            Sites = new List<Site>
        //                {
        //                    new Site { Id = 31, Name = "https://cryptojobslist.com" },
        //                    new Site { Id = 32, Name = "https://www.cryptojobs.com/jobs" },
        //                    new Site { Id = 33, Name = "https://cryptocurrencyjobs.com" },
        //                }
        //        }
        //    };

        //    // Add the new site to the existing configuration
        //    //existingConfiguration.SettingsSite.Sites.Add(newSite);

        //    // In a real application, you would update the configuration in the database here

        //    return existingConfiguration;
        //}
        //public List<Parse> SendToDb(int id) => new List<Parse>
        //{
        //     new Parse
        //    {
        //        Id = 1,
        //        SiteName = "Site1",
        //        IsSendToDb = true,
        //        ParsedXPaths = JsonSerializer.Serialize(new { JobTitleXPath = "xpath1", JobDescriptionXPath = "xpath2" }),
        //        ParsedData = JsonSerializer.Serialize(new { JobTitle = "Title1", JobDescription = "Description1" })
        //    },
        //    new Parse
        //    {
        //        Id = 2,
        //        SiteName = "Site2",
        //        IsSendToDb = false,
        //        ParsedXPaths = JsonSerializer.Serialize(new { JobTitleXPath = "xpath3", JobDescriptionXPath = "xpath4" }),
        //        ParsedData = JsonSerializer.Serialize(new { JobTitle = "Title2", JobDescription = "Description2" })
        //    },
        //    new Parse
        //    {
        //        Id = 3,
        //        SiteName = "Site3",
        //        IsSendToDb = false,
        //        ParsedXPaths = JsonSerializer.Serialize(new { JobTitleXPath = "xpath5", JobDescriptionXPath = "xpath6" }),
        //        ParsedData = JsonSerializer.Serialize(new { JobTitle = "Title3", JobDescription = "Description3" })
        //    }
        //};
    }
}