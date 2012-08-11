using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sematena.AudioVideoLib;

namespace AudioVideoLib.Test
{
    [TestClass]
    public class AvLibTest
    {
        private AvLib _lib = new AvLib();

        [TestInitialize]
        public void Setup()
        {
            _lib.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _lib.Close();
        }

        [TestMethod]
        [TestCategory("ThrowAway")]
        [Ignore]
        public void OnSimplePlayback_DevHearsSound()
        {
            _lib.LoadMedia(@"Media\48502_1745.mp4");
            _lib.Play();

            Thread.Sleep(500);

            _lib.Stop();
        }

        [TestMethod]
        [TestCategory("ThrowAway")]
        public void OnSimplePlayback_StartStopEventsRaised()
        {
            _lib.LoadMedia(@"Media\48502_1745.mp4");

            var playingEventWasRaised = new ManualResetEvent(false);
            _lib.Playing += (o, e) => playingEventWasRaised.Set();
            _lib.Play();

            Assert.IsTrue(playingEventWasRaised.WaitOne(500));

            var stoppedEventWasRaised = new ManualResetEvent(false);
            _lib.Stopped += (o, e) => stoppedEventWasRaised.Set();
            _lib.Stop();

            Assert.IsTrue(stoppedEventWasRaised.WaitOne(500));
        }
    }
}
