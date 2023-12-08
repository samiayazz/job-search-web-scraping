using WebAPI.Domain.Enums;

namespace WebAPI.Domain.Entities
{
    public sealed class Job
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Platform { get; set; }
        public string Position { get; set; }
        public DateTime? PostedAt { get; set; }

        public Job() { }
        public Job(string title, string companyName, string location, string description, string platform)
            => (Title, CompanyName, Location, Description, Platform) = (title, companyName, location, description, platform);
        public Job(string title, string companyName, string location, string description, string platform, string position)
            => (Title, CompanyName, Location, Description, Platform, Position) = (title, companyName, location, description, platform, position);
        public Job(string title, string companyName, string location, string description, string platform, DateTime postedAt)
            => (Title, CompanyName, Location, Description, Platform, PostedAt) = (title, companyName, location, description, platform, postedAt);
        public Job(string title, string companyName, string location, string description, string platform, string position, DateTime postedAt)
            => (Title, CompanyName, Location, Description, Platform, Position, PostedAt) = (title, companyName, location, description, platform, position, postedAt);
    }
}
