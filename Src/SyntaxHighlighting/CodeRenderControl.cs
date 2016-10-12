using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using CIPACE.Extension.SH;

namespace CIPACE.Extension
{
    [ParseChildren(true, "Code"), PersistChildren(false)]
    [DefaultProperty("Code")]
    public class CodeRenderControl : Control
    {
        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Code
        {
            get { return (string)ViewState["Code"]; }
            set { ViewState["Code"] = value; }
        }

        public CodeRenderControlService ScriptService
        {
            get
            {
                if (ViewState["ScriptService"] == null)
                {
                    ViewState["ScriptService"] = new CodeRenderControlService();
                }

                return (CodeRenderControlService)ViewState["ScriptService"];
            }

            set { ViewState["ScriptService"] = value; }
        }

        [DefaultValue(Brush.CSharp)]
        public Brush Brush
        {
            get { return ScriptService.Brush; }
            set { ScriptService.Brush = value; }
        }

        [DefaultValue(Theme.Default)]
        public Theme Theme
        {
            get { return ScriptService.Theme; }
            set { ScriptService.Theme = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptService.CanvasID = ClientID;

            ScriptService.Invoke(this);

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            HtmlGenericControl pre = new HtmlGenericControl("pre");

            pre.Attributes["class"] = String.Format("brush: {0}", BrushFinder.FindAlias(Brush));
            pre.Attributes["id"] = ClientID;
            pre.Attributes["name"] = UniqueID;

            pre.InnerText = Code;

            pre.RenderControl(writer);
        }

        #region Help Members

        private void RenderTheme(HtmlTextWriter writer)
        {
            if (Theme != Theme.Default)
            {
                string resourceName = ThemeFinder.Find(Theme);
            }

        }

        #endregion
    }
}