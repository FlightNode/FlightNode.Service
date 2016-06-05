using System;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Services.Models.Rookery
{
    public class WaterbirdForagingModel 
    {

        public Guid? SurveyIdentifier { get; set; }

        public int Step { get; set; }

        public int LocationId { get; set; }

        public string StartDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int? SiteTypeId { get; set; }

        public int? TideId { get; set; }

        public int? WaterHeightId { get; set; }

        public int? WeatherId { get; set; }

        public int? Temperature { get; set; }

        public int? WindSpeed { get; set; }

        public string Observers { get; set; }

        public int? VantagePointId { get; set; }

        public int? AccessPointId { get; set; }

        public string SurveyComments { get; set; }

        public string DisturbanceComments { get; set; }

        public List<DisturbanceModel> Disturbances { get; set; }

        public List<ObservationModel> Observations { get; set; }

        public int SurveyId { get; set; }

        
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
