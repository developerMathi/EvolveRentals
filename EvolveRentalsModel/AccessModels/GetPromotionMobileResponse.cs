﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    public class GetPromotionMobileResponse
    {
        public PromotionMobileResult PromResult { get; set; }
        public Promotion promotion { get; set; }
        public ApiMessage message { get; set; }
    }
}
