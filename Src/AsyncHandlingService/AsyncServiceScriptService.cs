#region Write Log
/*==============================================================================
 * Function:     Asynchronous service script.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 12/22/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;
using System.Web.UI;

[assembly: WebResource("CIPACE.Extension.Script.ServiceComponent.js", "text/javascript")]

namespace CIPACE.Extension
{
    /// <summary>
    /// Provides asynchronous service script service.
    /// </summary>
    [ServiceGuid("FD153B31-FE65-4665-A6B3-E9079D5DBCAB")]
    [Declare("CIPACE.Extension.Script.ServiceComponent.js", Discovery.WebResource)]
    [Serializable]
    public sealed class AsyncServiceScriptService : ScriptService
    {
        /// <summary>
        /// Gets or sets the handler URL.
        /// </summary>
        /// <value>
        /// The handler URL.
        /// </value>
        [DataProperty("url", Required = true)]
        public string HandlerUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of the service component.
        /// </summary>
        /// <value>
        /// The type of the service component.
        /// </value>
        [NonDataProperty]
        public Type ServiceComponentType { get; set; }

        /// <summary>
        /// Provides for subclass. Default do nothing. Before invoke, this method will be called.
        /// </summary>
        /// <param name="host">The host to deploy the resources.</param>
        /// <exception cref="System.InvalidOperationException">ServiceComponentType is required.</exception>
        protected override void Initialize(Page host)
        {
            if (ServiceComponentType == null)
            {
                throw new InvalidOperationException("ServiceComponentType is required.");
            }

            HandlerUrl = host.ResolveUrl(HandlerUrl);

            AddProperty(
                    "type",
                    Globals.GetTypeQualifiedName(ServiceComponentType)
                );
        }
    }
}