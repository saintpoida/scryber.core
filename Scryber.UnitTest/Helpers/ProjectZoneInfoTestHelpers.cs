using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scryber.Components;
using Scryber.Html.Components;
using Scryber.PDF.Layout;
using Scryber.PDF;
using Scryber;

namespace Scryber.Core.UnitTests.Helpers
{
    /// <summary>
    /// Helper methods and test data setup for ProjectZoneInfo component testing
    /// </summary>
    public static class ProjectZoneInfoTestHelpers
    {
        /// <summary>
        /// Creates a basic document with a page and header setup for testing zone info components
        /// </summary>
        public static (Document doc, Page page) CreateBasicTestDocument()
        {
            var doc = new Document();
            var page = new Page();
            doc.Pages.Add(page);

            return (doc, page);
        }

        /// <summary>
        /// Creates a document with multiple pages for cross-page testing
        /// </summary>
        public static Document CreateMultiPageTestDocument(int pageCount = 3)
        {
            var doc = new Document();

            for (int i = 0; i < pageCount; i++)
            {
                var page = new Page();
                doc.Pages.Add(page);

                // Components will be added to page contents directly

                // Add page-specific params
                page.Params.Add("pageNumber", (i + 1).ToString());
                page.Params.Add("pageName", $"Page {i + 1}");
            }

            return doc;
        }

        /// <summary>
        /// Adds standard test data to a document for zone info testing
        /// </summary>
        public static void AddStandardTestData(Document doc)
        {
            doc.Params.Add("region", "North America");
            doc.Params.Add("area", "Western States");
            doc.Params.Add("zone", "California");
            doc.Params.Add("client", "Acme Corporation");
            doc.Params.Add("project", "Website Redesign");
            doc.Params.Add("phase", "Development Phase");
        }

        /// <summary>
        /// Adds hierarchical test data with different levels of nesting
        /// </summary>
        public static void AddHierarchicalTestData(Document doc)
        {
            // Company hierarchy
            doc.Params.Add("company", "TechCorp Ltd");
            doc.Params.Add("division", "Software Development");
            doc.Params.Add("department", "Web Services");
            doc.Params.Add("team", "Frontend Team");

            // Geographic hierarchy
            doc.Params.Add("country", "United States");
            doc.Params.Add("state", "California");
            doc.Params.Add("city", "San Francisco");
            doc.Params.Add("district", "SOMA");

            // Project hierarchy
            doc.Params.Add("portfolio", "Customer Platform");
            doc.Params.Add("program", "Digital Transformation");
            doc.Params.Add("initiative", "Mobile First");
            doc.Params.Add("workstream", "iOS Development");
        }

        /// <summary>
        /// Creates a zone info component with standard configuration
        /// </summary>
        public static ProjectZoneInfoLabel CreateStandardZoneInfo()
        {
            return new ProjectZoneInfoLabel()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone"
            };
        }

        /// <summary>
        /// Creates an HTML zone info component with standard configuration
        /// </summary>
        public static HTMLProjectZoneInfo CreateStandardHtmlZoneInfo()
        {
            return new HTMLProjectZoneInfo()
            {
                Level0Name = "region",
                Level1Name = "area",
                Level2Name = "zone",
                StyleClass = "zone-info-test"
            };
        }

        /// <summary>
        /// Creates multiple zone info components with different configurations for testing
        /// </summary>
        public static List<ProjectZoneInfoLabel> CreateVariedZoneInfoComponents()
        {
            return new List<ProjectZoneInfoLabel>
            {
                // Full hierarchy
                new ProjectZoneInfoLabel()
                {
                    Level0Name = "company",
                    Level1Name = "division",
                    Level2Name = "department"
                },
                // Partial hierarchy - skip middle level
                new ProjectZoneInfoLabel()
                {
                    Level0Name = "country",
                    Level2Name = "city"
                },
                // Single level
                new ProjectZoneInfoLabel()
                {
                    Level0Name = "client"
                },
                // Different combination
                new ProjectZoneInfoLabel()
                {
                    Level0Name = "portfolio",
                    Level1Name = "program",
                    Level2Name = "initiative"
                }
            };
        }

        /// <summary>
        /// Executes a document and captures the layout for testing
        /// </summary>
        public static PDFLayoutDocument ExecuteDocumentAndCaptureLayout(Document doc)
        {
            PDFLayoutDocument layout = null;

            doc.LayoutComplete += (sender, args) => {
                var context = (PDFLayoutContext)(args.Context);
                layout = context.DocumentLayout;
            };

            using (var ms = new MemoryStream())
            {
                doc.SaveAsPDF(ms);
            }

            return layout;
        }

