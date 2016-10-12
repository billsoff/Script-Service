#region Write Log
/*==============================================================================
 * Function:     Flag ScriptService to provide service GUID and name.
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
    /// Flag ScriptService to provide service GUID and name.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ServiceGuidAttribute : Attribute
    {
        private readonly Guid _serviceId;
        private string _name;

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="serviceId">Service ID.</param>
        public ServiceGuidAttribute(string serviceId)
        {
            _serviceId = new Guid(serviceId);
        }

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="serviceId">Service GUID.</param>
        internal ServiceGuidAttribute(Guid serviceId)
        {
            _serviceId = serviceId;
        }

        /// <summary>
        /// Gets service ID (GUID).
        /// </summary>
        public Guid ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>
        /// Gets service name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}