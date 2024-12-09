using System.Text;

public class Disk
{
    private long[] Data;
    private MyFile[] Files;

    public Disk(string inputFile)
    {
        var parsedInput = File.ReadAllText(inputFile).ToCharArray().Select(c => long.Parse(c.ToString())).ToArray();
        Data = TranslateData(parsedInput);
        Files = TranslateFiles(parsedInput);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Data.Length; i++)
        {
            sb.Append(Data[i] == -1 ? "." : Data[i].ToString());
        }
        return sb.ToString();
    }

    public long CalculateChecksum() => Data.Select((v, i) => v != -1 ? v * i : 0).Sum();

    public void Defrag()
    {
        var moveToCursor = 0;
        var moveFromCursor = Data.Length - 1;
        while (moveToCursor < moveFromCursor)
        {
            if (Data[moveToCursor] != -1)
            {
                moveToCursor++;
            }
            else if (Data[moveFromCursor] == -1)
            {
                moveFromCursor--;
            }
            else
            {
                Data[moveToCursor] = Data[moveFromCursor];
                Data[moveFromCursor] = -1;
            }
        }
    }

    public void Defrag2()
    {
        var orderedFiles = Files.OrderBy(f => f.Position).ToArray();
        for (var i = Files.Length - 1; i >= 0; i--)
        {
            var fileToMove = Files[i];
            var moveTo = GetFittingGapIndex(orderedFiles, fileToMove);
            if (moveTo != -1)
            {
                fileToMove.Position = moveTo;
                orderedFiles = Files.OrderBy(f => f.Position).ToArray();
            }
        }
        SyncDataWithFiles();
    }

    private void SyncDataWithFiles()
    {
        for (var i = 0; i < Data.Length; i++)
        {
            Data[i] = -1;
        }
        foreach (var file in Files)
        {
            for (var i = 0; i < file.Size; i++)
            {
                Data[file.Position + i] = file.Id;
            }
        }
    }

    private long GetFittingGapIndex(MyFile[] orderedFiles, MyFile file)
    {
        for (var i = 0; i < orderedFiles.Length - 1; i++)
        {
            var currentFile = orderedFiles[i];

            if (currentFile.Id == file.Id)
            {
                return -1;
            }
            var nextFile = orderedFiles[i + 1];
            var gap = nextFile.Position - (currentFile.Position + currentFile.Size);
            if (gap >= file.Size)
            {
                return currentFile.Position + currentFile.Size;
            }
        }
        return -1;
    }

    private long[] TranslateData(long[] input)
    {
        var data = new List<long>();
        for (var i = 0; i < input.Length; i++)
        {
            if (i % 2 == 0)
            {
                var fileId = i / 2;
                for (var j = 0; j < input[i]; j++)
                {
                    data.Add(fileId);
                }
            }
            else
            {
                for (var j = 0; j < input[i]; j++)
                {
                    data.Add(-1);
                }
            }
        }
        return data.ToArray();
    }

    private MyFile[] TranslateFiles(long[] input)
    {
        var files = new List<MyFile>();
        long position = 0;
        for (var i = 0; i < input.Length; i++)
        {
            if (i % 2 == 0)
            {
                var fileId = i / 2;
                files.Add(new MyFile { Id = fileId, Size = input[i], Position = position });
            }
            position += input[i];
        }
        return files.ToArray();
    }
}

class MyFile
{
    public long Id { get; init; }
    public long Size { get; init; }
    public long Position { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Size: {Size}, Position: {Position}";
    }
}