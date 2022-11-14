using Core.Models;
using Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace Presentation.Models
{
    public class FileSystemTree
    {
        public ObservableCollection<IFileSystemNode> Root { get; }

        public FileSystemTree(FileSystemNode root)
        {
            Root = new ObservableCollection<IFileSystemNode>();
            IFileSystemNode presentationRoot = ConvertNode(root, root.Length);
            Root.Add(presentationRoot);
        }

        private static IFileSystemNode ConvertNode(FileSystemNode node, long parentLength)
        {
            double relativeSize = CalculateRelativeSize(node.Length, parentLength);
            if (node.Children is null)
            {
                return ConvertToFile(node, relativeSize);
            }

            return ConvertToDirectory(node, relativeSize);
        }

        private static double CalculateRelativeSize(long nodeLength, long parentLength)
        {
            double relativeSize = (double)nodeLength / parentLength * 100;
            if (double.IsNaN(relativeSize))
            {
                relativeSize = 0;
            }

            return relativeSize;
        }

        private static IFileSystemNode ConvertToDirectory(FileSystemNode node, double relativeSize)
        {
            Directory directory = new(node.Name, node.Length, relativeSize);
            foreach (FileSystemNode child in node.Children!)
            {
                directory.Children.Add(ConvertNode(child, node.Length));
            }

            return directory;
        }

        private static IFileSystemNode ConvertToFile(FileSystemNode node, double relativeSize)
        {
            return new File(node.Name, node.Length, relativeSize);
        }
    }
}
