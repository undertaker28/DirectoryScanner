using Core.Interfaces;
using Core.Models;
using System.Collections.Concurrent;

namespace Core.Services
{
    public class Scanner : IScanner
    {
        private int maxThreadCount;
        private CancellationTokenSource? cancellationTokenSource;
        private CancellationToken cancellationToken;
        private SemaphoreSlim? semaphore;
        private ConcurrentQueue<FileSystemNode>? queue;

        public bool IsRunning { get; private set; }

        public FileSystemTree Run(string path, int maxThreadCount)
        {
            if (File.Exists(path))
            {
                FileInfo fileInfo = new(path);
                FileSystemNode node = new(fileInfo.FullName, fileInfo.Name, fileInfo.Length);
                return new FileSystemTree(node);
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException($"Directory {path} doesn't exist.", nameof(path));
            }

            if (maxThreadCount <= 0)
            {
                throw new ArgumentException("Max thread count must be greater than 0.", nameof(maxThreadCount));
            }

            DirectoryInfo directoryInfo = new(path);
            FileSystemNode root = new(directoryInfo.FullName, directoryInfo.Name);
            InitializeFields(maxThreadCount);

            IsRunning = true;
            queue!.Enqueue(root);
            ScanDirectoriesParallel();
            IsRunning = false;

            return new FileSystemTree(root);
        }

        public void Stop()
        {
            if (IsRunning)
            {
                cancellationTokenSource?.Cancel();
                IsRunning = false;
            }
        }

        private void InitializeFields(int maxThreadCount)
        {
            this.maxThreadCount = maxThreadCount;
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            semaphore = new SemaphoreSlim(maxThreadCount);
            queue = new ConcurrentQueue<FileSystemNode>();
        }

        private void ScanDirectoriesParallel()
        {
            do
            {
                bool isDequeueSuccessful = queue!.TryDequeue(out var node);
                if (isDequeueSuccessful)
                {
                    try
                    {
                        semaphore!.Wait(cancellationToken);
                        Task.Run(() =>
                        {
                            ScanDirectory(node!);
                            semaphore.Release();
                        }, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // cancellationToken was cancelled.
                    }
                }
            } while (!cancellationToken.IsCancellationRequested && 
                    (semaphore!.CurrentCount != maxThreadCount || !queue.IsEmpty));
        }

        private void ScanDirectory(FileSystemNode node)
        {
            node.Children = new List<FileSystemNode>();
            DirectoryInfo directoryInfo = new(node.FullName);

            node.Children.AddRange(GetFiles(directoryInfo));
            node.Children.AddRange(GetDirectories(directoryInfo));
        }

        private List<FileSystemNode> GetFiles(DirectoryInfo directoryInfo)
        {
            List<FileSystemNode> result = new();
            FileInfo[] files;
            try
            {
                files = directoryInfo.GetFiles();
            }
            catch (Exception)
            {
                // The caller does not have the required permission.
                return result;
            }

            foreach (FileInfo file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return result;
                }

                if (file.LinkTarget is null)
                {
                    result.Add(new FileSystemNode(file.FullName, file.Name, file.Length));
                }
            }

            return result;
        }

        private List<FileSystemNode> GetDirectories(DirectoryInfo directoryInfo)
        {
            List<FileSystemNode> result = new();
            DirectoryInfo[] directories;
            try
            {
                directories = directoryInfo.GetDirectories();
            }
            catch (Exception)
            {
                // The caller does not have the required permission.
                return result;
            }

            foreach (DirectoryInfo directory in directories)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return result;
                }

                FileSystemNode childNode = new(directory.FullName, directory.Name);
                queue!.Enqueue(childNode);
                result.Add(childNode);
            }

            return result;
        }
    }
}
