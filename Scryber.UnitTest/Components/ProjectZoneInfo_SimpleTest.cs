using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.Components;

namespace Scryber.Core.UnitTests.Components
{
    [TestClass()]
    public class ProjectZoneInfo_SimpleTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get => testContextInstance;
            set => testContextInstance = value;
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_PropertyValidation()
        {
            // Arrange & Act
            var zoneInfo = new ProjectZoneInfoLabel();

            // Test property setters and getters
            zoneInfo.Level0Name = "test0";
            zoneInfo.Level1Name = "test1";
            zoneInfo.Level2Name = "test2";

            // Assert
            Assert.AreEqual("test0", zoneInfo.Level0Name, "Level0Name should be settable");
            Assert.AreEqual("test1", zoneInfo.Level1Name, "Level1Name should be settable");
            Assert.AreEqual("test2", zoneInfo.Level2Name, "Level2Name should be settable");
        }
    }
}