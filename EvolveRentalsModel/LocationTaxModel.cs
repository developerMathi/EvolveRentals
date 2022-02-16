﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    [Serializable]
    public class LocationTaxModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }

        public string DisplayValue
        {
            get
            {
                return Value.ToString("0.00");
            }
        }
        public decimal TaxValue { get; set; }
        public string LocationName { get; set; }
        public int LocationTaxID { get; set; }
        public int LocationId { get; set; }
        public int TaxId { get; set; }
        public bool IsOption { get; set; }

        public bool IsSelected { get; set; }
        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }
        public string LastUpdatedByName { get; set; }
        public bool isMantatory
        {
            get
            {
                return !IsOption;
            }
        }
    }

    public class LocationTaxViewModel
    {
        public List<LocationTaxModel> List { get; set; }
        public ApiMessage message { get; set; }
        public Action Action { get; set; }
    }
}
