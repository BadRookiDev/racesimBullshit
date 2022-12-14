using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {
        private Competition _competition;

        public Model_Competition_NextTrackShould()
        {
           
        }

        [SetUp]
        public void SetUp() {
            _competition = new Competition();
        }

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull(){
            var result = _competition.NextTrack();
            Assert.IsNull(result);
        }
    }
}
