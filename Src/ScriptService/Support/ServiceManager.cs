#region Write Log
/*==============================================================================
 * Function:     Service manager that responsible for deploying.
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
    /// Service manager that responsible for deploying.
    /// </summary>
    internal sealed class ServiceManager : JavaScriptLibraryManager
    {
        #region Static Members

        private static readonly string KEY_CURRENT;

        static ServiceManager()
        {
            Type t = typeof(ServiceManager);
            KEY_CURRENT = String.Format(
                    "{0}.Current,{1}",
                    t.FullName,
                    t.Assembly.FullName
                );
        }

        #endregion

        public static ServiceManager GetCurrent(Page host)
        {
            IDictionary cache = HttpContext.Current.Items;

            if (!cache.Contains(KEY_CURRENT))
            {
                cache.Add(KEY_CURRENT, new ServiceManager(host));
            }

            return (ServiceManager)cache[KEY_CURRENT];
        }

        private readonly List<DeclareJavaScriptItem> _declareds = new List<DeclareJavaScriptItem>();

        private ServiceManager(Page host)
            : base(host)
        {
        }

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
                    DeclareCodeBlock(
                            GetCodeBlock(item.Name, item.GetLocation(service)),
                            service.GetDeclareKey(),
                            item.AddScriptTags
                        );
                    break;

                case Discovery.CodeBlockHere:
                    DeclareCodeBlock(item.CodeBlock, service.GetDeclareKey(), item.AddScriptTags);
                    break;

                default:
                    return false;
            }

            _declareds.Add(item);

            return true;
        }
    }
}