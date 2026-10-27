using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerApplication.Models
{
    public class Job
    {
        public string JobId { get; set; }
        public string FunctionName { get; set; }
        public string Code { get; set; }
        public string Hash { get; set; }
        public List<object> Arguments { get; set; }
        public string Status { get; set; }

        public string ClaimedBy { get; set; }
        public string Result { get; set; }
    }
}

