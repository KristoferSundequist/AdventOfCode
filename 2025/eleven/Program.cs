var lines = File.ReadAllLines("input.txt");
var devices = lines.Select(Device.FromString).ToDictionary(d => d.Name);
devices.Add("out", new Device("out", new()));

var result1 = Part1("you");
Console.WriteLine($"Part1: {result1}");

var result = Part2("svr", false, false, new());
Console.WriteLine($"Part2: {result}");

long Part1(string currentDeviceName)
{
    if (currentDeviceName == "out")
    {
        return 1;
    }
    return devices[currentDeviceName].Outputs.Sum(outDeviceName => Part1(outDeviceName));
}

long Part2(string currentDeviceName, bool dac, bool fft, Dictionary<string, long> memo)
{
    var memoKey = (currentDeviceName, dac, fft).ToString();
    if (memo.TryGetValue(memoKey, out var memoizedCount))
    {
        return memoizedCount;
    }
    if (currentDeviceName == "out" && dac && fft)
    {
        return 1;
    }
    var result = devices[currentDeviceName].Outputs.Sum(outDeviceName => Part2(outDeviceName, dac || (outDeviceName == "dac"), fft || (outDeviceName == "fft"), memo));
    memo[memoKey] = result;
    return result;
}