﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    [Serializable]
    public class GetChecklistMobileResponse
    {

        public List<DefaultDamageList> DefaultDamageChecklistIn { get; set; }

        public List<DefaultDamageList> DefaultDamageChecklistout { get; set; }


        public ApiMessage message { get; set; }
    }
}
