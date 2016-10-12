#region Write Log
/*==============================================================================
 * Function:     Flag to ScriptService to instruct to deploy requisite resources to page.
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
    /// Flag to ScriptService to instruct to deploy requisite resources to page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PrerequisiteAttribute : Attribute
    {
        private readonly string _name;
        private readonly Discovery _discovery;

        private Type _resourceType;
        private ClientResourceType _itemType;

        private string _codeBlock;
        private bool _addTags;

        private int _loadOrder;

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="discovery">Resource location.</param>
        public PrerequisiteAttribute(string name, Discovery discovery)
        {
            _name = name;
            _discovery = discovery;
        }

        /// <summary>
        /// Get resource name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get resource location.
        /// </summary>
        public Discovery Discovery
        {
            get { return _discovery; }
        }

        /// <summary>
        /// Get or set resource type (Confirm assembly where resource resides).
        /// </summary>
        public Type ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; }
        }

        /// <summary>
        /// Get or set client resource (JavaScript or Stylesheet).
        /// </summary>
        public ClientResourceType ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        /// <summary>
        /// Get or set load order.
        /// </summary>
        public int LoadOrder
        {
            get { return _loadOrder; }
            set { _loadOrder = value; }
        }

        /// <summary>
        /// Get or set code block.
        /// </summary>
        public string CodeBlock
        {
            get { return _codeBlock; }
            set { _codeBlock = value; }
        }

        /// <summary>
        /// Get or set a value to instruct whether to add tags when deploy code block to page.
        /// </summary>
        public bool AddTags
        {
            get { return _addTags; }
            set { _addTags = value; }
        }

        /// <summary>
        /// Create declare item.
        /// </summary>
        /// <returns>Declare item.</returns>
        internal DeclareItem CreateDeclareItem()
        {
            switch (ItemType)
            {
                case ClientResourceType.JavaScript:
                    return new DeclareJavaScriptItem(
                            Name,
                            Discovery,
                            ResourceType,
                            CodeBlock,
                            AddTags
                        );

                case ClientResourceType.Stylesheet:
                    return new DeclareStylesheetItem(
                            Name,
                            Discovery,
                            ResourceType,
                            CodeBlock,
                            AddTags
                        );

                default:
                    throw new InvalidOperationException("Cannot recognize declare item type.");
            }
        }
    }
}