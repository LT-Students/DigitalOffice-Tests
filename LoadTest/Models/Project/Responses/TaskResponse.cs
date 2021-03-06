using System;
using System.Collections.Generic;
using DigitalOffice.LoadTesting.Models.Project.Models;

namespace DigitalOffice.LoadTesting.Models.Project.Responses
{
    public record TaskResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? PlannedMinutes { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public ProjectInfo Project { get; set; }
        public UserTaskInfo CreatedBy { get; set; }
        public UserTaskInfo AssignedUser { get; set; }
        public TaskPropertyInfo Status { get; set; }
        public TaskPropertyInfo Priority { get; set; }
        public TaskPropertyInfo Type { get; set; }
        public TaskInfo ParentTask { get; set; }

        public IEnumerable<TaskInfo> Subtasks { get; set; }
    }
}