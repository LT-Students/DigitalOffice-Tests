using System;

namespace DigitalOffice.LoadTesting.Models.Time.Requests
{
    public record EditWorkTimeRequest
    {
        public float? UserHours { get; set; }
        public float? ManagerHours { get; set; }
        public string Description { get; set; }
    }
}
