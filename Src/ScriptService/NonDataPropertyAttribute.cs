#region Write Log
/*==============================================================================
 * Function:     Flag the property DO NOT be serialize to service context arguments.
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
    /// Flag the property DO NOT be serialize to service context arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class NonDataPropertyAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NonDataPropertyAttribute()
        {
        }
    }
}