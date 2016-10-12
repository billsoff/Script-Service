#region Write Log
/*==============================================================================
 * Function:     Object serialize literally.
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
    /// Object serialize literally.
    /// </summary>
    public interface ILiteralObject : ILiterallySerializable
    {
        /// <summary>
        /// Get or set its value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Validate type. If don't support the type, an exception will be thrown.
        /// </summary>
        void ValidateType();
    }
}