﻿using DigitalOffice.LoadTesting.Models.Time.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.LoadTesting.Models.Time.Models
{
  public record UserStatInfo
  {
    public UserInfo User { get; set; }
    public PositionInfo Position { get; set; }
    public CompanyUserInfo CompanyUser { get; set; }
    public DepartmentInfo Department { get; set; }
    public List<LeaveTimeInfo> LeaveTimes { get; set; }
    public List<WorkTimeInfo> WorkTimes { get; set; }
    public WorkTimeMonthLimitInfo LimitInfo { get; set; }
  }
}