        /// <summary>
        /// Validates standard zone info output format
        /// </summary>
        public static void ValidateZoneInfoOutput(string output, string expectedLevel0 = null,
            string expectedLevel1 = null, string expectedLevel2 = null)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            // Should always start with "Level: "
            if (!output.StartsWith("Level: "))
                throw new AssertFailedException($"Output should start with 'Level: '. Actual: {output}");

            // Check for expected content
            if (!string.IsNullOrEmpty(expectedLevel0) && !output.Contains(expectedLevel0))
                throw new AssertFailedException($"Output should contain '{expectedLevel0}'. Actual: {output}");

            if (!string.IsNullOrEmpty(expectedLevel1) && !output.Contains(expectedLevel1))
                throw new AssertFailedException($"Output should contain '{expectedLevel1}'. Actual: {output}");

            if (!string.IsNullOrEmpty(expectedLevel2) && !output.Contains(expectedLevel2))
                throw new AssertFailedException($"Output should contain '{expectedLevel2}'. Actual: {output}");

            // If multiple levels are expected, should contain separator
            var expectedLevels = new[] { expectedLevel0, expectedLevel1, expectedLevel2 }
                .Where(s => !string.IsNullOrEmpty(s)).Count();

            if (expectedLevels > 1 && !output.Contains(" > "))
                throw new AssertFailedException($"Output should contain separator ' > ' for multiple levels. Actual: {output}");
        }

        /// <summary>
        /// Creates test data scenarios for edge cases
        /// </summary>
        public static class EdgeCaseData
        {
            public static void AddEmptyStringData(Document doc)
            {
                doc.Params.Add("empty", "");
                doc.Params.Add("whitespace", "   ");
                doc.Params.Add("valid", "Valid Data");
            }

            public static void AddSpecialCharacterData(Document doc)
            {
                doc.Params.Add("symbols", "Data & More > Info < Test");
                doc.Params.Add("unicode", "Données Français");
                doc.Params.Add("numbers", "12345");
                doc.Params.Add("mixed", "Mix3d Ch@r$");
            }

            public static void AddLongTextData(Document doc)
            {
                doc.Params.Add("long", new string('A', 100));
                doc.Params.Add("verylong", new string('B', 500));
                doc.Params.Add("normal", "Normal Length Text");
            }

            public static void AddNullEquivalentData(Document doc)
            {
                doc.Params.Add("null", null);
                doc.Params.Add("empty", "");
                doc.Params.Add("space", " ");
            }
        }

        /// <summary>
        /// Common validation patterns for zone info components
        /// </summary>
        public static class ValidationPatterns
        {
            public const string NoDataMessage = "Level: No zone info available";
            public const string LoadingMessage = "Level: Loading zone info...";
            public const string LevelPrefix = "Level: ";
            public const string Separator = " > ";

            public static bool IsValidZoneInfoOutput(string output)
            {
                return !string.IsNullOrEmpty(output) &&
                       output.StartsWith(LevelPrefix) &&
                       (output == NoDataMessage ||
                        output == LoadingMessage ||
                        output.Length > LevelPrefix.Length);
            }

            public static int CountDataLevels(string output)
            {
                if (string.IsNullOrEmpty(output) || !output.StartsWith(LevelPrefix))
                    return 0;

                var content = output.Substring(LevelPrefix.Length);

                if (content == "No zone info available" || content == "Loading zone info...")
                    return 0;

                return content.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }

        /// <summary>
        /// Performance testing helpers
        /// </summary>
        public static class Performance
        {
            public static Document CreateLargeTestDocument(int pageCount = 100)
            {
                var doc = CreateMultiPageTestDocument(pageCount);
                AddHierarchicalTestData(doc);

                // Add zone info to each page
                foreach (Page page in doc.Pages)
                {
                    var zoneInfo = CreateStandardZoneInfo();
                    page.Contents.Add(zoneInfo);
                }

                return doc;
            }

            public static TimeSpan MeasureExecutionTime(Action action)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                action();
                stopwatch.Stop();
                return stopwatch.Elapsed;
            }
        }
    }

    /// <summary>
    /// Custom exception for assertion failures in helper methods
    /// </summary>
    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message) : base(message) { }
        public AssertFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}