﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    public class DepositTotals
    {
        public decimal DepositAmount { set; get; }
        public decimal OnHoldAmount { set; get; }
        public decimal RefundAmount { set; get; }
        public decimal ReleaseAmount { set; get; }
        public decimal ForfeitureAmount { set; get; }
        public decimal TotalCanceledAmount { get; set; }
    }

}
