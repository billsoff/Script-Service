#region Write Log
/*==============================================================================
 * Function:        Flags service name.
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 1/26/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;

namespace CIPACE.Extension
{
    /// <summary>
    /// Flags service methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class DispatchedMethodAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>
        /// Initializes method name.
        /// </summary>
        /// <param name="name"></param>
        public DispatchedMethodAttribute(string name)
        {
            this._name = name;
        }

        /// <summary>
        /// Gets method name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}