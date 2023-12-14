//int[] times = [7, 15, 30];
//int[] distances = [9, 40, 200];
int[] times = [54, 94, 65, 92];
int[] distances = [302, 1476, 1029, 1404];

var result1 = Enumerable.Range(0, times.Length).Select(i => GetNumWinningWays(times[i], distances[i])).Aggregate((long)1, (product, v) => product * v);
Console.WriteLine($"Result 1: {result1}");

var result2 = GetNumWinningWays(54946592, 302147610291404);
Console.WriteLine($"Result 2: {result2}");


long GetNumWinningWays(long maxTime, long maxDistance)
{
    long numWays = 0;
    for (long waitTime = 0; waitTime < maxTime; waitTime++)
    {
        if ((maxTime - waitTime) * waitTime > maxDistance)
        {
            numWays++;
        }
    }
    return numWays;
}
