using System;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Services.Models.Rookery
{
    public class WaterbirdForagingModel 
    {

        public Guid? SurveyIdentifier { get; set; }

        public int Step { get; set; }

        public int LocationId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? SiteTypeId { get; set; }

        public int? TideInfoId { get; set; }

        public int? WaterHeightId { get; set; }

        public int? WeatherInfoId { get; set; }

        public int? Temperature { get; set; }

        public int? WindSpeed { get; set; }

        public string Observers { get; set; }

        public int? VantagePointInfoId { get; set; }

        public int? AccessPointInfoId { get; set; }

        public string SurveyComments { get; set; }

        public string DisturbanceComments { get; set; }

        public List<DisturbanceModel> Disturbances { get; private set; }

        public List<ObservationModel> Observations { get; private set; }

        public int SurveyId { get; set; }

        public DateTime? TimeOfLowTide { get; set; }

        public WaterbirdForagingModel()
        {
            SurveyIdentifier = Guid.Empty;
            Observations = new List<ObservationModel>();
            Disturbances = new List<DisturbanceModel>();
        }

        public WaterbirdForagingModel Add(ObservationModel observation)
        {
            Observations.Add(observation);
            return this;
        }

        public WaterbirdForagingModel Add(DisturbanceModel disturbanceModel)
        {
            Disturbances.Add(disturbanceModel);
            return this;
        }
    }
}
