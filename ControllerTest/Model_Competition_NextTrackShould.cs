using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class ModelCompetitionNextTrackShould
    {
        private Competition competition;

        public ModelCompetitionNextTrackShould()
        {
           
        }

        [SetUp]
        public void SetUp() {
            competition = new Competition();
        }

        [Test]
        public void NextTrackEmptyQueueReturnNull(){
            var result = competition.NextTrack();
            Assert.IsNull(result);
        }
    }
}
