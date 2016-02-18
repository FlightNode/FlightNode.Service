using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface IWaterbirdForagingManager
    {
        int Create(SurveyPending waterbirdForagingModel);
        Guid NewIdentifier();
        void Update(SurveyPending entity, int step);
    }

    /// <summary>
    /// Domain / business manager for waterbird foraging surveys.
    /// </summary>
    public class WaterbirdForagingManager : IWaterbirdForagingManager
    {
        private readonly ISurveyPersistence _persistence;

        /// <summary>
        /// Constructs a new instance of <see cref="WaterbirdForagingManager"/>
        /// </summary>
        /// <param name="persistence"></param>
        public WaterbirdForagingManager(ISurveyPersistence persistence)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException("persistence");
            }

            _persistence = persistence;
        }

        /// <summary>
        /// Creates a new foraging survey record.
        /// </summary>
        /// <param name="survey">Pending waterbird foraging survey.</param>
        /// <returns>Survey ID</returns>
        public int Create(SurveyPending survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            survey.Validate();


            // TODO: introduce transaction management

            LoadEntitiesIntoPersistenceLayer(survey);

            _persistence.SaveChanges();

            return survey.Id;
        }

        private void LoadEntitiesIntoPersistenceLayer(SurveyPending survey)
        {
            _persistence.SurveysPending.Add(survey);

            survey.Observations.ForEach(x => _persistence.Observations.Add(x));
            survey.Disturbances.ForEach(x => _persistence.Disturbances.Add(x));
        }

        /// <summary>
        /// Creates a new Guid
        /// </summary>
        /// <returns>Guid</returns>
        public Guid NewIdentifier()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Updates an existing foraging survey record
        /// </summary>
        /// <param name="survey">Update waterbird foraging survey</param>
        public void Update(SurveyPending survey, int step)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            survey.Validate();

            LoadModifiedEntitiesIntoPersistenceLayer(survey);

            if (step == 4)
            {
                SwitchToCompletedSurvey(survey);
            }

            _persistence.SaveChanges();
        }

        private void SwitchToCompletedSurvey(SurveyPending survey)
        {
            _persistence.SurveysPending.Remove(survey);

            var completed = survey.ToSurveyCompleted();

            _persistence.SurveysCompleted.Add(completed);
        }

        private void LoadModifiedEntitiesIntoPersistenceLayer(SurveyPending survey)
        {
            _persistence.SurveysPending.Add(survey);
            SetModifiedState(_persistence, survey);

            survey.Observations.ForEach(x =>
            {
                _persistence.Observations.Add(x);
                SetModifiedState(_persistence, x);
            });
            survey.Disturbances.ForEach(x =>
            {
                _persistence.Disturbances.Add(x);
                SetModifiedState(_persistence, x);
            });
        }

        public static Action<ISurveyPersistence, object> SetModifiedState = (ISurveyPersistence persistenceLayer, object input) =>
        {
            persistenceLayer.Entry(input).State = System.Data.Entity.EntityState.Modified;
        };
    }
}
