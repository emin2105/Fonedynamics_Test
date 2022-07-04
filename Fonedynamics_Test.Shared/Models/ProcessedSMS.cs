using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fonedynamics_Test.Shared.Models
{
    public class ProcessedSMS
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
    }
}
