﻿using System;

namespace DigitalOffice.LoadTesting.Models.Time.Requests
{
  public record CreateWorkTimeDayJobRequest
  {
    public Guid WorkTimeId { get; set; }
    public int Day { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Minutes { get; set; }
  }
}
