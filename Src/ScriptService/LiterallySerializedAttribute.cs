#region Write Log
/*==============================================================================
 * Function:     Flag on data property of ScriptService, instruct serialize it literally.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CIPACE.Extension
{
    /// <summary>
    /// Flag on data property of ScriptService, instruct serialize it literally.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class LiterallySerializedAttribute : Attribute
    {
        private readonly Type _literalObjectType;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LiterallySerializedAttribute()
        {
        }

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        /// <param name="literalObjectType">Provide type.</param>
        public LiterallySerializedAttribute(Type literalObjectType)
        {
            _literalObjectType = literalObjectType;
        }

        /// <summary>
        /// Get object type.
        /// </summary>
        public Type LiteralObjectType
        {
            get { return _literalObjectType; }
        }

        /// <summary>
        /// Create ILiterallySerializable object (wrapper object).
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        /// <param name="name">Property name. This is optional</param>
        /// <returns>Wrapped object.</returns>
        public ILiterallySerializable Create(object value, string name)
        {
            ConstructorInfo ctor = GetConstructor();

            if (ctor != null)
            {
                ILiteralObject literal = (ILiteralObject)ctor.Invoke(null);

                literal.Value = value;

                literal.ValidateType();

                return literal;
            }
            else
            {
                return CreateDefault(value, name);
            }
        }

        /// <summary>
        /// Create wrapped object to serialize literally.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <param name="name">Data property name. This is optional.</param>
        /// <returns>Wrapped object.</returns>
        public static ILiterallySerializable CreateDefault(object value, string name)
        {
            ILiterallySerializable e = value as ILiterallySerializable;

            if (e != null)
            {
                return e;
            }

            return new LiteralObjectImpl(value, name);
        }

        #region Help Members

        private ConstructorInfo _literalConstructor;

        private ConstructorInfo GetConstructor()
        {
            if ((_literalConstructor == null) && (_literalObjectType != null))
            {
                if (!typeof(ILiteralObject).IsAssignableFrom(_literalObjectType))
                {
                    throw new InvalidOperationException("LiteralObjectType is not an ILiteralObject.");
                }

                _literalConstructor = _literalObjectType.GetConstructor(Type.EmptyTypes);

                if (_literalObjectType == null)
                {
                    throw new InvalidOperationException(
                            String.Format(
                                    "{0} should have a default constructor.",
                                    _literalObjectType
                                )
                        );
                }
            }

            return _literalConstructor;
        }


        private sealed class LiteralObjectImpl : LiteralObjectBase
        {
            private readonly string _name;

            public LiteralObjectImpl(object value, string name)
                : base(value)
            {
                _name = name;
            }

            public string Name
            {
                get { return _name; }
            }

            public override void ValidateType()
            {
                // Do nothing
            }

            public override string ToString()
            {
                LiteralNode current = new LiteralNode(Value, _name);

                return DoSerializeLiteral(Value, current);
            }

            #region Help Members

            private static string DoSerializeLiteral(object value, LiteralNode parent)
            {
                if (value == null)
                {
                    return SerializationUtility.NULL_LITERAL;
                }

                ILiterallySerializable e = value as ILiterallySerializable;

                if (e != null)
                {
                    return e.ToString();
                }

                IDictionary dictionary = value as IDictionary;

                if (dictionary != null)
                {
                    DictionaryLiterallyEmit dictionaryLiteral = new DictionaryLiterallyEmit(dictionary, parent);

                    return dictionaryLiteral.ToString();
                }

                IEnumerable<ILiterallySerializable> list = value as IEnumerable<ILiterallySerializable>;

                if (list != null)
                {
                    ListLiterallyEmit listLiteral = new ListLiterallyEmit(list, parent);

                    return listLiteral.ToString();
                }

                return value.ToString();
            }

            private sealed class ListLiterallyEmit : ILiterallySerializable
            {
                private readonly IEnumerable _list;
                private LiteralNode _previous;

                public ListLiterallyEmit(IEnumerable list)
                {
                    _list = list;
                }

                public ListLiterallyEmit(IEnumerable list, LiteralNode previous)
                    : this(list)
                {
                    _previous = previous;
                }

                public override string ToString()
                {
                    string result = String.Format(
                            "[{0}]",
                            String.Join(
                                ",",
                                from item in _list.Cast<object>()
                                select DoSerializeLiteral(item, (new LiteralNode(item)).LinkPrevious(_previous))
                            )
                        );

                    return result;
                }
            }

            private sealed class DictionaryLiterallyEmit : ILiterallySerializable
            {
                private readonly IDictionary _dictionary;
                private LiteralNode _previous;

                public DictionaryLiterallyEmit(IDictionary dictionary)
                {
                    _dictionary = dictionary;
                }

                public DictionaryLiterallyEmit(IDictionary dictionary, LiteralNode previous)
                    : this(dictionary)
                {
                    _previous = previous;
                }

                public override string ToString()
                {
                    StringBuilder buffer = new StringBuilder();

                    foreach (DictionaryEntry entry in _dictionary)
                    {
                        if (buffer.Length != 0)
                        {
                            buffer.Append(",");
                        }

                        string name;

                        try
                        {
                            name = (string)entry.Key;
                        }
                        catch (InvalidCastException ex)
                        {
                            throw new InvalidOperationException(
                                    "The dictionary to be serialized literally which key is not string.",
                                    ex
                                );
                        }

                        buffer.AppendFormat("\"{0}\":", SerializationUtility.EncodePropertyName(name));

                        object value = entry.Value;
                        LiteralNode current = new LiteralNode(value, name);

                        current.LinkPrevious(_previous);

                        buffer.Append(DoSerializeLiteral(value, current));
                    }

                    buffer.Insert(0, "{");
                    buffer.Append("}");

                    return buffer.ToString();
                }
            }

            private sealed class LiteralNode
            {
                private readonly string _name;
                private readonly object _value;

                private LiteralNode _previous;

                public LiteralNode(object value)
                {
                    _value = value;
                }

                public LiteralNode(object value, string name)
                    : this(value)
                {
                    _name = name;
                }

                public string Name
                {
                    get { return _name; }
                }

                public object Value
                {
                    get { return _value; }
                }

                public LiteralNode Previous
                {
                    get { return _previous; }
                }

                public LiteralNode LinkPrevious(LiteralNode previous)
                {
                    CheckCircularReference(previous);

                    _previous = previous;

                    return this;
                }

                public override string ToString()
                {
                    if (!String.IsNullOrWhiteSpace(Name))
                    {
                        return Name;
                    }
                    else if (Value != null)
                    {
                        return Value.ToString();
                    }
                    else
                    {
                        return SerializationUtility.NULL_LITERAL;
                    }
                }

                #region Help Members

                private void CheckCircularReference(LiteralNode previous)
                {
                    if ((previous == null) || (previous.Value == null) || (this.Value == null))
                    {
                        return;
                    }

                    List<LiteralNode> list = new List<LiteralNode>();

                    while (previous != null)
                    {
                        if (previous.Value != null)
                        {
                            list.Insert(0, previous);
                        }

                        previous = previous.Previous;
                    }

                    int index = -1;

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (Object.ReferenceEquals(this.Value, list[i].Value))
                        {
                            index = i;

                            break;
                        }
                    }

                    if (index == -1)
                    {
                        return;
                    }

                    list.Add(this);

                    StringBuilder buffer = new StringBuilder();

                    for (int i = 0; i < list.Count; i++)
                    {
                        LiteralNode node = list[i];

                        if (i != 0)
                        {
                            buffer.Append(" -> ");
                        }

                        if ((i == index) || (i == (list.Count - 1)))
                        {
                            buffer.AppendFormat("[{0}]", node);
                        }
                        else
                        {
                            buffer.Append(node);
                        }
                    }

                    throw new InvalidOperationException(
                            String.Format(
                                    "Circular reference is found when serialize literal data member:\r\n{0}",
                                    buffer
                                )
                        );
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}