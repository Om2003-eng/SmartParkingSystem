using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class AdminReport
    {
        public int ReportId { get; set; }
        public int AdminId { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedAt { get; set; }

        public User Admin { get; set; }
    }

}
