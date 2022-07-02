using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xamarin.Forms;
using System.IO;
using System.Xml;

namespace EarphoneLeftAndRight.Helper
{
    public static partial class Helpers
    {
        public static string[] XhtmlToStrings(string? xml)
        {
            if (xml is null) return new string[0];
            //https://social.msdn.microsoft.com/Forums/en-US/acc75e12-4a66-43bd-805b-986620689ca4/formattedstring-is-killing-my-listview?forum=xamarinforms
            //https://github.com/xamarin/Xamarin.Forms/issues/5087
            //FormattedString is slow.
            using var sr = new StringReader(xml);
            using var xr = XmlReader.Create(sr, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });
            xr.ReadToFollowing("body");
            var result = new List<StringBuilder>() { new StringBuilder() };

            while (xr.Read())
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name.ToUpperInvariant())
                        {
                            case "B":
                            case "I":
                            case "SMALL":
                            default:
                                result[^1].Append(" ");
                                break;
                            case "BR":
                                result[^1].Append("\n");
                                break;
                            case "HR":
                                result.Add(new StringBuilder());
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Whitespace:
                        result[^1].Append(" ");
                        break;
                    case XmlNodeType.Text:
                        result[^1].Append(xr.Value.Replace("\r", "").Replace("\n", ""));
                        break;
                    default:
                        //throw new NotImplementedException();
                        break;
                }
            }
            return result.Select(a => a.ToString()).ToArray();
        }

        public static Label[] XhtmlToLabels(string xml)
        {
            //https://social.msdn.microsoft.com/Forums/en-US/acc75e12-4a66-43bd-805b-986620689ca4/formattedstring-is-killing-my-listview?forum=xamarinforms
            //https://github.com/xamarin/Xamarin.Forms/issues/5087
            //FormattedString is slow.
            using var sr = new StringReader(xml);
            using var xr = XmlReader.Create(sr, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });
            var result = new List<Label>();
            var currentSB = new StringBuilder();
            double basicFontSize = new Label().FontSize;
            xr.ReadToFollowing("body");

            void CloseString()
            {
                result.Add(new Label() { Text = currentSB.ToString(), TextType = TextType.Html });
                currentSB.Clear();
            }

            while (xr.Read())
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name.ToUpperInvariant())
                        {
                            case "B":
                            case "I":
                            case "SMALL":
                            default:
                                currentSB.Append($"<{xr.Name}>");
                                break;
                            case "BR":
                                currentSB.Append($"<br />");
                                break;
                            case "HR":
                                CloseString();
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        currentSB.Append($"</{xr.Name}>");
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Whitespace:
                        currentSB.Append(xr.Value.Replace("\r", "").Replace("\n", ""));
                        break;
                    default:
                        //throw new NotImplementedException();
                        break;
                }
            }
            CloseString();

            return result.ToArray();
        }

        public static Label[] XhtmlToLabelsClassical(string xml)
        {
            //https://social.msdn.microsoft.com/Forums/en-US/acc75e12-4a66-43bd-805b-986620689ca4/formattedstring-is-killing-my-listview?forum=xamarinforms
            //https://github.com/xamarin/Xamarin.Forms/issues/5087
            //FormattedString is slow.
            using var sr = new StringReader(xml);
            using var xr = XmlReader.Create(sr, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });
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
                                cString.Spans.Add(new Span() { Text = Environment.NewLine });
                                break;
                            case "HR":
                                CloseString();
                                break;
                            case "SMALL":
                                CloseSpan();
                                cSpan = new Span()
                                {
                                    Text = string.Empty,
                                    FontSize = basicFontSize * 0.8,
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

        public static string FormatFrequencyToHumanReadable(double hz)
        {
            var freq = hz;
            if (freq < 1000) return $"{freq:0.#} Hz";
            else if (freq < 10000) return $"{freq / 1000:0.###} kHz";
            else if (freq < 100000) return $"{freq / 1000:0.##} kHz";
            else return $"{freq:0.#} kHz";
        }
    }
}
