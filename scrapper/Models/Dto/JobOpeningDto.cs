namespace scrapper.Models.Dto
{
    public class Project
    {
        public string? property { get; set; }
        public Value? value { get; set; }
    }


    public class City
    {
        public string? property { get; set; }
        public Value? value { get; set; }
    }

    public class Content
    {
        public string? property { get; set; }
        public List<string?>? value { get; set; }
    }

    public class Avatar
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class Cover
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }
    public class Description
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class EmploymentTypes
    {
        public string? property { get; set; }
        public List<Value?>? value { get; set; }
    }

    public class JobOpening
    {
        public int? localId;
        public string? externalId;
        public Name? name { get; set; }
        public Description? description { get; set; }
        public Types? types { get; set; }
        public Content? content { get; set; }
        public Roles? roles { get; set; }
        public Skills? skills { get; set; }
        public EmploymentTypes? employmentTypes { get; set; }
        public Project? project { get; set; }
        public SalaryMin? salaryMin { get; set; }
        public SalaryMax? salaryMax { get; set; }
        public PublishDate? publishDate { get; set; }
        public Location? location { get; set; }
        public RelatedSpaces? relatedSpaces { get; set; }
        public WebURL? webURL { get; set; }
    }

    public class Location
    {
        public string? property { get; set; }
        public Value? value { get; set; }
    }

    public class Name
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class PublishDate
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class Region
    {
        public string? property { get; set; }
        public Value? value { get; set; }
    }

    public class RelatedSpaces
    {
        public string? property { get; set; }
        public List<Value?>? value { get; set; }
    }

    public class Roles
    {
        public string? property { get; set; }
        public List<Value?>? value { get; set; }
    }

    public class JobOpeningDto
    {
        public JobOpening? jobOpening { get; set; }
    }

    public class SalaryMax
    {
        public string? property { get; set; }
        public int? value { get; set; }
        public string? currency { get; set; }
    }

    public class SalaryMin
    {
        public string? property { get; set; }
        public int? value { get; set; }
        public string? currency { get; set; }
    }

    public class Skills
    {
        public string? property { get; set; }
        public List<Value?>? value { get; set; }
    }

    public class Types
    {
        public string property { get; set; }
        public List<Value> value { get; set; }
    }

    public class Value
    {
        public string? property { get; set; }
        public Value? value { get; set; }
        public Name? name { get; set; }
        public Description? description { get; set; }
        public Region? region { get; set; }
        public City? city { get; set; }
        public Types? types { get; set; }
        public Avatar? avatar { get; set; }
        public Cover? cover { get; set; }
        public Website? website { get; set; }
        public X? x { get; set; }
    }

    public class Website
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class WebURL
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

    public class X
    {
        public string? property { get; set; }
        public string? value { get; set; }
    }

}
