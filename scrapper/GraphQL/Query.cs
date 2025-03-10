
using scrapper.Models;
using Newtonsoft.Json;
using scrapper.Services;
using scrapper.Models.Dto;
using Microsoft.EntityFrameworkCore;


namespace scrapper.GraphQL
{
    public class Query
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Query> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public Query( [Service] HttpClient httpClient, ILogger<Query> logger, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;

        }

        public async Task<ICollection<JobOpeningDto>> GetDtosAsync()
        {
            var result = new List<JobOpeningDto>();
            var projectInList = _dbContext.projectView.
                         Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.skills).
                         Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.workFormats).
                         Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.jobOpening).
                         Include(x => x.ListOpenJobsInProject).ThenInclude(x => x.role).AsEnumerable();
                        
            foreach (var project in projectInList)
            {
                var data = project.ListOpenJobsInProject.Select(openJob =>
                        new JobOpeningDto
                        {
                            jobOpening = new Models.Dto.JobOpening
                            {
                                name = new Name
                                {
                                    property = "Name",
                                    value = openJob.jobOpening.name
                                },
                                description = new Description
                                {
                                    property = "Description",
                                    value = openJob.jobOpening.description
                                },
                                content = new Content
                                {
                                    property = "Content",
                                    value = GetStringContent(openJob.jobOpening.jobDescription, openJob.responsibilities, openJob.requirements)
                                },
                                roles = new Roles
                                {
                                    property = "Roles",
                                    value = new List<Value>
                                {
                                new Value
                                {
                                    property = "Role",
                                    value = new Value{
                                        name = new Name{
                                            property = "Name",
                                            value = openJob.role.name
                                        },
                                        description = new Description
                                        {
                                            property = "Description",
                                            value = openJob.role.description
                                        }
                                    }
                                }
                                }
                                },
                                skills = new Skills
                                {
                                    property = "Skills",
                                    value = openJob.skills.Select(skill =>
                                        new Value
                                        {
                                            property = "Skill",
                                            value = new Value
                                            {
                                                name = new Name
                                                {
                                                    property = "Name",
                                                    value = skill.name
                                                },
                                                description = new Description
                                                {
                                                    property = "Description",
                                                    value = skill.description
                                                }
                                            }
                                        }
                                ).ToList()
                                },
                                employmentTypes = new EmploymentTypes
                                {
                                    property = "Employment Types",
                                    value = openJob.workFormats.Select(workFormat => new Value
                                    {
                                        property = "Employment Type",
                                        value = new Value
                                        {
                                            name = new Name
                                            {
                                                property = "Name",
                                                value = workFormat.name
                                            },
                                            description = new Description
                                            {
                                                property = "Description",
                                                value = workFormat.description
                                            }
                                        }
                                    }).ToList()
                                },
                                salaryMax = GetSalaryMax(openJob.salary),
                                salaryMin = GetSalaryMin(openJob.salary),
                                publishDate = new PublishDate
                                {
                                    property = "Publish Date",
                                    value = openJob.publishDate.ToString(),
                                },
                                location = GetLocation(openJob.location),
                                webURL = new WebURL
                                {
                                    property = "Web URL",
                                    value = openJob.jobSourse
                                },
                                project = new Models.Dto.Project
                                {
                                    property = "Project",
                                    value = new Value
                                    {
                                        name = new Name
                                        {
                                            property = "Name",
                                            value = project.ProjectName,
                                        },
                                        description = new Description
                                        {
                                            property = "Description",
                                            value = project.ProjectDescription
                                        },
                                        website = new Website
                                        {
                                            property = "Web Site",
                                            value = project.ProjectWebsite.AbsoluteUri
                                        },
                                        x = new X
                                        {
                                            property = "X",
                                            value = project.ProjectX
                                        },
                                        avatar = new Avatar
                                        {
                                            property = "Avatar",
                                            value = project.ProjectAvatar
                                        },
                                        cover = new Cover
                                        {
                                            property = "Cover",
                                            value = project.ProjectCover
                                        },


                                    }
                                },

                            }
                        }
                ).ToList();
                result.AddRange(data);
            }
            return result;
        }

        private List<string> GetStringContent(string jobDescription, string responsibilities, string requirements)
        {
            List<string> result = new List<string>();
            var jobDescriptionArray = jobDescription.Split('\n');
            var responsibilitiesArray = responsibilities.Split('\n');
            var requirementsArray = requirements.Split('\n');
            result.AddRange(jobDescriptionArray);
            result.Add("### Candidate Profile");
            result.AddRange(responsibilitiesArray);
            result.Add("### Compensation & Perks");
            result.AddRange(requirementsArray);
            return result;
        }

        private SalaryMin GetSalaryMin(string salary)
        {
            try
            {
                var data = salary.Split(" ");
                if (data.Length == 2)
                {
                    var currency = data[0];
                    data = data[1].Split("/");
                    if (data.Length == 2)
                    {
                        return new SalaryMin()
                        {
                            property = "Salary Min",
                            value = int.Parse(data[0].Split("-")[0]),
                            currency = currency
                        };
                    }
                }
                return new SalaryMin()
                {
                    property = "Salary Min",
                    value = null,
                    currency = null
                };
            }
            catch
            {
                return new SalaryMin()
                {
                    property = "Salary Min",
                    value = null,
                    currency = null
                };

            }
        }

        private SalaryMax GetSalaryMax(string salary)
        {
            try
            {
                var data = salary.Split(" ");
                if (data.Length == 2)
                {
                    var currency = data[0];
                    data = data[1].Split("/");
                    if (data.Length == 2)
                    {
                        return new SalaryMax()
                        {
                            property = "Salary Max",
                            value = int.Parse(data[0].Split("-")[1]),
                            currency = currency
                        };
                    }
                }
                return new SalaryMax()
                {
                    property = "Salary Max",
                    value = null,
                    currency = null
                };
            }
            catch
            {
                return new SalaryMax()
                {
                    property = "Salary Max",
                    value = null,
                    currency = null
                };
            }
            
        }
        private Models.Dto.Location GetLocation(string location) { 
            var data = location.Split("/");
            if (data.Length == 2) {
                return new Models.Dto.Location()
                {
                    property = "Location",
                    value = new Value
                    {
                        region = new Region
                        {
                            property = "Region",
                            value = new Value
                            {
                                name = new Name
                                {
                                    property = "Name",
                                    value = data[0],
                                },
                            }
                        },
                        city = new City
                        {
                            property = "City",
                            value = new Value
                            {
                                name = new Name
                                {
                                    property = "Name",
                                    value = data[1]
                                },
                            }
                        }
                    }
                };
            }
            return null;

        }

    }
}