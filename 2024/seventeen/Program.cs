//var computer = new Computer(729,0,0,[0,1,5,4,3,0]);
//var computer = new Computer(56256477, 0, 0, [2,4,1,1,7,5,1,5,0,3,4,3,5,5,3,0]);
for (long i = 0; i < 10000; i++)
{
    Console.WriteLine(i);
    //var computer = new Computer(i, 0, 0, [0,3,5,4,3,0]);
    var computer = new Computer(i, 0, 0, [2,4,1,1,7,5,1,5,0,3,4,3,5,5,3,0]);
    if (computer.IsCorrect())
    {
        Console.WriteLine("--------------");
        Console.WriteLine(i);
        computer.PrintOutput();
    }
    computer.PrintOutput();

}

/*

2027059637
2,4,1,1,7,5,1,5,0,3,4

*/

// var computer2 = new Computer(117440, 0, 0, [0,3,5,4,3,0]);
// computer2.Run();

// 6, 14, 332, 2027059637