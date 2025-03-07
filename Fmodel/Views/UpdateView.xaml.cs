using System;
using System.Threading.Tasks;
using System.Windows;
using FModel.ViewModels;
using FModel.Views.Resources.Controls;

namespace FModel.Views
{
    public partial class UpdateView
    {
        public UpdateView()
        {
            // 优先初始化组件，然后再设置数据上下文，这样可以避免一些潜在的问题
            InitializeComponent();
            DataContext = new UpdateViewModel();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 使用模式匹配简化代码，同时检查数据上下文是否为预期类型
                if (DataContext is UpdateViewModel viewModel)
                {
                    await viewModel.Load();
                }
            }
            catch (Exception ex)
            {
                // 记录异常日志，这里可以使用合适的日志框架，比如Serilog等
                // 以下只是简单地在控制台输出异常信息，实际项目中应替换为更合适的方式
                Console.WriteLine($"加载数据时发生异常: {ex.Message}");
                MessageBox.Show("加载数据时发生错误，请检查日志或联系管理员。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnDownloadLatest(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is UpdateViewModel viewModel)
                {
                    viewModel.DownloadLatest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载最新版本时发生异常: {ex.Message}");
                MessageBox.Show("下载最新版本时发生错误，请检查网络连接或联系管理员。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
