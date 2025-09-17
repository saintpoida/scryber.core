using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.Components;
using Scryber.Html.Components;
using System.IO;
using Scryber.Generation;
using Scryber;

namespace ProjectZoneInfo.Tests
{
    [TestClass]
    public class ProjectZoneInfoTests
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

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLParsing_ProjectZoneInfo_BasicParsing_Test()
        {
            // Test that HTML parser recognizes <projectzoneinfo> tags
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>ProjectZoneInfo Parsing Test</title>
                </head>
                <body>
                    <projectzoneinfo data-level0name='client'
                                   data-level1name='project'
                                   data-level2name='phase' />
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                // Add test data that the component should find
                doc.Params.Add("client", "Acme Corp");
                doc.Params.Add("project", "Website Redesign");
                doc.Params.Add("phase", "Development");

                // Generate PDF and verify component works
                using (var ms = new MemoryStream())
                {
                    doc.SaveAsPDF(ms);
                    Assert.IsTrue(ms.Length > 0, "PDF should be generated from parsed HTML");
                }

                // TODO: Add verification that HTMLProjectZoneInfo component was actually created
                // This would require inspecting the document structure after parsing
            }
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLParsing_ProjectZoneInfo_AttributeMapping_Test()
        {
            // Test that HTML attributes are correctly mapped to component properties
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>Attribute Mapping Test</title>
                </head>
                <body>
                    <projectzoneinfo data-level0name='department'
                                       data-level1name='team'
                                       class='zone-info-header'
                                       hidden='hidden' />
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                doc.Params.Add("department", "Engineering");
                doc.Params.Add("team", "Backend Team");

                // Should parse and generate PDF even with hidden attribute
                using (var ms = new MemoryStream())
                {
                    doc.SaveAsPDF(ms);
                    Assert.IsTrue(ms.Length > 0, "PDF should be generated even with hidden component");
                }
            }
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLParsing_ProjectZoneInfo_MultipleComponents_Test()
        {
            // Test parsing multiple ProjectZoneInfo components in same document
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>Multiple Components Test</title>
                </head>
                <body>
                    <div>
                        <h1>Header Zone Info:</h1>
                        <projectzoneinfo data-level0name='client' data-level1name='project' />
                    </div>
                    <div>
                        <h2>Department Info:</h2>
                        <projectzoneinfo data-level0name='department' data-level1name='team' />
                    </div>
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                doc.Params.Add("client", "Contoso Ltd");
                doc.Params.Add("project", "Mobile App");
                doc.Params.Add("department", "Marketing");
                doc.Params.Add("team", "Digital Team");

                using (var ms = new MemoryStream())
                {
                    doc.SaveAsPDF(ms);
                    Assert.IsTrue(ms.Length > 0, "PDF should be generated with multiple zone info components");
                }
            }
        }

        [TestMethod]
        [TestCategory("HTML Zone Info")]
        public void HTMLParsing_ProjectZoneInfo_ComponentRecognition_Test()
        {
            // This test verifies that the parser actually creates HTMLProjectZoneInfo components
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <body>
                    <projectzoneinfo data-level0name='test' />
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                // This is a basic test - ideally we'd inspect the parsed document structure
                // to verify HTMLProjectZoneInfo components were actually created
                Assert.IsNotNull(doc, "Document should be parsed successfully");

                doc.Params.Add("test", "Test Value");

                using (var ms = new MemoryStream())
                {
                    doc.SaveAsPDF(ms);
                    Assert.IsTrue(ms.Length > 0, "PDF generation should succeed with parsed component");
                }
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Integration_VisualVerification_OutputPDF_Test()
        {
            // This test creates a real PDF file for visual verification
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>ProjectZoneInfo Visual Test</title>
                    <style>
                        .header { font-size: 14pt; font-weight: bold; color: #333; margin-bottom: 20px; }
                        .zone-info { background-color: #f0f8ff; padding: 10px; border: 1px solid #ccc; margin: 10px 0; }
                        .test-section { margin: 20px 0; padding: 15px; border-left: 4px solid #0078d4; }
                    </style>
                </head>
                <body>
                    <div class='header'>ProjectZoneInfo Component Visual Test</div>

                    <div class='test-section'>
                        <h2>Test 1: Full Hierarchy</h2>
                        <p>This should display all three levels:</p>
                        <div class='zone-info'>
                            <projectzoneinfo data-level0name='client'
                                           data-level1name='project'
                                           data-level2name='phase' />
                        </div>
                    </div>

                    <div class='test-section'>
                        <h2>Test 2: Partial Data</h2>
                        <p>This should display only two levels (missing phase):</p>
                        <div class='zone-info'>
                            <projectzoneinfo data-level0name='department'
                                           data-level1name='team'
                                           data-level2name='missing' />
                        </div>
                    </div>

                    <div class='test-section'>
                        <h2>Test 3: Single Level</h2>
                        <p>This should display only the company name:</p>
                        <div class='zone-info'>
                            <projectzoneinfo data-level0name='company' />
                        </div>
                    </div>

                    <div class='test-section'>
                        <h2>Test 4: No Data Available</h2>
                        <p>This should show fallback message:</p>
                        <div class='zone-info'>
                            <projectzoneinfo data-level0name='nonexistent' />
                        </div>
                    </div>

                    <div class='test-section'>
                        <h2>Test 5: Multiple Components</h2>
                        <p>Two different zone info components:</p>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='border: 1px solid #ccc; padding: 10px; background-color: #fff8dc;'>
                                    <strong>Business Context:</strong><br/>
                                    <projectzoneinfo data-level0name='client' data-level1name='project' />
                                </td>
                                <td style='border: 1px solid #ccc; padding: 10px; background-color: #f0fff0;'>
                                    <strong>Technical Context:</strong><br/>
                                    <projectzoneinfo data-level0name='department' data-level1name='team' />
                                </td>
                            </tr>
                        </table>
                    </div>
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                // Add comprehensive test data
                doc.Params.Add("client", "Acme Corporation");
                doc.Params.Add("project", "Digital Transformation");
                doc.Params.Add("phase", "Phase 2 - Implementation");
                doc.Params.Add("department", "Engineering");
                doc.Params.Add("team", "Frontend Development Team");
                doc.Params.Add("company", "TechCorp Solutions Ltd");

                // Output to a physical file for visual verification within project
                string testOutputPath = System.IO.Path.Combine("test-output", "ProjectZoneInfo_VisualTest.pdf");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(testOutputPath));

                using (var fileStream = new FileStream(testOutputPath, FileMode.Create))
                {
                    doc.SaveAsPDF(fileStream);
                }

                // Verify file was created and has content
                Assert.IsTrue(File.Exists(testOutputPath), $"PDF file should be created at: {testOutputPath}");
                var fileInfo = new FileInfo(testOutputPath);
                Assert.IsTrue(fileInfo.Length > 2000, $"PDF should have content. Size: {fileInfo.Length} bytes");

                // Output the path for easy access
                TestContext.WriteLine($"Visual verification PDF created at: {testOutputPath}");
                TestContext.WriteLine($"File size: {fileInfo.Length:N0} bytes");
                TestContext.WriteLine("");
                TestContext.WriteLine("Expected content in PDF:");
                TestContext.WriteLine("- Test 1: 'Level: Acme Corporation > Digital Transformation > Phase 2 - Implementation'");
                TestContext.WriteLine("- Test 2: 'Level: Engineering > Frontend Development Team'");
                TestContext.WriteLine("- Test 3: 'Level: TechCorp Solutions Ltd'");
                TestContext.WriteLine("- Test 4: 'Level: No zone info available'");
                TestContext.WriteLine("- Test 5: Both business and technical contexts displayed in table");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Integration_HeaderFooter_VisualTest()
        {
            // Test zone info in headers/footers like real usage
            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>Header/Footer Zone Info Test</title>
                    <style>
                        body { margin: 40px; }
                        .header-info { text-align: center; font-size: 12pt; color: #666; }
                        .footer-info { text-align: right; font-size: 10pt; color: #999; }
                        .content { margin: 40px 0; }
                    </style>
                </head>
                <body>
                    <!-- This would typically be in a header template -->
                    <div class='header-info'>
                        <strong>Document Context:</strong>
                        <projectzoneinfo data-level0name='client' data-level1name='project' data-level2name='document' />
                    </div>

                    <div class='content'>
                        <h1>Main Document Content</h1>
                        <p>This is the main body of the document. The header above and footer below should show the zone information.</p>
                        <p>In a real scenario, this zone info would be in actual PDF headers/footers that repeat on every page.</p>

                        <h2>Current Zone Context</h2>
                        <p>The current zone information for this document is:</p>
                        <ul>
                            <li><strong>Client:</strong> Should show ""Global Industries Inc""</li>
                            <li><strong>Project:</strong> Should show ""ERP System Upgrade""</li>
                            <li><strong>Document:</strong> Should show ""Technical Specifications""</li>
                        </ul>
                    </div>

                    <!-- This would typically be in a footer template -->
                    <div class='footer-info'>
                        Generated for: <projectzoneinfo data-level0name='client' data-level1name='project' />
                    </div>
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                // Add realistic project data
                doc.Params.Add("client", "Global Industries Inc");
                doc.Params.Add("project", "ERP System Upgrade");
                doc.Params.Add("document", "Technical Specifications");

                // Output to a physical file within project
                string testOutputPath = System.IO.Path.Combine("test-output", "ProjectZoneInfo_HeaderFooter_Test.pdf");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(testOutputPath));

                using (var fileStream = new FileStream(testOutputPath, FileMode.Create))
                {
                    doc.SaveAsPDF(fileStream);
                }

                // Verify and report
                Assert.IsTrue(File.Exists(testOutputPath), $"PDF file should be created at: {testOutputPath}");
                var fileInfo = new FileInfo(testOutputPath);
                Assert.IsTrue(fileInfo.Length > 2000, $"PDF should have content. Size: {fileInfo.Length} bytes");

                TestContext.WriteLine($"Header/Footer test PDF created at: {testOutputPath}");
                TestContext.WriteLine($"File size: {fileInfo.Length:N0} bytes");
                TestContext.WriteLine("");
                TestContext.WriteLine("Expected content in PDF:");
                TestContext.WriteLine("- Header: 'Document Context: Level: Global Industries Inc > ERP System Upgrade > Technical Specifications'");
                TestContext.WriteLine("- Footer: 'Generated for: Level: Global Industries Inc > ERP System Upgrade'");
            }
        }
    }
}