using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Dump.Core;
using ReactiveUI;
using Splat;

namespace Dump.ViewModels
{
    public class HomeViewModel : ReactiveObject
    {
        private DumpResult data;
        private string path = "";
        private string search = "";
        private ObservableAsPropertyHelper<DumpData> selectedRow;
        private DumpData userSelectedRow;

        /// <summary>
        /// Gets the importer that is used to load the data.
        /// </summary>
        public IDumpImporter Importer { get; }

        /// <summary>
        /// Gets or sets the path that the data should be loaded from.
        /// </summary>
        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }

        /// <summary>
        /// Gets the data that has been loaded.
        /// </summary>
        public DumpResult Data
        {
            get => data;
            private set => this.RaiseAndSetIfChanged(ref data, value);
        }

        /// <summary>
        /// Gets the row that should currently be selected.
        /// </summary>
        public DumpData SelectedRow
        {
            get => userSelectedRow ?? selectedRow.Value;
            set => this.RaiseAndSetIfChanged(ref userSelectedRow, value);
        }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string Search
        {
            get => search;
            set => this.RaiseAndSetIfChanged(ref search, value);
        }

        /// <summary>
        /// A command that can be used to trigger loading the data from the path.
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadData { get; }
        public ReactiveCommand<Unit, Unit> LoadPath { get; }

        public Interaction<Unit, string> LoadPathInteraction { get; } = new Interaction<Unit, string>();

        public HomeViewModel(IDumpImporter importer = null)
        {
            Importer = importer ?? Locator.Current.GetService<IDumpImporter>();

            var canLoadData = this.WhenAnyValue(vm => vm.Path)
                .Select(path => !string.IsNullOrEmpty(path));
            LoadData = ReactiveCommand.CreateFromTask(LoadDataImpl, canExecute: canLoadData, outputScheduler: RxApp.MainThreadScheduler);
            LoadPath = ReactiveCommand.CreateFromTask(LoadPathImpl);

            LoadPath.InvokeCommand(LoadData);

            selectedRow = this.WhenAnyValue(vm => vm.Search)
                .Where(s => Data != null)
                .Select(s => Data.Documents.SelectMany(doc => doc.Data).FirstOrDefault(d => d.Key == s))
                .Do(data =>
                {
                    if (data != null)
                    {
                        userSelectedRow = null;
                    }
                })
                .ToProperty(this, vm => vm.SelectedRow);
        }

        /// <summary>
        /// Gets the start and end indexes that should be selected for the given data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public (int start, int end) GetSelectionForData(DumpData data, string text)
        {
            var currentLine = 0;
            var lineStart = 0;
            var start = 0;
            var end = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var nc = i < text.Length - 1 ? text[i + 1] : '\0';

                if (c == '\n' || (c == '\r' && nc != '\n'))
                {
                    currentLine++;
                    if (currentLine == data.LineNumber)
                    {
                        start = lineStart;
                        end = i;
                        break;
                    }
                    lineStart = i + 1;
                }
            }
            return (start, end);
        }

        private async Task LoadPathImpl()
        {
            var p = await LoadPathInteraction.Handle(Unit.Default).FirstAsync();
            if (p != null)
            {
                Path = p;
            }
        }

        private async Task LoadDataImpl()
        {
            Data = await Task.Run(() => Importer.LoadFromFileAsync(Path));
        }
    }
}
