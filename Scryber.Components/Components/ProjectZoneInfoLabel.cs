/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Scryber.PDF.Native;
using Scryber.Styles;
using Scryber.PDF.Layout;
using Scryber.PDF;

namespace Scryber.Components
{
    /// <summary>
    /// The ProjectZoneInfo component outputs hierarchical zone information 
    /// for the current page context, displaying Level0, Level1, and Level2 names.
    /// </summary>
    [PDFParsableComponent("ProjectZoneInfo")]
    public class ProjectZoneInfoLabel : TextBase
    {

        #region public string Level0Name

        /// <summary>
        /// Gets or sets the data field name for Level 0 zone information
        /// </summary>
        [PDFAttribute("data-Level0Name")]
        public virtual string Level0Name { get; set; }

        #endregion

        #region public string Level1Name

        /// <summary>
        /// Gets or sets the data field name for Level 1 zone information
        /// </summary>
        [PDFAttribute("data-Level1Name")]
        public virtual string Level1Name { get; set; }

        #endregion

        #region public string Level2Name

        /// <summary>
        /// Gets or sets the data field name for Level 2 zone information
        /// </summary>
        [PDFAttribute("data-Level2Name")]
        public virtual string Level2Name { get; set; }

        #endregion

        // local reference to the layout document
        private PDFLayoutDocument _doc;

        // local reference to the layout context for data access
        private PDFLayoutContext _layoutContext;

        // local value for the page index of this label
        private int _renderpageindex = -1;

        protected int RenderPageIndex { get { return _renderpageindex; } set { _renderpageindex = value; } }

        // local reference to the full style of this label
        private Style _fullstyle = null;

        protected Style RenderStyle { get { return _fullstyle; } set { _fullstyle = value; } }

        // The text proxy op who's text will be replaced with the zone info on render complete.
        Scryber.Text.PDFTextProxyOp _zoneProxy;

        protected override string BaseText
        {
            get
            {
                return this.GetDisplayText(false);
            }
            set
            {
                throw new InvalidOperationException(Errors.CannotSetBaseTextOfPageNumber);
            }
        }

