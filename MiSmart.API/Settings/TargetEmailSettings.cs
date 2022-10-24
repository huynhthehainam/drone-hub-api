

using System;
using System.Collections.Generic;

namespace MiSmart.API.Settings;
public class UserEmail
{
    public String Email { get; set; }
    public String UUID { get; set; }
}

public class TargetEmailSettings
{

    public List<String> LowBattery { get; set; }
    public List<String> DailyReport { get; set; }
    public List<UserEmail> LogReport { get; set; }
    public List<String> ApprovedLogReport{get; set;}
}