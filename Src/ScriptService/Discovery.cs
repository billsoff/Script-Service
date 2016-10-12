#region Write Log
/*==============================================================================
 * Function:     Instruct resource location type.
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
    /// Instruct resource location type.
    /// </summary>
    public enum Discovery
    {
        /// <summary>
        /// Web resource.
        /// </summary>
        WebResource = 1,

        /// <summary>
        /// Web site file.
        /// </summary>
        WebSiteFile = 2,

        /// <summary>
        /// Code block embedded into assembly.
        /// </summary>
        CodeBlockInResource = 3,

        /// <summary>
        /// Provides code block directly.
        /// </summary>
        CodeBlockHere = 4
    }
}