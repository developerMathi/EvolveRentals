using EvolveRentalsModel.AccessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    public class SearchAllCustomerResponse
    {
        public List<CustomerSeachResult> serachResult { get; set; }
        public ApiMessage message { get; set; }
    }
}
