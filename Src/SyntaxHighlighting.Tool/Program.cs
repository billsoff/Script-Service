using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SyntaxHighlighting.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteIncludes();
            WriteThemes();

            Resources r = GetResources();

            Console.WriteLine(r.Namespace);

            Console.WriteLine(r.Includes.Styles.Namespace);
            Console.WriteLine(r.Includes.Scripts.Namespace);

            foreach (ItemInfo item in r.Includes.Styles.Items)
            {
                Console.WriteLine(item.Name);
            }

            foreach (ItemInfo item in r.Includes.Scripts.Items)
            {
                Console.WriteLine(item.Name);
            }

            foreach (BrushInfo brush in r.Brushes)
            {
                Console.WriteLine("{0}/{1}", brush.Name, brush.Type);
            }

            foreach (ThemeInfo theme in r.Themes)
            {
                Console.WriteLine("{0}/{1}", theme.Name, theme.Type);
            }

            using (StreamWriter writer = new StreamWriter(@"..\..\..\SyntaxHighlighting\SHResource.cs", false, Encoding.UTF8))
            {
                writer.WriteLine("using System.Web.UI;");
                writer.WriteLine();

                foreach (ItemInfo item in r.Includes.Scripts.Items)
                {
                    writer.WriteLine(
                            "[assembly: WebResource(\"{0}.{1}.{2}.js\", \"text/javascript\")]",
                            r.Namespace,
                            r.Includes.Scripts.Namespace,
                            item.Name
                        );
                }

                writer.WriteLine();

                foreach (ItemInfo item in r.Includes.Styles.Items)
                {
                    writer.WriteLine(
                            "[assembly: WebResource(\"{0}.{1}.{2}.css\", \"text/css\")]",
                            r.Namespace,
                            r.Includes.Styles.Namespace,
                            item.Name
                        );
                }

                writer.WriteLine();

                writer.WriteLine("namespace {0}", r.Namespace);
                writer.WriteLine("{");

                RenderBrushDefinition(writer, r, 1);

                writer.WriteLine();

                RenderThemeDefinition(writer, r, 1);

                writer.WriteLine();

                RenderThemeFinder(writer, r, 1);

                writer.WriteLine();

                BrushFinderClassRender.Render(writer, r, 1);

                writer.Write("}");
            }

            Console.ReadKey();
        }

        private static void RenderBrushDefinition(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}public enum Brush", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderBrushMemberDefinitions(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderBrushMemberDefinitions(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            for (int i = 0; i < r.Brushes.Length; i++)
            {
                if (i > 0)
                {
                    writer.WriteLine();
                }

                writer.Write("{0}{1} = {2}", leadingSpace, r.Brushes[i].Type, i);

                if (i < r.Brushes.Length - 1)
                {
                    writer.Write(",");
                }

                writer.WriteLine();
            }
        }

        private static void RenderThemeFinder(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}public static class ThemeFinder", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderThemeFinderClassBody(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderThemeFinderClassBody(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}public static string Find(Theme theme)", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderThemeFinderSwitchDefinition(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderThemeFinderSwitchDefinition(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}switch (theme)", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderThemeFinderCaseStatements(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderThemeFinderCaseStatements(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            for (int i = 0; i < r.Themes.Length; i++)
            {
                ThemeInfo theme = r.Themes[i];

                writer.WriteLine("{0}case Theme.{1}:", leadingSpace, theme.Type);

                if (i == 0)
                {
                    writer.WriteLine("{0}default:", leadingSpace);
                }

                RendThemeFinderCaseReturnStatement(writer, r, theme, indent + 1);

                if (i < r.Themes.Length - 1)
                {
                    writer.WriteLine();
                }
            }
        }

        private static void RendThemeFinderCaseReturnStatement(StreamWriter writer, Resources r, ThemeInfo theme, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            writer.WriteLine(
                    "{0}return \"{1}.styles.{2}.css\";",
                    leadingSpace,
                    r.Namespace,
                    theme.Name
                );
        }

        private static void WriteIncludes()
        {
            string[] scripts = Directory.GetFiles(@"..\..\..\SyntaxHighlighting\SH\scripts");
            string[] styles = Directory.GetFiles(@"..\..\..\SyntaxHighlighting\SH\styles");
            Resources r = GetResources();

            r.Includes.Scripts.Items = (
                                            from s in scripts
                                            orderby s
                                            select new ItemInfo { Name = Path.GetFileNameWithoutExtension(s) }
                                       ).ToArray();
            r.Includes.Styles.Items = (
                                            from s in styles
                                            orderby s
                                            select new ItemInfo { Name = Path.GetFileNameWithoutExtension(s) }
                                      ).ToArray();

            SaveResources(r);
        }

        private static void RenderThemeDefinition(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}public enum Theme", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderThemeMemberDefinitions(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderThemeMemberDefinitions(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            for (int i = 0; i < r.Themes.Length; i++)
            {
                ThemeInfo t = r.Themes[i];

                if (i > 0)
                {
                    writer.WriteLine(",");
                    writer.WriteLine();
                }

                writer.Write("{0}{1} = {2}", leadingSpace, t.Type, i);
            }

            writer.WriteLine();
        }

        private static void WriteThemes()
        {
            List<ThemeInfo> all = new List<ThemeInfo>();

            using (StreamReader reader = new StreamReader(@"..\..\themes.txt", Encoding.UTF8))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] splits = line.Split(new char[] { ' ' });

                    all.Add(
                            new ThemeInfo
                                {
                                    Name = Path.GetFileNameWithoutExtension(splits[1]),
                                    Type = splits[0]
                                }
                        );
                }
            }

            Resources r = GetResources();

            r.Themes = all.ToArray();

            SaveResources(r);
        }

        private static Resources GetResources()
        {
            Resources r;

            using (StreamReader reader = new StreamReader(@"..\..\ResourceDescription.xml", Encoding.UTF8))
            {
                r = Resources.Deserialize(reader);
            }

            return r;
        }

        private static void SaveResources(Resources r)
        {
            using (StreamWriter writer = new StreamWriter(@"..\..\ResourceDescription.xml", false, Encoding.UTF8))
            {
                r.Serialize(writer);
            }
        }
    }
}