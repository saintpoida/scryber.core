using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.Components;
using Scryber.Html.Components;
using Scryber.Styles;
using Scryber.PDF.Layout;
using Scryber.PDF;
using Scryber;

namespace Scryber.Core.UnitTests.Html
{
    [TestClass()]
    public class HTMLProjectZoneInfo_Test
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

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_BasicFunctionality()
        {
            // Arrange
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var htmlZoneInfo = new HTMLProjectZoneInfo()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone"
            };
            page.Contents.Add(htmlZoneInfo);

            // Add test data
            doc.Params.Add("region", "Europe");
            doc.Params.Add("area", "Nordic Countries");
            doc.Params.Add("zone", "Sweden");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert
            Assert.IsNotNull(htmlZoneInfo.Proxy, "HTML zone info should have a text proxy");
            string outputValue = htmlZoneInfo.OutputValue;

            Assert.IsTrue(outputValue.Contains("Europe"), $"Output should contain Level0 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains("Nordic Countries"), $"Output should contain Level1 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.Contains("Sweden"), $"Output should contain Level2 data. Actual: {outputValue}");
            Assert.IsTrue(outputValue.StartsWith("Level: "), $"Output should start with 'Level: '. Actual: {outputValue}");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_AttributeMappingTest()
        {
            // Arrange
            var htmlZoneInfo = new HTMLProjectZoneInfo();

            // Test HTML attribute mappings (lowercase as per HTML component pattern)
            htmlZoneInfo.Level0Name = "customer";
            htmlZoneInfo.Level1Name = "project";
            htmlZoneInfo.Level2Name = "phase";

            // Test CSS styling attributes
            htmlZoneInfo.StyleClass = "zone-info";

            // Test visibility attribute
            htmlZoneInfo.Hidden = "hidden";

            // Assert property mappings work correctly
            Assert.AreEqual("customer", htmlZoneInfo.Level0Name, "Level0Name mapping should work");
            Assert.AreEqual("project", htmlZoneInfo.Level1Name, "Level1Name mapping should work");
            Assert.AreEqual("phase", htmlZoneInfo.Level2Name, "Level2Name mapping should work");
            Assert.AreEqual("zone-info", htmlZoneInfo.StyleClass, "StyleClass mapping should work");
            Assert.IsFalse(htmlZoneInfo.Visible, "Hidden attribute should set Visible to false");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_HiddenAttributeBehavior()
        {
            // Test various hidden attribute values
            var htmlZoneInfo1 = new HTMLProjectZoneInfo();
            var htmlZoneInfo2 = new HTMLProjectZoneInfo();
            var htmlZoneInfo3 = new HTMLProjectZoneInfo();
            var htmlZoneInfo4 = new HTMLProjectZoneInfo();

            // Test "hidden" value
            htmlZoneInfo1.Hidden = "hidden";
            Assert.IsFalse(htmlZoneInfo1.Visible, "Setting Hidden='hidden' should make component not visible");
            Assert.AreEqual("hidden", htmlZoneInfo1.Hidden, "Hidden getter should return 'hidden'");

            // Test empty value
            htmlZoneInfo2.Hidden = "";
            Assert.IsTrue(htmlZoneInfo2.Visible, "Setting Hidden='' should make component visible");
            Assert.AreEqual("", htmlZoneInfo2.Hidden, "Hidden getter should return empty string when visible");

            // Test null value
            htmlZoneInfo3.Hidden = null;
            Assert.IsTrue(htmlZoneInfo3.Visible, "Setting Hidden=null should make component visible");

            // Test other value
            htmlZoneInfo4.Hidden = "other";
            Assert.IsTrue(htmlZoneInfo4.Visible, "Setting Hidden to non-'hidden' value should make component visible");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_VisibilityToHiddenMapping()
        {
            // Test that Visible property correctly maps to Hidden attribute
            var htmlZoneInfo1 = new HTMLProjectZoneInfo();
            var htmlZoneInfo2 = new HTMLProjectZoneInfo();

            // Test setting Visible to false
            htmlZoneInfo1.Visible = false;
            Assert.AreEqual("hidden", htmlZoneInfo1.Hidden, "Setting Visible=false should return Hidden='hidden'");

            // Test setting Visible to true
            htmlZoneInfo2.Visible = true;
            Assert.AreEqual("", htmlZoneInfo2.Hidden, "Setting Visible=true should return Hidden=''");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_StyleAttributeMapping()
        {
            // Arrange
            var htmlZoneInfo = new HTMLProjectZoneInfo();

            // Test style class setting
            htmlZoneInfo.StyleClass = "test-class";

            // Assert
            Assert.AreEqual("test-class", htmlZoneInfo.StyleClass, "StyleClass should be settable");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_TitleAttributeMapping()
        {
            // Arrange & Act
            var htmlZoneInfo = new HTMLProjectZoneInfo();
            htmlZoneInfo.OutlineTitle = "Zone Information Component";

            // Assert
            Assert.AreEqual("Zone Information Component", htmlZoneInfo.OutlineTitle,
                "Title attribute should map to OutlineTitle property");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_InheritanceFromBaseComponent()
        {
            // Arrange
            var htmlZoneInfo = new HTMLProjectZoneInfo();

            // Test that HTML component inherits base functionality
            htmlZoneInfo.Level0Name = "category";

            // Act - should inherit base validation and behavior
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);

            page.Contents.Add(htmlZoneInfo);

            doc.Params.Add("category", "Test Category");
            doc.LayoutComplete += Doc_LayoutCompleted;

            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert - should work like base component
            Assert.IsNotNull(htmlZoneInfo.Proxy, "Should inherit proxy functionality from base");
            string output = htmlZoneInfo.OutputValue;
            Assert.IsTrue(output.Contains("Test Category"), $"Should inherit data access from base. Actual: {output}");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_CombinedAttributesScenario()
        {
            // Test a realistic HTML scenario with multiple attributes
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var htmlZoneInfo = new HTMLProjectZoneInfo()
            {
                Level0Name = "client",
                Level1Name = "project",
                Level2Name = "deliverable",
                StyleClass = "header-zone-info",
                OutlineTitle = "Project Zone Information",
                Hidden = "" // Visible
            };

            // Test setting style class

            page.Contents.Add(htmlZoneInfo);

            // Add test data
            doc.Params.Add("client", "Acme Corp");
            doc.Params.Add("project", "Website Redesign");
            doc.Params.Add("deliverable", "Phase 1");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert all attributes work together
            Assert.IsTrue(htmlZoneInfo.Visible, "Component should be visible");
            Assert.AreEqual("header-zone-info", htmlZoneInfo.StyleClass, "CSS class should be preserved");
            Assert.AreEqual("Project Zone Information", htmlZoneInfo.OutlineTitle, "Title should be preserved");
            Assert.AreEqual("header-zone-info", htmlZoneInfo.StyleClass, "CSS class should be preserved");

            string output = htmlZoneInfo.OutputValue;
            Assert.AreEqual("Level: Acme Corp > Website Redesign > Phase 1", output,
                "Should combine all three levels of data correctly");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_EmptyAndNullScenarios()
        {
            // Test HTML component handles empty/null scenarios gracefully
            Document doc = new Document();
            Page page = new Page();
            doc.Pages.Add(page);


            var htmlZoneInfo = new HTMLProjectZoneInfo()
            {
                Level0Name = null,        // null field name
                Level1Name = "",          // empty field name
                Level2Name = "status",    // valid field name
                StyleClass = null,        // null CSS class
                OutlineTitle = ""         // empty title
            };

            page.Contents.Add(htmlZoneInfo);

            // Add only one piece of data
            doc.Params.Add("status", "Active");

            doc.LayoutComplete += Doc_LayoutCompleted;

            // Act
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            // Assert - should handle nulls/empties gracefully
            Assert.IsNull(htmlZoneInfo.StyleClass, "Null CSS class should remain null");
            Assert.AreEqual("", htmlZoneInfo.OutlineTitle, "Empty title should remain empty");

            string output = htmlZoneInfo.OutputValue;
            Assert.AreEqual("Level: Active", output, "Should only show the one valid field");
        }

        [TestMethod()]
        [TestCategory("HTML Zone Info")]
        public void HTMLProjectZoneInfo_ParseableComponentAttribute()
        {
            // This test verifies the component can be identified by its parseable attribute
            // The actual parsing would be tested in integration tests, but we can verify the attribute exists

            var type = typeof(HTMLProjectZoneInfo);
            var attributes = type.GetCustomAttributes(typeof(PDFParsableComponentAttribute), false);

            Assert.AreEqual(1, attributes.Length, "HTMLProjectZoneInfo should have PDFParsableComponent attribute");

            var parseableAttr = (PDFParsableComponentAttribute)attributes[0];
            Assert.AreEqual("projectzoneinfo", parseableAttr.ElementName,
                "Component should be parseable as 'projectzoneinfo' tag");
        }
    }
}