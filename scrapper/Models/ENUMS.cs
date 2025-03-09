namespace scrapper.Models
{
    public enum PAGE
    {
        MAIN = 0,
        DETAILS = 1
    }
    public enum AI
    {
        GPT = 0,
        Claude = 1
    }
    public enum TaskStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }
    public enum XPACHTYPE
    {
        COMPANYLIST,
        COMPANYDETAILS,
        OPENJOBDETAILS
    }
}
