#region Write Log
/*==============================================================================
 * Function:     Java script library manager.
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
using System.Reflection;
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Java script library manager.
    /// </summary>
    internal abstract class JavaScriptLibraryManager : ClientLibraryManager
    {
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="host">Page to deploy resource.</param>
        public JavaScriptLibraryManager(Page host)
            : base(host)
        {
        }

        /// <summary>
        /// Queries whether is in asynchronous post back.
        /// </summary>
        public bool IsInAsyncPostBack
        {
            get { return ScriptManager.IsInAsyncPostBack; }
        }

        /// <summary>
        /// Deploys web resource.
        /// </summary>
        /// <param name="jsQualifiedName">Java script qualified name.</param>
        /// <param name="assembly">Assembly where resource resides.</param>
        protected void DeclareWebResource(string jsQualifiedName, Assembly assembly)
        {
            if (assembly == null)
            {
                throw new InvalidOperationException("assembly should be provided to locate web resource.");
            }

            ScriptManager.Scripts.Add(new ScriptReference(jsQualifiedName, assembly.FullName));
        }

        /// <summary>
        /// Deploys web site script file.
        /// </summary>
        /// <param name="scriptPath">Script path.</param>
        protected void DeclareScriptPath(string scriptPath)
        {
            ScriptManager.Scripts.Add(new ScriptReference(scriptPath));
        }

        /// <summary>
        /// Deploys code block.
        /// </summary>
        /// <param name="jsQualifiedName">Java script qualified name.</param>
        /// <param name="assembly">Assembly where resource resides.</param>
        /// <param name="addScriptTags">Instruct whether to add script tags.</param>
        protected void DeclareCodeBlock(string jsQualifiedName, Assembly assembly, bool addScriptTags = true)
        {
            string jsBlock = GetCodeBlock(jsQualifiedName, assembly);
            string name = String.Format("{0}/{1}", assembly.FullName, jsQualifiedName);

            DeclareCodeBlock(jsBlock, name, addScriptTags);
        }

        /// <summary>
        /// Deploy code block.
        /// </summary>
        /// <param name="jsBlock">Java script code block.</param>
        /// <param name="name">As declare key.</param>
        /// <param name="addScriptTags">Instruct whether to add script tags.</param>
        protected void DeclareCodeBlock(string jsBlock, string name, bool addScriptTags = true)
        {
            if (jsBlock == null)
            {
                throw new InvalidOperationException("Code block cannot be null.");
            }

            ScriptManager.RegisterStartupScript(
                    Host,
                    Host.GetType(),
                    name,
                    jsBlock,
                    addScriptTags
                );
        }
    }
}