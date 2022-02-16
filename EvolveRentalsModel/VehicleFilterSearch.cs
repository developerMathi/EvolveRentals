﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel
{
    public class VehicleFilterSearch
    {
        public List<string> vehicleTypes { get; set; }
        public List<string> vehicleMakes { get; set; }
        public List<string> vehicleModels { get; set; }
        public List<int> VehicleYear { get; set; }
        public List<int> NumberOfseats { get; set; }
        public List<string> Colors { get; set; }
    }
}
