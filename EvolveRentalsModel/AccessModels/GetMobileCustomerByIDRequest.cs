using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    [Serializable]
    public class GetMobileCustomerByIDRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int CustomerId { get; set; }
    }
}