        /// <summary>
        /// Gets the text value (to be) rendered for this project zone info
        /// </summary>
        public string OutputValue
        {
            get
            {
                if (null != _zoneProxy)
                    return _zoneProxy.Text;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the actual text proxy for this project zone info.
        /// </summary>
        public Scryber.Text.PDFTextProxyOp Proxy
        {
            get { return _zoneProxy; }
        }


        public ProjectZoneInfoLabel()
            : base(ObjectTypes.Text)
        {
        }

        /// <summary>
        /// Overrides the base text reader to create a new array of operations that has one op - a proxy that this component will update once the layout is completed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        protected override Text.PDFTextReader CreateReader(ContextBase context, Styles.Style style)
        {
            if (context is PDF.PDFLayoutContext layout)
            {
                _doc = layout.DocumentLayout;
                _layoutContext = layout;
                _renderpageindex = _doc.CurrentPageIndex;
                _fullstyle = style;

                string text = this.GetDisplayText(_renderpageindex, style, false);
                Scryber.Text.PDFTextProxyOp op = new Text.PDFTextProxyOp(this, "ProjectZoneInfo", text);
                Scryber.Text.PDFArrayTextReader array = new Text.PDFArrayTextReader(new Text.PDFTextOp[] { op });
                _zoneProxy = op;

                return array;
            }
            else
                return null;
        }

        /// <summary>
        /// Once layout is complete then we can replace the text that was used when not rendering
        /// with the text that has the actual zone information.
        /// </summary>
        internal override void RegisterLayoutComplete(LayoutContext context)
        {
            base.RegisterLayoutComplete(context);

            ComponentArrangement arrange = this.GetFirstArrangement();
            if (null != arrange)
            {
                this._renderpageindex = arrange.PageIndex;
                this._fullstyle = arrange.FullStyle;
            }

            if (null != this._doc && null != this._zoneProxy)
            {
                string text = this.GetDisplayText(this._renderpageindex, this._fullstyle, true);
                this._zoneProxy.Text = text;
            }
        }


        #region private string GetDisplayText(bool rendering)

        protected virtual string GetDisplayText(bool rendering)
        {
            if (null == this._doc)
                return string.Empty;

            var text = this.GetDisplayText(this._renderpageindex, this._fullstyle, rendering);
            return text;
        }

        /// <summary>
        /// Gets the full text for the project zone info label
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="style"></param>
        /// <param name="rendering"></param>
        /// <returns></returns>
        protected virtual string GetDisplayText(int pageindex, Style style, bool rendering)
        {
            if (null == this._layoutContext)
                return "No context available";

            if (!rendering)
                return "Level: Loading zone info...";

            try
            {
                return GetZoneInfoForPage(pageindex);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string GetZoneInfoForPage(int pageIndex)
        {
            var zoneParts = new List<string>();
            var debugInfo = new List<string>();

            debugInfo.Add($"PageIndex: {pageIndex}");

            // Try to get Level0Name data
            if (!string.IsNullOrEmpty(Level0Name))
            {
                string level0Value = GetDataValue(Level0Name);
                debugInfo.Add($"Level0({Level0Name}): '{level0Value}'");
                if (!string.IsNullOrEmpty(level0Value))
                    zoneParts.Add(level0Value);
            }

            // Try to get Level1Name data
            if (!string.IsNullOrEmpty(Level1Name))
            {
                string level1Value = GetDataValue(Level1Name);
                debugInfo.Add($"Level1({Level1Name}): '{level1Value}'");
                if (!string.IsNullOrEmpty(level1Value))
                    zoneParts.Add(level1Value);
            }

            // Try to get Level2Name data
            if (!string.IsNullOrEmpty(Level2Name))
            {
                string level2Value = GetDataValue(Level2Name);
                debugInfo.Add($"Level2({Level2Name}): '{level2Value}'");
                if (!string.IsNullOrEmpty(level2Value))
                    zoneParts.Add(level2Value);
            }

            // Join non-empty parts with " > " separator and add "Level: " prefix
            string zoneText = zoneParts.Count > 0 ? string.Join(" > ", zoneParts) : "No zone info available";
            string debugPrefix = $"[{string.Join(", ", debugInfo)}] ";
            return debugPrefix + "Level: " + zoneText;
        }

        private string GetDataValue(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || _layoutContext == null)
                return null;

            // 1. Try to get data from page-specific context first (if we have a page index)
            if (_renderpageindex >= 0 && _doc != null && _doc.AllPages != null && _renderpageindex < _doc.AllPages.Count)
            {
                try
                {
                    var page = _doc.AllPages[_renderpageindex];
                    if (page is PDFLayoutPage layoutPage)
                    {
                        // Try to get data from the page's owner component if it has parameters
                        var pageComponent = layoutPage.Owner;
                        if (pageComponent != null)
                        {
                            // Look for data in the page component's hierarchy
                            var comp = pageComponent;
                            while (comp != null)
                            {
                                // Check if component has params/data context
                                if (comp is Component withParams)
                                {
                                    // Try to access data through various methods
                                    try
                                    {
                                        // Method 1: Check if component has direct access to data
                                        var dataValue = TryGetComponentData(withParams, fieldName);
                                        if (!string.IsNullOrEmpty(dataValue))
                                            return dataValue;
                                    }
                                    catch { }
                                }
                                comp = comp.Parent;
                            }
                        }
                    }
                }
                catch
                {
                    // Continue to global context if page-specific fails
                }
            }

            // 2. Try to get data from global context Items
            try
            {
                if (_layoutContext.Items != null)
                {
                    var value = _layoutContext.Items[fieldName];
                    if (value != null)
                        return value.ToString();
                }
            }
            catch
            {
                // Continue to document parameters
            }

            // 3. Try to get data from document parameters
            try
            {
                if (_layoutContext.Document != null && _layoutContext.Document is Document doc)
                {
                    if (doc.Params != null)
                    {
                        try
                        {
                            var value = doc.Params[fieldName];
                            if (value != null)
                                return value.ToString();
                        }
                        catch
                        {
                            // Parameter doesn't exist, continue
                        }
                    }
                }
            }
            catch
            {
                // Final fallback
            }

            return null;
        }

        private string TryGetComponentData(Component component, string fieldName)
        {
            // Look for hidden data divs with specific IDs on the current page
            // Format: data-{fieldName}-{pageIndex} or data-{fieldName}
            if (_renderpageindex >= 0 && _doc != null)
            {
                try
                {
                    // Method 1: Look for page-specific data div IDs
                    string pageSpecificId = $"data-{fieldName}-page{_renderpageindex}";
                    var pageSpecificValue = FindHiddenDataById(pageSpecificId);
                    if (!string.IsNullOrEmpty(pageSpecificValue))
                        return pageSpecificValue;

                    // Method 2: Look for general data div IDs on current page content
                    string generalId = $"data-{fieldName}";
                    var generalValue = FindHiddenDataById(generalId);
                    if (!string.IsNullOrEmpty(generalValue))
                        return generalValue;

                    // Method 3: Look for data attributes in page content
                    var dataAttrValue = FindDataAttributeOnPage(fieldName);
                    if (!string.IsNullOrEmpty(dataAttrValue))
                        return dataAttrValue;
                }
                catch
                {
                    // Continue if data extraction fails
                }
            }

            return null;
        }

        private string FindHiddenDataById(string elementId)
        {
            if (_doc == null || _renderpageindex < 0 || _renderpageindex >= _doc.AllPages.Count)
                return null;

            try
            {
                var page = _doc.AllPages[_renderpageindex];
                if (page is PDFLayoutPage layoutPage)
                {
                    // Recursively search for components with the specified ID
                    var foundComponent = SearchForComponentById(layoutPage.Owner as Component, elementId);
                    if (foundComponent != null)
                    {
                        // Try to extract text content from the found component
                        if (foundComponent is TextBase textComp)
                        {
                            // Access the text content if possible
                            return ExtractTextFromComponent(textComp);
                        }
                        else if (foundComponent is Component comp)
                        {
                            // Try to get text from child components
                            return ExtractTextFromComponentHierarchy(comp);
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors in data extraction
            }

            return null;
        }

        private Component SearchForComponentById(Component root, string targetId)
        {
            if (root == null)
                return null;

            // Check if this component has the target ID
            if (!string.IsNullOrEmpty(root.ID) && root.ID == targetId)
                return root;

            // Search in child components if this is a container
            if (root is IContainerComponent container && container.Content != null)
            {
                foreach (Component child in container.Content)
                {
                    var found = SearchForComponentById(child, targetId);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        private string ExtractTextFromComponent(TextBase textComponent)
        {
            try
            {
                // We can't access BaseText directly from outside the class due to protected access
                // For now, we'll try alternative approaches or simplified data access
                return null;
            }
            catch
            {
                return null;
            }
        }

        private string ExtractTextFromComponentHierarchy(Component component)
        {
            try
            {
                // Look for text in immediate children
                if (component is IContainerComponent container && container.Content != null)
                {
                    foreach (Component child in container.Content)
                    {
                        if (child is TextBase textChild)
                        {
                            var text = ExtractTextFromComponent(textChild);
                            if (!string.IsNullOrEmpty(text))
                                return text;
                        }
                    }
                }
            }
            catch
            {
                // Ignore extraction errors
            }

            return null;
        }

        private string FindDataAttributeOnPage(string fieldName)
        {
            // This could be extended to search for data attributes in the page content
            // For now, return null as we're focusing on the hidden div approach
            return null;
        }

        #endregion
    }
}