using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerApplication.Models
   
  {
    // helps with storing the deatils of job so that it can be used to be distributed to a required peer
      public class Job
    {
        // id of job
         public string JobId { get; set; }
        
        // name of the fuc that required to be executed
        public string FunctionName { get; set; }
        // source code of function
          public string Code { get; set; }
        // hash that is used to verify the code of the job
        public string Hash { get; set; }
       
        
        
          // arg that are passed to the function when it is executed
        public List<object> Arguments { get; set; }
        // status of the job (available, claimed, completed)
        public string Status { get; set; }
        // ID of the peer that has claimed the job
        public string ClaimedBy { get; set; }
          // result after the job is executed
        public string Result { get; set; }
    }
}

