#region Write Log
/*==============================================================================
 * Function:     Provides information to target control event of ScriptServcie. This event can be canceled.
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
    /// Provides information to target control event of ScriptServcie. This event can be canceled.
    /// </summary>
    public abstract class TargetRevocableEventArgs : CancelEventArgs
    {
        private readonly Control _target;

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="target">Target control to invoke the service script.</param>
        public TargetRevocableEventArgs(Control target)
        {
            _target = target;
        }

        /// <summary>
        /// Get target control;
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