#region Write Log
/*==============================================================================
 * Function:     Help class to parse script service.
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
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CIPACE.Extension
{
    internal sealed class ServiceInfo
    {
        #region Static Members

        /// <summary>
        /// Builds the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>The service information.</returns>
        public static ServiceInfo Build(ScriptService service)
        {
            ServiceInfo info = new ServiceInfo();

            AbstractProperties(service, info);
            AppendAdditionalDataProperties(service, info);

            return info;
        }

        /// <summary>
        /// Abstracts the properties.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="info">The information.</param>
        private static void AbstractProperties(ScriptService service, ServiceInfo info)
        {
            Type t = service.GetType();

            foreach (PropertyInfo p in t.GetProperties())
            {
                if (!p.CanRead)
                {
                    continue;
                }

                if (Attribute.IsDefined(p, typeof(NonDataPropertyAttribute)))
                {
                    continue;
                }

                if (Attribute.IsDefined(p, typeof(DefaultMemberAttribute)))
                {
                    continue;
                }

                DataPropertyAttribute attr = (DataPropertyAttribute)Attribute.GetCustomAttribute(
                        p,
                        typeof(DataPropertyAttribute)
                    );
                string dataMemberName = (attr != null) ? attr.Name : p.Name;

                CheckMemberNameDuplication(info, dataMemberName);

                object value = p.GetValue(service, null);

                if ((attr != null) && attr.Required)
                {
                    info._requiredMembers.Add(dataMemberName, value);
                }

                LiterallySerializedAttribute literalAttr;
                bool isLiteralMember = IsLiteralMember(p, out literalAttr);
                bool isService = IsService(p.PropertyType);

                if (isLiteralMember)
                {
                    info._literalMembers.Add(
                            dataMemberName,
                            (literalAttr != null)
                            ? literalAttr.Create(value, dataMemberName)
                            : LiterallySerializedAttribute.CreateDefault(value, dataMemberName)
                        );

                    if (isService)
                    {
                        info._childServices.AddRange(AbstractServices(value));
                    }
                }
                else
                {
                    info._dataMembers.Add(dataMemberName, value);
                }
            }
        }

        /// <summary>
        /// Appends the additional data properties.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="info">The information.</param>
        private static void AppendAdditionalDataProperties(ScriptService service, ServiceInfo info)
        {
            IList<DataProperty> additionalDataProperties = service.GetAdditionalDataProperties();

            if (additionalDataProperties == null)
            {
                return;
            }

            foreach (DataProperty p in additionalDataProperties)
            {
                CheckMemberNameDuplication(info, p.Name);

                ILiterallySerializable literal = p.TryConvertToLiteralObject();

                if (literal != null)
                {
                    info._literalMembers.Add(p.Name, literal);

                    ScriptService childService = p.Value as ScriptService;

                    if (childService != null)
                    {
                        info._childServices.Add(childService);
                    }
                }
                else
                {
                    info._dataMembers.Add(p.Name, p.Value);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property type is service.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns></returns>
        private static bool IsService(Type propertyType)
        {
            return typeof(ScriptService).IsAssignableFrom(propertyType)
                || typeof(IEnumerable<ScriptService>).IsAssignableFrom(propertyType);
        }

        /// <summary>
        /// Checks the member name duplication.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="dataMemberName">Name of the data member.</param>
        /// <exception cref="System.InvalidOperationException">
        /// If the member name is duplicate.
        /// </exception>
        private static void CheckMemberNameDuplication(ServiceInfo info, string dataMemberName)
        {
            if (info._allDataProperties.Contains(dataMemberName))
            {
                throw new InvalidOperationException(
                        String.Format("\"{0}\" is declared more than once.", dataMemberName)
                    );
            }

            info._allDataProperties.Add(dataMemberName);
        }

        /// <summary>
        /// Determines whether is literal member by the specified property.
        /// </summary>
        /// <param name="p">The property information.</param>
        /// <param name="literalAttr">The literal attribute.</param>
        /// <returns><value>true</value> if it is literal; otherwise <value>false</value></returns>
        private static bool IsLiteralMember(PropertyInfo p, out LiterallySerializedAttribute literalAttr)
        {
            Type propertyType = p.PropertyType;
            literalAttr = (LiterallySerializedAttribute)Attribute.GetCustomAttribute(
                    p,
                    typeof(LiterallySerializedAttribute)
                );

            if (literalAttr != null)
            {
                if (typeof(IDictionary).IsAssignableFrom(propertyType))
                {
                    EnsureKeyIsString(
                            propertyType,
                            "Literal member is dictionary and its key should be string."
                        );
                }

                return true;
            }

            if (typeof(ILiterallySerializable).IsAssignableFrom(propertyType)
                || typeof(IEnumerable<ILiterallySerializable>).IsAssignableFrom(propertyType))
            {
                return true;
            }

            if (propertyType.IsGenericType)
            {
                Type typeDef = propertyType.GetGenericTypeDefinition();

                if (typeof(IDictionary).IsAssignableFrom(typeDef))
                {
                    Type[] typeArgs = propertyType.GetGenericArguments();

                    if ((typeArgs[0] == typeof(string)) && typeof(ILiterallySerializable).IsAssignableFrom(typeArgs[1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Ensures the key is string.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="errMsg">The error MSG.</param>
        /// <exception cref="System.InvalidOperationException">If the key is not string type.</exception>
        private static void EnsureKeyIsString(Type propertyType, string errMsg)
        {
            if (!typeof(IDictionary).IsAssignableFrom(propertyType)
                || !propertyType.IsGenericType)
            {
                return;
            }

            Type[] typeArgs = propertyType.GetGenericArguments();

            if ((typeArgs.Length != 0) && (typeArgs[0] != typeof(string)))
            {
                throw new InvalidOperationException(errMsg);
            }
        }

        /// <summary>
        /// Abstracts the services.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot abstract service.</exception>
        private static IEnumerable<ScriptService> AbstractServices(object value)
        {
            if (value == null)
            {
                return Enumerable.Empty<ScriptService>();
            }

            ScriptService service = value as ScriptService;

            if (service != null)
            {
                return new ScriptService[] { service };
            }

            IEnumerable<ScriptService> results = value as IEnumerable<ScriptService>;

            if (results != null)
            {
                return results;
            }

            // If we go here, error occurs
            throw new InvalidOperationException("Cannot abstract service.");
        }

        #endregion

        #region Fields

        private readonly List<ScriptService> _childServices = new List<ScriptService>();

        private readonly List<string> _allDataProperties = new List<string>();
        private readonly Dictionary<string, object> _requiredMembers = new Dictionary<string, object>();

        // key: data property name; value: data property value
        private readonly Dictionary<string, object> _dataMembers = new Dictionary<string, object>();

        // key: data property name; value: IEmitLiterally (i.e. function or service creation statement)
        private readonly Dictionary<string, ILiterallySerializable> _literalMembers = new Dictionary<string, ILiterallySerializable>();

        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceInfo"/> class from being created.
        /// </summary>
        private ServiceInfo()
        {
        }

        /// <summary>
        /// Gets the child services.
        /// </summary>
        /// <value>
        /// The child services.
        /// </value>
        public IList<ScriptService> ChildServices
        {
            get { return _childServices; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        public void Validate()
        {
            foreach (KeyValuePair<string, object> kvp in _requiredMembers)
            {
                if (kvp.Value == null)
                {
                    throw new InvalidOperationException(String.Format("{0} is required.", kvp.Key));
                }

                object emptyValue = GetEmptyValue(Convert.GetTypeCode(kvp.Value));

                if (kvp.Value is string)
                {
                    if (((string)kvp.Value).Trim().Equals(emptyValue))
                    {
                        throw new InvalidOperationException(String.Format("{0} is required.", kvp.Key));
                    }
                }
                else if (kvp.Value.Equals(emptyValue))
                {
                    throw new InvalidOperationException(String.Format("{0} is required.", kvp.Key));
                }
            }
        }

        /// <summary>
        /// Gets the empty value.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        /// <returns>Empty value to the type.</returns>
        private object GetEmptyValue(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte:
                    return 0;

                case TypeCode.Char:
                    return 0;

                case TypeCode.Decimal:
                    return 0M;

                case TypeCode.Double:
                    return 0.0;

                case TypeCode.Int16:
                    return 0;

                case TypeCode.Int32:
                    return 0;

                case TypeCode.Int64:
                    return 0L;

                case TypeCode.SByte:
                    return 0;

                case TypeCode.Single:
                    return 0.0F;

                case TypeCode.UInt16:
                    return 0;

                case TypeCode.UInt32:
                    return 0;

                case TypeCode.UInt64:
                    return 0L;

                case TypeCode.DateTime:
                    return DateTime.MinValue;

                case TypeCode.DBNull:
                    return DBNull.Value;

                case TypeCode.String:
                    return String.Empty;

                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.Boolean:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Determines whether this instance has data.
        /// </summary>
        /// <returns><value>true</value> if has data; otherwise <value>false</value></returns>
        public bool HasData()
        {
            return HasDataMembers() || HasLiteralMembers();
        }

        /// <summary>
        /// Determines whether [has data members].
        /// </summary>
        /// <returns></returns>
        public bool HasDataMembers()
        {
            return (_dataMembers.Count != 0);
        }

        public bool HasServiceMembers()
        {
            return (_childServices.Count != 0);
        }

        public bool HasLiteralMembers()
        {
            return (_literalMembers.Count != 0);
        }

        public string SerializeData(JavaScriptSerializer serializer)
        {
            if (serializer == null)
            {
                serializer = new JavaScriptSerializer();
            }

            bool hasDataMembers = HasDataMembers();
            bool hasLiteralMembers = HasLiteralMembers();
            string initialData;

            if (hasDataMembers && hasLiteralMembers)
            {
                string data = SerializeDataMember(serializer);
                string literal = SerializeLiteralMember();

                initialData = MergeJson(data, literal);
            }
            else if (hasDataMembers)
            {
                string data = SerializeDataMember(serializer);

                initialData = String.Format(
                       "{0}",
                       data
                   );
            }
            else if (hasLiteralMembers)
            {
                initialData = SerializeLiteralMember();
            }
            else
            {
                initialData = String.Empty;
            }

            return initialData;
        }

        public string SerializeDataMember(JavaScriptSerializer serializer)
        {
            if (serializer == null)
            {
                serializer = new JavaScriptSerializer();
            }

            string result = SerializationUtility.PreprocessJsonDate(
                    serializer.Serialize(_dataMembers)
                );

            return result;
        }

        public string SerializeLiteralMember()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (KeyValuePair<string, ILiterallySerializable> kvp in _literalMembers)
            {
                if (buffer.Length != 0)
                {
                    buffer.Append(",");
                }

                string name = kvp.Key;

                buffer.AppendFormat(
                        "\"{0}\":{1}",
                        SerializationUtility.EncodePropertyName(name),
                        kvp.Value.ToString()
                    );
            }

            buffer.Insert(0, "{");
            buffer.Append("}");

            return buffer.ToString();
        }

        #region Help Members

        private static string MergeJson(string left, string right)
        {
            if (String.IsNullOrEmpty(left))
            {
                return right;
            }
            else if (String.IsNullOrEmpty(right))
            {
                return left;
            }

            string leftBody = GetJsonBody(left);
            string rightBody = GetJsonBody(right);
            string result = "{" + leftBody + "," + rightBody + "}";

            return result;
        }

        private static string GetJsonBody(string json)
        {
            Regex ex = new Regex(@"^\{(.+)\}$", RegexOptions.IgnorePatternWhitespace);
            Match m = ex.Match(json);
            string body = m.Success ? m.Groups[1].Value : String.Empty;

            return body;
        }

        #endregion
    }
}