using System;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Services.Models.Survey
{
    /// <summary>
    /// Use for displaying a compact list of waterbird foraging survey results.
    /// </summary>
    public class WaterbirdForagingListItem
    {
        public string StartDate { get; set; }

        public string Location { get; set; }

        public Guid SurveyIdentifier { get; set; }

        public string SurveyComments { get; set; }

        public string Status { get; set; }
    }
}
