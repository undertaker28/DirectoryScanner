using Presentation.Interfaces;

namespace Presentation.Models
{
    public class File : IFileSystemNode
    {
        public string Name { get; }
        public long Length { get; }
        public double RelativeSize { get; }

        public File(string name, long length, double relativeSize)
        {
            Name = name;
            Length = length;
            RelativeSize = relativeSize;
        }
    }
}
