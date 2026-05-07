using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using SkiaSharp;
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Core.Geometry.Point;

public class GraphRenderer
{
    public void RenderGraph(Graph graph, string outputFilename, string? topNodeId = null)
    {
        PrepareGraphForRendering(graph);

        if (topNodeId != null
            && graph.LayoutAlgorithmSettings is SugiyamaLayoutSettings sugiyama
            && graph.FindNode(topNodeId) is { } topNode)
        {
            sugiyama.PinNodesToMaxLayer(topNode.GeometryNode);
        }

        LayoutHelpers.CalculateLayout(graph.GeometryGraph, graph.LayoutAlgorithmSettings, null);

        using var stream = File.Create(outputFilename);

        var svgWriter = new FontAwareSvgGraphWriter(stream, graph);
        svgWriter.Write();
    }

    private void PrepareGraphForRendering(Graph graph)
    {
        var nodeFontSize = 10;
        var edgeFontSize = 6;

        foreach (var node in graph.Nodes)
        {
            node.Attr.Shape = Shape.Box;
            node.Attr.FillColor = Color.LightBlue;
            node.Attr.LineWidth = 1;
            node.Label.FontName = "Arial";
            node.Label.FontSize = nodeFontSize;
            node.Label.FontColor = Color.Black;
        }

        foreach (var edge in graph.Edges)
        {
            edge.Attr.ArrowheadAtTarget = ArrowStyle.Normal;
            edge.Attr.LineWidth = 1;

            if (edge.Label != null)
            {
                edge.Label.FontName = "Arial";
                edge.Label.FontSize = edgeFontSize;
            }
        }

        graph.CreateGeometryGraph();

        using var nodeFont = new SKFont(SKTypeface.FromFamilyName("Arial"), nodeFontSize);
        using var edgeFont = new SKFont(SKTypeface.FromFamilyName("Arial"), edgeFontSize);

        var nodeMetrics = nodeFont.Metrics;
        double nodeLineHeight = nodeMetrics.Descent - nodeMetrics.Ascent;

        var edgeMetrics = edgeFont.Metrics;
        double edgeLineHeight = edgeMetrics.Descent - edgeMetrics.Ascent;

        foreach (var node in graph.Nodes)
        {
            var label = node.LabelText ?? node.Id;

            var advance = nodeFont.MeasureText(label);

            double width = advance + 20;
            double height = nodeLineHeight + 15;

            node.Label.Width = advance;
            node.Label.Height = nodeLineHeight;
            node.GeometryNode.BoundaryCurve = CurveFactory.CreateRectangle(width, height, new Point(0, 0));
        }

        foreach (var edge in graph.Edges)
        {
            if (edge.Label == null) continue;

            var advance = edgeFont.MeasureText(edge.LabelText ?? string.Empty);

            edge.Label.Width = advance;
            edge.Label.Height = edgeLineHeight;
        }
    }
}