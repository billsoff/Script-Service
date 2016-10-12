#region Write Log
/*==============================================================================
 * Function:     Java script declare item.
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
    /// Java script declare item.
    /// </summary>
    [Serializable]
    internal sealed class DeclareJavaScriptItem : DeclareItem
    {
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="discovery">Resource location type.</param>
        /// <param name="resourceType">Resource type.</param>
        /// <param name="codeBlock">Code block.</param>
        /// <param name="addScriptTags">Instruct whether to add script tags.</param>
        public DeclareJavaScriptItem(
                string name,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addScriptTags = true
            )
            : base(name, discovery, resourceType, codeBlock, addScriptTags)
        {
        }

        /// <summary>
        /// Gets client resource type.
        /// </summary>
        public override ClientResourceType ItemType
        {
            get { return ClientResourceType.JavaScript; }
        }

        /// <summary>
        /// Instructs whether to add script tags.
        /// </summary>
        public bool AddScriptTags { get { return base.AddTags; } }
    }
}