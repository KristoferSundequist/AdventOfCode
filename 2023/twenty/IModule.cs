public interface IModule
{
    public List<Pulse> OnPulse(Pulse pulse, long iteration);
}