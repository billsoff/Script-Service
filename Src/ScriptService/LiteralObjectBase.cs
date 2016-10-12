#region Write Log
/*==============================================================================
 * Function:     Provide a default implementation to ILiteralObject.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 8/29/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

namespace CIPACE.Extension
{
    /// <summary>
    /// Provide a default implementation to ILiteralObject.
    /// </summary>
    public abstract class LiteralObjectBase : ILiteralObject
    {
        /// <summary>
        /// Create a default wrapper object.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <returns>Wrapped object.</returns>
        public static ILiteralObject CreateDefault(object value)
        {
            return new LiteralObjectImpl(value);
        }

        private object _value;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected LiteralObjectBase()
        {
        }

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        protected LiteralObjectBase(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Get or set object to serialize.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Validate whether support type.
        /// </summary>
        public abstract void ValidateType();

        /// <summary>
        /// Build serialization result.
        /// </summary>
        /// <returns>Serialization result.</returns>
        public override abstract string ToString();

        #region Default Literal Object

        private sealed class LiteralObjectImpl : LiteralObjectBase
        {
            public LiteralObjectImpl(object value)
                : base(value)
            {
            }

            public override void ValidateType()
            {
                // Do nothing
            }

            public override string ToString()
            {
                return (Value != null) ? Value.ToString() : SerializationUtility.NULL_LITERAL;
            }
        }

        #endregion
    }
}