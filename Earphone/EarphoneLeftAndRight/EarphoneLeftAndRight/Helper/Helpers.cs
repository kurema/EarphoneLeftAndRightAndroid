using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using System.IO;
using System.Xml;

namespace EarphoneLeftAndRight.Helper
{
    public static class Helpers
    {
        public static FormattedString XhtmlToFormattedString(string xml)
        {
            using (var sr = new StringReader(xml))
            using (var xr = XmlReader.Create(sr))
            {
                var result = new FormattedString();
                var cSpan = new Span() { Text = string.Empty }; // c is current.
                xr.ReadToFollowing("body");
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xr.Name.ToUpperInvariant())
                            {
                                case "B":
                                    cSpan = new Span()
                                    {
                                        FontAttributes = FontAttributes.Bold,
                                        Text = string.Empty
                                    };
                                    break;
                                case "I":
                                    cSpan = new Span()
                                    {
                                        FontAttributes = FontAttributes.Italic,
                                        Text = string.Empty
                                    };
                                    break;
                                case "BR":

                                    break;

                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (string.IsNullOrEmpty(cSpan.Text)) break;
                            result.Spans.Add(cSpan);
                            cSpan = new Span() { Text = string.Empty };
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:
                            cSpan.Text += xr.Name;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                return result;
            }
        }
    }
}
