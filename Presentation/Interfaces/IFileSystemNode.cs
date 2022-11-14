namespace Presentation.Interfaces
{
    public interface IFileSystemNode
    {
        public string Name { get; }
        public long Length { get; }
        public double RelativeSize { get; }
    }
}
