#region Write Log
/*==============================================================================
 * Function:        Indicate service parameter type.
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 9/12/2014
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
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AsyncServiceAttribute : Attribute
    {
        private Type _parameterType;

        public AsyncServiceAttribute()
        {
        }

        public Type ParameterType
        {
            get { return _parameterType; }
            set { _parameterType = value; }
        }
    }
}