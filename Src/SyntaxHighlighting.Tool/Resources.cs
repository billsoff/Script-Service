using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SyntaxHighlighting.Tool
{
    [XmlRoot("resource")]
    public sealed class Resources
    {
        [XmlAttribute("ns")]
        public string Namespace { get; set; }

        [XmlElement("includes")]
        public Includes Includes { get; set; }

        [XmlArray("brushes")]
        [XmlArrayItem("brush")]
        public BrushInfo[] Brushes { get; set; }

        [XmlArray("themes")]
        [XmlArrayItem("theme")]
        public ThemeInfo[] Themes { get; set; }

        public static Resources Deserialize(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Resources));

            Resources r = (Resources)serializer.Deserialize(reader);

            return r;
        }

        public void Serialize(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Resources));

            serializer.Serialize(writer, this);
        }
    }

    public sealed class Includes
    {
        [XmlElement("style")]
        public IncludeStyles Styles { get; set; }

        [XmlElement("javaScript")]
        public IncludeJavaScripts Scripts { get; set; }
    }

    public sealed class IncludeStyles
    {
        [XmlAttribute("ns")]
        public string Namespace { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public ItemInfo[] Items { get; set; }
    }

    public sealed class IncludeJavaScripts
    {
        [XmlAttribute("ns")]
        public string Namespace { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public ItemInfo[] Items { get; set; }
    }

    public sealed class ItemInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public sealed class BrushInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("alias")]
        public string Alias { get; set; }
    }

    public sealed class ThemeInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}