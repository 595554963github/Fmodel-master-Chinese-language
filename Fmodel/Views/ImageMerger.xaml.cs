using AdonisUI.Controls;
using FModel.Extensions;
using FModel.Settings;
using FModel.Views.Resources.Controls;
using Microsoft.Win32;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FModel.Views;

public partial class ImageMerger
{
    private const string FILENAME = "预览.png";
    private byte[] _imageBuffer;

    public ImageMerger()
    {
        InitializeComponent();
    }

    private async void DrawPreview(object sender, DragCompletedEventArgs dragCompletedEventArgs)
    {
        if (ImagePreview.Source != null)
            await DrawPreview().ConfigureAwait(false);
    }

    private async void Click_DrawPreview(object sender, MouseButtonEventArgs e)
    {
        if (ImagePreview.Source != null)
            await DrawPreview().ConfigureAwait(false);
    }

    private async Task DrawPreview()
    {
        AddButton.IsEnabled = false;
        UpButton.IsEnabled = false;
        DownButton.IsEnabled = false;
        DeleteButton.IsEnabled = false;
        ClearButton.IsEnabled = false;
        SizeSlider.IsEnabled = false;
        OpenImageButton.IsEnabled = false;
        SaveImageButton.IsEnabled = false;

        var margin = UserSettings.Default.ImageMergerMargin;
        int num = 1, curW = 0, curH = 0, maxWidth = 0, maxHeight = 0, lineMaxHeight = 0, imagesPerRow = Convert.ToInt32(SizeSlider.Value);
        var positions = new Dictionary<int, SKPoint>();
        var images = new SKBitmap[ImagesListBox.Items.Count];
        for (var i = 0; i < images.Length; i++)
        {
            var item = (ListBoxItem) ImagesListBox.Items[i];
            var ms = new MemoryStream();
            var stream = new FileStream(item.ContentStringFormat, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (item.ContentStringFormat.EndsWith(".tif"))
            {
                await using var tmp = new MemoryStream();
                await stream.CopyToAsync(tmp);
                System.Drawing.Image.FromStream(tmp).Save(ms, ImageFormat.Png);
            }
            else
            {
                await stream.CopyToAsync(ms);
            }

            var image = SKBitmap.Decode(ms.ToArray());
            positions[i] = new SKPoint(curW, curH);
            images[i] = image;

            if (image.Height > lineMaxHeight)
                lineMaxHeight = image.Height;

            if (num % imagesPerRow == 0)
            {
                maxWidth = curW + image.Width + margin;
                curH += lineMaxHeight + margin;
                if (curH > maxHeight)
                    maxHeight = curH;

                curW = 0;
                lineMaxHeight = 0;
            }
            else
            {
                maxHeight = curH + lineMaxHeight + margin;
                curW += image.Width + margin;
                if (curW > maxWidth)
                    maxWidth = curW;
            }

            num++;
        }

        await Task.Run(() =>
        {
            using var bmp = new SKBitmap(maxWidth - margin, maxHeight - margin, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bmp);

            for (var i = 0; i < images.Length; i++)
            {
                using (images[i])
                {
                    canvas.DrawBitmap(images[i], positions[i], new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true });
                }
            }

            using var data = bmp.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream(_imageBuffer = data.ToArray());
            var photo = new BitmapImage();
            photo.BeginInit();
            photo.CacheOption = BitmapCacheOption.OnLoad;
            photo.StreamSource = stream;
            photo.EndInit();
            photo.Freeze();

            Application.Current.Dispatcher.Invoke(delegate { ImagePreview.Source = photo; });
        }).ContinueWith(t =>
        {
            AddButton.IsEnabled = true;
            UpButton.IsEnabled = true;
            DownButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
            SizeSlider.IsEnabled = true;
            OpenImageButton.IsEnabled = true;
            SaveImageButton.IsEnabled = true;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private async void OnImageAdd(object sender, RoutedEventArgs e)
    {
        var fileBrowser = new OpenFileDialog
        {
            Title = "添加图像",
            InitialDirectory = Path.Combine(UserSettings.Default.OutputDirectory, "导出"),
            Multiselect = true,
            Filter = "图像文件(*.png,*.bmp,*.jpg,*.jpeg,*.jfif,*.jpe,*.tiff,*.tif)|*.png;*.bmp;*.jpg;*.jpeg;*.jfif;*.jpe;*.tiff;*.tif|所有文件(*.*)|*.*"
        };
        var result = fileBrowser.ShowDialog();
        if (!result.HasValue || !result.Value) return;

        foreach (var file in fileBrowser.FileNames)
        {
            ImagesListBox.Items.Add(new ListBoxItem
            {
                ContentStringFormat = file,
                Content = Path.GetFileNameWithoutExtension(file)
            });
        }

        SizeSlider.Value = Math.Min(ImagesListBox.Items.Count, Math.Round(Math.Sqrt(ImagesListBox.Items.Count)));
        await DrawPreview().ConfigureAwait(false);
    }

    private async void ModifyItemInList(object sender, RoutedEventArgs e)
    {
        if (ImagesListBox.Items.Count <= 0 || ImagesListBox.SelectedItems.Count <= 0) return;
        var indices = ImagesListBox.SelectedItems.Cast<ListBoxItem>().Select(i => ImagesListBox.Items.IndexOf(i)).ToArray();
        var reloadImage = false;

        switch (((Button) sender).Name)
        {
            case "UpButton":
            {
                if (indices.Length > 0 && indices[0] > 0)
                {
                    for (var i = 0; i < ImagesListBox.Items.Count; i++)
                    {
                        if (!indices.Contains(i)) continue;
                        var item = (ListBoxItem) ImagesListBox.Items[i];
                        ImagesListBox.Items.Remove(item);
                        ImagesListBox.Items.Insert(i - 1, item);
                        item.IsSelected = true;
                        reloadImage = true;
                    }
                }

                ImagesListBox.SelectedItems.Add(indices);
                if (reloadImage)
                {
                    await DrawPreview().ConfigureAwait(false);
                }

                break;
            }
            case "DownButton":
            {
                if (indices.Length > 0 && indices[^1] < ImagesListBox.Items.Count - 1)
                {
                    for (var i = ImagesListBox.Items.Count - 1; i > -1; --i)
                    {
                        if (!indices.Contains(i)) continue;
                        var item = (ListBoxItem) ImagesListBox.Items[i];
                        ImagesListBox.Items.Remove(item);
                        ImagesListBox.Items.Insert(i + 1, item);
                        item.IsSelected = true;
                        reloadImage = true;
                    }
                }

                if (reloadImage)
                {
                    await DrawPreview().ConfigureAwait(false);
                }

                break;
            }
            case "DeleteButton":
            {
                if (ImagesListBox.Items.Count > 0 && ImagesListBox.SelectedItems.Count > 0)
                {
                    for (var i = ImagesListBox.SelectedItems.Count - 1; i >= 0; --i)
                        ImagesListBox.Items.Remove(ImagesListBox.SelectedItems[i]);
                }

                await DrawPreview().ConfigureAwait(false);

                break;
            }
        }
    }

    private void OnClear(object sender, RoutedEventArgs e)
    {
        ImagesListBox.Items.Clear();
        ImagePreview.Source = null;
    }

    private void OnOpenImage(object sender, RoutedEventArgs e)
    {
        if (ImagePreview.Source == null) return;
        Helper.OpenWindow<AdonisWindow>("合并图像", () =>
        {
            new ImagePopout
            {
                Title = "合并图像",
                Width = ImagePreview.Source.Width,
                Height = ImagePreview.Source.Height,
                WindowState = ImagePreview.Source.Height > 1000 ? WindowState.Maximized : WindowState.Normal,
                ImageCtrl = { Source = ImagePreview.Source }
            }.Show();
        });
    }

    private void OnSaveImage(object sender, RoutedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(delegate
        {
            if (ImagePreview.Source == null) return;
            var saveFileDialog = new SaveFileDialog
            {
                Title = "保存图像",
                FileName = FILENAME,
                InitialDirectory = UserSettings.Default.OutputDirectory,
                Filter = "Png文件(*.png)|*.png|所有文件(*.*)|*.*"
            };
            var result = saveFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                fs.Write(_imageBuffer, 0, _imageBuffer.Length);
            }

            SaveCheck(saveFileDialog.FileName, Path.GetFileName(saveFileDialog.FileName));
        });
    }

    private static void SaveCheck(string path, string fileName)
    {
        if (File.Exists(path))
        {
            Log.Information("{文件名称}成功保存", fileName);
            FLogger.Append(ELog.Information, () =>
            {
                FLogger.Text("成功保存", Constants.WHITE);
                FLogger.Link(fileName, path, true);
            });
        }
        else
        {
            Log.Error("{文件名}无法保存", fileName);
            FLogger.Append(ELog.Error, () => FLogger.Text($"无法保存'{fileName}'", Constants.WHITE, true));
        }
    }

    private void OnCopyImage(object sender, RoutedEventArgs e)
    {
        ClipboardExtensions.SetImage(_imageBuffer, FILENAME);
    }
}
