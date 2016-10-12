#region Write Log
/*==============================================================================
 * Function:     Serialization utility.
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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace CIPACE.Extension
{
    internal static class SerializationUtility
    {
        /// <summary>
        /// "null".
        /// </summary>
        public const string NULL_LITERAL = "null";

        /// <summary>
        /// Encodes the name of the property (encode double quote).
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Encoded property name.</returns>
        public static string EncodePropertyName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            string result = name.Replace("\"", "\\\"");

            return result;
        }

        /// <summary>
        /// Pre-processes the JSON date.
        /// </summary>
        /// <param name="json">The JSON.</param>
        /// <returns>Substituted with JS Date type.</returns>
        public static string PreprocessJsonDate(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                return json;
            }

            Regex ex = new Regex(@"(?<!\\)""\\/(Date\(-?[0-9]+\))\\/(?<!\\)""");

            string result = ex.Replace(json, "new $1");

            return result;
        }

        /// <summary>
        /// Copies the service.
        /// </summary>
        /// <typeparam name="T">Type of script service.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The copy of source.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// When the source cannot support binary formatting.
        /// </exception>
        public static T CopyService<T>(T source) where T : ScriptService
        {
            Type serviceType = typeof(T);

            if (!serviceType.IsSerializable)
            {
                throw new InvalidOperationException(
                        String.Format(
                                "Type \"{0}\" is not serializable.",
                                serviceType.FullName
                            )
                    );
            }

            IFormatter f = new BinaryFormatter();

            using (Stream stream = new MemoryStream())
            {
                try
                {
                    f.Serialize(stream, source);
                }
                catch (SerializationException se)
                {
                    throw new InvalidOperationException(
                            String.Format(
                                    "Serialize type \"{0}\" failed. Please flag NonSerializedAttribute to its fields not supporting serializing function.",
                                    serviceType.FullName
                                ),
                            se
                        );
                }

                stream.Seek(0, SeekOrigin.Begin);

                T target = (T)f.Deserialize(stream);

                return target;
            }
        }
    }
}