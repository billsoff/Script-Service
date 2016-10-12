#region Write Log
/*==============================================================================
 * Function:     Manages client resource.
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Manages client resource.
    /// </summary>
    internal abstract class ClientLibraryManager
    {
        #region Fields

        private readonly Page _host;
        private ScriptManager _scriptManager;

        #endregion

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="host">Page to deploy resource.</param>
        protected ClientLibraryManager(Page host)
        {
            _host = host;
        }

        /// <summary>
        /// Gets page to deploy resource.
        /// </summary>
        public Page Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Gets ScriptManager instance on current page.
        /// </summary>
        public ScriptManager ScriptManager
        {
            get
            {
                if (_scriptManager == null)
                {
                    _scriptManager = GetScriptManager(Host);
                }

                return _scriptManager;
            }
        }

        /// <summary>
        /// Gets code block from assembly.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="assembly">Assembly resource resides.</param>
        /// <returns>Code block in resource.</returns>
        protected static string GetCodeBlock(string resourceName, Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "assembly should be provided.");
            }

            Stream stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new InvalidOperationException(
                        String.Format(
                                "Cannot retrieve code block \"{0}\" from \"{1}\"",
                                resourceName,
                                assembly.GetName().Name
                            )
                    );
            }

            string codeBlock;

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                codeBlock = reader.ReadToEnd();
            }

            return codeBlock;
        }

        #region Help Members

        /// <summary>
        /// Gets ScriptManager instance on page.
        /// </summary>
        /// <param name="host">Page to deploy resource.</param>
        /// <returns>ScriptManager instance.</returns>
        private ScriptManager GetScriptManager(Page host)
        {
            ScriptManager manager = ScriptManager.GetCurrent(host);

            if (manager == null)
            {
                throw new InvalidOperationException("ScriptManager control should be declared.");
            }

            return manager;
        }

        #endregion
    }
}