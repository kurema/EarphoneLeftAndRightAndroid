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
        public static Label[] XhtmlToFormattedString(string xml)
        {
            using (var sr = new StringReader(xml))
            using (var xr = XmlReader.Create(sr, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore }))
            {
                var result = new List<Label>();
                var cString = new FormattedString();
                var cSpan = new Span() { Text = string.Empty }; // c is current.
                double basicFontSize = new Label().FontSize;
                xr.ReadToFollowing("body");

                void CloseSpan()
                {
                    if (!string.IsNullOrEmpty(cSpan.Text)) cString.Spans.Add(cSpan);
                    cSpan = new Span() { Text = string.Empty };
                }
                void CloseString()
                {
                    CloseSpan();
                    for (int i = cString.Spans.Count - 1; cString.Spans[i].Text == Environment.NewLine && cString.Spans.Count >= 0; i--)
                    {
                        cString.Spans.RemoveAt(i);
                    }
                    result.Add(new Label() { FormattedText = cString });
                    cString = new FormattedString();
                }

                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xr.Name.ToUpperInvariant())
                            {
                                case "B":
                                    CloseSpan();
                                    cSpan = new Span()
                                    {
                                        FontAttributes = FontAttributes.Bold,
                                        Text = string.Empty
                                    };
                                    break;
                                case "I":
                                    CloseSpan();
                                    cSpan = new Span()
                                    {
                                        FontAttributes = FontAttributes.Italic,
                                        Text = string.Empty
                                    };
                                    break;
                                case "BR":
                                    CloseSpan();
                                    cString.Spans.Add(new Span() { Text = System.Environment.NewLine });
                                    break;
                                case "HR":
                                    CloseString();
                                    break;
                                case "SMALL":
                                    CloseSpan();
                                    cSpan = new Span()
                                    {
                                        Text = string.Empty,
                                        FontSize = basicFontSize * 0.6,
                                    };
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            CloseSpan();
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:
                            cSpan.Text += xr.Value.Replace("\r", "").Replace("\n", "");
                            break;
                        default:
                            //throw new NotImplementedException();
                            break;
                    }
                }
                CloseString();

                return result.ToArray();
            }
        }
    }
}
