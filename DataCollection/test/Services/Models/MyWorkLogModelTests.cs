using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models;
using FlightNode.DataCollection.Services.Models.WorkLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Models
{
    public class MyWorkLogModelTests
    {
        private const int ID = 10;
        public const string WORK_MONTH = "January 2016";
        public const string WORK_DATE = "01/15/2016";
        public const decimal WORK_HOURS = 1.2m;
        public const decimal TRAVEL_HOURS = 0.1m;
        public const string LOCATION = "somewhere";
        public const string WORK_TYPE = "work type";

        public WorkLogReportRecord CreateInput()
        {
            return new WorkLogReportRecord
            {
                Id = ID,
                LocationName = LOCATION,
                TravelTimeHours = TRAVEL_HOURS,
                WorkDate = WORK_DATE,
                WorkHours = WORK_HOURS,
                WorkType = WORK_TYPE
            };
        }

        public MyWorkLogModel Create()
        {
            return MyWorkLogModel.CreateFrom(CreateInput());
        }

        [Fact]
        public void ConfirmCreateFromMapsId()
        {
            Assert.Equal(ID, Create().Id);
        }

        [Fact]
        public void ConfirmCreateFromMapsWorkMonth()
        {
            Assert.Equal(WORK_MONTH, Create().WorkMonth);
        }

        [Fact]
        public void ConfirmCreateFromMapsWorkDate()
        {
            Assert.Equal(WORK_DATE, Create().WorkDate);
        }

        [Fact]
        public void ConfirmCreateFromMapsWorkHours()
        {
            Assert.Equal(WORK_HOURS, Create().WorkHours);
        }

        [Fact]
        public void ConfirmCreateFromMapsTravelHours()
        {
            Assert.Equal(TRAVEL_HOURS, Create().TravelTimeHours);
        }

        [Fact]
        public void ConfirmCreateFromMapsLocation()
        {
            Assert.Equal(LOCATION, Create().LocationName);
        }

        [Fact]
        public void ConfirmCreateFromMapsWorkType()
        {
            Assert.Equal(WORK_TYPE, Create().WorkType);
        }
    }
}
