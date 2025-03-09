namespace scrapper.Models
{
    public class ProjectView
    {
        public int Id { get; set; }
        public Models.TaskStatus Status { get; set; }
        public string ProjectSourse { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectWebsite { get; set; }
        public string ProjectX { get; set; }
        public string ProjectAvatar { get; set; }
        public string ProjectCover { get; set; }
        public List<OpenJobs> ListOpenJobsInProject { get; set; }
    }
    public class OpenJobs
    {
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
        public string name { get; set; }
        public string description { get; set; }
        public string jobDescription { get; set; }
    }

    public class Role
    {
        public string name { get; set; }
        public string description { get; set; }
    }
    public class Skill
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class WorkFormat
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
