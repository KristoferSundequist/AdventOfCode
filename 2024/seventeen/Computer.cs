public class Computer
{
    private long A;
    private long B;
    private long C;
    private List<short> Program;
    private short InstructionPointer;
    public List<short> Output = [];
    public long NumJumps = 0;

    public Computer(long a, long b, long c, List<short> program)
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
            var opcode = Program[InstructionPointer];
            var operand = Program[InstructionPointer + 1];
            RunInstruction(opcode, operand);
        }
    }

    public bool IsCorrectUptil(int i)
    {
        while (InstructionPointer < Program.Count && Output.Count <= i)
        {
            var opcode = Program[InstructionPointer];
            var operand = Program[InstructionPointer + 1];
            RunInstruction(opcode, operand);
            if (Output.Count > 0)
            {
                if (Output[^1] != Program[Output.Count - 1])
                {
                    return false;
                }
                else if (Output.Count == i + 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void RunInstruction(short opcode, short operand)
    {
        switch (opcode)
        {
            case 0:
                Xdv('A', operand);
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
                    NumJumps++;
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
                Xdv('B', operand);
                break;
            case 7:
                Xdv('C', operand);
                break;
            default:
                throw new ArgumentException($"Invalid opcode {opcode}");
        }

        InstructionPointer += 2;
    }

    private void Bxl(short operand)
    {
        B = B ^ operand;
    }

    private void Bst(short operand)
    {
        B = GetComboOperand(operand) % 8;
    }

    private bool Jnz(short operand)
    {
        if (A != 0)
        {
            InstructionPointer = operand;
            return true;
        }
        return false;
    }

    private void Bxc(short operand)
    {
        B = B ^ C;
    }

    private void Out(short operand)
    {
        Output.Add((short)(GetComboOperand(operand) % 8));
    }

    private void Xdv(char register, short operand)
    {
        var truncatedResult = A >> (int)GetComboOperand(operand);
        if (register == 'A')
        {
            A = truncatedResult;
        }
        else if (register == 'B')
        {
            B = truncatedResult;
        }
        else if (register == 'C')
        {
            C = truncatedResult;
        }
        else
        {
            throw new ArgumentException($"Invalid register {register}");
        }
    }

    private long GetComboOperand(short value)
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