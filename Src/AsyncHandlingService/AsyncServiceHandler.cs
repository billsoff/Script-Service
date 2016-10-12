#region Write Log
/*==============================================================================
 * Function:         Asynchronous service handler (.ashx class).
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 1/30/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System.Web;
using System.Web.SessionState;

namespace CIPACE.Extension
{
    /// <summary>
    /// Get file handler.
    /// </summary>
    public class AsyncServiceHandler : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Process entry point.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            string type = context.Request["type"];
            string paramStr = context.Request["param"];

            string result = AsyncServiceComponentExecutor.Execute(context, type, paramStr);

            context.Response.Write(result);
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }
    }
}