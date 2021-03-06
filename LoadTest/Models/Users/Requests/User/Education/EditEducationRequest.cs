using DigitalOffice.LoadTesting.Models.Users.Enums;
using System;

namespace DigitalOffice.LoadTesting.Models.Users.Requests.User.Education
{
    public class EditEducationRequest
    {
        public string UniversityName { get; set; }
        public string QualificationName { get; set; }
        public FormEducation FormEducation { get; set; }
        public DateTime AdmissionAt { get; set; }
        public DateTime? IssueAt { get; set; }
        public bool IsActive { get; set; }
    }
}
