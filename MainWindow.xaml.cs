using Microsoft.Win32;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using TxtDownload.ViewModel;

namespace TxtDownload {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private readonly MainViewModel _ctx;
		private readonly HttpClient _client;

		public MainWindow() {
			InitializeComponent();

			_ctx = new MainViewModel();
			DataContext = _ctx;

			TableTest.IsEnabled = false;
			ChapterTest.IsEnabled = false;
			Download.IsEnabled = false;
			TableResult.Visibility = Visibility.Collapsed;
			ChapterResult.Visibility = Visibility.Collapsed;

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			_client = new HttpClient(new HttpClientHandler {
				AutomaticDecompression = DecompressionMethods.GZip
			});

			_client.DefaultRequestHeaders.Add(
				"User-Agent",
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
				"AppleWebKit/537.36 (KHTML, like Gecko) " +
				"Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.64");
		}

		// 打开目录网址。
		private async void Open_Click(object sender, RoutedEventArgs e) {
			try {
				Open.IsEnabled = false;
				await Task.Delay(3);

				await _ctx.SetContentAsync(_client);

				TableTest.IsEnabled = true;
				Download.IsEnabled = true;
			} catch (Exception ex) {
				_ctx.TableContent = ex.GetError();
			} finally {
				Open.IsEnabled = true;
			}
		}

		// 下载。
		private async void Download_Click(object sender, RoutedEventArgs e) {
			try {
				Download.IsEnabled = false;
				await Task.Delay(3);

				var c = await _ctx.GetChaptersAsync(_client);

				var save = new SaveFileDialog {
					Filter = "文本文件|*.txt|所有文件|*.*"
				};
				if (save.ShowDialog() == true) {
					using var sw = new StreamWriter(save.FileName);
					sw.Write(c);
				}

				MessageBox.Show("保存成功");
			} catch (Exception ex) {
				MessageBox.Show(ExceptionHelper.GetError(ex));
			} finally {
				Download.IsEnabled = true;
			}
		}

		// 关闭。
		private void Close_Click(object sender, RoutedEventArgs e) {
			_client.Dispose();
			Close();
		}

		// 目录测试。
		private async void TableTest_Click(object sender, RoutedEventArgs e) {
			try {
				TableTest.IsEnabled = false;
				await Task.Delay(3);

				var r = _ctx.GetTable();
				TableResult.Text = r;

				ChapterTest.IsEnabled = true;
			} catch (Exception ex) {
				TableResult.Text = ExceptionHelper.GetError(ex);
			}
			finally {
				TableTest.IsEnabled = true;
				TableResult.Visibility = Visibility.Visible;
			}
		}

		// 章节测试。
		private async void ChapterTest_Click(object sender, RoutedEventArgs e) {
			try {
				ChapterTest.IsEnabled = false;
				await Task.Delay(3);

				var r = await _ctx.GetChapterAsync(_client);
				ChapterResult.Text = r;
			} catch (Exception ex) {
				ChapterResult.Text = ExceptionHelper.GetError(ex);
			} finally {
				ChapterTest.IsEnabled = true;
				ChapterResult.Visibility = Visibility.Visible;
			}
		}
	}
}
