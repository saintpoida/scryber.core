using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.Components;
using Scryber.Html.Components;
using System.IO;

namespace ProjectZoneInfo.Tests
{
    [TestClass]
    public class ProjectZoneInfoTests
    {
        [TestMethod]
        [TestCategory("Zone Info")]
        public void PropertyValidation_Test()
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

        [TestMethod]
        [TestCategory("Zone Info")]
        public void BasicIntegration_Test()
        {
            // Test that the component can be created and added to a document without errors
            var doc = new Document();
            var page = new Page();
            doc.Pages.Add(page);

            var zoneInfo = new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area"
            };
            page.Contents.Add(zoneInfo);

            doc.Params.Add("region", "Test Region");
            doc.Params.Add("area", "Test Area");

            // Act - Generate PDF to ensure no runtime errors
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
                Assert.IsTrue(ms.Length > 0, "PDF should be generated");
            }
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLComponent_PropertyValidation_Test()
        {
            // Arrange & Act
            var htmlZoneInfo = new HTMLProjectZoneInfo();

            // Test property setters and getters
            htmlZoneInfo.Level0Name = "client";
            htmlZoneInfo.Level1Name = "project";
            htmlZoneInfo.Level2Name = "phase";
            htmlZoneInfo.StyleClass = "zone-info";

            // Assert
            Assert.AreEqual("client", htmlZoneInfo.Level0Name, "Level0Name should be settable");
            Assert.AreEqual("project", htmlZoneInfo.Level1Name, "Level1Name should be settable");
            Assert.AreEqual("phase", htmlZoneInfo.Level2Name, "Level2Name should be settable");
            Assert.AreEqual("zone-info", htmlZoneInfo.StyleClass, "StyleClass should be settable");
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLComponent_VisibilityAttribute_Test()
        {
            // Test visibility attribute behavior
            var htmlZoneInfo1 = new HTMLProjectZoneInfo();
            var htmlZoneInfo2 = new HTMLProjectZoneInfo();

            // Test "hidden" value
            htmlZoneInfo1.Hidden = "hidden";
            Assert.IsFalse(htmlZoneInfo1.Visible, "Setting Hidden='hidden' should make component not visible");
            Assert.AreEqual("hidden", htmlZoneInfo1.Hidden, "Hidden getter should return 'hidden'");

            // Test empty value
            htmlZoneInfo2.Hidden = "";
            Assert.IsTrue(htmlZoneInfo2.Visible, "Setting Hidden='' should make component visible");
            Assert.AreEqual("", htmlZoneInfo2.Hidden, "Hidden getter should return empty string when visible");
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLComponent_BasicIntegration_Test()
        {
            // Test that the HTML component works like the base component
            var doc = new Document();
            var page = new Page();
            doc.Pages.Add(page);

            var htmlZoneInfo = new HTMLProjectZoneInfo()
            {
                Level0Name = "department",
                Level1Name = "team",
                StyleClass = "header-info"
            };
            page.Contents.Add(htmlZoneInfo);

            doc.Params.Add("department", "Engineering");
            doc.Params.Add("team", "Backend Team");

            // Act - Generate PDF to ensure no runtime errors
            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
                Assert.IsTrue(ms.Length > 0, "PDF should be generated with HTML component");
            }
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLComponent_InheritanceFromBase_Test()
        {
            // Verify HTML component inherits all base functionality
            var htmlZoneInfo = new HTMLProjectZoneInfo();
            var baseZoneInfo = new ProjectZoneInfoLabel();

            // Test that both components have the same core properties
            htmlZoneInfo.Level0Name = "test";
            baseZoneInfo.Level0Name = "test";

            Assert.AreEqual(htmlZoneInfo.Level0Name, baseZoneInfo.Level0Name,
                "HTML component should inherit base properties");
        }
    }
}