#region Write Log
/*==============================================================================
 * Function:         Asychronous service. It compose of multiple methods.
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 1/26/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CIPACE.Extension
{
    public abstract class AsyncDispatchableServiceComponent : AsyncServiceComponent
    {
        public sealed override object Execute()
        {
            InputData inputs = GetParameter<InputData>();

            object result = inputs.Invoke(this);

            return result;
        }

        protected override Type GetParameterType()
        {
            return typeof(InputData);
        }
    }


    [Serializable]
    internal sealed class InputData
    {
        /// <summary>
        /// Method name.
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// Method serialized args.
        /// </summary>
        public string args { get; set; }

        public object Invoke(AsyncServiceComponent serviceComponent)
        {
            MethodInfo methodInfo = Dispatch(serviceComponent);
            ParameterInfo parameterInfo = methodInfo.GetParameters()[0];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object parameter = serializer.Deserialize(args, parameterInfo.ParameterType);

            object result = methodInfo.Invoke(serviceComponent, new object[] { parameter });

            return result;
        }

        #region Help Members

        private MethodInfo Dispatch(AsyncServiceComponent serviceComponent)
        {
            Type type = serviceComponent.GetType();
            MethodInfo[] all = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<MethodInfo> candidates = from method in all where HasOnlyOneParameter(method) select method;

            Dictionary<MethodMatchLevel, IEnumerable<MethodInfo>> results = new Dictionary<MethodMatchLevel, IEnumerable<MethodInfo>>();

            results.Add(MethodMatchLevel.FlagExactly, from method in candidates where IsFlaggedExactly(method) select method);
            results.Add(MethodMatchLevel.FlagCaseInsensitively, from method in candidates where IsFlaggedCaseInsensitively(method) select method);
            results.Add(MethodMatchLevel.NameMatchExactly, from method in candidates where IsNameMatchExactly(method) select method);
            results.Add(MethodMatchLevel.NameMatchCaseInsensitively, from method in candidates where IsNameMatchCaseInsensitively(method) select method);

            Dictionary<MethodMatchLevel, string> messages = new Dictionary<MethodMatchLevel, string>();

            messages.Add(MethodMatchLevel.FlagExactly, String.Format("There are more than one methods flagged exactly with name: {0} in type {1}.", this.method, type.FullName));
            messages.Add(MethodMatchLevel.FlagCaseInsensitively, String.Format("There are more than one methods flagged case insensitively with name: {0} in type {1}.", this.method, type.FullName));
            messages.Add(MethodMatchLevel.NameMatchExactly, String.Format("There are more than one methods owned name {0} exactly in type {1}.", this.method, type.FullName));
            messages.Add(MethodMatchLevel.NameMatchCaseInsensitively, String.Format("There are more than one methods owned name {0} case insensitively in type {1}.", this.method, type.FullName));

            MethodMatchLevel[] levels = 
                {
                    MethodMatchLevel.FlagExactly,
                    MethodMatchLevel.FlagCaseInsensitively,
                    MethodMatchLevel.NameMatchExactly,
                    MethodMatchLevel.NameMatchCaseInsensitively
                };

            foreach (MethodMatchLevel level in levels)
            {
                List<MethodInfo> methods = results[level].ToList();

                if (methods.Count == 1)
                {
                    return methods[0];
                }

                if (methods.Count > 1)
                {
                    throw new InvalidOperationException(messages[level]);
                }
            }

            throw new InvalidOperationException(String.Format("Cannot find method {0} in type {1}.", this.method, type.FullName));
        }

        private bool HasOnlyOneParameter(MethodInfo method)
        {
            return (method.GetParameters().Length == 1);
        }

        private bool IsFlaggedExactly(MethodInfo method)
        {
            DispatchedMethodAttribute attr = (DispatchedMethodAttribute)Attribute.GetCustomAttribute(method, typeof(DispatchedMethodAttribute));

            return (attr != null) && (String.Equals(attr.Name, this.method, StringComparison.Ordinal));
        }

        private bool IsFlaggedCaseInsensitively(MethodInfo method)
        {
            DispatchedMethodAttribute attr = (DispatchedMethodAttribute)Attribute.GetCustomAttribute(method, typeof(DispatchedMethodAttribute));

            return (attr != null) && (String.Equals(attr.Name, this.method, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsNameMatchExactly(MethodInfo method)
        {
            return String.Equals(method.Name, this.method, StringComparison.Ordinal);
        }

        private bool IsNameMatchCaseInsensitively(MethodInfo method)
        {
            return String.Equals(method.Name, this.method, StringComparison.OrdinalIgnoreCase);
        }


        private enum MethodMatchLevel
        {
            FlagExactly = 1,

            FlagCaseInsensitively = 2,

            NameMatchExactly = 3,

            NameMatchCaseInsensitively = 4
        }

        #endregion
    }
}