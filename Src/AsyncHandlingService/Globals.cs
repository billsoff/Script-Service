#region Write Log
/*==============================================================================
 * Function:        Provide tool methods.
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 9/12/2014
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CIPACE.Extension
{
    internal static class Globals
    {
        public static string GetTypeQualifiedName(Type type)
        {
            return (type != null) ? String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name) : null;
        }

        public static Type FindServiceComponentFromCurrentAppDomain(string typeQualifiedName)
        {
            string[] results = Regex.Split(typeQualifiedName, @",\s*", RegexOptions.IgnorePatternWhitespace);

            if (results.Length != 2)
            {
                throw new InvalidOperationException(
                        String.Format("Service component type qualified name \"{0}\" is invalid", typeQualifiedName)
                    );
            }

            string typeFullName = results[0];
            string assemblyName = results[1];

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!String.Equals(assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (String.Equals(type.FullName, typeFullName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (typeof(AsyncServiceComponent).IsAssignableFrom(type))
                        {
                            return type;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return null;
        }
    }
}