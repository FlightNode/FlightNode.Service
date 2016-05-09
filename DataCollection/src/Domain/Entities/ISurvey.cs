using System;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Domain.Entities
{
    public interface ISurvey
    {

        Guid SurveyIdentifier { get; set; }

        int Id { get; set; }


        int LocationId { get; set; }

        DateTime? StartDate { get; set; }

        DateTime? EndDate { get; set; }

        int? AssessmentId { get; set; }

        int? VantagePointId { get; set; }

        int? AccessPointId { get; set; }

        string GeneralComments { get; set; }

        string DisturbanceComments { get; set; }


        int SurveyTypeId { get; set; }


        int SubmittedBy { get; set; }

        int? WeatherId { get; set; }

        int? StartTemperature { get; set; }

        int? EndTemperature { get; set; }

        int? WindSpeed { get; set; }

        int? TideId { get; set; }

        DateTime? TimeOfLowTide { get; set; }

        List<Observation> Observations { get; }

        List<Disturbance> Disturbances { get; }

        string Observers { get; set; }

        int Step { get;  }

        string LocationName { get; set; }

        int? WaterHeightId { get; set; }

        ISurvey Add(Observation item);

        ISurvey Add(Disturbance item);

    }
}
