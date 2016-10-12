#region Write Log
/*==============================================================================
 * Function:     Provide information for PrepareInvoke event of ScriptService.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 8/29/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Provide information for PrepareInvoke event of ScriptService.
    /// </summary>
    public sealed class PrepareInvokeEventArgs : TargetRevocableEventArgs
    {
        /// <summary>
        /// Initialize new instance. The subscriber can cancel the invoke by setting Cancel to true.
        /// </summary>
        /// <param name="target">Target control to invoke script service.</param>
        public PrepareInvokeEventArgs(Control target)
            : base(target)
        {
        }
    }
}