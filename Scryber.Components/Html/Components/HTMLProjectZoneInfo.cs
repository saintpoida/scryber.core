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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scryber.Styles;
using Scryber.Components;
using Scryber.Text;

namespace Scryber.Html.Components
{
    /// <summary>
    /// HTML implementation of the ProjectZoneInfo component that displays hierarchical zone information
    /// for the current page context, displaying Level0, Level1, and Level2 names.
    /// </summary>
    [PDFParsableComponent("projectzoneinfo")]
    public class HTMLProjectZoneInfo : Scryber.Components.ProjectZoneInfoLabel
    {
        /// <summary>
        /// Gets or sets the CSS class attribute for styling
        /// </summary>
        [PDFAttribute("class")]
        public override string StyleClass { get => base.StyleClass; set => base.StyleClass = value; }

        /// <summary>
        /// Gets or sets the inline style attribute
        /// </summary>
        [PDFAttribute("style")]
        public override Style Style { get => base.Style; set => base.Style = value; }

        /// <summary>
        /// Global Html hidden attribute used with xhtml as hidden='hidden'
        /// </summary>
        [PDFAttribute("hidden")]
        public string Hidden
        {
            get
            {
                if (this.Visible)
                    return string.Empty;
                else
                    return "hidden";
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value != "hidden")
                    this.Visible = true;
                else
                    this.Visible = false;
            }
        }

        /// <summary>
        /// Gets or sets the data field name for Level 0 zone information
        /// </summary>
        [PDFAttribute("data-level0name")]
        public override string Level0Name { get => base.Level0Name; set => base.Level0Name = value; }

        /// <summary>
        /// Gets or sets the data field name for Level 1 zone information
        /// </summary>
        [PDFAttribute("data-level1name")]
        public override string Level1Name { get => base.Level1Name; set => base.Level1Name = value; }

        /// <summary>
        /// Gets or sets the data field name for Level 2 zone information
        /// </summary>
        [PDFAttribute("data-level2name")]
        public override string Level2Name { get => base.Level2Name; set => base.Level2Name = value; }

        /// <summary>
        /// Gets or sets the title attribute for the component
        /// </summary>
        [PDFAttribute("title")]
        public override string OutlineTitle
        {
            get => base.OutlineTitle;
            set => base.OutlineTitle = value;
        }

        public HTMLProjectZoneInfo()
            : base()
        {
        }
    }
}