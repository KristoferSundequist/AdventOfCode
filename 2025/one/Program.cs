var lines = System.IO.File.ReadAllLines("input.txt");

var position = 50;

var password = 0;
foreach (var line in lines)
{
    position = Rotate(position, line);
    if (position == 0)
    {
        password += 1;
    }
}
Console.WriteLine("Password 1: " + password);

var password2 = 0;
position = 50;
foreach (var line in lines)
{
    var (numZeros, newPosition) = Rotate2(position, line);
    password2 += numZeros;
    position = newPosition;
}
Console.WriteLine("Password 2: " + password2);

int Rotate(int position, string rotation)
{
    var direction = rotation[0];
    var degrees = int.Parse(rotation[1..]) % 100;

    var turn = direction == 'R' ? degrees : -degrees;
    var newPosition = position + turn;
    if (newPosition < 0)
    {
        return 100 + newPosition;
    }
    else if (newPosition >= 100)
    {
        return newPosition - 100;
    }
    else
    {
        return newPosition;
    }
}

(int numZeros, int newPosition) Rotate2(int position, string rotation)
{
    var direction = rotation[0];
    var degrees = int.Parse(rotation[1..]);

    var numZeros = 0;
    for (int i = 0; i < degrees; i++)
    {
        if (i != 0 && position == 0)
        {
            numZeros += 1;
        }

        if (direction == 'R')
        {
            position += 1;
            if (position == 100)
            {
                position = 0;
            }
        }
        else
        {
            position -= 1;
            if (position == -1)
            {
                position = 99;
            }
        }
    }
    if (position == 0 && degrees > 0)
    {
        numZeros += 1;
    }
    return (numZeros, position);
}