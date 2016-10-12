using System;
using System.Web.UI;

using CIPACE.Extension.SH;

[assembly: WebResource("CIPACE.Extension.CodeRenderControlService.js", "text/javascript")]

namespace CIPACE.Extension
{
    [
        Prerequisite(
                "CIPACE.Extension.SH.styles.shCore.css",
                Discovery.WebResource,
                ItemType = ClientResourceType.Stylesheet,
                LoadOrder = 10
            )
    ]
    [
        Prerequisite(
                "CIPACE.Extension.SH.scripts.shCore.js",
                Discovery.WebResource,
                LoadOrder = 10
            )
    ]
    [Serializable]
    [ServiceGuid("1E37E794-301E-4C41-8CC1-B418ED453617", Name = "Code Render")]
    [
        Declare(
                "CIPACE.Extension.CodeRenderControlService.js",
                Discovery.WebResource
            )
    ]
    public sealed class CodeRenderControlService : ScriptService
    {
        [NonDataProperty]
        public Brush Brush { get; set; }

        [NonDataProperty]
        public Theme Theme { get; set; }

        [LiterallySerialized]
        [DataProperty("sh")]
        public string SyntaxHighlighterObject
        {
            get { return "SyntaxHighlighter"; }
        }

        [DataProperty("canvas", Required = true)]
        public string CanvasID
        {
            get;
            set;
        }

        protected override void Initialize(Page host)
        {
            string themeResource = ThemeFinder.Find(Theme);
            string brushResource = BrushFinder.FindResource(Brush);

            ClearAdditionalPrerequisiteJavaScripts();
            ClearAdditionalPrerequisiteStylesheets();

            AddPrerequisiteStylesheet(themeResource, Discovery.WebResource);
            AddPrerequisiteJavaScript(brushResource, Discovery.WebResource);
        }
    }
}