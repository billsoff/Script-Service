using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CIPACE.Extension
{
    [ParseChildren(true, "Code"), PersistChildren(false)]
    [DefaultProperty("Code")]
    public class CSharpRenderControl : Control
    {
        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Code
        {
            get { return (string)ViewState["Code"]; }
            set { ViewState["Code"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            CSharpCodeRenderService service = new CSharpCodeRenderService();

            service.CanvasID = ClientID;

            service.Invoke(this);

            // Declare prerequisite scripts and style sheet
            //AppendStyleSheet(CSharpCodeRenderService.STYLE_SHEET_SH_CORE, Page);
            //AppendStyleSheet(CSharpCodeRenderService.STYLE_SHEET_SH_THEME_DEFAULT, Page);

            //DeclareWebResource(CSharpCodeRenderService.SCRIPT_SH_CORE, GetType().Assembly);
            //DeclareWebResource(CSharpCodeRenderService.SCRIPT_SH_BRUSH_CSHARP, GetType().Assembly);

            //Assembly assembly = Assembly.GetExecutingAssembly();
            //Stream stream = assembly.GetManifestResourceStream(CSharpCodeRenderService.CODE_NAME);

            //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            //{
            //    Code = reader.ReadToEnd();
            //}
            //ScriptManager.RegisterStartupScript(
            //        this,
            //        GetType(),
            //        "Highlight",
            //        String.Format(
            //                "SyntaxHighlighter.highlight(null, document.getElementById('{0}'));\r\n",
            //                ClientID
            //            ),
            //        true
            //    );
        }

        protected override void Render(HtmlTextWriter writer)
        {
            HtmlGenericControl pre = new HtmlGenericControl("pre");

            pre.Attributes["class"] = "brush: csharp";
            pre.Attributes["id"] = ClientID;
            pre.Attributes["name"] = UniqueID;

            pre.InnerText = Code;

            pre.RenderControl(writer);

            //writer.AddAttribute(HtmlTextWriterAttribute.Class, "brush: csharp");

            //writer.RenderBeginTag(HtmlTextWriterTag.Pre);
            //writer.Write(HttpUtility.HtmlEncode(Code));
            //writer.RenderEndTag();
        }

        public void DeclareWebResource(string jsQualifiedName, Assembly assembly)
        {
            if (assembly == null)
            {
                throw new InvalidOperationException("assembly should be provided to locate web resource.");
            }

            ScriptManager.Scripts.Add(new ScriptReference(jsQualifiedName, assembly.FullName));
        }

        private ScriptManager _scriptManager;

        public ScriptManager ScriptManager
        {
            get
            {
                if (_scriptManager == null)
                {
                    _scriptManager = ScriptManager.GetCurrent(Page);
                }

                return _scriptManager;
            }
        }

        #region Help Members

        private void AppendStyleSheet(string name, Page host)
        {
            HtmlLink style = CreteStyleSheet(name, host);
            HtmlHead head = host.Header;

            if (head != null)
            {
                head.Controls.Add(style);
            }
            else
            {
                host.Form.Controls.AddAt(0, style);
            }
        }

        private HtmlLink CreteStyleSheet(string name, Page host)
        {
            HtmlLink style = new HtmlLink();

            style.Href = GetWebResourceUrl(name, host);

            style.Attributes["type"] = "text/css";
            style.Attributes["rel"] = "stylesheet";

            return style;
        }

        private string GetWebResourceUrl(string name, Page host)
        {
            string url = host.ClientScript.GetWebResourceUrl(GetType(), name);

            return url;
        }

        #endregion
    }
}