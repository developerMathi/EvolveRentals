using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    [Serializable]
    public class ApiMessage
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }
    }
}
