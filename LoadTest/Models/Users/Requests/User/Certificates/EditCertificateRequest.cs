using DigitalOffice.LoadTesting.Models.Users.Enums;
using System;

namespace DigitalOffice.LoadTesting.Models.Users.Requests.User.Certificates
{
    public record EditCertificateRequest
    {
        public Guid UserId { get; set; }
        public AddImageRequest Image { get; set; }
        public EducationType EducationType { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public DateTime ReceivedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
