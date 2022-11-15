using Core.Models;
using Core.Services;
using System.Diagnostics;

namespace Tests
{
    public class ScannerTest : BaseTest
    {
        [Theory]
        [InlineData("invDir", 5)]
        [InlineData("", 5)]
        public void Run_InvalidPath_ThrowsArgumentException(string path, int maxThreadCount)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => scanner.Run(path, maxThreadCount));
        }

        [Theory]
        [InlineData(@"C:\Windows", -2)]
        [InlineData(@"C:\Windows", 0)]
        public void Run_MaxThreaCountLessOrEqualsZero_ThrowsArgumentException(string path, int maxThreadCount)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => scanner.Run(path, maxThreadCount));
        }

        [Theory]
        [InlineData(@"..\..\..\TestFolder\LoremIpsum.txt", 80, "LoremIpsum.txt", 4289)]
        public void Run_CorrectFile_ReturnsCorrectInfo(string path, int maxThreadCount, string expectedName,
            long expectedLength)
        {
            // Arrange

            // Act
            FileSystemTree tree = scanner.Run(path, maxThreadCount);

            // Assert
            Assert.Multiple(
                () => Assert.Equal(expectedName, tree.Root.Name),
                () => Assert.Equal(expectedLength, tree.Root.Length),
                () => Assert.Null(tree.Root.Children));
        }

        [Theory]
        [InlineData(@"..\..\..\TestFolder", 80, "TestFolder", 3, 4289)]
        public void Run_CorrectDirectory_ReturnsCorrectInfo(string path, int maxThreadCount, string expectedName,
            int expectedChildrenCount, long expectedLength)
        {
            // Arrange

            // Act
            FileSystemTree tree = scanner.Run(path, maxThreadCount);

            // Assert
            Assert.Multiple(
                () => Assert.Equal(expectedName, tree.Root.Name),
                () => Assert.Equal(expectedLength, tree.Root.Length),
                () => Assert.NotNull(tree.Root.Children),
                () => Assert.Equal(expectedChildrenCount, tree.Root.Children!.Count));
        }

        [Theory]
        [InlineData(@"C:\Windows", 80)]
        public void Stop_StopLengthLessThanFullLength(string path, int maxThreadCount)
        {
            // Arrange
            Scanner stopScanner = new();
            FileSystemTree? fullTree = null;
            FileSystemTree? stopTree = null;
            Action fullScannerAction = () => fullTree = scanner.Run(path, maxThreadCount);
            Action stopScannerAction = () => stopTree = stopScanner.Run(path, maxThreadCount);

            // Act
            Task fullScannerTask = Task.Run(fullScannerAction);
            Task stopScannerTask = Task.Run(stopScannerAction);

            Thread.Sleep(300);
            stopScanner.Stop();
            Task.WaitAll(fullScannerTask, stopScannerTask);

            // Assert
            Assert.True(fullTree?.Root.Length > stopTree?.Root.Length);
        }
    }
}
