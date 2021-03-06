using DigitalOffice.LoadTesting.Models.Project.Enums;
using System;

namespace DigitalOffice.LoadTesting.Models.Project.Models.ProjectUser
{
    public class ProjectUserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public double Rate { get; set; }
        public int ProjectCount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public bool IsActive { get; set; }
        public ProjectUserRoleType Role { get; set; }
        public ImageInfo AvatarImage { get; set; }
        public DepartmentInfo Department { get; set; }
        public PositionInfo Position { get; set; }
    }
}
