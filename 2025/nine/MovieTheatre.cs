public class MovieTheatre
{
    public List<Vec2> tiles = new();

    private static string Scale(double value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public MovieTheatre(string[] lines)
    {
        foreach (var line in lines)
        {
            tiles.Add(Vec2.FromString(line));
        }
    }

    public IEnumerable<Rectangle> GetRectangles()
    {
        for (var i = 0; i < tiles.Count; i++)
        {
            for (var j = i + 1; j < tiles.Count; j++)
            {
                yield return new Rectangle(tiles[i], tiles[j]);
            }
        }
    }

    public long Part1()
    {
        var biggestArea = 0L;
        foreach (var rect in GetRectangles())
        {
            var area = rect.Area();
            if (area > biggestArea)
            {
                biggestArea = area;
            }
        }
        return biggestArea;
    }

    public long Part2()
    {
        var biggestArea = 0L;
        var allLines = GetOuterLines();
        var biggestRect = new Rectangle(new Vec2(0, 0), new Vec2(0, 0));
        foreach (var rect in GetRectangles())
        {
            var area = rect.Area();
            if (area > biggestArea && !allLines.Any(line => rect.CrossesLine(line)))
            {
                biggestArea = area;
                biggestRect = rect;
            }
        }
        Console.WriteLine($"Biggest rectangle: {biggestRect}");
        return biggestArea;
    }

    public void CreateSvg(double scale = 0.01)
    {
        var points = string.Join(" ", tiles.Select(t => $"{Scale(t.X * scale)},{Scale(t.Y * scale)}"));
        var width = Scale(tiles.Max(t => t.X) * scale + 10);
        var height = Scale(tiles.Max(t => t.Y) * scale + 10);

        var outerLines = GetOuterLines().Select(l => $"<Line x1=\"{Scale(l.Start.X * scale)}\" y1=\"{Scale(l.Start.Y * scale)}\" x2=\"{Scale(l.End.X * scale)}\" y2=\"{Scale(l.End.Y * scale)}\" style=\"stroke:blue;stroke-width:1\" />");

        // a blue rect showing Biggest rectangle: Rectangle(Coordinate { X = 5477, Y = 67450 }, Coordinate { X = 94703, Y = 50308 })
        // Biggest rectangle: Rectangle(Vec2 { X = 5424, Y = 67450 }, Vec2 { X = 94703, Y = 50308 })
        var rectPoints = new List<Vec2>
        {
            new Vec2(5424, 67450),
            new Vec2(94703, 67450),
            new Vec2(94703, 50308),
            new Vec2(5424, 50308)
        };


        var svg = @$"
        <html>
        <body>
        <svg width=""{width}"" height=""{height}"">
            <polygon points=""{points}"" style=""fill:red;stroke:red;stroke-width:0"" />
            {string.Join("\n", outerLines)}
            <polygon points=""{string.Join(" ", rectPoints.Select(t => $"{Scale(t.X * scale)},{Scale(t.Y * scale)}"))}"" style=""fill:none;stroke:blue;stroke-width:2"" />
        </svg>
        </body>
        </html>
        ";
        File.WriteAllText("visualization.html", svg);
    }

    public List<Line> GetLines()
    {
        var lines = new List<Line>();
        for (var i = 1; i < tiles.Count; i++)
        {
            lines.Add(new Line(tiles[i - 1], tiles[i]));
        }
        // wrap
        lines.Add(new Line(tiles[^1], tiles[0]));
        return lines;
    }

    public List<Line> GetOuterLines()
    {
        var outTiles = GetOuterTiles();
        var outerLines = new List<Line>();
        for (var i = 1; i < outTiles.Count; i++)
        {
            outerLines.Add(new Line(outTiles[i - 1], outTiles[i]));
        }
        // wrap
        outerLines.Add(new Line(outTiles[^1], outTiles[0]));
        return outerLines;

    }

    public List<Vec2> GetOuterTiles()
    {
        var outerPerimeterTiles = new List<Vec2> { tiles[0] with { X = tiles[0].X + 1, Y = tiles[0].Y - 1 } }; // outer corner of input.txt
        //var outerPerimeterTiles = new List<Vec2> { tiles[0] with { X = tiles[0].X - 1, Y = tiles[0].Y - 1 } }; // outer corner of testinput.txt
        
        //var outerPerimeterTiles = new List<Vec2> { tiles[0] with { X = tiles[0].X - 1, Y = tiles[0].Y - 1 } };
        
        for (var i = 1; i < tiles.Count; i++)
        {
            var prevTile = tiles[i - 1];
            var currentTile = tiles[i];
            var nextTile = tiles[(i + 1) % tiles.Count];


            var prevOuterTile = outerPerimeterTiles[i - 1];
            var prevYDiff = prevOuterTile.Y - prevTile.Y;
            var prevXDiff = prevOuterTile.X - prevTile.X;

            // P == prevTile
            // C == currentTile
            // N == nextTile


            // TOP LEFT

            //  O
            //   P      C
            if (prevYDiff == -1 && prevXDiff == -1 && prevTile.X < currentTile.X)
            {
                //  O           O
                //   P         C
                //           
                //             N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }
                //             N
                //    
                //  O         O 
                //   P         C
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 1");
                }
            }

            //       O
            //  C     P
            else if (prevYDiff == -1 && prevXDiff == -1 && prevTile.X > currentTile.X)
            {
                // O     O
                //  C     P
                // 
                //  N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }

                //  N
                //
                //   O   O
                //  C     P
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }

                else
                {
                    throw new Exception("Shouldnt happen 2");
                }
            }

            //   C
            //    
            //  O
            //   P
            else if (prevYDiff == -1 && prevXDiff == -1 && prevTile.Y > currentTile.Y)
            {
                //  O
                //   C    N
                //    
                //  O
                //   P
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }
                // N   C
                //    O 
                //    O
                //     P
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen qweqweqw3");
                }
            }

            //  O
            //   P
            //
            //   C
            else if (prevYDiff == -1 && prevXDiff == -1 && prevTile.Y < currentTile.Y)
            {
                //  O
                //   P
                //
                //   C    N
                //  O
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }
                //      O
                //       P
                //      O
                //  N    C
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen rthrthrthrth");
                }
            }

            // TOP RIGHT

            //      O
            //     P     C
            else if (prevYDiff == -1 && prevXDiff == 1 && prevTile.X < currentTile.X)
            {
                //    O         O
                //   P         C
                //           
                //             N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }
                //             N
                //    
                //    O       O 
                //   P         C
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 3");
                }
            }

            //         O
            //  C     P
            else if (prevYDiff == -1 && prevXDiff == 1 && prevTile.X > currentTile.X)
            {
                // O       O
                //  C     P
                // 
                //  N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }

                //  N
                //
                //   O     O
                //  C     P
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }

                else
                {
                    throw new Exception("Shouldnt happen 4");
                }
            }

            //   C
            //    
            //    O
            //   P
            else if (prevYDiff == -1 && prevXDiff == 1 && prevTile.Y > currentTile.Y)
            {
                //   C    N
                //    O
                //    O
                //   P
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }
                //      O
                // N   C
                //       
                //      O
                //     P
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen qweqweqw3");
                }
            }

            //    O
            //   P
            //
            //   C
            else if (prevYDiff == -1 && prevXDiff == 1 && prevTile.Y < currentTile.Y)
            {
                //    O
                //   P
                //    O
                //   C    N
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }
                //        O
                //       P
                //        
                //  N    C
                //        O
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen rthrthrthrth");
                }
            }

            // BOTTOM LEFT

            //   P      C
            //  O
            else if (prevYDiff == 1 && prevXDiff == -1 && prevTile.X < currentTile.X)
            {
                //   P      C
                //  O      O
                //          N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }

                //          N
                //
                //   P      C
                //  O        O
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }

            //   C      P
            //         O
            else if (prevYDiff == 1 && prevXDiff == -1 && prevTile.X > currentTile.X)
            {

                //   C      P
                //    O    O
                //
                //   N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }

                //   N
                //
                //   C      P
                //  O      O
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 6");
                }
            }

            //   P
            //  O
            //
            //   C
            else if (prevYDiff == 1 && prevXDiff == -1 && prevTile.Y < currentTile.Y)
            {
                //   P
                //  O
                //
                //   C     N
                //  O
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }

                //      P
                //     O
                //     O
                // N    C 
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }

            //
            //   C
            //
            //   P
            //  O
            else if (prevYDiff == 1 && prevXDiff == -1 && prevTile.Y > currentTile.Y)
            {
                //  O
                //   C    N
                //
                //   P
                //  O
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y - 1));
                }

                //   N    C
                //       O
                //        P
                //       O
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }

            // BOTTOM RIGHT

            //   P      C
            //    O
            else if (prevYDiff == 1 && prevXDiff == 1 && prevTile.X < currentTile.X)
            {
                //   P      C
                //    O    O
                //          N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }

                //          N
                //
                //   P      C
                //    O      O
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }

            //   C      P
            //           O
            else if (prevYDiff == 1 && prevXDiff == 1 && prevTile.X > currentTile.X)
            {

                //   C      P
                //    O      O
                //
                //   N
                if (currentTile.Y < nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }

                //   N
                //
                //   C      P
                //  O        O
                else if (currentTile.Y > nextTile.Y)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X - 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 6");
                }
            }

            //   P
            //    O
            //
            //   C
            else if (prevYDiff == 1 && prevXDiff == 1 && prevTile.Y < currentTile.Y)
            {
                //   P
                //    O
                //    O
                //   C     N
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }

                //       P
                //        O
                //    
                // N     C 
                //        O
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }

            //
            //   C
            //
            //   P
            //    O
            else if (prevYDiff == 1 && prevXDiff == 1 && prevTile.Y > currentTile.Y)
            {
                //   C    N
                //    O
                //   P
                //    O
                if (currentTile.X < nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y + 1));
                }

                //         O
                //   N    C
                //       
                //        P
                //         O
                else if (currentTile.X > nextTile.X)
                {
                    outerPerimeterTiles.Add(new Vec2(currentTile.X + 1, currentTile.Y - 1));
                }
                else
                {
                    throw new Exception("Shouldnt happen 5");
                }
            }
        }


        return outerPerimeterTiles;

    }
}