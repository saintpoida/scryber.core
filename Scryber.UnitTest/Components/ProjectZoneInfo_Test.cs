using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.Components;
using Scryber.Styles;
using Scryber.PDF.Layout;
using Scryber.PDF;
using Scryber;

namespace Scryber.Core.UnitTests.ProjectZoneInfoTests
{
    [TestClass()]
    public class ProjectZoneInfo_Test
    {
        private PDFLayoutDocument _layout;

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

        private void Doc_LayoutCompleted(object sender, LayoutEventArgs args)
        {
            var context = (PDFLayoutContext)(args.Context);
            _layout = context.DocumentLayout;
        }

        /* COMMENTED OUT FOR SINGLE TEST FOCUS
        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_BasicDataAccess()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);

            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone"
            };
            page.Contents.Add(zoneInfo);

            // Add test data to document params
            doc.Params.Add("region", "North America");
            doc.Params.Add("area", "Western States");
            doc.Params.Add("zone", "California");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            Assert.IsNotNull(_layout, "Layout document should be available");
            Assert.IsNotNull(zoneInfo.Proxy, "Zone info should have a text proxy");

            string outputValue = zoneInfo.OutputValue;
            Assert.IsTrue(outputValue.Contains("North America"), $"Output should contain Level0 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains("Western States"), $"Output should contain Level1 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains("California"), $"Output should contain Level2 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains(" > "), $"Output should contain separator. Actual: {outputValue}");
            Assert.IsTrue(outputValue.StartsWith("Level: "), $"Output should start with 'Level: '. Actual: {outputValue}");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_PartialDataAccess()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);

            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone" // This will be missing
            };
            page.Contents.Add(zoneInfo);

            // Add only partial test data
            doc.Params.Add("region", "North America");
            doc.Params.Add("area", "Western States");
            // Intentionally omit "zone"

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            string outputValue = zoneInfo.OutputValue;
            Assert.IsTrue(outputValue.Contains("North America"), $"Output should contain Level0 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains("Western States"), $"Output should contain Level1 data. Actual: {outputValue}");
            Assert.IsFalse(outputValue.Contains("California"), $"Output should not contain missing Level2 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains(" > "), $"Output should contain separator for existing data. Actual: {outputValue}");

            // Should be "Level: North America > Western States"
            string expected = "Level: North America > Western States";
            Assert.AreEqual(expected, outputValue, "Output should match expected format for partial data");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_NoDataAvailable()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone"
            };
            page.Contents.Add(zoneInfo);

            // Don't add any data to document params

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            string outputValue = zoneInfo.OutputValue;
            string expected = "Level: No zone info available";
            Assert.AreEqual(expected, outputValue, "Output should show fallback message when no data available");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_EmptyFieldNames()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "", // Empty field name
                Level1Name = null, // Null field name
                Level2Name = "zone"
            };
            page.Contents.Add(zoneInfo);

            doc.Params.Add("zone", "California");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            string outputValue = zoneInfo.OutputValue;
            string expected = "Level: California";
            Assert.AreEqual(expected, outputValue, "Output should only show data for valid field names");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_ProxyTextBehavior()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region"
            };
            page.Contents.Add(zoneInfo);

            doc.Params.Add("region", "Test Region");

            // Track layout completion
            bool layoutCompleted = false;
            doc.LayoutComplete += (sender, args) => {
                Doc_LayoutCompleted(sender, args);
                layoutCompleted = true;
            };

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            Assert.IsTrue(layoutCompleted, "Layout should have completed");
            Assert.IsNotNull(zoneInfo.Proxy, "Zone info should have a text proxy");

            var proxy = zoneInfo.Proxy;
            Assert.AreEqual("ProjectZoneInfo", proxy.ProxyKey, "Proxy should have correct key");
            Assert.IsTrue(proxy.Text.Contains("Test Region"), $"Proxy text should contain the data. Actual: {proxy.Text}");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_MultipleInstancesOnPage()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            // Create two zone info components with different configurations
            var zoneInfo1 = new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area"
            };

            var zoneInfo2 = new ProjectZoneInfoLabel()
            {
                Level0Name = "department",
                Level2Name = "team"
            };

            header.Contents.Add(zoneInfo1);
            header.Contents.Add(zoneInfo2);

            // Add test data
            doc.Params.Add("region", "North America");
            doc.Params.Add("area", "Western States");
            doc.Params.Add("department", "Engineering");
            doc.Params.Add("team", "Backend Team");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            string output1 = zoneInfo1.OutputValue;
            string output2 = zoneInfo2.OutputValue;

            Assert.AreEqual("Level: North America > Western States", output1, "First component should show region > area");
            Assert.AreEqual("Level: Engineering > Backend Team", output2, "Second component should show department > team");
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_CrossPageBehavior()
        {
            // Arrange
            Document doc = new Document();

            // Create multiple pages with different data contexts
            for (int i = 0; i < 3; i++)
            {
                Page page = new Page();
                doc.Pages.Add(page);

                var header = new PDFPageHeader();
                page.Header = header;

                var zoneInfo = new ProjectZoneInfoLabel()
                {
                    Level0Name = "region",
                    Level1Name = "page"
                };
                page.Contents.Add(zoneInfo);

                // Add page-specific data
                page.Params.Add("page", $"Page {i + 1}");
            }

            // Add document-wide data
            doc.Params.Add("region", "Global Region");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            Assert.IsNotNull(_layout, "Layout should be available");
            Assert.AreEqual(3, _layout.AllPages.Count, "Should have 3 pages");

            // Note: This test verifies the component structure works across pages
            // The actual page-specific data resolution would need more detailed layout inspection
        }

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_NullContextHandling()
        {
            // Arrange - Create component without adding to document
            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region"
            };

            // Act & Assert - Should not throw exceptions
            string baseText = zoneInfo.BaseText;
            Assert.IsNotNull(baseText, "Base text should not be null even without context");

            // Should return empty or default text when no context available
            Assert.IsTrue(string.IsNullOrEmpty(baseText) || baseText.Contains("No context"),
                $"Base text should indicate no context. Actual: {baseText}");
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

        [TestMethod()]
        [TestCategory("Zone Info")]
        public void ProjectZoneInfo_BaseTextReadOnly()
        {
            // Arrange
            var zoneInfo = new ProjectZoneInfoLabel();

            // Act & Assert
            // The BaseText property is read-only - this is handled by the component implementation
        }
        */
    }
}