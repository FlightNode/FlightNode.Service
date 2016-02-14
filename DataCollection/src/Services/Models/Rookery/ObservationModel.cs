using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Services.Models.Rookery
{
    public class ObservationModel
    {
        public int BirdSpeciesId { get; set; }

        public int Adults { get; set; }

        public int Juveniles { get; set; }

        public int PrimaryActivityId { get; set; }

        public int SecondaryActivityId { get; set; }

        public int HabitatId { get; set; }

        public int FeedingId { get; set; }
    }
}
