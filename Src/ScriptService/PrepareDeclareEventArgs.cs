#region Write Log
/*==============================================================================
 * Function:     Provide information to PrepareDeclare event of Script Service.
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
    /// Provide information to PrepareDeclare event of Script Service.
    /// </summary>
    public sealed class PrepareDeclareEventArgs : HostEventArgs
    {
        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="page">Page to deploy resource.</param>
        public PrepareDeclareEventArgs(Page page)
            : base(page)
        {
        }
    }
}