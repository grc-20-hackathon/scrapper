using System.Text.RegularExpressions;
using Anthropic.SDK;
using scrapper.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace scrapper.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private Kernel _kernel;
        private readonly ILogger<AIService> _logger;
        private OpenAIPromptExecutionSettings promptExecutionSettings;

        public AIService(IConfiguration configuration, ILogger<AIService> logger, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            //TODO Replace with value from the database
            string apiKey = configuration["AI:Claude:ApiKey"]
                ?? throw new InvalidOperationException("Claude API Key is not configured");
            string apiModel = configuration["AI:Claude:Model"]
                ?? throw new InvalidOperationException("Claude API Key is not configured");
            AI aiType = AI.Claude;
            InitAi(apiKey, apiModel, aiType);
        }

        public void InitAi(string apiKey, string apiModel, AI aI)
        {
#pragma warning disable SKEXP0010 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.
            promptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ModelId = apiModel,// AnthropicModels.Claude35Haiku
                MaxTokens = 1024,
                ResponseFormat = "json_object",
                Temperature = 0.1
            };
#pragma warning restore SKEXP0010 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.

            switch (aI)
            {
                case AI.Claude:
#pragma warning disable SKEXP0001 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.
                    var claudeService =
                     new ChatClientBuilder(new AnthropicClient(apiKey).Messages)
                    .UseFunctionInvocation()
                    .Build()
                    .AsChatCompletionService();
#pragma warning restore SKEXP0001 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.
                    var kernelBuilder = Kernel.CreateBuilder();
                    kernelBuilder.Services.AddSingleton<IChatCompletionService>(claudeService);
                    _kernel = kernelBuilder.Build();

                    break;
                case AI.GPT:
                    _kernel = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(modelId: apiKey, apiKey: apiModel)
                    .Build();
#pragma warning disable SKEXP0010 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.
                    promptExecutionSettings.ResponseFormat = typeof(SelectorModel);
#pragma warning restore SKEXP0010 // Type is intended for evaluation and may be changed or removed in future updates. To continue, suppress this diagnostic.
                    break;
            }
        }

        public async Task<SelectorModel> GetSelector(string url, PAGE page)
        {
            try
            {
                string html = await ReducingSiteVolume(url);
                var prompt = GetPromt(page, html);
                var response = await _kernel.InvokePromptAsync(prompt, new(promptExecutionSettings));
                if (response != null)
                {
                    var selector = ParserJsonSelector(response.ToString());
                    return selector;
                }
                else
                    throw new Exception("The response from AI was empty");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<OpenJobs> GetData(string text, PAGE page)
        {
            try
            {
                var prompt = GetPromt(page, text);
                var response = await _kernel.InvokePromptAsync(prompt, new(promptExecutionSettings));
                if (response != null)
                {
                    var data = ParserJsonData(response.ToString());
                    return data;
                }
                else
                    throw new Exception("The response from AI was empty");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        private static SelectorModel ParserJsonSelector(string jsonString)
        {
            try
            {
                var match = Regex.Match(jsonString, @"\{[^{}]*\}");
                return JsonConvert.DeserializeObject<SelectorModel>(match.Value);
            }
            catch
            {
                throw new Exception("The response from AI was incorrect JSON");
            }
        }
        private static OpenJobs ParserJsonData(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<OpenJobs>(jsonString);
            }
            catch(Exception ex)
            {
                throw new Exception("The response from AI was incorrect JSON");
            }
        }
        private string GetPromt(PAGE page, string html)
        {
            switch (page)
            {
                case PAGE.MAIN:
                    return $@"Task: Analyze the website's HTML structure and determine the optimal XPath selectors for extracting detailed job listing information.

                        Input Data:
                        - Site Structure: {html}
                        - Page Type: Job Vacancies Page
                        
                        Extraction Requirements:
                        1. The XPath selector for references to the job title. Output it according to JSON and don't write any comments.
                        
                        Analysis Instructions:
                        1. Examine the HTML structure of the job vacancies page
                        2. Identify the most precise and stable selectors for each job listing component
                        3. Prioritize semantic HTML and meaningful class/id attributes
                        
                        Selector Selection Criteria:
                        - Maximum specificity
                        - Resistance to site structure changes
                        - Preference for semantic tags and meaningful classes
                        - Avoid overly complex or dynamic selectors
                        
                        Response Format:
                        Provide a JSON object with the following structure:
                        {{
                          ""TitleUrlXPath"": ""XPath selector""
                        }}
                        
                        Limitations:
                        - Do not write additional comments
                        - Avoid selectors dependent on exact DOM structure
                        - Prefer stable, semantic selectors
                        
                        Provide the most accurate and stable selectors for each job listing component.";
                case PAGE.DETAILS:
                    //return $"I want to collect information about a job vacancy from the website. Please extract the following data from the page:{html}" + " and present it in JSON format:\r\n\r\n{\r\n  \"jobOpening\": {\r\n    \"name\": \"Job Title\",\r\n    \"description\": \"A brief description of the job, including main responsibilities and requirements.\",\r\n    \"jobDescription\": \"A detailed description of the job, including information about the technology, team, and project.\"\r\n  },\r\n  \"role\": {\r\n    \"name\": \"Role Title\",\r\n    \"description\": \"A description of the role, including key skills and experience required to perform the duties.\"\r\n  },\r\n  \"skills\": [\r\n    {\r\n      \"name\": \"Skill Name\",\r\n      \"description\": \"A description of the skill and its application in the context of the job.\"\r\n    },\r\n    {\r\n      \"name\": \"Skill Name\",\r\n      \"description\": \"A description of the skill and its application in the context of the job.\"\r\n    },\r\n    {\r\n      \"name\": \"Skill Name\",\r\n      \"description\": \"A description of the skill and its application in the context of the job.\"\r\n    }\r\n  ],\r\n  \"workFormats\": [\r\n    {\r\n      \"name\": \"Work Format\",\r\n      \"description\": \"A description of the work format, such as remote work or office work.\"\r\n    },\r\n    {\r\n      \"name\": \"Work Format\",\r\n      \"description\": \"A description of the work format, such as full-time employment.\"\r\n    }\r\n  ],\r\n  \"company\": {\r\n    \"name\": \"Company Name\",\r\n    \"Description\": \"A description of the company, including information about its activities and culture.\"\r\n  },\r\n  \"salary\": \"Salary level or compensation information.\",\r\n  \"publishDate\": \"Job posting date.\",\r\n  \"location\": \"Job location.\",\r\n  \"sourceURL\": \"URL of the job page.\"\r\n}";
                    return $@"I want to collect information about a job vacancy from the website. Please extract the following data from the text {html} and present it in JSON format:
                    {{
                      ""jobOpening"": {{
                        ""name"": ""Job Title"",
                        ""description"": ""Brief job description"",
                        ""jobDescription"": ""A detailed description of the assignment, including information about the technology, team, and project. So what's the same as in the original, no changes. replace html formatting with markdown.""
                      }},
                      ""role"": {{
                        ""name"": ""Role Title. Description of this role. Without specifying the grade(senior, middle, junior ...)"",
                        ""description"": ""A description of the role, including key skills and experience required to perform the duties.""
                      }},
                      ""skills"": [
                        {{
                          ""name"": ""Skill Name. The skill has a maximum of 2 keywords"",
                          ""description"": ""A description of the skill and its application in the context of the job.""
                        }},
                        {{
                          ""name"": ""Skill Name. The skill has a maximum of 2 keywords"",
                          ""description"": ""A description of the skill and its application in the context of the job.""
                        }},
                        {{
                          ""name"": ""Skill Name. The skill has a maximum of 2 keywords"",
                          ""description"": ""A description of the skill and its application in the context of the job.""
                        }}
                        ...
                      ],
                      ""workFormats"": [
                        {{
                          ""name"": ""Work Format. The Work Format has a maximum of 2 keywords"",
                          ""description"": ""A description of the work format, such as remote work or office work.""
                        }},
                        {{
                          ""name"": ""Work Format. The Work Format has a maximum of 2 keywords"",
                          ""description"": ""A description of the work format, such as full-time employment.""
                        }}
                        ...
                      ],
                      ""salary"": ""Salary level or compensation information. In the ""from-to"" format and the currency"",
                      ""publishDate"": ""Job posting date. In the ""2022-10-03T14:07:11.000Z"" format. If ""Posted: 1 day ago"" is specified, then calculate the date starting from the current date. It may be missing. Ñurrent date {DateTime.Now}"",
                      ""location"": ""Job location. In the ""Country/City"" format""
                      ""responsibilities"": ""Responsibilities for this vacancy. So what's the same as in the original, no changes. replace html formatting with markdown."",
                      ""requirements"": ""Requirements for the candidate. So what's the same as in the original, no changes. replace html formatting with markdown.""
                    }}
                    Limitations:
                        - Do not write additional comments
                    ";
                default:
                    return null;

            }
        }
        private async Task<string> ReducingSiteVolume(string url)
        {

            var html = await _httpClient.GetStringAsync(url);
            if (string.IsNullOrWhiteSpace(html))
                throw new Exception($"Error getting data from the site {url}");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            html = doc.DocumentNode.SelectSingleNode("/html/body").InnerHtml.Replace("/r", " ").Trim();


            return html;
        }
    }
}
