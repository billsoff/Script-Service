#region Write Log
/*==============================================================================
 * Function:     Data property to transmit to service script context args.
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
    /// Data property to transmit to service script context arguments.
    /// </summary>
    [Serializable]
    internal sealed class DataProperty
    {
        #region Fields

        private readonly string _name;
        private readonly object _value;
        private readonly bool _literallySerialize;

        #endregion

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Data property name.</param>
        /// <param name="value">Data property value.</param>
        /// <param name="literallySerialize">Instructs whether serialize literally.</param>
        public DataProperty(string name, object value, bool literallySerialize = false)
        {
            _name = name;
            _value = value;

            _literallySerialize = literallySerialize;
        }

        #region Public Properties

        /// <summary>
        /// Gets data property name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets data property value.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Instructs whether to serialize literally.
        /// </summary>
        public bool LiterallySerialize
        {
            get { return _literallySerialize; }
        }

        /// <summary>
        /// Try convert to literal object.
        /// </summary>
        /// <returns>If convert successfully, return a wrapper object; otherwise null.</returns>
        public ILiterallySerializable TryConvertToLiteralObject()
        {
            ILiterallySerializable literal;
            literal = Value as ILiterallySerializable;

            if ((literal == null) && LiterallySerialize)
            {
                literal = LiteralObjectBase.CreateDefault(Value);
            }

            return literal;
        }

        #endregion
    }
}