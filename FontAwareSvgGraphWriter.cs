using System.Text.RegularExpressions;
using Microsoft.Msagl.Drawing;
using Label = Microsoft.Msagl.Drawing.Label;
using Color = Microsoft.Msagl.Drawing.Color;

// Claude AI generated this class to allow the SvgGraphWriter to properly handle font attributes
// when writing labels to the SVG output. It overrides the WriteLabel method to include font-family,
// font-size, and fill attributes based on the Label's properties. Additionally, it handles multi-line
// labels by splitting the text and using <tspan> elements to position each line correctly within the SVG.
// This ensures that the rendered graph in SVG format maintains the intended styling of node and edge labels.
public class FontAwareSvgGraphWriter : SvgGraphWriter
{
    private readonly Graph _graph;
    private bool _backgroundWritten;

    public Color BackgroundColor { get; set; } = Color.White;

    public FontAwareSvgGraphWriter(Stream stream, Graph graph) : base(stream, graph)
    {
        _graph = graph;
    }

    protected override void WriteLabel(Label label)
    {
        // The SvgGraphWriter creates a transparent background, so we hook into the label writing process
        // to write a background rectangle that covers the entire graph area.  It looks like a label is
        // always written immediately after the graph's bounding box is calculated, so this appears to be
        // a good place to write the background;
        if (!_backgroundWritten)
        {
            _backgroundWritten = true;
            var box = _graph.BoundingBox;
            WriteStartElement("rect");
            WriteAttribute("x", box.Left);
            WriteAttribute("y", box.Bottom);
            WriteAttribute("width", box.Width);
            WriteAttribute("height", box.Height);
            WriteAttribute("fill", MsaglColorToSvgColor(BackgroundColor));
            WriteEndElement();
        }

        if (label == null || string.IsNullOrEmpty(label.Text) || label.Width == 0.0)
            return;

        double x = label.Center.X - label.Width / 2.0;
        double y = label.Center.Y + label.Height / 3.0;
        double fontSize = label.FontSize > 0 ? label.FontSize : 16;
        string fontFamily = string.IsNullOrEmpty(label.FontName) ? "Arial" : label.FontName;

        WriteStartElement("text");
        WriteAttribute("x", x);
        WriteAttribute("y", y);
        WriteAttribute("font-family", fontFamily);
        WriteAttribute("font-size", fontSize);
        WriteAttribute("fill", MsaglColorToSvgColor(label.FontColor));
        WriteLabelText(label.Text, x, fontSize);
        WriteEndElement();
    }

    private void WriteLabelText(string text, double xContainer, double fontSize)
    {
        var endOfLines = new[] { "\r\n", "\r", "\n" };
        var lines = Regex.Split(NodeSanitizer(text), "(\r\n|\r|\n)")
            .Where(s => !endOfLines.Contains(s))
            .ToList();

        bool first = true;
        foreach (var line in lines)
        {
            WriteStartElement("tspan");
            WriteAttribute("x", xContainer);
            if (first)
            {
                first = false;
                WriteAttribute("dy", -1.0 * fontSize * (lines.Count - 1));
            }
            else
            {
                WriteAttribute("dy", fontSize);
            }
            XmlWriter.WriteRaw(line);
            WriteEndElement();
        }
    }
}
