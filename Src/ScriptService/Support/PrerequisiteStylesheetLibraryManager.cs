#region Write Log
/*==============================================================================
 * Function:     Style sheet manager.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 8/29/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CIPACE.Extension
{
    /// <summary>
    /// Style sheet manager.
    /// </summary>
    internal sealed class PrerequisiteStylesheetLibraryManager : ClientLibraryManager
    {
        #region Static Members

        private static readonly string KEY_CURRENT;

        /// <summary>
        /// Initializes the <see cref="PrerequisiteStylesheetLibraryManager"/> class.
        /// </summary>
        static PrerequisiteStylesheetLibraryManager()
        {
            Type t = typeof(PrerequisiteStylesheetLibraryManager);
            KEY_CURRENT = String.Format(
                    "{0}.Current,{1}",
                    t.FullName,
                    t.Assembly.FullName
                );
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>The current manager.</returns>
        public static PrerequisiteStylesheetLibraryManager GetCurrent(Page host)
        {
            IDictionary cache = HttpContext.Current.Items;

            if (!cache.Contains(KEY_CURRENT))
            {
                cache[KEY_CURRENT] = new PrerequisiteStylesheetLibraryManager(host);
            }

            return (PrerequisiteStylesheetLibraryManager)cache[KEY_CURRENT];
        }

        #endregion

        #region Fields

        private readonly List<DeclareStylesheetItem> _declareds = new List<DeclareStylesheetItem>();

        #endregion

        private PrerequisiteStylesheetLibraryManager(Page host)
            : base(host)
        {
        }

        /// <summary>
        /// Deploys style sheet to page.
        /// </summary>
        /// <param name="item">Style sheet item.</param>
        /// <param name="service">Script service.</param>
        /// <param name="container">Style sheet container.</param>
        /// <returns>If really deploy the item, return true; otherwise, false.</returns>
        public bool Declare(DeclareStylesheetItem item, ScriptService service, Control container)
        {
            if (ScriptManager.IsInAsyncPostBack)
            {
                return false;
            }

            if (_declareds.Contains(item))
            {
                return false;
            }

            switch (item.Discovery)
            {
                case Discovery.WebResource:
                    DeclareWebResource(item.Name, item.GetResourceType(service), container);
                    break;

                case Discovery.WebSiteFile:
                    DeclareWebSiteFile(item.Name, container);
                    break;

                case Discovery.CodeBlockInResource:
                    DeclareCodeBlock(item.Name, item.GetLocation(service), item.AddStyleTags, container);
                    break;

                case Discovery.CodeBlockHere:
                    DeclareCodeBlock(item.CodeBlock, item.AddStyleTags, container);
                    break;

                default:
                    return false;
            }

            _declareds.Add(item);

            return true;
        }

        #region Help Members

        private void DeclareWebResource(string name, Type type, Control container)
        {
            string url = Host.ClientScript.GetWebResourceUrl(type, name);
            HtmlLink style = CreateStylesheet(url);

            container.Controls.Add(style);
        }

        private void DeclareWebSiteFile(string url, Control container)
        {
            HtmlLink style = CreateStylesheet(url);
            container.Controls.Add(style);
        }

        private void DeclareCodeBlock(string name, Assembly location, bool addStyleTags, Control container)
        {
            string codeBlock = GetCodeBlock(name, location);

            DeclareCodeBlock(codeBlock, addStyleTags, container);
        }

        private void DeclareCodeBlock(string codeBlock, bool addStyleTags, Control container)
        {
            Control style;

            if (addStyleTags)
            {
                HtmlGenericControl control = new HtmlGenericControl("style");
                control.InnerText = codeBlock;
                style = control;
            }
            else
            {
                style = new LiteralControl(codeBlock);
            }

            style.EnableViewState = false;

            container.Controls.Add(style);
        }

        private HtmlLink CreateStylesheet(string url)
        {
            HtmlLink style = new HtmlLink();

            style.Href = url;

            style.Attributes["type"] = "text/css";
            style.Attributes["rel"] = "stylesheet";

            return style;
        }

        #endregion
    }
}