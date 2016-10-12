#region Write Log
/*==============================================================================
 * Function:     Provide information to CompleteInvoke event of ScriptService.
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
    /// Provide information to CompleteInvoke event of ScriptService.
    /// </summary>
    public sealed class CompleteInvokeEventArgs : TargetEventArgs
    {
        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="target">Target control to invoke service.</param>
        public CompleteInvokeEventArgs(Control target)
            : base(target)
        {
        }
    }
}