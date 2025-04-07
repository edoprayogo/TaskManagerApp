using System;
using Domain.Constants.Enums;

namespace Domain.Entities;

public class Task
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
}
