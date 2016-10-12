#region Write Log
/*==============================================================================
 * Function:     Flag on ScriptService to deploy service script.
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
    /// Flag on ScriptService to deploy service script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DeclareAttribute : Attribute
    {
        private readonly string _name;
        private readonly Discovery _discovery;

        private bool _addScriptTags = true;

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="name">Script resource name.</param>
        /// <param name="discovery">Location type.</param>
        public DeclareAttribute(string name, Discovery discovery)
        {
            _name = name;
            _discovery = discovery;
        }

        /// <summary>
        /// Resource name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Location type.
        /// </summary>
        public Discovery Discovery
        {
            get { return _discovery; }
        }

        /// <summary>
        /// Resource type.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Code block.
        /// </summary>
        public string CodeBlock { get; set; }

        /// <summary>
        /// Instruct whether to add script tags (Only for CodeBlock).
        /// </summary>
        public bool AddScriptTags
        {
            get { return _addScriptTags; }
            set { _addScriptTags = value; }
        }
    }
}