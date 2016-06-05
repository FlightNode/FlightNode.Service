using System;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyPending : SurveyBase, ISurvey
    {
        public SurveyPending Add(Observation item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Observations.Add(item);
            return this;
        }

        public SurveyPending Add(Disturbance item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Disturbances.Add(item);
            return this;
        }

        public SurveyCompleted ToSurveyCompleted()
        {
            var completed = new SurveyCompleted
            {
                AccessPointId = this.AccessPointId,
                AssessmentId = this.AssessmentId,
                DisturbanceComments = this.DisturbanceComments,
                EndDate = this.EndDate.Value,
                EndTemperature = this.EndTemperature,
                GeneralComments = this.GeneralComments,
                LocationId = this.LocationId,
                StartDate = this.StartDate.Value,
                StartTemperature = this.StartTemperature,
                SubmittedBy = this.SubmittedBy,
                SurveyIdentifier = this.SurveyIdentifier,
                SurveyTypeId = this.SurveyTypeId,
                TideId = this.TideId,
                TimeOfLowTide = this.TimeOfLowTide,
                VantagePointId = this.VantagePointId,
                WeatherId = this.WeatherId,
                WindSpeed = this.WindSpeed,
                WaterHeightId = this.WaterHeightId
            };

            completed.Observations.AddRange(this.Observations);
            completed.Disturbances.AddRange(this.Disturbances);
            completed.Observers = this.Observers;

            return completed;
        }


        ISurvey ISurvey.Add(Observation item)
        {
            return Add(item);
        }

        ISurvey ISurvey.Add(Disturbance item)
        {
            return Add(item);
        }
    }
}
