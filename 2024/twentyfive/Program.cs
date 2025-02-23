var lines = File.ReadAllLines("data.txt");
var data = lines.Chunk(8).Select(chunk => new LockOrKey(chunk)).ToList();

var locks = data.Where(x => x.IsLock()).ToList();
var keys = data.Where(x => !x.IsLock()).ToList();

var numFits = keys.Sum(key => locks.Count(_lock => _lock.Fits(key)));

Console.WriteLine(numFits);