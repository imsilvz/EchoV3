using EchoV3.Models.FFXIV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.Echo
{
    public class JobSetting
    {
        public required Common.ClassJob JobId { get; set; }
        public string? JobColor { get; set; }
    }
    public class EchoSettings
    {
        public required Dictionary<string, JobSetting> JobSettings { get; set; }
    }
}
