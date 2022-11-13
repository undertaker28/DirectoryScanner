namespace Core.Interfaces
{
    public interface IScanner
    {
        public bool IsRunning { get; }
        public void Stop();

    }
}
