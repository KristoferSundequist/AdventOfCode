using System.ComponentModel;

public class Region
{
    public int Width;
    public int Height;
    public List<int> ShapeRequirements;

    public Region(int width, int height, List<int> shapeRequirements)
    {
        Width = width;
        Height = height;
        ShapeRequirements = shapeRequirements;
    }

    public static Region FromString(string line)
    {
        var parts = line.Split(':');
        var dimensions = parts[0].Split('x');
        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        var shapeParts = parts[1].Trim().Split(' ');
        List<int> shapeRequirements = shapeParts.Select(int.Parse).ToList();

        return new Region(width, height, shapeRequirements);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}: {string.Join(" ", ShapeRequirements)}";
    }

    public int Area()
    {
        return Width * Height;
    }

    public int TotalShapeArea(List<int> shapeAreas)
    {
        return Enumerable.Range(0, ShapeRequirements.Count)
            .Sum(i => ShapeRequirements[i] * shapeAreas[i]);
    }

    public bool CanFitShapes(List<int> shapeAreas)
    {
        int totalShapeArea = TotalShapeArea(shapeAreas);
        var totalArea = Area();
        var slackArea = totalArea - totalShapeArea;

        if (slackArea > 0 && slackArea < totalArea * 0.1)
        {
            throw new Exception($"Insufficient slack area: {slackArea} in region {this}");
        }
        return slackArea >= 0;
    }
}