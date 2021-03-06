using DigitalOffice.LoadTesting.Models.Users.Enums;
using System;

namespace DigitalOffice.LoadTesting.Models.Users.Requests.User
{
    public class EditUserRequest
    {
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid OfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string About { get; set; }
        public UserGender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string City { get; set; }
        public AddImageRequest AvatarImage { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? StartWorkingAt { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
    }
}