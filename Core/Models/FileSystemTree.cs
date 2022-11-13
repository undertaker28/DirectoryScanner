namespace Core.Models
{
    public class FileSystemTree
    {
        public FileSystemNode Root { get; }

        public FileSystemTree(FileSystemNode root)
        {
            Root = root;
            Root.Length = GetNodeLength(root);
        }

        public static long GetNodeLength(FileSystemNode node)
        {
            if (node.Children is null)
            {
                return node.Length;
            }

            foreach (FileSystemNode child in node.Children)
            {
                node.Length += GetNodeLength(child);
            }

            return node.Length;
        }
    }
}
