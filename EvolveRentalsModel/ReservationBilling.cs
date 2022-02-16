﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    [Serializable]
    public class ReservationBilling
    {
        public ReservationBilling()
        { }
        [Key]
        public int BillingInfoID { get; set; }
        public int ReservationID { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public bool IsActive { get; set; }
        public string BillingBy { get; set; }
    }
}
