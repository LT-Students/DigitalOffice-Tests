﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DigitalOffice.LoadTesting.Models.Project.Enums
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum ProjectStatusType
  {
    Active,
    Closed,
    Suspend,
    DoesNotExist = 100
  }
}
