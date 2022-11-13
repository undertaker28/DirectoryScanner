using Core.Models;

namespace Core.Interfaces
{
    public interface IScanner
    {
        public bool IsRunning { get; }
        public FileSystemTree Run(string path, int maxThreadCount);
        public void Stop();

    }
}
