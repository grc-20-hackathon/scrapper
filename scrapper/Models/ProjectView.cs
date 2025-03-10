namespace scrapper.Models
{
    public class ProjectView
    {
        public int Id { get; set; }
        public Models.TaskStatus Status { get; set; }
        public string ProjectSourse { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public Uri ProjectWebsite { get; set; }
        public string ProjectX { get; set; }
        public string ProjectAvatar { get; set; }
        public string ProjectCover { get; set; }
        public List<OpenJobs> ListOpenJobsInProject { get; set; }
    }
    public class OpenJobs
    {
        public int? Id { get; set; }
        public string jobSourse { get; set; }
        public JobOpening jobOpening { get; set; }
        public Role role { get; set; }
        public List<Skill> skills { get; set; }
        public List<WorkFormat> workFormats { get; set; }
        public string salary { get; set; }
        public DateTime publishDate { get; set; }
        public string location { get; set; }
        public string responsibilities { get; set; }
        public string requirements { get; set; }
    }
    public class JobOpening
    {
        public int? Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string jobDescription { get; set; }
    }

    public class Role
    {
        public int? Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
    public class Skill
    {
        public int? Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class WorkFormat
    {
        public int? Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
