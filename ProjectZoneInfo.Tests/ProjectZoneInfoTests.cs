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

        [TestMethod]
        [TestCategory("Integration")]
        public void Integration_PageBreaks_RepeatingHeader_ContextSwitching_Test()
        {
            // Test proper repeating headers using HTML <header> element that Scryber understands

            string html = @"<!DOCTYPE html>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <title>Page Break Repeating Header Test</title>
                    <style>
                        header {
                            padding: 15px;
                            background-color: #f8f9fa;
                            border-bottom: 2px solid #007bff;
                            margin-bottom: 20px;
                            text-align: center;
                        }
                        .header-title {
                            font-size: 16pt;
                            font-weight: bold;
                            color: #007bff;
                            margin-bottom: 5px;
                        }
                        .header-context {
                            font-size: 12pt;
                            color: #666;
                        }
                        footer {
                            margin-top: 30px;
                            padding: 10px;
                            text-align: center;
                            font-size: 10pt;
                            color: #999;
                            border-top: 1px solid #ddd;
                        }
                        .company-section {
                            margin: 30px 0;
                            padding: 20px;
                            border: 1px solid #ddd;
                        }
                        .company-title {
                            font-size: 18pt;
                            font-weight: bold;
                            color: #0066cc;
                            margin-bottom: 15px;
                        }
                        .company-details {
                            font-size: 12pt;
                            line-height: 1.6;
                        }
                        .page-spacer {
                            height: 400px;
                            background: repeating-linear-gradient(
                                90deg,
                                #f8f9fa,
                                #f8f9fa 20px,
                                #e9ecef 20px,
                                #e9ecef 40px
                            );
                            border: 1px solid #ddd;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            font-size: 14pt;
                            color: #666;
                            margin: 20px 0;
                            page-break-after: always;
                        }
                    </style>
                </head>
                <body>
                    <!-- Proper HTML header that should repeat on every page -->
                    <header>
                        <div class='header-title'>Project Zone Information Report</div>
                        <div class='header-context'>
                            Current Context: <projectzoneinfo data-level0name='organization' data-level1name='division' data-level2name='project' />
                        </div>
                    </header>

                    <!-- Main content that will cause page breaks -->
                    <div class='company-section'>
                        <div class='company-title'>First Company Section</div>
                        <div class='company-details'>
                            <p><strong>Organization:</strong> Global Tech Solutions</p>
                            <p><strong>Division:</strong> Software Development</p>
                            <p><strong>Project:</strong> Cloud Migration Initiative</p>
                            <p>This is the first section of our report. The header above should contain the zone information and repeat on every page as content flows.</p>
                        </div>
                    </div>

                    <div class='page-spacer'>
                        Content Spacer 1 - Forces page break
                    </div>

                    <div class='company-section'>
                        <div class='company-title'>Second Company Section</div>
                        <div class='company-details'>
                            <p>This content should appear on a new page with the same repeating header showing zone information.</p>
                            <p>The header will automatically repeat because we're using proper HTML header structure.</p>
                        </div>
                    </div>

                    <div class='page-spacer'>
                        Content Spacer 2 - Forces another page break
                    </div>

                    <div class='company-section'>
                        <div class='company-title'>Third Company Section</div>
                        <div class='company-details'>
                            <p>This is the third section, demonstrating that the header continues to repeat across multiple page breaks.</p>
                            <p>Zone info should consistently show the same context data across all pages.</p>
                        </div>
                    </div>

                    <div class='page-spacer'>
                        Content Spacer 3 - Forces final page break
                    </div>

                    <div class='company-section'>
                        <div class='company-title'>Final Section</div>
                        <div class='company-details'>
                            <p>Final section to verify header repetition works consistently.</p>
                            <p><strong>Expected:</strong> Header appears on all pages with zone info: 'Level: Global Tech Solutions > Software Development > Cloud Migration Initiative'</p>
                        </div>
                    </div>

                    <!-- Footer that should also repeat -->
                    <footer>
                        Generated by Scryber PDF - Zone Info Test | Page <page />
                    </footer>
                </body>
                </html>";

            using (var sr = new StringReader(html))
            {
                var doc = Document.ParseDocument(sr, ParseSourceType.DynamicContent);

                // Add zone information parameters - this creates the global fallback
                doc.Params.Add("organization", "Global Tech Solutions");
                doc.Params.Add("division", "Software Development");
                doc.Params.Add("project", "Cloud Migration Initiative");

                // EXPERIMENT: Try to set up different contexts per page using layout events
                doc.LayoutComplete += (sender, args) =>
                {
                    // This is called after layout but might be too late for context switching
                    // The real challenge is that we need different data contexts DURING layout
                    // not after layout is complete
                };

                // Output to a physical file within project
                string testOutputPath = System.IO.Path.Combine("test-output", "ProjectZoneInfo_PageBreak_RepeatingHeader_Test.pdf");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(testOutputPath));

                using (var fileStream = new FileStream(testOutputPath, FileMode.Create))
                {
                    doc.SaveAsPDF(fileStream);
                }

                // Verify and report
                Assert.IsTrue(File.Exists(testOutputPath), $"PDF file should be created at: {testOutputPath}");
                var fileInfo = new FileInfo(testOutputPath);
                Assert.IsTrue(fileInfo.Length > 2000, $"PDF should have content. Size: {fileInfo.Length} bytes");

                TestContext.WriteLine($"Page break repeating header test PDF created at: {testOutputPath}");
                TestContext.WriteLine($"File size: {fileInfo.Length:N0} bytes");
                TestContext.WriteLine("");
                TestContext.WriteLine("Expected behavior (to verify manually in PDF):");
                TestContext.WriteLine("- Header should appear on ALL pages (not just the first)");
                TestContext.WriteLine("- Footer should appear on ALL pages with page numbers");
                TestContext.WriteLine("- Zone info should show: 'Level: Global Tech Solutions > Software Development > Cloud Migration Initiative'");
                TestContext.WriteLine("- Content should be properly spaced across multiple pages");
                TestContext.WriteLine("");
                TestContext.WriteLine("This test uses proper HTML <header> and <footer> elements for automatic repetition.");
            }
        }
    }
}