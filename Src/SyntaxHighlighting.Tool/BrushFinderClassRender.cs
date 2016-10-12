using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxHighlighting.Tool
{
    class BrushFinderClassRender
    {
        public static void Render(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}public static class BrushFinder", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderFindResourceMethod(writer, r, indent + 1);

            writer.WriteLine();

            RenderFindAliasMethod(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderFindResourceMethod(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine(
                    "{0}public static string FindResource(Brush brush)",
                    leadingSpace
                );
            writer.WriteLine("{0}{{", leadingSpace);

            RenderFindResourceSwitchBody(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderFindResourceSwitchBody(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);
            writer.WriteLine("{0}switch (brush)", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderFindResourceCaseStatements(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderFindResourceCaseStatements(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            for (int i = 0; i < r.Brushes.Length; i++)
            {
                BrushInfo brush = r.Brushes[i];

                writer.WriteLine("{0}case Brush.{1}:", leadingSpace, brush.Type);

                if (i == 0)
                {
                    writer.WriteLine("{0}default:", leadingSpace);
                }

                RenderFindResourceCaseReturnStatement(writer, r, brush, indent + 1);

                if (i < r.Brushes.Length - 1)
                {
                    writer.WriteLine();
                }
            }
        }

        private static void RenderFindResourceCaseReturnStatement(StreamWriter writer, Resources r, BrushInfo brush, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            writer.WriteLine("{0}return \"{1}.scripts.{2}.js\";", leadingSpace, r.Namespace, brush.Name);
        }

        private static void RenderFindAliasMethod(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            writer.WriteLine("{0}public static string FindAlias(Brush brush)", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderFindAliasMethodBody(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderFindAliasMethodBody(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            writer.WriteLine("{0}switch (brush)", leadingSpace);
            writer.WriteLine("{0}{{", leadingSpace);

            RenderFindAliasCaseStatements(writer, r, indent + 1);

            writer.WriteLine("{0}}}", leadingSpace);
        }

        private static void RenderFindAliasCaseStatements(StreamWriter writer, Resources r, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            for (int i = 0; i < r.Brushes.Length; i++)
            {
                BrushInfo brush = r.Brushes[i];

                writer.WriteLine("{0}case Brush.{1}:", leadingSpace, brush.Type);

                if (i == 0)
                {
                    writer.WriteLine("{0}default:", leadingSpace);
                }

                RenerFindAliasCaseReturnStatement(writer, r, brush, indent + 1);

                if (i < r.Brushes.Length - 1)
                {
                    writer.WriteLine();
                }
            }
        }

        private static void RenerFindAliasCaseReturnStatement(StreamWriter writer, Resources r, BrushInfo brush, int indent)
        {
            string leadingSpace = String.Empty.PadLeft(indent * 4);

            writer.WriteLine("{0}return \"{1}\";", leadingSpace, brush.Alias);
        }
    }
}