using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.LicenseManagement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Dump.ViewModels;
using ReactiveUI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Dump
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IViewFor<HomeViewModel>
    {
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new HomeViewModel();

            if (!DesignMode.DesignModeEnabled)
            {
                this.WhenActivated(d =>
                {
                    d(this.Bind(ViewModel, vm => vm.Path, view => view.Path.Text));
                    d(this.Bind(ViewModel, vm => vm.Search, view => view.SearchBox.Text));
                    d(this.Bind(ViewModel, vm => vm.SelectedRow, view => view.Data.SelectedItem));

                    d(this.WhenAnyValue(view => view.ViewModel.Data.Documents)
                        .Select(documents => documents.SelectMany(doc => doc.Data))
                        .BindTo(this, view => view.Data.ItemsSource));

                    d(this.WhenAnyValue(view => view.ViewModel.Data.Documents)
                        .Select(documents => String.Join(Environment.NewLine, documents.Select(doc => doc.Text)))
                        .BindTo(this, view => view.DocumentText.Text));

                    d(this.WhenAnyValue(view => view.ViewModel.Data.Documents)
                        .Select(documents => documents.SelectMany(doc => doc.Data))
                        .BindTo(this, view => view.SearchBox.AutoCompleteSource));

                    d(this.WhenAnyValue(view => view.ViewModel.SelectedRow)
                        .Where(row => row != null)
                        .Select(row => ViewModel.GetSelectionForData(row, DocumentText.Text))
                        .Do(selection => DocumentText.Select(selection.start, selection.end - selection.start))
                        .Subscribe());

                    d(this.BindCommand(ViewModel, vm => vm.LoadPath, view => view.PathPicker));

                    d(ViewModel.LoadPathInteraction.RegisterHandler(async ctx =>
                    {
                        var picker = new FileOpenPicker
                        {
                            ViewMode = PickerViewMode.List,
                            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                        };
                        picker.FileTypeFilter.Add(".xml");

                        var file = await picker.PickSingleFileAsync();
                        if (file != null)
                        {
                            StorageApplicationPermissions.FutureAccessList.Add(file);
                        }
                        ctx.SetOutput(file?.Path);
                    }));
                });
            }
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (HomeViewModel)value;
        }

        public HomeViewModel ViewModel { get; set; }


    }
}
