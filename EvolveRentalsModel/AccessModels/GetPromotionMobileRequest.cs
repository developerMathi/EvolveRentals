﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    public class GetPromotionMobileRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string PromotionCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int VehicleTypeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LocationId { get; set; }

    }
}
