var movieTheatre = new MovieTheatre(File.ReadAllLines("input.txt"));

Console.WriteLine(movieTheatre.Part1());
Console.WriteLine(movieTheatre.Part2());

movieTheatre.CreateSvg(0.1);