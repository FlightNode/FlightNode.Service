using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface IWaterbirdForagingManager
    {
        SurveyPending Create(SurveyPending waterbirdForagingModel);
        Guid NewIdentifier();
        void Update(SurveyPending entity, int step);
        ISurvey FindBySurveyId(Guid guid);
        IList<ISurvey> FindBySubmitterId(int userId);
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
        /// Looks up either a pending or completed survey by its unique identifier.
        /// </summary>
        /// <param name="surveyIdentifier">Survey's unique identifier</param>
        /// <returns>
        /// Either <see cref="SurveyCompleted"/> or <see cref="SurveyPending"/> as an <see cref="ISurvey"/>.
        /// </returns>
        public ISurvey FindBySurveyId(Guid surveyIdentifier)
        {
            var pending = _persistence.SurveysPending
                                        .FirstOrDefault(x => x.SurveyIdentifier == surveyIdentifier);

            if (pending != null)
            {
                return pending;
            }

            return _persistence.SurveysCompleted
                            .FirstOrDefault(x => x.SurveyIdentifier == surveyIdentifier);
        }

        /// <summary>
        /// Looks up all waterbird survey results submitted by a particular user, whether pending or complete.
        /// </summary>
        /// <param name="userId">
        /// The ID for the user who submitted the surveys.
        /// </param>
        /// <returns>
        /// List of both <see cref="SurveyCompleted"/> and <see cref="SurveyPending"/>.
        /// </returns>
        public IList<ISurvey> FindBySubmitterId(int userId)
        {
            var list = new List<ISurvey>();

            var pending = FindPendingSurveysSubmittedBy(userId);
            list.AddRange(pending);

            var completed = FindCompletedSurveysSubmittedBy(userId);
            list.AddRange(completed);

            return LoadLocationNames(list);
        }

        private IList<ISurvey> LoadLocationNames(List<ISurvey> list)
        {
            list.ForEach(x =>
            {
                // due to a foreign key relationship, this should never *not* find a result
                x.LocationName = _persistence.Locations
                                    .First(y => y.Id == x.LocationId)
                                    .SiteName;
            });

            return list;
        }

        private List<SurveyPending> FindPendingSurveysSubmittedBy(int userId)
        {
            return _persistence.SurveysPending
                                                    .Where(x => x.SubmittedBy == userId)
                                                    .ToList();
        }

        private List<SurveyCompleted> FindCompletedSurveysSubmittedBy(int userId)
        {
            return _persistence.SurveysCompleted
                                        .Where(x => x.SubmittedBy == userId)
                                        .ToList();
        }

        /// <summary>
        /// Creates a new foraging survey record.
        /// </summary>
        /// <param name="survey">Pending waterbird foraging survey.</param>
        /// <returns>Survey ID</returns>
        public SurveyPending Create(SurveyPending survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            survey.Validate();


            // TODO: introduce transaction management

            LoadEntitiesIntoPersistenceLayer(survey);

            _persistence.SaveChanges();

            return survey;
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
            var completed = survey.ToSurveyCompleted();
            _persistence.SurveysCompleted.Add(completed);

            _persistence.SurveysPending.Remove(survey);
            _persistence.SaveChanges();
        }

        private void LoadModifiedEntitiesIntoPersistenceLayer(SurveyPending survey)
        {
            _persistence.SurveysPending.Add(survey);
            SetModifiedState(_persistence, survey);
            _persistence.SaveChanges();

            survey.Observations.ForEach(x =>
            {
                _persistence.Observations.Add(x);
                SetModifiedState(_persistence, x);
                _persistence.SaveChanges();
            });
            survey.Disturbances.ForEach(x =>
            {
                _persistence.Disturbances.Add(x);
                SetModifiedState(_persistence, x);
                _persistence.SaveChanges();
            });
        }

        public static Action<ISurveyPersistence, object> SetModifiedState = (ISurveyPersistence persistenceLayer, object input) =>
        {
            persistenceLayer.Entry(input).State = System.Data.Entity.EntityState.Modified;
        };
    }
}
