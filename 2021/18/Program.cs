var lines = System.IO.File.ReadAllLines("data.txt");

Part1();
Part2();

void Part1()
{

    SnailfishNumber? cur = null;
    foreach (var line in lines)
    {
        var newSfn = new SnailfishNumber(line, null);
        if (cur is null)
        {
            cur = newSfn;
        }
        else
        {
            cur = new SnailfishNumber(cur, newSfn);
        }
        while (cur.Reduce()) ;
    }
    Console.WriteLine(cur?.GetString());
    Console.WriteLine($"Magnitude is: {cur?.Magnitude()}");
}

void Part2()
{
    long curMax = 0;
    for(var i = 0; i < lines.Length; i++) {
        for(var j = 0; j < lines.Length; j++) {
            var sfn = new SnailfishNumber(new SnailfishNumber(lines[i], null), new SnailfishNumber(lines[j], null));
            while(sfn.Reduce());
            curMax = Math.Max(curMax, sfn.Magnitude());
            
            var sfn2 = new SnailfishNumber(new SnailfishNumber(lines[j], null), new SnailfishNumber(lines[i], null));
            while(sfn2.Reduce());
            curMax = Math.Max(curMax, sfn2.Magnitude());
        }
    }
    Console.WriteLine($"Largest magnitude is: {curMax}");
}

class SnailfishNumber
{
    SnailfishNumber? parent { get; set; }
    public long? RegularNumber { get; set; }
    SnailfishNumber? left { get; set; }
    SnailfishNumber? right { get; set; }
    public readonly Guid id = Guid.NewGuid();

    public SnailfishNumber(long value, SnailfishNumber? parent = null)
    {
        this.RegularNumber = value;
        this.parent = parent;
    }

    public SnailfishNumber(SnailfishNumber left, SnailfishNumber right)
    {
        left.parent = this;
        right.parent = this;
        this.left = left;
        this.right = right;
    }

    public SnailfishNumber(string str, SnailfishNumber? parent = null)
    {
        this.parent = parent;
        if (str.Length == 1)
        {
            this.RegularNumber = Int64.Parse(str);
            return;
        }
        var rest = str[1..^1];
        if (rest[0] == '[')
        {
            (var foundStr, rest) = GetParenthesis(rest);
            this.left = new SnailfishNumber(foundStr, this);
        }
        else
        {
            this.left = new SnailfishNumber(rest[0].ToString(), this);
            rest = rest[1..];
        }

        rest = rest[1..]; // jump comma

        if (rest[0] == '[')
        {
            (var foundStr, rest) = GetParenthesis(rest);
            this.right = new SnailfishNumber(foundStr, this);
        }
        else
        {
            this.right = new SnailfishNumber(rest[0].ToString(), this);
        }
    }

    public long Magnitude()
    {
        if (this.RegularNumber is not null)
        {
            return (long)this.RegularNumber;
        }

        if (left is null || right is null) throw new Exception("didnt expect left or right to be null in magnitude calce");

        return left.Magnitude() * 3 + right.Magnitude() * 2;
    }

    public string GetString()
    {
        if (this.RegularNumber is not null)
        {
            return $"{this.RegularNumber}";
        }
        return $"[{this.left?.GetString()},{this.right?.GetString()}]";
    }

    public List<SnailfishNumber> GetList(List<SnailfishNumber> list)
    {
        if (this.RegularNumber is not null)
        {
            list.Add(this);
            return list;
        }
        if (this.left is null || this.right is null)
        {
            throw new Exception("didnt expect left or right to be null");
        }

        var lefts = left.GetList(list);
        return right.GetList(lefts);
    }

    private SnailfishNumber GetRoot()
    {
        var cur = this;
        while (cur.parent is not null)
        {
            cur = cur.parent;
        };
        return cur;
    }

    private static (string, string) GetParenthesis(string str)
    {
        var count = 0;
        var i = 0;
        do
        {
            count += str[i] switch
            {
                '[' => 1,
                ']' => -1,
                _ => 0
            };
            i++;
        } while (count > 0);
        return (str[..i], str[i..]);
    }

    public bool Reduce()
    {
        var exploded = FindExplode(0);
        if (exploded)
        {
            return true;
        }
        return FindSplit();
    }

    private bool FindExplode(int depth)
    {
        if (depth == 4 && this.RegularNumber is null)
        {
            //Console.WriteLine("Exploding");
            Explode();
            return true;
        }

        if (left is not null)
        {
            var leftExploded = left.FindExplode(depth + 1);
            if (leftExploded)
            {
                return true;
            }
            else if (right is not null)
            {
                return right.FindExplode(depth + 1);
            }
        }
        return false;
    }

    private bool FindSplit()
    {
        if (left is not null)
        {
            var leftSplit = left.FindSplit();
            if (leftSplit)
            {
                return true;
            }
            else if (right is not null)
            {
                return right.FindSplit();
            }
            throw new Exception("if left is not null, then didnt excpect right to be");
        }
        else if (RegularNumber >= 10)
        {
            //Console.WriteLine("Splitting");
            this.Split();
            return true;
        }
        return false;
    }

    private void Explode()
    {
        if (left is null)
        {
            throw new Exception("Explode precondition violated left is null");
        }
        if (right is null)
        {
            throw new Exception("Explode precondition violated right is null");
        }
        if (RegularNumber is not null)
        {
            throw new Exception("Explode precondition violated regular is not null");
        }
        if (left.RegularNumber is null)
        {
            throw new Exception("Explode precondition violated left.regular is null");
        }
        if (right.RegularNumber is null)
        {
            throw new Exception("Explode precondition violated right.regular is null");
        }
        if (parent is null)
        {
            throw new Exception("Explode precondition violated parent is null");
        }

        //parent.AddToLeft((long)left.RegularNumber);
        //parent.AddToRight((long)right.RegularNumber);

        var root = this.GetRoot();
        var list = root.GetList(new List<SnailfishNumber> {});
        var rightIndex = list.FindIndex(sfn => sfn.id == this.right.id);
        if (rightIndex == -1)
        {
            throw new Exception("Didnt find index");
        }

        if (rightIndex + 1 < list.Count)
        {
            list[rightIndex + 1].RegularNumber += right.RegularNumber;
        }
        if (rightIndex - 2 >= 0)
        {
            list[rightIndex - 2].RegularNumber += left.RegularNumber;
        }

        this.RegularNumber = 0;
        left = null;
        right = null;
    }

    private void Split()
    {
        if (this.RegularNumber is null)
        {
            throw new Exception("Vioalated split precondition");
        }

        this.left = new SnailfishNumber((long)this.RegularNumber / 2, this);
        this.right = new SnailfishNumber(((long)this.RegularNumber + 1) / 2, this);
        this.RegularNumber = null;
    }
}