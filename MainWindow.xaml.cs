﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;
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
				if (_ctx.Downloading) {
					_ctx.Cancel();
				} else {
					Download.Content = "取消下载";

					var c = await _ctx.GetChaptersAsync(_client);

					var save = new SaveFileDialog {
						Filter = "文本文件|*.txt|所有文件|*.*"
					};

					if (save.ShowDialog() != true) {
						_ctx.Cancel();
					} else {
						using var sw = new StreamWriter(save.FileName);
						sw.Write(c);
						MessageBox.Show("保存成功");
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(ExceptionHelper.GetError(ex));
			} finally {
				Download.Content = "开始下载";
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
				new ResultWindow(_ctx.TableBody, r).ShowDialog();

				ChapterTest.IsEnabled = true;
			} catch (Exception ex) {
				MessageBox.Show(ExceptionHelper.GetError(ex));
			} finally {
				TableTest.IsEnabled = true;
			}
		}

		// 章节测试。
		private async void ChapterTest_Click(object sender, RoutedEventArgs e) {
			try {
				ChapterTest.IsEnabled = false;
				await Task.Delay(3);

				var r = await _ctx.GetChapterAsync(_client);
				new ResultWindow(_ctx.ChapterBody, r).ShowDialog();
			} catch (Exception ex) {
				MessageBox.Show(ExceptionHelper.GetError(ex));
			} finally {
				ChapterTest.IsEnabled = true;
			}
		}

		// 文本内容改变时，滚动到最后。
		private void Content_TextChanged(object sender, TextChangedEventArgs e) {
			var box = e.OriginalSource as TextBox;
			if (box is null) return;

			box.ScrollToEnd();
		}

		// 网址变化时，清除下载缓存。
		private void Url_TextChanged(object sender, TextChangedEventArgs e) {
			_ctx.Content = null;
		}
	}
}
