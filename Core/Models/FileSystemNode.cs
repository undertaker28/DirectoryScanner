namespace Core.Models
{
    public class FileSystemNode
    {
        public string FullName { get; }
        public string Name { get; }
        public long Length { get; set; }
        public List<FileSystemNode>? Children { get; set; }

        public FileSystemNode(string fullName, string name)
        {
            FullName = fullName;
            Name = name;
        }

        public FileSystemNode(string fullName, string name, long length) : this(fullName, name)
        {
            Length = length;
        }

        public FileSystemNode(string fullName, string name, List<FileSystemNode>? children) : this(fullName, name)
        {
            Children = children;
        }
    }
}
