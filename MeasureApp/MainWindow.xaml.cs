using MeasureApp.Model;
using MeasureApp.ViewModel;
using Microsoft.Xaml.Behaviors;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowDataContext();

            // 默认文件选择路径加载
            if (Properties.Settings.Default.DefaultDirectory == string.Empty)
                Properties.Settings.Default.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            _documents = new ObservableCollection<DocumentViewModel>();
            Items.ItemsSource = _documents;
            Loaded += MainWindow_Loaded;
        }
        private RoslynHost _host;
        private readonly ObservableCollection<DocumentViewModel> _documents;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _host = new RoslynHost(additionalAssemblies: new[]
            {
                Assembly.Load("RoslynPad.Roslyn.Windows"),
                Assembly.Load("RoslynPad.Editor.Windows")
            }, RoslynHostReferences.NamespaceDefault.With(assemblyReferences: new[]
            {
                typeof(object).Assembly,
                typeof(System.Text.RegularExpressions.Regex).Assembly,
                typeof(System.Linq.Enumerable).Assembly,
                typeof(ViewModel.MainWindowDataContext).Assembly
            }));

            AddNewDocument();
        }

        private void AddNewDocument(DocumentViewModel previous = null)
        {
            _documents.Add(new DocumentViewModel(_host, previous));
        }

        private void OnItemLoaded(object sender, EventArgs e)
        {
            var editor = (RoslynCodeEditor)sender;
            editor.Loaded -= OnItemLoaded;
            editor.Focus();

            var viewModel = (DocumentViewModel)editor.DataContext;
            var workingDirectory = Directory.GetCurrentDirectory();

            var previous = viewModel.LastGoodPrevious;
            if (previous != null)
            {
                editor.CreatingDocument += (o, args) =>
                {
                    args.DocumentId = _host.AddRelatedDocument(previous.Id, new DocumentCreationArgs(
                        args.TextContainer, workingDirectory, args.ProcessDiagnostics,
                        args.TextContainer.UpdateText));
                };
            }

            var documentId = editor.Initialize(_host, new ClassificationHighlightColors(),
                workingDirectory, string.Empty);

            viewModel.Initialize(documentId);
        }

        private async void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var editor = (RoslynCodeEditor)sender;
                if (editor.IsCompletionWindowOpen)
                {
                    return;
                }

                e.Handled = true;

                var viewModel = (DocumentViewModel)editor.DataContext;
                if (viewModel.IsReadOnly) return;

                viewModel.Text = editor.Text;
                if (await viewModel.TrySubmit())
                {
                    AddNewDocument(viewModel);
                }
            }
        }
    }
}
