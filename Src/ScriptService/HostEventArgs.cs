#region Write Log
/*==============================================================================
 * Function:     Host event arguments.
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
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Host event arguments. 
    /// </summary>
    public abstract class HostEventArgs : EventArgs
    {
        private readonly Page _page;

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="page">Page to deploy resource.</param>
        protected HostEventArgs(Page page)
        {
            _page = page;
        }

        /// <summary>
        /// Page to deploy resource.
        /// </summary>
        public Page Page
        {
            get { return _page; }
        } 
    }
}