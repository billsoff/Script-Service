#region Write Log
/*==============================================================================
 * Function:     Flag data property for script service context arguments.
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

namespace CIPACE.Extension
{
    /// <summary>
    /// Flag data property for script service context arguments.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DataPropertyAttribute : Attribute
    {
        private readonly string _name;
        private bool _reqired;

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="name">Data property name.</param>
        public DataPropertyAttribute(string name)
        {
            _name = !String.IsNullOrWhiteSpace(name) ? name.Trim() : null;
        }

        /// <summary>
        /// Data property name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Indicate whether this data property is required.
        /// </summary>
        public bool Required
        {
            get { return _reqired; }
            set { _reqired = value; }
        }
    }
}