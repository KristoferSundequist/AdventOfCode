public class Computer
{
    private long A;
    private long B;
    private long C;
    private List<long> Program;
    private long InstructionPointer;
    private List<long> Output = [];

    public Computer(long a, long b, long c, List<long> program)
    {
        A = a;
        B = b;
        C = c;
        Program = program;
        InstructionPointer = 0;
    }

    public void Run()
    {
        while (InstructionPointer < Program.Count)
        {
            var opcode = Program[(int)InstructionPointer];
            var operand = Program[(int)InstructionPointer + 1];
            RunInstruction(opcode, operand);
        }
    }

    public void PrintOutput()
    {
        Console.WriteLine(string.Join(",", Output));
    }

    public bool IsCorrect()
    {
        while (InstructionPointer < Program.Count)
        {
            var opcode = Program[(int)InstructionPointer];
            var operand = Program[(int)InstructionPointer + 1];
            RunInstruction(opcode, operand);
            if (!IsCorrect(Program, Output))
            {
                return false;
            }

        }
        return true;
    }

    private static bool IsCorrect(List<long> expectedOutput, List<long> actualOutput)
    {
        for (int i = 0; i < actualOutput.Count; i++)
        {
            if (expectedOutput[i] != actualOutput[i])
            {
                return false;
            }
        }
        return true;
    }

    private void RunInstruction(long opcode, long operand)
    {
        switch (opcode)
        {
            case 0:
                Adv(operand);
                break;
            case 1:
                Bxl(operand);
                break;
            case 2:
                Bst(operand);
                break;
            case 3:
                if (Jnz(operand))
                {
                    return;
                }
                break;
            case 4:
                Bxc(operand);
                break;
            case 5:
                Out(operand);
                break;
            case 6:
                Bdv(operand);
                break;
            case 7:
                Cdv(operand);
                break;
            default:
                throw new ArgumentException($"Invalid opcode {opcode}");
        }

        InstructionPointer += 2;
    }

    private void Adv(long operand)
    {
        var result = A / (long)Math.Pow(2, GetComboOperand(operand));
        A = (long)Math.Truncate((decimal)result);
    }

    private void Bxl(long operand)
    {
        B = B ^ operand;
    }

    private void Bst(long operand)
    {
        B = GetComboOperand(operand) % 8;
    }

    private bool Jnz(long operand)
    {
        if (A != 0)
        {
            InstructionPointer = operand;
            return true;
        }
        return false;
    }

    private void Bxc(long operand)
    {
        B = B ^ C;
    }

    private void Out(long operand)
    {
        Output.Add(GetComboOperand(operand) % 8);
    }

    private void Bdv(long operand)
    {
        var result = A / (long)Math.Pow(2, GetComboOperand(operand));
        B = (long)Math.Truncate((decimal)result);
    }

    private void Cdv(long operand)
    {
        var result = A / (long)Math.Pow(2, GetComboOperand(operand));
        C = (long)Math.Truncate((decimal)result);
    }

    private long GetComboOperand(long value)
    {
        return value switch
        {
            <= 3 => value,
            4 => A,
            5 => B,
            6 => C,
            _ => throw new ArgumentException($"Invalid combo operand {value}")
        };

    }
}