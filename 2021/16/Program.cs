using System.Text;
var hexstring = "60556F980272DCE609BC01300042622C428BC200DC128C50FCC0159E9DB9AEA86003430BE5EFA8DB0AC401A4CA4E8A3400E6CFF7518F51A554100180956198529B6A700965634F96C0B99DCF4A13DF6D200DCE801A497FF5BE5FFD6B99DE2B11250034C00F5003900B1270024009D610031400E70020C0093002980652700298051310030C00F50028802B2200809C00F999EF39C79C8800849D398CE4027CCECBDA25A00D4040198D31920C8002170DA37C660009B26EFCA204FDF10E7A85E402304E0E60066A200F4638311C440198A11B635180233023A0094C6186630C44017E500345310FF0A65B0273982C929EEC0000264180390661FC403006E2EC1D86A600F43285504CC02A9D64931293779335983D300568035200042A29C55886200FC6A8B31CE647880323E0068E6E175E9B85D72525B743005646DA57C007CE6634C354CC698689BDBF1005F7231A0FE002F91067EF2E40167B17B503E666693FD9848803106252DFAD40E63D42020041648F24460400D8ECE007CBF26F92B0949B275C9402794338B329F88DC97D608028D9982BF802327D4A9FC10B803F33BD804E7B5DDAA4356014A646D1079E8467EF702A573FAF335EB74906CF5F2ACA00B43E8A460086002A3277BA74911C9531F613009A5CCE7D8248065000402B92D47F14B97C723B953C7B22392788A7CD62C1EC00D14CC23F1D94A3D100A1C200F42A8C51A00010A847176380002110EA31C713004A366006A0200C47483109C0010F8C10AE13C9CA9BDE59080325A0068A6B4CF333949EE635B495003273F76E000BCA47E2331A9DE5D698272F722200DDE801F098EDAC7131DB58E24F5C5D300627122456E58D4C01091C7A283E00ACD34CB20426500BA7F1EBDBBD209FAC75F579ACEB3E5D8FD2DD4E300565EBEDD32AD6008CCE3A492F98E15CC013C0086A5A12E7C46761DBB8CDDBD8BE656780";
var hexstring4 = "A0016C880162017C3686B18A3D4780";
var sumstring = "C200B40A82";
var last = "9C0141080250320F1802104A08";

var binarystring = string.Join("", hexstring.SelectMany(GetBinary));

//Console.WriteLine(str);

const string LITERAL_PACKET_ID = "100";

long versionSum = 0;
var (finalValue, rest) = ReadPacket(binarystring);
//Console.WriteLine(abc);
Console.WriteLine(versionSum);
Console.WriteLine(finalValue);


(long, string) ReadPacket(string binary)
{
    var version = binary[0..3];
    versionSum += BinaryToDecimal(version);
    var typeId = binary[3..6];
    var rest = binary[6..];

    return typeId switch
    {
        LITERAL_PACKET_ID => ReadLiteralPacket(rest),
        _ => ReadOperatorPacket(rest, (Operator)BinaryToDecimal(typeId))
    };
}

(long, string) ReadLiteralPacket(string binary)
{
    var bitsRead = 0;
    var valueBits = new StringBuilder();
    var zeroFound = false;

    while (zeroFound == false)
    {
        if (binary[bitsRead] == '0')
        {
            zeroFound = true;
        }

        valueBits.Append(binary[(bitsRead + 1)..(bitsRead + 5)]);
        bitsRead += 5;
    };

    //Console.WriteLine($"LiteralValue: {valueBits.ToString()}");
    return (BinaryToDecimal(valueBits.ToString()), binary[bitsRead..]);
}

(long, string) ReadOperatorPacket(string binary, Operator op)
{
    var bitsRead = 0;
    var lengthTypeId = binary[0];
    bitsRead++;

    var values = new List<long> { };

    if (lengthTypeId == '0')
    {
        var subPacketLength = (int)BinaryToDecimal(binary[bitsRead..(bitsRead + 15)]);
        bitsRead += 15;
        var subPacketBits = binary[bitsRead..(bitsRead + subPacketLength)];
        while (subPacketBits.Length > 0)
        {
            (var newValue, subPacketBits) = ReadPacket(subPacketBits);
            values.Add(newValue);
        }
        bitsRead += subPacketLength;
    }
    else
    {
        var numberOfPacketsInPacket = BinaryToDecimal(binary[bitsRead..(bitsRead + 11)]);
        bitsRead += 11;
        var rest = binary[bitsRead..];
        for (var i = 0; i < numberOfPacketsInPacket; i++)
        {
            var (newValue, temp) = ReadPacket(rest);
            values.Add(newValue);
            bitsRead += rest.Length - temp.Length;
            rest = temp;
        }
    }

    var operatorResult = ApplyOperator(values, op);

    return (operatorResult, binary[bitsRead..]);
}

long ApplyOperator(List<long> subValues, Operator op) => op switch
{
    Operator.Sum => subValues.Sum(),
    Operator.Product => subValues.Aggregate((long)1, (product, v) => product * v),
    Operator.Min => subValues.Min(),
    Operator.Max => subValues.Max(),
    Operator.GreaterThan => subValues[0] > subValues[1] ? 1 : 0,
    Operator.LessThan => subValues[0] < subValues[1] ? 1 : 0,
    Operator.Equals => subValues[0] == subValues[1] ? 1 : 0,
    _ => throw new Exception($"Unexpected operator {op}")
};

long BinaryToDecimal(string binary)
{
    double sum = 0;
    var power = binary.Length;
    foreach (var c in binary)
    {
        power--;
        sum += Double.Parse(c.ToString()) * Math.Pow((double)2, (double)power);
    }
    return (long)sum;
}

string GetBinary(char c)
{
    return c switch
    {
        '0' => "0000",
        '1' => "0001",
        '2' => "0010",
        '3' => "0011",
        '4' => "0100",
        '5' => "0101",
        '6' => "0110",
        '7' => "0111",
        '8' => "1000",
        '9' => "1001",
        'A' => "1010",
        'B' => "1011",
        'C' => "1100",
        'D' => "1101",
        'E' => "1110",
        'F' => "1111",
        _ => throw new Exception($"Unexpected character: {c} when parsing hex to binary.")
    };
};

enum Operator
{
    Sum = 0,
    Product = 1,
    Min = 2,
    Max = 3,
    GreaterThan = 5,
    LessThan = 6,
    Equals = 7
};