using Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace Presentation.Models
{
    public class Directory : IFileSystemNode
    {
        public string Name { get; }
        public long Length { get; }
        public double RelativeSize { get; }
        public ObservableCollection<IFileSystemNode> Children { get; }
        public Directory(string name, long length, double relativeSize)
        {
            Name = name;
            Length = length;
            RelativeSize = relativeSize;
            Children = new ObservableCollection<IFileSystemNode>();
        }
    }
}
