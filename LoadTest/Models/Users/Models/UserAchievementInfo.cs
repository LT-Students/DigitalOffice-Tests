using System;

namespace DigitalOffice.LoadTesting.Models.Users.Models
{
    public record UserAchievementInfo
    {
        public Guid Id { get; set; }
        public Guid AchievementId { get; set; }
        public string Name { get; set; }
        public DateTime ReceivedAt { get; set; }
        public ImageInfo Image { get; set; }
    }
}