using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Services.Models.Rookery
{
    public class RookeryCensusModel
    {
        public Guid SurveyIdentifer { get; set; }

        public int Step { get; set; }

        public int LocationId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SiteTypeId { get; set; }

        public int TideInfoId { get; set; }

        public int WeatherInfoId { get; set; }

        public List<int> Observers { get; private set; }

        public int VantagePointInfoId { get; set; }

        public int AccessPointInfoId { get; set; }

        public string SurveyComments { get; set; }

        public string DisturbanceComments { get; set; }

        public List<DisturbanceModel> Disturbances { get; set; }

        public List<ObservationModel> Observations { get; set; }

        public RookeryCensusModel()
        {
            SurveyIdentifer = Guid.Empty;
            Observers = new List<int>();
            Observations = new List<ObservationModel>();
            Disturbances = new List<DisturbanceModel>();
        }
    }
}
