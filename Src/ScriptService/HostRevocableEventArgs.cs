#region Write Log
/*==============================================================================
 * Function:     Represent revocable host event arguments.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 8/29/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System.ComponentModel;
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Represent revocable host event arguments.
    /// </summary>
    public abstract class HostRevocableEventArgs : CancelEventArgs
    {
        private readonly Page _page;

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="page">Page to deploy resources.</param>
        protected HostRevocableEventArgs(Page page)
        {
            _page = page;
        }

        /// <summary>
        /// Page to deploy resources.
        /// </summary>
        public Page Page
        {
            get { return _page; }
        }
    }
}