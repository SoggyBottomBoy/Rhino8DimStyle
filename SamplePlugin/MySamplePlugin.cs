using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace SamplePlugin;

public class MySamplePlugin : Rhino.PlugIns.PlugIn
{
    public MySamplePlugin() { }

    internal static TestConduit Test { get; set; } = new() { Enabled = true, };

    static MySamplePlugin()
    {
        var myPoints = new List<MyPoint>();
        var dimStyle = new DimensionStyle()
        {
            TextHorizontalAlignment = TextHorizontalAlignment.Center,
            TextVerticalAlignment = TextVerticalAlignment.Bottom,
            TextHeight = 0.1
        };

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var point = new Point3d(i, j, 0);
                myPoints.Add(
                    new(
                        point,
                        TextEntity.Create(
                            $"({i},{j})",
                            new Plane(point, new(0, 0, 1)),
                            dimStyle,
                            false,
                            0,
                            0
                        )
                    )
                );
            }
        }

        Test.Points = myPoints;
    }
}

public record MyPoint(Point3d Point, TextEntity Text);

internal class TestConduit : DisplayConduit
{
    public IEnumerable<MyPoint> Points = Array.Empty<MyPoint>();

    protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
    {
        base.CalculateBoundingBox(e);

        var boundingBox = new BoundingBox(Points.Select(p => p.Point));
        boundingBox.Inflate(boundingBox.Diagonal.Length);
        boundingBox.Union(e.Display.Viewport.ConstructionPlane().Origin);
        e.IncludeBoundingBox(boundingBox);
    }

    protected override void PreDrawObjects(DrawEventArgs e)
    {
        base.PreDrawObjects(e);

        foreach (var point in Points)
        {
            e.Display.DrawPoint(point.Point);
            e.Display.DrawText(point.Text, Color.Red);
        }
    }
}
