using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolveRentalsModel.AccessModels
{
    [Serializable]
    public class CreateReservationMobileRequest
    {
        public ReservationView reversationData { get; set; }
    }
}
