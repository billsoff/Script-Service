#region Write Log
/*==============================================================================
 * Function:     Responsible for deploy, startup client script.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIPACE.Extension
{
    /// <summary>
    /// Responsible for deploy, startup client script.
    /// </summary>
    [Serializable]
    public abstract class ScriptService : ILiterallySerializable, ICloneable
    {
        #region Static Members

        /// <summary>
        /// Create a general script service.
        /// </summary>
        /// <param name="serviceGuid">Service GUID. This is required.</param>
        /// <param name="serviceName">Service name. This is required.</param>
        /// <param name="scriptName">Script name. This is required.</param>
        /// <param name="discovery">Instruct where to locate the source. This is required.</param>
        /// <param name="resourceType">Resource type (resource is embedded into its assembly). This is ignored if discovery is CodeBlockHere and codeBlock is provided.</param>
        /// <param name="codeBlock">Code block to deploy.</param>
        /// <param name="addScriptTags">Indicate whether to add script tags when deploy.</param>
        /// <returns>Script service created.</returns>
        public static ScriptService Create(
                Guid serviceGuid,
                string serviceName,
                string scriptName,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addScriptTags = true
            )
        {
            ServiceGuidAttribute guidAttr = new ServiceGuidAttribute(serviceGuid);

            guidAttr.Name = serviceName;

            DeclareJavaScriptItem script = new DeclareJavaScriptItem(
                    scriptName,
                    discovery,
                    resourceType,
                    codeBlock,
                    addScriptTags
                );

            GeneralScriptService service = new GeneralScriptService(guidAttr, script);

            return service;
        }


        /// <summary>
        /// General implementation for script service.
        /// </summary>
        [Serializable]
        private sealed class GeneralScriptService : ScriptService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GeneralScriptService" /> class.
            /// </summary>
            /// <param name="guidAttr">The unique identifier attribute.</param>
            /// <param name="script">The script.</param>
            public GeneralScriptService(ServiceGuidAttribute guidAttr, DeclareJavaScriptItem script)
                : base(guidAttr, script)
            {
            }
        }

        #endregion

        #region Fields

        private ServiceGuidAttribute _serviceGuid;
        private DeclareJavaScriptItem _script;

        private string _instanceId;
        private string _invokeKey;

        private List<DataProperty> _additionalDataProperties;

        private List<DeclareJavaScriptItem> _additonalPrerequisitelJavaScripts;
        private List<DeclareStylesheetItem> _additionalPrerequisiteStylesheets;

        [NonSerialized]
        private Control _stylesheetContainer;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptService"/> class.
        /// </summary>
        protected ScriptService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptService"/> class.
        /// </summary>
        /// <param name="serviceGuid">The service unique identifier.</param>
        /// <param name="script">The script.</param>
        private ScriptService(ServiceGuidAttribute serviceGuid, DeclareJavaScriptItem script)
        {
            _serviceGuid = serviceGuid;
            _script = script;
        }

        #region Public Events

        /// <summary>
        /// This event is raised before invoke the script service. The subscriber is given an opportunity to cancel the invocation.
        /// </summary>
        public event EventHandler<PrepareInvokeEventArgs> PrepareInvoke;

        /// <summary>
        /// This event is raised before initialize the current script service.
        /// </summary>
        public event EventHandler<PrepareInitializeEventArgs> PrepareInitialize;

        /// <summary>
        /// This event is raised before declare the script.
        /// </summary>
        public event EventHandler<PrepareDeclareEventArgs> PrepareDeclare;

        /// <summary>
        /// This event is raised before validate.
        /// </summary>
        public event EventHandler<PrepareValidateEventArgs> PrepareValidate;

        /// <summary>
        /// This event is raised before serialize data. The subscriber can adjust serializer behavior.
        /// </summary>
        public event EventHandler<PrepareSerializeDataEventArgs> PrepareSerializeData;

        /// <summary>
        /// This event is raised after serialize data.
        /// </summary>
        public event EventHandler CompleteSerializeData;

        /// <summary>
        /// This event is raised after validate.
        /// </summary>
        public event EventHandler<CompleteValidateEventArgs> CompleteValidate;

        /// <summary>
        /// This event is raised after initialize.
        /// </summary>
        public event EventHandler<CompleteInitializeEventArgs> CompleteInitialize;

        /// <summary>
        /// This event is raised after declare.
        /// </summary>
        public event EventHandler<CompleteDeclareEventArgs> CompleteDeclare;

        /// <summary>
        /// This event is raised after invoke.
        /// </summary>
        public event EventHandler<CompleteInvokeEventArgs> CompleteInvoke;

        #endregion

        /// <summary>
        /// Adds a data property. This data property will be transmitted to script function.
        /// </summary>
        /// <param name="name">Property name. This is required and unique.</param>
        /// <param name="value">Property value.</param>
        /// <param name="literallySerialize">Instruct whether to literally serialize. This is optional. The default value is false.</param>
        /// <returns>Current script service.</returns>
        public ScriptService AddProperty(string name, object value, bool literallySerialize = false)
        {
            EnsureAdditionalDataPropertiesStorageCreated();

            bool exists = _additionalDataProperties.Any(
                    p => String.Equals(p.Name, name, StringComparison.Ordinal)
                );

            if (exists)
            {
                throw new InvalidOperationException(
                        String.Format(
                                "Property \"{0}\" already exists",
                                name
                            )
                    );
            }

            _additionalDataProperties.Add(new DataProperty(name, value, literallySerialize));

            return this;
        }

        /// <summary>
        /// Adds data properties.
        /// </summary>
        /// <param name="properties">Property collection. Each name should be unique. If the value implements ILiteralObject, it is serialized literally.</param>
        /// <returns>Current script service.</returns>
        public ScriptService AddProperties(IDictionary<string, object> properties)
        {
            if ((properties == null) || (properties.Count == 0))
            {
                return this;
            }

            foreach (KeyValuePair<string, object> kvp in properties)
            {
                AddProperty(kvp.Key, kvp.Value);
            }

            return this;
        }

        /// <summary>
        /// Adds prerequisite java script. (Each is guaranteed to deploy only once)
        /// </summary>
        /// <param name="name">Java script name. (If it's an embedded resource, it's the name)</param>
        /// <param name="discovery">Instruct how to find the resource.</param>
        /// <param name="resourceType">Confirm the assembly to locate the resource.</param>
        /// <param name="codeBlock">Code block to deploy. This is optional. (Only discovery is CodeBlockHere, this parameter is used)</param>
        /// <param name="addScriptTags">Instruct whether to add script tags when deploy.</param>
        /// <returns>Current script service.</returns>
        public ScriptService AddPrerequisiteJavaScript(
                string name,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addScriptTags = true
            )
        {
            EnsureAdditionalPrerequisiteJavaScriptsStorageCreated();

            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    name,
                    discovery,
                    resourceType,
                    codeBlock,
                    addScriptTags
                );

            _additonalPrerequisitelJavaScripts.Add(item);

            return this;
        }

        /// <summary>
        /// Adds prerequisite style sheet.
        /// </summary>
        /// <param name="name">Style sheet name. (If this is an embedded resource, it's the name)</param>
        /// <param name="discovery">Instruct how to find the resource.</param>
        /// <param name="resourceType">Confirm the assembly to locate the resource.</param>
        /// <param name="codeBlock">Code block to deploy. This is optional. (Only discovery is CodeBlockHere, this parameter is used)</param>
        /// <param name="addStyleTags">Instruct whether to add style tag.</param>
        /// <returns>Current script service.</returns>
        public ScriptService AddPrerequisiteStylesheet(
                string name,
                Discovery discovery,
                Type resourceType = null,
                string codeBlock = null,
                bool addStyleTags = true
            )
        {
            EnsureAdditionalPrerequisiteStylesheetsStorageCreated();

            DeclareStylesheetItem item = new DeclareStylesheetItem(
                    name,
                    discovery,
                    resourceType,
                    codeBlock,
                    addStyleTags
                );

            _additionalPrerequisiteStylesheets.Add(item);

            return this;
        }

        /// <summary>
        /// Clears additional properties. (i.e. added by AddProperty or AddProperties method)
        /// </summary>
        /// <returns>Current script service.</returns>
        public ScriptService ClearAdditionalProperties()
        {
            if (_additionalDataProperties != null)
            {
                _additionalDataProperties.Clear();
            }

            return this;
        }

        /// <summary>
        /// Clears additional prerequisite java script. (i.e. added by AddPrerequisiteJavaScript method)
        /// </summary>
        /// <returns>Current script service.</returns>
        public ScriptService ClearAdditionalPrerequisiteJavaScripts()
        {
            if (_additonalPrerequisitelJavaScripts != null)
            {
                _additonalPrerequisitelJavaScripts.Clear();
            }

            return this;
        }

        /// <summary>
        /// Clears additional prerequisite style sheet. (i.e. added by AddPrerequisiteStylesheet method)
        /// </summary>
        /// <returns>Current script service.</returns>
        public ScriptService ClearAdditionalPrerequisiteStylesheets()
        {
            if (_additionalPrerequisiteStylesheets != null)
            {
                _additionalPrerequisiteStylesheets.Clear();
            }

            return this;
        }

        /// <summary>
        /// Sets style sheet container. Generally, style is placed in header. If no header is found, then is placed at the begin of the form; this may occur error, so the client can specified a container to place the style.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>Current script service.</returns>
        public ScriptService SetStylesheetContainer(Control container)
        {
            _stylesheetContainer = container;

            return this;
        }

        /// <summary>
        /// Gets service ID (GUID).
        /// </summary>
        /// <returns>Current script service.</returns>
        public Guid GetServiceId()
        {
            ServiceGuidAttribute attr = GetServiceGuid();

            return attr.ServiceId;
        }

        /// <summary>
        /// Gets service declare key. The service script is guaranteed only declare once per response (no deployment is made for asynchronous response).
        /// </summary>
        /// <returns>Current script service.</returns>
        public string GetDeclareKey()
        {
            return GetServiceId().ToString().ToUpper();
        }

        /// <summary>
        /// Gets invoke key. Generally, for specified target, the invoke is occurred only once per response. You can change invoke key by call SetInvokeKey to modify the behavior.
        /// </summary>
        /// <param name="target">Invoke target control.</param>
        /// <returns>Current script service.</returns>
        public string GetInvokeKey(Control target)
        {
            if (_invokeKey == null)
            {
                _invokeKey = String.Format("{0}/{1}", GetDeclareKey(), target.ClientID);
            }

            return _invokeKey;
        }

        /// <summary>
        /// Sets invoke key.
        /// </summary>
        /// <param name="invokeKey">Invoke key.</param>
        /// <returns>Current script service.</returns>
        public ScriptService SetInvokeKey(string invokeKey)
        {
            _invokeKey = invokeKey;

            return this;
        }

        /// <summary>
        /// Clones current script service. The cloned script service is same as current except instance ID.
        /// </summary>
        /// <param name="deep">Instruct whether to deep clone. Default false. (If you want to deep clone, your script service should support binary serialization)</param>
        /// <returns>Current script service.</returns>
        public ScriptService Clone(bool deep = false)
        {
            ScriptService target;

            if (deep)
            {
                target = SerializationUtility.CopyService<ScriptService>(this);

                target.Reset();
            }
            else
            {
                ICloneable clonable = this;
                target = (ScriptService)clonable.Clone();
            }

            return target;
        }

        /// <summary>
        /// Gets service name.
        /// </summary>
        /// <returns>Service name.</returns>
        public string GetServiceName()
        {
            ServiceGuidAttribute attr = GetServiceGuid();

            return !String.IsNullOrEmpty(attr.Name) ? attr.Name : GetDeclareKey();
        }

        /// <summary>
        /// Gets service reference (i.e. client function reference).
        /// </summary>
        /// <returns>Service reference.</returns>
        public string GetServiceReference()
        {
            return String.Format(
                    "window['{0}']",
                    GetDeclareKey()
                );
        }

        /// <summary>
        /// Gets service instance (i.e. client invoke statement).
        /// </summary>
        /// <param name="suppressEventNotification">Instruct whether to suppress event raising.</param>
        /// <returns>Service instance.</returns>
        public string GetServiceInstance(bool suppressEventNotification = false)
        {
            ServiceInfo info = ServiceInfo.Build(this);

            info.Validate();

            PrepareSerializeDataEventArgs arguments = new PrepareSerializeDataEventArgs();

            if (!suppressEventNotification)
            {
                OnPrepareSerialzieData(arguments);
            }

            JavaScriptSerializer serializer = arguments.CreateSerializer();
            string context = info.SerializeData(serializer);

            string invokeBlock = String.Format(
                        "{0}{1}({2})",
                        GetServiceReference(),
                        !String.IsNullOrEmpty(context) ? ".call" : String.Empty,
                        context
                    );

            if (!suppressEventNotification)
            {
                OnCompleteSerializeData(EventArgs.Empty);
            }

            return invokeBlock;
        }

        /// <summary>
        /// Gets service instance ID. This is unique for every instance.
        /// </summary>
        /// <returns>Service instance ID.</returns>
        public string GetServiceInstanceId()
        {
            if (_instanceId == null)
            {
                _instanceId = Guid.NewGuid().ToString().ToUpper();
            }

            return _instanceId;
        }

        /// <summary>
        /// Gets all service instances (including current instance).
        /// </summary>
        /// <returns>All service instance sequence (begin from very leaf).</returns>
        public IEnumerable<ScriptService> GetAllServiceInstances()
        {
            List<ScriptServiceWrapper> all = ScriptServiceWrapper.GetAll(this);
            IEnumerable<ScriptService> results = from w in all orderby w.Level descending select w.Service;

            return results;
        }

        /// <summary>
        /// Declares all services.
        /// </summary>
        /// <param name="host">The page all services to deploy.</param>
        /// <returns>Current script service.</returns>
        public ScriptService DeclareAll(Page host)
        {
            List<ScriptServiceWrapper> all = ScriptServiceWrapper.GetAll(this);

            DeclareAll(host, all);

            return this;
        }

        /// <summary>
        /// Invoke the script service.
        /// </summary>
        /// <param name="target">The target control to invoke. This is useful when the target is in a UpdatePanel. For general response, the Page is well.</param>
        public void Invoke(Control target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "target should be provided.");
            }

            if (!target.Visible)
            {
                return;
            }

            bool canInvoke = QueryContinueInvoke(target);

            if (!canInvoke)
            {
                return;
            }

            ServiceManager manager = ServiceManager.GetCurrent(target.Page);

            List<ScriptServiceWrapper> all = ScriptServiceWrapper.GetAll(this);

            InitializeAll(target.Page, all);
            ValidateAll(target.Page, all);

            if (!manager.IsInAsyncPostBack)
            {
                DeclareAll(target.Page, all);
            }

            string invokeBlock = "\r\n" + GetServiceInstance() + ";\r\n";

            ScriptManager.RegisterStartupScript(
                    target,
                    target.GetType(),
                    GetInvokeKey(target),
                    invokeBlock,
                    true
                );

            OnCompleteInvoke(new CompleteInvokeEventArgs(target));
        }

        /// <summary>
        /// Same as GetServiceInstance(false).
        /// </summary>
        /// <returns>Service instance.</returns>
        public override string ToString()
        {
            return GetServiceInstance();
        }

        /// <summary>
        /// Declares all resources.
        /// </summary>
        /// <param name="host">The page to deploy resources.</param>
        /// <returns>Current script service.</returns>
        public ScriptService Declares(Page host)
        {
            DeclarePrerequisites(host);
            DeclareMe(host);

            return this;
        }

        /// <summary>
        /// Shadow clone.
        /// </summary>
        /// <returns>Cloned script service.</returns>
        object ICloneable.Clone()
        {
            ScriptService target = (ScriptService)MemberwiseClone();

            target.Reset();

            return target;
        }

        /// <summary>
        /// Declares current service script.
        /// </summary>
        /// <param name="host">The page to deploy the script.</param>
        /// <returns>Current script service.</returns>
        internal protected virtual ScriptService DeclareMe(Page host)
        {
            DeclareJavaScriptItem item = GetDeclareItem(host);
            ServiceManager manager = ServiceManager.GetCurrent(host);

            manager.Declare(item, this);

            return this;
        }

        /// <summary>
        /// Write code block. Default throw NotImplementedException. If specify Discovery.CodeBlockHere and not provide at DeclareAttribute, the service will call this method to get the code block.
        /// </summary>
        /// <param name="declareKey">Declares key.</param>
        /// <param name="host">The page to deploy the script.</param>
        /// <returns>Code block.</returns>
        protected virtual string WriteCodeBlock(string declareKey, Page host)
        {
            throw new NotImplementedException("The subclass should overridden this method.");
        }

        /// <summary>
        /// Provide for subclass. Default do nothing. Before invoke, this method will be called.
        /// </summary>
        /// <param name="host">The host to deploy the resources.</param>
        protected virtual void Initialize(Page host)
        {
            // Default do nothing
        }

        /// <summary>
        /// Provide for subclass. Default do nothing. Before invoke, this method will be called. The subclass should throw exception if validate failed.
        /// </summary>
        /// <param name="host">The page to deploy the resources.</param>
        protected virtual void Validate(Page host)
        {
            // Default do nothing
        }

        /// <summary>
        /// Declares code block.
        /// </summary>
        /// <param name="host">The page to deploy script code block.</param>
        /// <param name="jsQualifiedName">Java script qualified name.</param>
        /// <param name="resourceType">Confirm the assembly where the resource locate.</param>
        /// <param name="addScriptTags">Instruct whether to add script tags.</param>
        /// <returns>Current script service.</returns>
        protected ScriptService DeclareCodeBlock(Page host, string jsQualifiedName, Type resourceType = null, bool addScriptTags = true)
        {
            ServiceManager manager = ServiceManager.GetCurrent(host);
            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    jsQualifiedName,
                    Discovery.CodeBlockInResource,
                    resourceType,
                    (string)null,
                    addScriptTags
                );

            manager.Declare(item, this);

            return this;
        }

        /// <summary>
        /// Declares code block.
        /// </summary>
        /// <param name="host">The page to deploy the script code block.</param>
        /// <param name="jsBlock">Java script code block.</param>
        /// <param name="addScriptTags">Instructs whether to add script tags.</param>
        /// <returns>Current script service.</returns>
        protected ScriptService DeclareCodeBlock(Page host, string jsBlock, bool addScriptTags = true)
        {
            ServiceManager manager = ServiceManager.GetCurrent(host);
            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    "Unspecified",
                    Discovery.CodeBlockHere,
                    (Type)null,
                    jsBlock,
                    addScriptTags
                );

            manager.Declare(item, this);

            return this;
        }

        /// <summary>
        /// Declares web resource.
        /// </summary>
        /// <param name="host">The page to deploy the script.</param>
        /// <param name="jsQualifiedName">Java script qualified name.</param>
        /// <param name="resourceType">Confirm the assembly where the resource locate</param>
        /// <returns>Current script service.</returns>
        protected ScriptService DeclareWebResource(Page host, string jsQualifiedName, Type resourceType = null)
        {
            ServiceManager manager = ServiceManager.GetCurrent(host);
            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    jsQualifiedName,
                    Discovery.WebResource,
                    resourceType
                );

            manager.Declare(item, this);

            return this;
        }

        /// <summary>
        /// Declares script path.
        /// </summary>
        /// <param name="host">The page to include the script.</param>
        /// <param name="scriptPath">Script path. (Allow server side path, i.e. ~)</param>
        /// <returns>Current script service.</returns>
        protected ScriptService DeclareScriptPath(Page host, string scriptPath)
        {
            ServiceManager manager = ServiceManager.GetCurrent(host);
            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    scriptPath,
                    Discovery.WebSiteFile
                );

            manager.Declare(item, this);

            return this;
        }

        /// <summary>
        /// Gets service GUID attribute.
        /// </summary>
        /// <returns>Service GUID attribute.</returns>
        internal protected ServiceGuidAttribute GetServiceGuid()
        {
            if (_serviceGuid == null)
            {
                _serviceGuid = (ServiceGuidAttribute)Attribute.GetCustomAttribute(
                       GetType(),
                       typeof(ServiceGuidAttribute)
                   );
            }

            if (_serviceGuid == null)
            {
                throw new InvalidOperationException("Cannot get ServiceGuidAttribute for current service.");
            }

            return _serviceGuid;
        }

        /// <summary>
        /// Gets additional data properties.
        /// </summary>
        /// <returns>All additional properties.</returns>
        internal IList<DataProperty> GetAdditionalDataProperties()
        {
            return _additionalDataProperties;
        }

        #region Event Triggers

        /// <summary>
        /// Raises PrepareInvoke event.
        /// </summary>
        /// <param name="e">Event arguments. It contains the target control reference.</param>
        protected virtual void OnPrepareInvoke(PrepareInvokeEventArgs e)
        {
            EventHandler<PrepareInvokeEventArgs> handler = PrepareInvoke;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises PrepareDeclare event.
        /// </summary>
        /// <param name="e">Event arguments. It contains the page reference.</param>
        protected virtual void OnPrepareDeclare(PrepareDeclareEventArgs e)
        {
            EventHandler<PrepareDeclareEventArgs> handler = PrepareDeclare;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises PrepareInitialize event.
        /// </summary>
        /// <param name="e">Event arguments. It contains target control reference.</param>
        protected virtual void OnPrepareInitialize(PrepareInitializeEventArgs e)
        {
            EventHandler<PrepareInitializeEventArgs> handler = PrepareInitialize;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises PrepareValidate event.
        /// </summary>
        /// <param name="e">Event arguments. It contains page reference.</param>
        protected virtual void OnPrepareValidate(PrepareValidateEventArgs e)
        {
            EventHandler<PrepareValidateEventArgs> handler = PrepareValidate;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises PrepareSerializeData event.
        /// </summary>
        /// <param name="e">Event arguments. May adjust behaviors using the arguments.</param>
        protected virtual void OnPrepareSerialzieData(PrepareSerializeDataEventArgs e)
        {
            EventHandler<PrepareSerializeDataEventArgs> handler = PrepareSerializeData;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises CompleteSerializeData event
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnCompleteSerializeData(EventArgs e)
        {
            EventHandler handler = CompleteSerializeData;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises ComleteValidate event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnCompleteValidate(CompleteValidateEventArgs e)
        {
            EventHandler<CompleteValidateEventArgs> handler = CompleteValidate;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises CompleteIntialize event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnCompleteInitialize(CompleteInitializeEventArgs e)
        {
            EventHandler<CompleteInitializeEventArgs> handler = CompleteInitialize;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises CompleteDeclare event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnCompleteDeclare(CompleteDeclareEventArgs e)
        {
            EventHandler<CompleteDeclareEventArgs> handler = CompleteDeclare;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises CompleteInvoke event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnCompleteInvoke(CompleteInvokeEventArgs e)
        {
            EventHandler<CompleteInvokeEventArgs> handler = CompleteInvoke;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Help Members

        /// <summary>
        /// Declares the prerequisites.
        /// </summary>
        /// <param name="host">The host.</param>
        private void DeclarePrerequisites(Page host)
        {
            IList<DeclareItem> all = GetAllPrerequisites();
            IList<DeclareJavaScriptItem> javaScripts = all.OfType<DeclareJavaScriptItem>().ToList();
            IList<DeclareStylesheetItem> stylesheets = all.OfType<DeclareStylesheetItem>().ToList();

            DeclarePrerequisiteStylesheets(host, stylesheets);
            DeclarePrerequisiteJavaScripts(host, javaScripts);
        }

        /// <summary>
        /// Declares the prerequisite stylesheets.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="allStylesheets">All stylesheets.</param>
        private void DeclarePrerequisiteStylesheets(Page host, IList<DeclareStylesheetItem> allStylesheets)
        {
            if ((allStylesheets == null) || (allStylesheets.Count == 0))
            {
                return;
            }

            PrerequisiteStylesheetLibraryManager manager = PrerequisiteStylesheetLibraryManager.GetCurrent(host);
            Control container = GetOrCreateStylesheetContainer(host);

            foreach (DeclareStylesheetItem item in allStylesheets)
            {
                manager.Declare(item, this, container);
            }
        }

        /// <summary>
        /// Declares the prerequisite java scripts.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="allJavaScripts">All java scripts.</param>
        private void DeclarePrerequisiteJavaScripts(Page host, IList<DeclareJavaScriptItem> allJavaScripts)
        {
            if ((allJavaScripts == null) || (allJavaScripts.Count == 0))
            {
                return;
            }

            PrerequisiteJavaScriptLibraryManager manager = PrerequisiteJavaScriptLibraryManager.GetCurrent(host);

            foreach (DeclareJavaScriptItem item in allJavaScripts)
            {
                manager.Declare(item, this);
            }
        }

        /// <summary>
        /// Gets all prerequisites.
        /// </summary>
        /// <returns></returns>
        private IList<DeclareItem> GetAllPrerequisites()
        {
            PrerequisiteAttribute[] all = (PrerequisiteAttribute[])Attribute.GetCustomAttributes(
                    GetType(),
                    typeof(PrerequisiteAttribute)
                );
            IEnumerable<DeclareItem> list = from item in all
                                            orderby item.LoadOrder
                                            select item.CreateDeclareItem();

            if (_additonalPrerequisitelJavaScripts != null)
            {
                list = list.Union(_additonalPrerequisitelJavaScripts);
            }

            if (_additionalPrerequisiteStylesheets != null)
            {
                list = list.Union(_additionalPrerequisiteStylesheets);
            }

            return list.ToList();
        }

        /// <summary>
        /// Gets the or create stylesheet container.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        private Control GetOrCreateStylesheetContainer(Page host)
        {
            if (_stylesheetContainer != null)
            {
                return _stylesheetContainer;
            }
            else if (host.Header != null)
            {
                return host.Header;
            }
            else
            {
                PlaceHolder container = new PlaceHolder();

                container.EnableViewState = false;
                host.Form.Controls.AddAt(0, container);

                return container;
            }
        }

        /// <summary>
        /// Gets the declare item.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        private DeclareJavaScriptItem GetDeclareItem(Page host)
        {
            if (_script != null)
            {
                return _script;
            }

            Type current = GetType();
            DeclareAttribute attr = (DeclareAttribute)Attribute.GetCustomAttribute(
                    current,
                    typeof(DeclareAttribute)
                );

            if (attr == null)
            {
                throw new InvalidOperationException(
                        String.Format(
                                "DeclareAttribute should be flagged on {0} or Declares(Page host) should be overridden.",
                                current.FullName
                            )
                    );
            }

            string name = null;

            switch (attr.Discovery)
            {
                case Discovery.WebResource:
                case Discovery.WebSiteFile:
                case Discovery.CodeBlockInResource:
                default:
                    if (String.IsNullOrWhiteSpace(attr.Name))
                    {
                        throw new InvalidOperationException(
                                String.Format(
                                        "Name property of DeclareAttribute flagged on \"{0}\" is empty.",
                                        current.FullName
                                    )
                            );
                    }

                    name = attr.Name.Trim();
                    break;

                case Discovery.CodeBlockHere:
                    break;
            }

            string codeBlock = attr.CodeBlock;

            if (attr.Discovery == Discovery.CodeBlockHere && String.IsNullOrEmpty(codeBlock))
            {
                codeBlock = WriteCodeBlock(GetDeclareKey(), host);
            }

            bool addScriptTags = attr.AddScriptTags;

            DeclareJavaScriptItem item = new DeclareJavaScriptItem(
                    name,
                    attr.Discovery,
                    attr.ResourceType,
                    codeBlock,
                    addScriptTags
                );

            return item;
        }

        /// <summary>
        /// Declares all.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="all">All.</param>
        private void DeclareAll(Page host, IEnumerable<ScriptServiceWrapper> all)
        {
            ILookup<Guid, ScriptServiceWrapper> lookups = all.ToLookup(w => w.Service.GetServiceId());

            foreach (var g in lookups)
            {
                List<ScriptServiceWrapper> currents = g.ToList();

                for (int i = 0; i < currents.Count; i++)
                {
                    ScriptService service = currents[i].Service;

                    service.OnPrepareDeclare(new PrepareDeclareEventArgs(host));

                    if (i == 0)
                    {
                        service.Declares(host);
                    }

                    service.OnCompleteDeclare(new CompleteDeclareEventArgs(host));
                }
            }
        }

        /// <summary>
        /// Initializes all.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="all">All.</param>
        private void InitializeAll(Page host, IEnumerable<ScriptServiceWrapper> all)
        {
            IEnumerable<ScriptServiceWrapper> ordereds = from w in all orderby w.Level descending select w;

            foreach (ScriptServiceWrapper w in ordereds)
            {
                w.Service.OnPrepareInitialize(new PrepareInitializeEventArgs(host));
                w.Service.Initialize(host);
                w.Service.OnCompleteInitialize(new CompleteInitializeEventArgs(host));
            }
        }

        /// <summary>
        /// Validates all.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="all">All.</param>
        private void ValidateAll(Page host, IEnumerable<ScriptServiceWrapper> all)
        {
            IEnumerable<ScriptServiceWrapper> ordereds = from w in all orderby w.Level descending select w;

            foreach (ScriptServiceWrapper w in ordereds)
            {
                w.Service.OnPrepareValidate(new PrepareValidateEventArgs(host));
                w.Service.Validate(host);
                w.Service.OnCompleteValidate(new CompleteValidateEventArgs(host));
            }
        }

        /// <summary>
        /// Queries the continue invoke.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private bool QueryContinueInvoke(Control target)
        {
            PrepareInvokeEventArgs e = new PrepareInvokeEventArgs(target);

            OnPrepareInvoke(e);

            return !e.Cancel;
        }

        /// <summary>
        /// Ensures the additional data properties storage created.
        /// </summary>
        private void EnsureAdditionalDataPropertiesStorageCreated()
        {
            if (_additionalDataProperties == null)
            {
                _additionalDataProperties = new List<DataProperty>();
            }
        }

        /// <summary>
        /// Ensures the additional prerequisite java scripts storage created.
        /// </summary>
        private void EnsureAdditionalPrerequisiteJavaScriptsStorageCreated()
        {
            if (_additonalPrerequisitelJavaScripts == null)
            {
                _additonalPrerequisitelJavaScripts = new List<DeclareJavaScriptItem>();
            }
        }

        /// <summary>
        /// Ensures the additional prerequisite style sheets storage created.
        /// </summary>
        private void EnsureAdditionalPrerequisiteStylesheetsStorageCreated()
        {
            if (_additionalPrerequisiteStylesheets == null)
            {
                _additionalPrerequisiteStylesheets = new List<DeclareStylesheetItem>();
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            _instanceId = null;
        }

        #endregion


        #region Help Classes

        /// <summary>
        /// Script service wrapper.
        /// </summary>
        private sealed class ScriptServiceWrapper
        {
            #region Static Members

            /// <summary>
            /// Gets all.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <returns>All script service wrappers.</returns>
            public static List<ScriptServiceWrapper> GetAll(ScriptService root)
            {
                ScriptServiceWrapper current = new ScriptServiceWrapper(0, root);
                List<ScriptServiceWrapper> all = new List<ScriptServiceWrapper>();

                all.Add(current);
                DoCollectAll(all, current);

                return all;
            }

            /// <summary>
            /// Does the collect all.
            /// </summary>
            /// <param name="all">All.</param>
            /// <param name="current">The current.</param>
            private static void DoCollectAll(List<ScriptServiceWrapper> all, ScriptServiceWrapper current)
            {
                ServiceInfo info = ServiceInfo.Build(current.Service);

                foreach (ScriptService childService in info.ChildServices)
                {
                    ScriptServiceWrapper childWrapper = new ScriptServiceWrapper(current.Level + 1, childService);

                    childWrapper.LinkPrevious(current);
                    all.Add(childWrapper);

                    DoCollectAll(all, childWrapper);
                }
            }

            #endregion

            private readonly int _level;
            private readonly ScriptService _service;

            private ScriptServiceWrapper _previous;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScriptServiceWrapper"/> class.
            /// </summary>
            /// <param name="level">The level.</param>
            /// <param name="service">The service.</param>
            public ScriptServiceWrapper(int level, ScriptService service)
            {
                _level = level;
                _service = service;
            }

            /// <summary>
            /// Gets the level.
            /// </summary>
            /// <value>
            /// The level.
            /// </value>
            public int Level
            {
                get { return _level; }
            }

            /// <summary>
            /// Gets the service.
            /// </summary>
            /// <value>
            /// The service.
            /// </value>
            public ScriptService Service
            {
                get { return _service; }
            }

            /// <summary>
            /// Gets the previous.
            /// </summary>
            /// <value>
            /// The previous.
            /// </value>
            public ScriptServiceWrapper Previous
            {
                get { return _previous; }
            }

            #region Help Members

            /// <summary>
            /// Links the previous.
            /// </summary>
            /// <param name="previous">The previous.</param>
            private void LinkPrevious(ScriptServiceWrapper previous)
            {
                CheckServiceCircularReference(previous);

                _previous = previous;
            }

            /// <summary>
            /// Checks the service circular reference.
            /// </summary>
            /// <param name="previous">The previous.</param>
            /// <exception cref="System.InvalidOperationException"></exception>
            private void CheckServiceCircularReference(ScriptServiceWrapper previous)
            {
                List<ScriptService> queue = GetServiceQueue(previous);
                int index = queue.IndexOf(this.Service);

                if (index < 0)
                {
                    return;
                }

                queue.Add(this.Service);

                StringBuilder buffer = new StringBuilder();

                for (int i = 0; i < queue.Count; i++)
                {
                    ScriptService service = queue[i];

                    if (i != 0)
                    {
                        buffer.Append(" -> ");
                    }

                    if ((i == index) || (i == (queue.Count - 1)))
                    {
                        buffer.AppendFormat("[{0}]", service.GetServiceName());
                    }
                    else
                    {
                        buffer.Append(service.GetServiceName());
                    }
                }

                throw new InvalidOperationException(
                        String.Format(
                                "A script service circular reference is found:\r\n{0}",
                                buffer
                            )
                    );
            }

            /// <summary>
            /// Gets the service queue.
            /// </summary>
            /// <param name="current">The current.</param>
            /// <returns>Service list.</returns>
            private List<ScriptService> GetServiceQueue(ScriptServiceWrapper current)
            {
                List<ScriptService> all = new List<ScriptService>();

                all.Add(current.Service);

                ScriptServiceWrapper previous = current.Previous;

                while (previous != null)
                {
                    all.Insert(0, previous.Service);

                    previous = previous.Previous;
                }

                return all;
            }

            #endregion
        }

        #endregion
    }
}