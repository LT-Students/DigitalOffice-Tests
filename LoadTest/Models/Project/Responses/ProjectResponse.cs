using DigitalOffice.LoadTesting.Models.Project.Models;
using DigitalOffice.LoadTesting.Models.Project.Models.ProjectUser;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class ProjectResponse
    {
        public ProjectInfo Project { get; set; }
        public IEnumerable<ProjectUserInfo> Users { get; set; }
        public IEnumerable<ProjectFileInfo> Files { get; set; }
        public IEnumerable<ImageInfo> Images { get; set; }
    }
}
