#region Write Log
/*==============================================================================
 * Function:     Provide information to PrepareValidate event of ScriptService.
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
    /// Provide information to PrepareValidate event of ScriptService.
    /// </summary>
    public sealed class PrepareValidateEventArgs : HostEventArgs
    {
        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="page">Page to deploy resource.</param>
        public PrepareValidateEventArgs(Page page)
            : base(page)
        {
        }
    }
}