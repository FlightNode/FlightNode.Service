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
        void Update(SurveyPending entity);
        ISurvey FindBySurveyId(Guid guid);
        IList<ISurvey> FindBySubmitterId(int userId);
        void Finish(SurveyPending survey);
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

            SaveChanges();

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
        public void Update(SurveyPending survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            survey.Validate();

            LoadPendingSurveyIntoPersistenceLayer(survey);
            LoadObservationsIntoPersistenceLayer(survey);
            LoadDisturbancesIntoPersistenceLayer(survey);

            SaveChanges();
        }

        /// <summary>
        /// Updates an existing survey and converts it to a completed survey.
        /// </summary>
        /// <param name="survey">Update waterbird foraging survey</param>
        public void Finish(SurveyPending survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            survey.Validate();

            LoadObservationsIntoPersistenceLayer(survey);
            LoadDisturbancesIntoPersistenceLayer(survey);
            SwitchToCompletedSurvey(survey);
            RemovePendingSurvey(survey);

            SaveChanges();
        }

        private void SaveChanges()
        {
            _persistence.SaveChanges();
        }


        private void SwitchToCompletedSurvey(SurveyPending survey)
        {
            var completed = survey.ToSurveyCompleted();
            _persistence.SurveysCompleted.Add(completed);

        }

        private void RemovePendingSurvey(SurveyPending survey)
        {
            LoadPendingSurveyIntoPersistenceLayer(survey);
//            _persistence.SetModifiedStateOn(survey);
            _persistence.SurveysPending.Remove(survey);
        }

        private void LoadPendingSurveyIntoPersistenceLayer(SurveyPending survey)
        {
            survey.Id = _persistence.SurveysPending
                            .Where(x => x.SurveyIdentifier == survey.SurveyIdentifier)
                            .Select(x => x.Id)
                            .FirstOrDefault();
            if (survey.Id == 0)
            {
                throw new InvalidOperationException("No pending survey exists for identifer " + survey.SurveyIdentifier);
            }

            _persistence.SurveysPending.Add(survey);
            _persistence.SetModifiedStateOn(survey);

        }

        private void LoadObservationsIntoPersistenceLayer(SurveyPending survey)
        {
            survey.Observations.ForEach(x =>
            {

                _persistence.Observations.Add(x);
                //Set the state to Modified only if the object is already created.
                if (x.Id > 0)
                {
                    _persistence.SetModifiedStateOn(x);
                }
            });
        }

        private void LoadDisturbancesIntoPersistenceLayer(SurveyPending survey)
        {
            survey.Disturbances.ForEach(x =>
            {
                _persistence.Disturbances.Add(x);
                //Set the state to Modified only if the object is already created.
                if (x.Id > 0)
                {
                    _persistence.SetModifiedStateOn(x);
                }
            });
        }

    }
}
