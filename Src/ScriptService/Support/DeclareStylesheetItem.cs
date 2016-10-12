#region Write Log
/*==============================================================================
 * Function:     Style sheet declare item.
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
    /// Style sheet declare item.
    /// </summary>
    [Serializable]
    internal sealed class DeclareStylesheetItem : DeclareItem
    {
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="discovery">Resource location type.</param>
        /// <param name="resourceType">Assembly where resource resides.</param>
        /// <param name="codeBlock">Code block.</param>
        /// <param name="addStyleTags">Instruct whether to add style tags.</param>
        public DeclareStylesheetItem(
                string name,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addStyleTags = true
            )
            : base(name, discovery, resourceType, codeBlock, addStyleTags)
        {
        }

        /// <summary>
        /// Gets client resource type.
        /// </summary>
        public override ClientResourceType ItemType
        {
            get { return ClientResourceType.Stylesheet; }
        }

        /// <summary>
        /// Instructs whether to add style tags.
        /// </summary>
        public bool AddStyleTags { get { return base.AddTags; } }
    }
}