public record struct Pulse(string source, string destination, PulseType type);
public enum PulseType
{
    High,
    Low
}