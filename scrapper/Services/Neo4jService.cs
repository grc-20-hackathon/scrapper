using System.ComponentModel.Design;
using scrapper.Models;
using Neo4j.Driver;

namespace scrapper.Services
{
    public class Neo4jService : INeo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService(IDriver driver)
        {
            _driver = driver;
        }
        //public async Task SaveSelectorModel(string id, SelectorModel selectorModel)
        //{
        //    using (var session = _driver.AsyncSession())
        //    {
        //        await session.RunAsync("CREATE (s:SelectorModel {id: $id, TitleUrlXPath: $TitleUrlXPath})",
        //            new
        //            {
        //                id = id, // Using the passed id
        //                TitleUrlXPath = selectorModel.TitleUrlXPath
        //            });
        //    }
        //}
        //public async Task<SelectorModel?> GetSelectorModel(string id)
        //{
        //    try
        //    {
        //        using (var session = _driver.AsyncSession())
        //        {
        //            var result = await session.RunAsync("MATCH (s:SelectorModel) WHERE s.id = $id RETURN s",
        //                new { id });

        //            var record = await result.SingleAsync();
        //            if (record == null)
        //            {
        //                return null; // If the node is not found, return null
        //            }

        //            // Extracting data from the record
        //            var selectorNode = record["s"].As<Neo4j.Driver.INode>();
        //            var selectorModel = new SelectorModel
        //            {
        //                TitleUrlXPath = selectorNode.Properties["TitleUrlXPath"].As<string>()
        //            };

        //            return selectorModel;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}
        //public async Task<List<SelectorModel>> GetAllSelectorModels()
        //{
        //    var selectorModels = new List<SelectorModel>();
        //    using (var session = _driver.AsyncSession())
        //    {
        //        var result = await session.RunAsync("MATCH (s:SelectorModel) RETURN s");
        //        while (await result.FetchAsync())
        //        {
        //            var selectorNode = result.Current["s"].As<Neo4j.Driver.INode>();
        //            var selectorModel = new SelectorModel
        //            {
        //                TitleUrlXPath = selectorNode.Properties["TitleUrlXPath"].As<string>()
        //            };
        //            selectorModels.Add(selectorModel);
        //        }
        //    }
        //    return selectorModels;
        //}
    }
}
