#region Write Log
/*==============================================================================
 * Function:        Execute service request.
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
using System.Web;
using System.Web.Script.Serialization;

namespace CIPACE.Extension
{
    public static class AsyncServiceComponentExecutor
    {
        public static string Execute(HttpContext context, string type, string paramStr)
        {
            string result;

            try
            {
                object returnValue = DoExecute(context, type, paramStr);
                ExecutionResult wrapper = new ExecutionResult(new ReturnValueWrapper(returnValue));

                result = wrapper.Serialize();
            }
            catch (Exception ex)
            {
                ExecutionResult wrpper = new ExecutionResult(new Error(ex));
                result = wrpper.Serialize();
            }

            return result;
        }

        #region Help Methods

        private static object DoExecute(HttpContext context, string type, string paramStr)
        {
            AsyncServiceComponent serviceComponent = CreateServiceComponent(type);
            serviceComponent.Initialize(context, paramStr);
            object result = serviceComponent.Execute();

            return result;
        }

        private static AsyncServiceComponent CreateServiceComponent(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "Service type cannot be null.");
            }

            Type t = Type.GetType(type);

            // If assembly loader resolve failed, look for it directly in current application domain
            if (t == null)
            {
                t = Globals.FindServiceComponentFromCurrentAppDomain(type);
            }

            if (t == null)
            {
                throw new InvalidOperationException(String.Format("Cannot find type \"{0}\"", type));
            }

            if (!typeof(AsyncServiceComponent).IsAssignableFrom(t))
            {
                throw new InvalidOperationException(String.Format("Service type \"{0}\" should inherit from GeneralServiceBase.", type));
            }

            ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);

            if (ctor == null)
            {
                throw new InvalidOperationException(String.Format("Service type \"{0}\" has not a public parametless constructor.", type));
            }

            AsyncServiceComponent serviceComponent = (AsyncServiceComponent)ctor.Invoke(null);

            return serviceComponent;
        }

        #endregion


        #region Classes

        [Serializable]
        private sealed class ExecutionResult
        {
            private readonly bool _success;
            private readonly ReturnValueWrapper _result;
            private readonly Error _error;

            public ExecutionResult(ReturnValueWrapper returnValue)
            {
                _success = true;
                _result = returnValue;
            }

            public ExecutionResult(Error error)
            {
                _error = error;
            }

            public bool Success
            {
                get { return _success; }
            }

            public object ReturnValue
            {
                get { return (_result != null) ? _result.Value : null; }
            }

            public Error Error
            {
                get { return _error; }
            }

            public string Serialize()
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string result = serializer.Serialize(this);

                return result;
            }
        }

        [Serializable]
        private sealed class ReturnValueWrapper
        {
            private readonly object _value;

            public ReturnValueWrapper(object returnValue)
            {
                _value = returnValue;
            }

            public object Value
            {
                get { return _value; }
            }
        }

        [Serializable]
        private sealed class Error
        {
            private readonly string _message;
            private readonly string _stack;
            private readonly string _detail;

            public Error(Exception ex)
            {
                _message = ex.Message;
                _stack = ex.StackTrace;
                _detail = ex.ToString();
            }

            public string Message
            {
                get { return _message; }
            }

            public string Stack
            {
                get { return _stack; }
            }

            public string Detail
            {
                get { return _detail; }
            }
        }

        #endregion
    }
}