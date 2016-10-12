﻿#region Write Log
/*==============================================================================
 * Function:     Provide information for CompleteDeclare event of ScriptService.
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
    /// Provide information for CompleteDeclare event of ScriptService.
    /// </summary>
    public sealed class CompleteDeclareEventArgs : HostEventArgs
    {
        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="page">Page to deploy service script.</param>
        public CompleteDeclareEventArgs(Page page)
            : base(page)
        {
        }
    }
}