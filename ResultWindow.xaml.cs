using System.Windows;

namespace TxtDownload {
	/// <summary>
	/// ResultWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ResultWindow : Window {
		public ResultWindow(string content, string body) {
			InitializeComponent();

			Before.Text = content;
			After.Text = body;
		}

		// 关闭窗口。
		private void Close_Click(object sender, RoutedEventArgs e) {
			Close();
		}
	}
}
