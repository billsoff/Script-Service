using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CIPACE.Extension
{
    /// <summary>
    /// Renders csharp code.
    /// </summary>
    [Serializable]
    [ServiceGuid("B4CDDB31-1C56-46BB-80F2-225C8B1C475B", Name = "Highlight CSharp Code")]
    [Prerequisite(STYLE_SHEET_SH_CORE, Discovery.WebResource, ItemType = ClientResourceType.Stylesheet, LoadOrder = 1)]
    [Prerequisite(STYLE_SHEET_SH_THEME_DEFAULT, Discovery.WebResource, ItemType = ClientResourceType.Stylesheet, LoadOrder = 2)]
    [Prerequisite(SCRIPT_SH_CORE, Discovery.WebResource, LoadOrder = 1)]
    [Prerequisite(SCRIPT_SH_BRUSH_CSHARP, Discovery.WebResource, LoadOrder = 2)]
    [Declare(CSharpCodeRenderService.SCRIPT_NAME, Discovery.WebResource)]
    public sealed class CSharpCodeRenderService : ScriptService
    {
        internal const string STYLE_SHEET_SH_CORE = "CIPACE.Extension.SH.styles.shCore.css";
        internal const string STYLE_SHEET_SH_THEME_DEFAULT = "CIPACE.Extension.SH.styles.shThemeDefault.css";

        internal const string SCRIPT_SH_CORE = "CIPACE.Extension.SH.scripts.shCore.js";
        internal const string SCRIPT_SH_BRUSH_CSHARP = "CIPACE.Extension.SH.scripts.shBrushCSharp.js";
        internal const string SCRIPT_NAME = "CIPACE.Extension.CSharpCodeRenderService.js";

        internal const string CODE_NAME = "CIPACE.Extension.Code.Sample.txt";

        private FunctionDeclaration _getSyntaxHighlighterObject;

        [DataProperty("getSH")]
        public FunctionDeclaration SyntaxHighlighterObject
        {
            get
            {
                if (_getSyntaxHighlighterObject == null)
                {
                    _getSyntaxHighlighterObject = new FunctionDeclaration();

                    _getSyntaxHighlighterObject.Statements = "return SyntaxHighlighter;";
                }

                return _getSyntaxHighlighterObject;
            }
        }

        [DataProperty("canvas", Required = true)]
        public string CanvasID
        {
            get;
            set;
        }
    }
}