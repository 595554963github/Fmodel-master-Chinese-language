﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CUE4Parse.FileProvider.Objects;
using FModel.Services;
using FModel.ViewModels;

namespace FModel.Views;

public partial class SearchView
{
    private ThreadWorkerViewModel _threadWorkerView => ApplicationService.ThreadWorkerView;
    private ApplicationViewModel _applicationView => ApplicationService.ApplicationView;

    public SearchView()
    {
        DataContext = _applicationView;
        InitializeComponent();

        Activate();
        WpfSuckMyDick.Focus();
        WpfSuckMyDick.SelectAll();
    }

    private void OnDeleteSearchClick(object sender, RoutedEventArgs e)
    {
        _applicationView.CUE4Parse.SearchVm.FilterText = string.Empty;
        _applicationView.CUE4Parse.SearchVm.RefreshFilter();
    }

    private async void OnAssetDoubleClick(object sender, RoutedEventArgs e)
    {
        if (SearchListView.SelectedItem is not GameFile entry)
            return;

        WindowState = WindowState.Minimized;
        MainWindow.YesWeCats.AssetsListName.ItemsSource = null;
        var folder = _applicationView.CustomDirectories.GoToCommand.JumpTo(entry.Directory);
        if (folder == null) return;

        MainWindow.YesWeCats.Activate();

        do { await Task.Delay(100); } while (MainWindow.YesWeCats.AssetsListName.Items.Count < folder.AssetsList.Assets.Count);

        MainWindow.YesWeCats.LeftTabControl.SelectedIndex = 2; // assets tab
        do
        {
            await Task.Delay(100);
            MainWindow.YesWeCats.AssetsListName.SelectedItem = entry;
            MainWindow.YesWeCats.AssetsListName.ScrollIntoView(entry);
        } while (MainWindow.YesWeCats.AssetsListName.SelectedItem == null);
    }

    private async void OnAssetExtract(object sender, RoutedEventArgs e)
    {
        if (SearchListView.SelectedItem is not GameFile entry)
            return;

        WindowState = WindowState.Minimized;
        await _threadWorkerView.Begin(cancellationToken => _applicationView.CUE4Parse.Extract(cancellationToken, entry, true));

        MainWindow.YesWeCats.Activate();
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        _applicationView.CUE4Parse.SearchVm.RefreshFilter();
    }

    private void OnStateChanged(object sender, EventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                Activate();
                WpfSuckMyDick.Focus();
                WpfSuckMyDick.SelectAll();
                return;
        }
    }
}
