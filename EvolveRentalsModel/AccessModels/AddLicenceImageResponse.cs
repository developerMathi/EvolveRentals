﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    public class AddLicenceImageResponse
    {
        public int FrontImageid { get; set; }
        public int BackImageid { get; set; }
        public ApiMessage message { get; set; }
    }
}
