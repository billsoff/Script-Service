#region Write Log
/*==============================================================================
 * Function:     Java script manager.
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
using System.Web;
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Java script manager.
    /// </summary>
    internal sealed class PrerequisiteJavaScriptLibraryManager : JavaScriptLibraryManager
    {
        #region Static Members

        private static readonly string KEY_CURRENT;

        /// <summary>
        /// Initializes the <see cref="PrerequisiteJavaScriptLibraryManager"/> class.
        /// </summary>
        static PrerequisiteJavaScriptLibraryManager()
        {
            Type t = typeof(PrerequisiteJavaScriptLibraryManager);
            KEY_CURRENT = String.Format(
                    "{0}.Current,{1}",
                    t.FullName,
                    t.Assembly.FullName
                );
        }

        /// <summary>
        /// Gets current java script manager.
        /// </summary>
        /// <param name="host">Page to deploy java script.</param>
        /// <returns>Current java script manager.</returns>
        public static PrerequisiteJavaScriptLibraryManager GetCurrent(Page host)
        {
            IDictionary cache = HttpContext.Current.Items;

            if (!cache.Contains(KEY_CURRENT))
            {
                cache[KEY_CURRENT] = new PrerequisiteJavaScriptLibraryManager(host);
            }

            return (PrerequisiteJavaScriptLibraryManager)cache[KEY_CURRENT];
        }

        #endregion

        #region Fields

        private readonly List<DeclareJavaScriptItem> _declareds = new List<DeclareJavaScriptItem>();

        #endregion

        private PrerequisiteJavaScriptLibraryManager(Page host)
            : base(host)
        {
        }

        /// <summary>
        /// Deploys java script item.
        /// </summary>
        /// <param name="item">Java script item.</param>
        /// <param name="service">Script service.</param>
        /// <returns>If deploy at this time, return true; otherwise, false.</returns>
        public bool Declare(DeclareJavaScriptItem item, ScriptService service)
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
                    DeclareWebResource(item.Name, item.GetLocation(service));
                    break;

                case Discovery.WebSiteFile:
                    DeclareScriptPath(item.Name);
                    break;

                case Discovery.CodeBlockInResource:
                    DeclareCodeBlock(item.Name, item.GetLocation(service), item.AddScriptTags);
                    break;

                case Discovery.CodeBlockHere:
                    DeclareCodeBlock(item.CodeBlock, item.Name, item.AddScriptTags);
                    break;

                default:
                    return false;
            }

            _declareds.Add(item);

            return true;
        }
    }
}