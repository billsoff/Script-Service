using System.Web.UI;

[assembly: WebResource("CIPACE.Extension.SH.scripts.shAutoloader.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushAppleScript.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushAS3.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushBash.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushColdFusion.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushCpp.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushCSharp.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushCss.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushDelphi.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushDiff.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushErlang.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushGroovy.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushJava.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushJavaFX.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushJScript.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushPerl.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushPhp.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushPlain.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushPowerShell.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushPython.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushRuby.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushSass.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushScala.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushSql.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushVb.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shBrushXml.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shCore.js", "text/javascript")]
[assembly: WebResource("CIPACE.Extension.SH.scripts.shLegacy.js", "text/javascript")]

[assembly: WebResource("CIPACE.Extension.SH.styles.shCore.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreDefault.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreDjango.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreEclipse.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreEmacs.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreFadeToGrey.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreMDUltra.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreMidnight.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shCoreRDark.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeDefault.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeDjango.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeEclipse.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeEmacs.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeFadeToGrey.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeMDUltra.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeMidnight.css", "text/css")]
[assembly: WebResource("CIPACE.Extension.SH.styles.shThemeRDark.css", "text/css")]

namespace CIPACE.Extension.SH
{
    public enum Brush
    {
        CSharp = 0,

        JavaScript = 1,

        Sql = 2,

        ActionScript3 = 3,

        Bash_Shell = 4,

        ColdFusion = 5,

        Cpp = 6,

        Css = 7,

        Delphi = 8,

        Diff = 9,

        Erlang = 10,

        Groovy = 11,

        Java = 12,

        JavaFX = 13,

        Perl = 14,

        Php = 15,

        PlainText = 16,

        PowerShell = 17,

        Python = 18,

        Ruby = 19,

        Scala = 20,

        VisualBasic = 21,

        Xml = 22
    }

    public enum Theme
    {
        Default = 0,

        Django = 1,

        Eclipse = 2,

        Emacs = 3,

        FadeToGrey = 4,

        Midnight = 5,

        RDark = 6
    }

    public static class ThemeFinder
    {
        public static string Find(Theme theme)
        {
            switch (theme)
            {
                case Theme.Default:
                default:
                    return "CIPACE.Extension.SH.styles.shThemeDefault.css";

                case Theme.Django:
                    return "CIPACE.Extension.SH.styles.shThemeDjango.css";

                case Theme.Eclipse:
                    return "CIPACE.Extension.SH.styles.shThemeEclipse.css";

                case Theme.Emacs:
                    return "CIPACE.Extension.SH.styles.shThemeEmacs.css";

                case Theme.FadeToGrey:
                    return "CIPACE.Extension.SH.styles.shThemeFadeToGrey.css";

                case Theme.Midnight:
                    return "CIPACE.Extension.SH.styles.shThemeMidnight.css";

                case Theme.RDark:
                    return "CIPACE.Extension.SH.styles.shThemeRDark.css";
            }
        }
    }

    public static class BrushFinder
    {
        public static string FindResource(Brush brush)
        {
            switch (brush)
            {
                case Brush.CSharp:
                default:
                    return "CIPACE.Extension.SH.scripts.shBrushCSharp.js";

                case Brush.JavaScript:
                    return "CIPACE.Extension.SH.scripts.shBrushJScript.js";

                case Brush.Sql:
                    return "CIPACE.Extension.SH.scripts.shBrushSql.js";

                case Brush.ActionScript3:
                    return "CIPACE.Extension.SH.scripts.shBrushAS3.js";

                case Brush.Bash_Shell:
                    return "CIPACE.Extension.SH.scripts.shBrushBash.js";

                case Brush.ColdFusion:
                    return "CIPACE.Extension.SH.scripts.shBrushColdFusion.js";

                case Brush.Cpp:
                    return "CIPACE.Extension.SH.scripts.shBrushCpp.js";

                case Brush.Css:
                    return "CIPACE.Extension.SH.scripts.shBrushCss.js";

                case Brush.Delphi:
                    return "CIPACE.Extension.SH.scripts.shBrushDelphi.js";

                case Brush.Diff:
                    return "CIPACE.Extension.SH.scripts.shBrushDiff.js";

                case Brush.Erlang:
                    return "CIPACE.Extension.SH.scripts.shBrushErlang.js";

                case Brush.Groovy:
                    return "CIPACE.Extension.SH.scripts.shBrushGroovy.js";

                case Brush.Java:
                    return "CIPACE.Extension.SH.scripts.shBrushJava.js";

                case Brush.JavaFX:
                    return "CIPACE.Extension.SH.scripts.shBrushJavaFX.js";

                case Brush.Perl:
                    return "CIPACE.Extension.SH.scripts.shBrushPerl.js";

                case Brush.Php:
                    return "CIPACE.Extension.SH.scripts.shBrushPhp.js";

                case Brush.PlainText:
                    return "CIPACE.Extension.SH.scripts.shBrushPlain.js";

                case Brush.PowerShell:
                    return "CIPACE.Extension.SH.scripts.shBrushPowerShell.js";

                case Brush.Python:
                    return "CIPACE.Extension.SH.scripts.shBrushPython.js";

                case Brush.Ruby:
                    return "CIPACE.Extension.SH.scripts.shBrushRuby.js";

                case Brush.Scala:
                    return "CIPACE.Extension.SH.scripts.shBrushScala.js";

                case Brush.VisualBasic:
                    return "CIPACE.Extension.SH.scripts.shBrushVb.js";

                case Brush.Xml:
                    return "CIPACE.Extension.SH.scripts.shBrushXml.js";
            }
        }

        public static string FindAlias(Brush brush)
        {
            switch (brush)
            {
                case Brush.CSharp:
                default:
                    return "csharp";

                case Brush.JavaScript:
                    return "js";

                case Brush.Sql:
                    return "sql";

                case Brush.ActionScript3:
                    return "as3";

                case Brush.Bash_Shell:
                    return "bash";

                case Brush.ColdFusion:
                    return "cf";

                case Brush.Cpp:
                    return "c";

                case Brush.Css:
                    return "css";

                case Brush.Delphi:
                    return "delphi";

                case Brush.Diff:
                    return "diff";

                case Brush.Erlang:
                    return "erl";

                case Brush.Groovy:
                    return "groovy";

                case Brush.Java:
                    return "java";

                case Brush.JavaFX:
                    return "jfx";

                case Brush.Perl:
                    return "perl";

                case Brush.Php:
                    return "php";

                case Brush.PlainText:
                    return "text";

                case Brush.PowerShell:
                    return "ps";

                case Brush.Python:
                    return "py";

                case Brush.Ruby:
                    return "ruby";

                case Brush.Scala:
                    return "scala";

                case Brush.VisualBasic:
                    return "vb";

                case Brush.Xml:
                    return "xml";
            }
        }
    }
}