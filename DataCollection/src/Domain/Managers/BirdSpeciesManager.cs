using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Entity.Infrastructure;
using FlightNode.Common.Exceptions;
using System.Globalization;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface IBirdSpeciesDomainManager : ICrudManager<BirdSpecies>
    {
        IEnumerable<BirdSpecies> GetBirdSpeciesBySurveyTypeId(int surveyTypeId);
        void AddSpeciesToSurveyType(int speciesId, int surveyTypeId);
        void RemoveSpeciesFromSurveyType(int speciesId, int surveyTypeId);
    }

    public class BirdSpeciesDomainManager : DomainManagerBase<BirdSpecies>, IBirdSpeciesDomainManager
    {
        private const string SpeciesDoesNotExistPattern = "Species ID {0} not found.";
        private const string SurveyTypeDoesNotExistPattern = "Survey Type ID {0} not found.";

        /// <summary>
        /// Returns the persistence layer as the specific type instead of generic type
        /// </summary>
        private IBirdSpeciesPersistence BirdSpeciesPersistence
        {
            get
            {
                return _persistence as IBirdSpeciesPersistence;
            }
        }

        public BirdSpeciesDomainManager(IBirdSpeciesPersistence birdSpeciesPersistence) : base(birdSpeciesPersistence)
        {
        }

        public IEnumerable<BirdSpecies> GetBirdSpeciesBySurveyTypeId(int surveyTypeId)
        {
            var returnVal = BirdSpeciesPersistence.Collection
                                    .Where(birdItem => birdItem.SurveyTypes.Any(surveyItem => surveyItem.Id == surveyTypeId))
                                    .ToList();
            return returnVal;
        }

        public void AddSpeciesToSurveyType(int speciesId, int surveyTypeId)
        {
            try
            {
                var bird = RetrieveBirdSpeciesExpectedToExist(speciesId);
                var surveyType = RetrieveSurveyTypeExpectedToExist(surveyTypeId);

                AddBirdToSurveyType(bird, surveyType);
            }
            catch (DbUpdateException updateException)
            {
                if (DuplicateKeyExceptionOccurred(updateException))
                {
                    // Not worried about duplicate keys. All the user will care about is that the value has been inserted.
                    return;
                }

                throw;
            }
        }

        public void RemoveSpeciesFromSurveyType(int speciesId, int surveyTypeId)
        {
            var bird = RetrieveBirdSpeciesExpectedToExist(speciesId);

            // Force EF to query for surveys
            _persistence.Entry(bird)
                .Collection(nameof(BirdSpecies.SurveyTypes))
                .Load();

            var toRemove = bird.SurveyTypes
                               .FirstOrDefault(x => x.Id == surveyTypeId);

            if (toRemove == null)
            {
                // This bird is already not associated with this survey type. Don't worry about it
                return;
            }

            bird.SurveyTypes.Remove(toRemove);

            _persistence.SaveChanges();
        }

        private void AddBirdToSurveyType(BirdSpecies bird, SurveyType surveyType)
        {
            bird.SurveyTypes.Add(surveyType);
            BirdSpeciesPersistence.SaveChanges();
        }

        private SurveyType RetrieveSurveyTypeExpectedToExist(int surveyTypeId)
        {
            var surveyType = BirdSpeciesPersistence.SurveyTypes.FirstOrDefault(x => x.Id == surveyTypeId);
            if (surveyType == null)
            {
                throw new DoesNotExistException(string.Format(CultureInfo.InvariantCulture, SurveyTypeDoesNotExistPattern, surveyTypeId));
            }

            return surveyType;
        }

        private BirdSpecies RetrieveBirdSpeciesExpectedToExist(int speciesId)
        {
            var bird = FindById(speciesId);
            if (bird == null)
            {
                throw new DoesNotExistException(string.Format(CultureInfo.InvariantCulture, SpeciesDoesNotExistPattern, speciesId));
            }

            return bird;
        }

        private static bool DuplicateKeyExceptionOccurred(DbUpdateException updateException)
        {
            return updateException.InnerException != null &&
                                updateException.InnerException.InnerException != null &&
                                updateException.InnerException.InnerException.Message.Contains("Cannot insert duplicate key");
        }
    }
}
