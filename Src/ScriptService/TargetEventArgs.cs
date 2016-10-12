#region Write Log
/*==============================================================================
 * Function:     Provides information for event to target control.
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
    /// Provides information for event to target control.
    /// </summary>
    public abstract class TargetEventArgs : EventArgs
    {
        private readonly Control _target;

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="target">Target control to invoke the service script.</param>
        protected TargetEventArgs(Control target)
        {
            _target = target;
        }

        /// <summary>
        /// Get target control to invoke service script.
        /// </summary>
        public Control Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Get page to deploy resource.
        /// </summary>
        public Page Page
        {
            get { return _target.Page; }
        }
    }
}