﻿using LT.DigitalOffice.LoadTesting.Models.Users.Models;
using System;
using System.Collections.Generic;

namespace DigitalOffice.LoadTesting.Models.Users.Models
{
  public record EducationInfo
  {
    public Guid Id { get; set; }
    public string UniversityName { get; set; }
    public string QualificationName { get; set; }
    public EducationFormInfo EducationForm { get; set; }
    public EducationTypeInfo EducationType { get; set; }
    public string Completeness { get; set; }
    public DateTime AdmissionAt { get; set; }
    public DateTime? IssueAt { get; set; }
    public List<Guid> ImagesIds { get; set; }
  }
}
