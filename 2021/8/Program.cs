var lines = System.IO.File.ReadAllLines("data.txt");

var parseWords = (String str) => str.Split(" ");
var displays = lines.Select(line => line.Split(" | ")).Select(arr => new { Signal = arr[0].Split(" "), Output = arr[1].Split(" ") }).ToList();


var sum = 0;
foreach (var display in displays)
{

    foreach (var output in display.Output)
    {
        sum += output.Length switch
        {
            2 => 1,
            3 => 1,
            4 => 1,
            7 => 1,
            _ => 0
        };
    }
}
Console.WriteLine(sum);

var sum2 = 0;
foreach (var display in displays)
{
    var qwe = new Display(display.Signal);
    sum2 += qwe.Translate(display.Output);
}
Console.WriteLine(sum2);

public class Display
{
    static HashSet<char> allChars = new HashSet<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
    public HashSet<char> Top = new HashSet<char>(allChars);
    public HashSet<char> TopLeft = new HashSet<char>(allChars);
    public HashSet<char> TopRight = new HashSet<char>(allChars);
    public HashSet<char> Middle = new HashSet<char>(allChars);
    public HashSet<char> BottomLeft = new HashSet<char>(allChars);
    public HashSet<char> BottomRight = new HashSet<char>(allChars);
    public HashSet<char> Bottom = new HashSet<char>(allChars);

    public Display(string[] signals)
    {
        foreach (var signal in signals)
        {
            AddUnique(signal);
        }
        foreach (var signal in signals)
        {
            AddThree(signal);
        }
        foreach (var signal in signals)
        {
            AddSix(signal);
        }
    }

    public int Translate(string[] output)
    {
        return Int32.Parse(string.Join("", output.Select(TranslateDigit)));
    }

    private char TranslateDigit(string str)
    {
        var arr = str.ToCharArray();
        switch (arr.Length)
        {
            case 2:
                return '1';
            case 3:
                return '7';
            case 4:
                return '4';
            case 5:
                // 2,3,5
                if (str.Contains(TopLeft.Single()))
                {
                    return '5';
                }
                else if (str.Contains(BottomRight.Single()))
                {
                    return '3';
                }
                else
                {
                    return '2';
                }
            case 6:
                // 0, 6, 9
                if (!str.Contains(Middle.Single()))
                {
                    return '0';
                }
                else if (!str.Contains(TopRight.Single()))
                {
                    return '6';
                }
                else
                {
                    return '9';
                }
            case 7:
                return '8';
            default:
                throw new Exception("Unexpcetd num segments");
        }
    }

    private void AddUnique(string str)
    {
        var set = str.ToCharArray().ToHashSet();
        switch (set.Count)
        {
            case 2: // number 1
                Top = Top.Except(set).ToHashSet();
                TopLeft = TopLeft.Except(set).ToHashSet();
                TopRight = TopRight.Intersect(set).ToHashSet();
                Middle = Middle.Except(set).ToHashSet();
                BottomLeft = BottomLeft.Except(set).ToHashSet();
                BottomRight = BottomRight.Intersect(set).ToHashSet();
                Bottom = Bottom.Except(set).ToHashSet();
                break;
            case 3: // number 7
                Top = Top.Intersect(set).ToHashSet();
                TopLeft = TopLeft.Except(set).ToHashSet();
                TopRight = TopRight.Intersect(set).ToHashSet();
                Middle = Middle.Except(set).ToHashSet();
                BottomLeft = BottomLeft.Except(set).ToHashSet();
                BottomRight = BottomRight.Intersect(set).ToHashSet();
                Bottom = Bottom.Except(set).ToHashSet();
                break;
            case 4: // number 4
                Top = Top.Except(set).ToHashSet();
                TopLeft = TopLeft.Intersect(set).ToHashSet();
                TopRight = TopRight.Intersect(set).ToHashSet();
                Middle = Middle.Intersect(set).ToHashSet();
                BottomLeft = BottomLeft.Except(set).ToHashSet();
                BottomRight = BottomRight.Intersect(set).ToHashSet();
                Bottom = Bottom.Except(set).ToHashSet();
                break;
            default:
                return;
        }
    }

    private void AddThree(string str)
    {
        var set = str.ToCharArray().ToHashSet();
        if (set.Count == 5 && set.Intersect(TopRight).Count() == 2 && set.Intersect(BottomRight).Count() == 2)
        {
            Top = Top.Intersect(set).ToHashSet();
            TopLeft = TopLeft.Except(set).ToHashSet();
            TopRight = TopRight.Intersect(set).ToHashSet();
            Middle = Middle.Intersect(set).ToHashSet();
            BottomLeft = BottomLeft.Except(set).ToHashSet();
            BottomRight = BottomRight.Intersect(set).ToHashSet();
            Bottom = Bottom.Intersect(set).ToHashSet();
        }
    }

    private void AddSix(string str)
    {
        var set = str.ToCharArray().ToHashSet();
        if (set.Count == 6 && set.Intersect(TopRight).Count() == 1 && set.Intersect(BottomRight).Count() == 1)
        {
            BottomRight = BottomRight.Intersect(set).ToHashSet();
            TopRight = TopRight.Except(BottomRight).ToHashSet();
        }
    }

    public void Print()
    {
        Console.WriteLine("Top: " + string.Join(", ", Top));
        Console.WriteLine("TopLeft: " + string.Join(", ", TopLeft));
        Console.WriteLine("TopRight: " + string.Join(", ", TopRight));
        Console.WriteLine("Middle: " + string.Join(", ", Middle));
        Console.WriteLine("BottomLeft: " + string.Join(", ", BottomLeft));
        Console.WriteLine("BottomRight: " + string.Join(", ", BottomRight));
        Console.WriteLine("Bottom: " + string.Join(", ", Bottom));
    }
}