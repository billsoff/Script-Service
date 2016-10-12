#region Write Log
/*==============================================================================
 * Function:     An item (java script/style sheet) to deploy to the page.
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
using System.Reflection;

namespace CIPACE.Extension
{
    /// <summary>
    /// An item (java script/style sheet) to deploy to the page.
    /// </summary>
    [Serializable]
    internal abstract class DeclareItem : IEquatable<DeclareItem>
    {
        #region Fields

        private readonly string _name;
        private readonly Discovery _discovery;
        private readonly Type _resourceType;

        private readonly string _codeBlock;
        private readonly bool _addTags;

        #endregion

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="discovery">Resource location type.</param>
        /// <param name="resourceType">Confirm assembly where resource resides.</param>
        /// <param name="codeBlock">Code block.</param>
        /// <param name="addTags">Instruct whether to add tags.</param>
        protected DeclareItem(
                string name,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addTags = true
            )
        {
            CheckDeclareItemArguments(name, discovery, resourceType, codeBlock);

            _name = name;
            _discovery = discovery;
            _resourceType = resourceType;

            _codeBlock = codeBlock;
            _addTags = addTags;
        }

        #region Public Properties

        /// <summary>
        /// Get client resource type.
        /// </summary>
        public abstract ClientResourceType ItemType
        {
            get;
        }

        /// <summary>
        /// Get resource name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get resource location type.
        /// </summary>
        public Discovery Discovery
        {
            get { return _discovery; }
        }

        /// <summary>
        /// Get code block.
        /// </summary>
        public string CodeBlock
        {
            get { return _codeBlock; }
        }

        /// <summary>
        /// Instruct whether to add tags.
        /// </summary>
        public bool AddTags
        {
            get { return _addTags; }
        }

        #endregion

        /// <summary>
        /// Compares to another declare item and determine whether they are same.
        /// </summary>
        /// <param name="other">Another declare item.</param>
        /// <returns>If name, type, location and assembly all same, return true; otherwise, false.</returns>
        public bool Equals(DeclareItem other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.ItemType != other.ItemType)
            {
                return false;
            }

            if (this.Discovery != other.Discovery)
            {
                return false;
            }

            if (this.Name != other.Name)
            {
                return false;
            }

            switch (Discovery)
            {
                case Discovery.WebResource:
                case Discovery.CodeBlockInResource:
                    return ResourceSiteEquals(this._resourceType, other._resourceType);

                case Discovery.WebSiteFile:
                    return true;

                case Discovery.CodeBlockHere:
                    return (this.CodeBlock == other.CodeBlock);

                default:
                    return false;
            }
        }

        /// <summary>
        /// General identically comparison.
        /// </summary>
        /// <param name="obj">Another object.</param>
        /// <returns>Same as Equals(DeclareItem).</returns>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            DeclareItem other = obj as DeclareItem;

            return Equals(other);
        }

        /// <summary>
        /// Gets hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetHashCode(ItemType)
                ^ GetHashCode(Name)
                ^ GetResourceSiteHashCode(_resourceType)
                ^ GetHashCode(CodeBlock);
        }

        /// <summary>
        /// Overrides equality operator.
        /// </summary>
        /// <param name="left">One declare item.</param>
        /// <param name="right">Another declare item.</param>
        /// <returns>Same as Equals(DeclareItem).</returns>
        public static bool operator ==(DeclareItem left, DeclareItem right)
        {
            if (Object.ReferenceEquals(left, null) || Object.ReferenceEquals(null, right))
            {
                return Object.ReferenceEquals(left, right);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Overrides non-inequality operator.
        /// </summary>
        /// <param name="left">One declare item.</param>
        /// <param name="right">Another declare item.</param>
        /// <returns>Negative Equals(DeclareItem).</returns>
        public static bool operator !=(DeclareItem left, DeclareItem right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets resource type.
        /// </summary>
        /// <param name="service">Script service.</param>
        /// <returns>Type, if not specified, return service type.</returns>
        public Type GetResourceType(ScriptService service)
        {
            return (_resourceType != null) ? _resourceType : service.GetType();
        }

        /// <summary>
        /// Gets assembly resource resides.
        /// </summary>
        /// <param name="service">Script service.</param>
        /// <returns>Assembly resource resides.</returns>
        public Assembly GetLocation(ScriptService service)
        {
            return GetResourceType(service).Assembly;
        }

        #region Help Members

        /// <summary>
        /// Gets a object hash code.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>Object hash code.</returns>
        private int GetHashCode(object obj)
        {
            return (obj != null) ? obj.GetHashCode() : 0;
        }

        /// <summary>
        /// Checks declare item argument.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="discovery">Location type.</param>
        /// <param name="resourceType">Resource type.</param>
        /// <param name="codeBlock">Code block.</param>
        private void CheckDeclareItemArguments(
                string name,
                Discovery discovery,
                Type resourceType,
                string codeBlock
            )
        {
            switch (discovery)
            {
                case Discovery.WebResource:
                case Discovery.WebSiteFile:
                case Discovery.CodeBlockInResource:
                    if (String.IsNullOrEmpty(name))
                    {
                        throw new InvalidOperationException("Should provide resource name.");
                    }
                    break;

                case Discovery.CodeBlockHere:
                    break;

                default:
                    throw new InvalidOperationException(
                            String.Format("Cannot recognize discovery type.")
                        );
            }
        }

        /// <summary>
        /// Determines two resource locations whether same.
        /// </summary>
        /// <param name="left">One type.</param>
        /// <param name="right">Another type.</param>
        /// <returns>If same, true; otherwise, false.</returns>
        private static bool ResourceSiteEquals(Type left, Type right)
        {
            if ((left == null) || (right == null))
            {
                return Object.ReferenceEquals(left, right);
            }

            return Object.ReferenceEquals(left.Assembly, right.Assembly);
        }

        /// <summary>
        /// Gets resource site hash code.
        /// </summary>
        /// <param name="resourceType">Resource type.</param>
        /// <returns>Resource site hash code.</returns>
        private static int GetResourceSiteHashCode(Type resourceType)
        {
            return (resourceType != null) ? resourceType.Assembly.GetHashCode() : 0;
        }

        #endregion
    }
}