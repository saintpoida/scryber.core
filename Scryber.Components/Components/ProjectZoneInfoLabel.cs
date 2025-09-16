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
using Scryber.Native;
using Scryber.Styles;

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
        private Layout.PDFLayoutDocument _doc;

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
            : base(PDFObjectTypes.Text)
        {
        }

        /// <summary>
        /// Overrides the base text reader to create a new array of operations that has one op - a proxy that this component will update once the layout is completed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        protected override Text.PDFTextReader CreateReader(PDFLayoutContext context, Styles.Style style)
        {
            _doc = context.DocumentLayout;
            _layoutContext = context;
            _renderpageindex = _doc.CurrentPageIndex;
            _fullstyle = style;

            string text = this.GetDisplayText(_renderpageindex, style, false);
            Scryber.Text.PDFTextProxyOp op = new Text.PDFTextProxyOp(this, "ProjectZoneInfo", text);
            Scryber.Text.PDFArrayTextReader array = new Text.PDFArrayTextReader(new Text.PDFTextOp[] { op });
            _zoneProxy = op;

            return array;
        }

        /// <summary>
        /// Once layout is complete then we can replace the text that was used when not rendering
        /// with the text that has the actual zone information.
        /// </summary>
        internal override void RegisterLayoutComplete(PDFLayoutContext context)
        {
            base.RegisterLayoutComplete(context);

            PDFComponentArrangement arrange = this.GetFirstArrangement();
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

            // Try to get Level0Name data
            if (!string.IsNullOrEmpty(Level0Name))
            {
                string level0Value = GetDataValue(Level0Name);
                if (!string.IsNullOrEmpty(level0Value))
                    zoneParts.Add(level0Value);
            }

            // Try to get Level1Name data
            if (!string.IsNullOrEmpty(Level1Name))
            {
                string level1Value = GetDataValue(Level1Name);
                if (!string.IsNullOrEmpty(level1Value))
                    zoneParts.Add(level1Value);
            }

            // Try to get Level2Name data
            if (!string.IsNullOrEmpty(Level2Name))
            {
                string level2Value = GetDataValue(Level2Name);
                if (!string.IsNullOrEmpty(level2Value))
                    zoneParts.Add(level2Value);
            }

            // Join non-empty parts with " > " separator and add "Level: " prefix
            string zoneText = zoneParts.Count > 0 ? string.Join(" > ", zoneParts) : "No zone info available";
            return "Level: " + zoneText;
        }

        private string GetDataValue(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || _layoutContext == null)
                return null;

            // 1. Try to get data from context Items using the indexer (PDFItemCollection inherits from NameObjectCollectionBase)
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
                // Continue to next method if indexer fails
            }

            // 2. For now, return null - we can add more sophisticated data access later
            // TODO: Add DataStack access when we understand the full data binding context
            
            return null;
        }

        #endregion
    }
}