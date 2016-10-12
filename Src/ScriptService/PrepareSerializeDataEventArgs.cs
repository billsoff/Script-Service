#region Write Log
/*==============================================================================
 * Function:     Provide PrepareSerializationData event of ScriptService.
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
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace CIPACE.Extension
{
    /// <summary>
    /// Provide PrepareSerializationData event of ScriptService.
    /// </summary>
    public sealed class PrepareSerializeDataEventArgs : EventArgs
    {
        private List<JavaScriptConverter> _registeredConverters;

        /// <summary>
        /// Get or set type resolver to serializer.
        /// </summary>
        public JavaScriptTypeResolver TypeResolver { get; set; }

        /// <summary>
        /// Get or set max length.
        /// </summary>
        public int MaxJsonLength { get; set; }

        /// <summary>
        /// Get or set recursion limit.
        /// </summary>
        public int RecursionLimit { get; set; }

        /// <summary>
        /// Get registered converters.
        /// </summary>
        public IList<JavaScriptConverter> RegisteredConverters
        {
            get { return _registeredConverters; }
        }

        /// <summary>
        /// Register converters.
        /// </summary>
        /// <param name="converters">JSON converters.</param>
        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            if (converters == null)
            {
                return;
            }

            IList<JavaScriptConverter> list = converters as IList<JavaScriptConverter>;

            if (list == null)
            {
                list = new List<JavaScriptConverter>(converters);
            }

            if (list.Count == 0)
            {
                return;
            }

            if (_registeredConverters == null)
            {
                _registeredConverters = new List<JavaScriptConverter>();
            }

            _registeredConverters.AddRange(list);
        }

        /// <summary>
        /// Create java script serializer. This method take type resolver and registered converters and other settings into account.
        /// </summary>
        /// <returns>Java script serializer.</returns>
        public JavaScriptSerializer CreateSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(TypeResolver);

            if (_registeredConverters != null)
            {
                serializer.RegisterConverters(_registeredConverters);
            }

            if (MaxJsonLength > 0)
            {
                serializer.MaxJsonLength = MaxJsonLength;
            }

            if (RecursionLimit > 0)
            {
                serializer.RecursionLimit = RecursionLimit;
            }

            return serializer;
        }
    }
}