using System;
using System.Collections.Generic;
using System.Text;

namespace Week1.Core
{
    public record Reading(
        string station, 
        string day, 
        float temperature, 
        float rainfall_mm
        );
}
