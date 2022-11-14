using Presentation.Commands;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.Threading.Tasks;
using Presentation.Models;
using Core.Services;

namespace Presentation.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private const int MaxThreadCount = 80;

        private readonly Scanner scanner;
        private string path;
        private FileSystemTree tree;
        private volatile bool isRunning;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand ChooseDirectory { get; private set; }
        public ICommand StartScan { get; private set; }
        public ICommand StopScan { get; private set; }

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                Notify(nameof(IsRunning));
            }
        }

        public FileSystemTree Tree
        {
            get => tree;
            set
            {
                tree = value;
                Notify(nameof(Tree));
            }
        }

        public AppViewModel()
        {
            scanner = new Scanner();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ChooseDirectory = new Command(_ =>
            {
                using var openFolderDialog = new FolderBrowserDialog();
                if (openFolderDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                path = openFolderDialog.SelectedPath;
            }, _ => !IsRunning);

            StartScan = new Command(_ =>
            {
                Task.Run(() =>
                {
                    IsRunning = true;
                    var result = scanner.Run(path, MaxThreadCount);
                    IsRunning = false;
                    Tree = new FileSystemTree(result.Root);
                });
            }, _ => !IsRunning && path is not null);

            StopScan = new Command(_ =>
            {
                scanner.Stop();
                IsRunning = false;
            }, _ => IsRunning);
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
