var initial = File.ReadAllLines("initial.txt");
var gates = File.ReadAllLines("gates.txt");

var device = new Device(initial, gates);
//device.Visualize();
device.Run();
var output = device.GetOutput();
Console.WriteLine(output);
Console.WriteLine(Convert.ToString(output, 2));


// Part2: Visualize the computation graph from the "Advance method" in the device class and find where the wires are wrongly connected by visual inspection
// gbs,hwq,thm,wrm,wss,z08,z22,z29