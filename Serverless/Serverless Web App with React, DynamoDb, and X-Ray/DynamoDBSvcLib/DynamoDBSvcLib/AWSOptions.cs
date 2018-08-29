using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSAppService
{
    public class AWSOptions
    {
        public AWSOptions() { }

        public string CognitoPoolId { get; set; }
        public string Region { get; set; }
    }
}
