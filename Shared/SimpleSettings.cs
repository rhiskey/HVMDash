using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;

namespace HVMDash.Shared
{
    public class SimpleSettings //:Configuration
    {
        public int Id { get; set; }
        public int HoursPeriod { get; set; }
        public int MinutesPeriod { get; set; }
    }
}
