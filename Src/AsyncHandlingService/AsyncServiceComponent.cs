#region Write Log
/*==============================================================================
 * Function:        Service component; provided as the base class to extend.
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
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace CIPACE.Extension
{
    public abstract class AsyncServiceComponent
    {
        public object Parameter { get; internal set; }

        public HttpContext Context { get; internal set; }

        public HttpRequest Request { get { return Context.Request; } }

        public HttpResponse Response { get { return Context.Response; } }

        public HttpSessionState Session { get { return HttpContext.Current.Session; } }

        public abstract object Execute();

        internal void Initialize(HttpContext context, string paramStr)
        {
            this.Context = context;

            if (paramStr == null)
            {
                return;
            }

            Type paramType = GetParameterType();

            if (paramType == null)
            {
                throw new InvalidOperationException("Should use AsyncServiceAttribute on service class to indicate the parameter type.");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Parameter = serializer.Deserialize(paramStr, paramType);
        }

        protected T GetParameter<T>()
        {
            return (Parameter != null) ? (T)Parameter : default(T);
        }

        #region Help Methods

        protected virtual Type GetParameterType()
        {
            Type t = GetType();
            AsyncServiceAttribute attr = (AsyncServiceAttribute)Attribute.GetCustomAttribute(t, typeof(AsyncServiceAttribute));

            return (attr != null) ? attr.ParameterType : typeof(object);
        }

        #endregion
    }
}