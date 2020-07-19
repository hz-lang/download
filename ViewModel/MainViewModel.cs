using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TxtDownload.Model;

namespace TxtDownload.ViewModel {
	internal sealed class MainViewModel : NotifyPropertyChanged {
		private IList<ChapterModel> _list;

		public bool Downloading { get; private set; }
		public string TableBody { get; private set; }
		public string ChapterBody { get; private set; }

		/// <summary>
		/// 下载的文件内容。
		/// </summary>
		public string Content { get; set; }

		private string _tableUrl = "https://www.booktxt.net/6_6678/";
		public string TableUrl {
			get { return _tableUrl; }
			set { _tableUrl = value; OnChanged(); }
		}

		private string _content;
		public string TableContent {
			get { return _content; }
			set { _content = value; OnChanged(); }
		}

		private ConfigViewModel _config = new ConfigViewModel();
		public ConfigViewModel Config {
			get { return _config; }
			set { _config = value; OnChanged(); }
		}

		/// <summary>
		/// 获取网页的完整内容。
		/// </summary>
		/// <param name="client">网络客户端。</param>
		/// <returns>无。</returns>
		public async Task SetContentAsync(HttpClient client) {
			if (string.IsNullOrEmpty(TableUrl)) return;
			if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			var content = await HttpClientHelper.GetStringAsync(client, TableUrl);

			TableBody = HtmlHelper.GetBody(content);
			TableContent = TableBody;
			Content = null;
		}

		/// <summary>
		/// 测试目录解析结果。
		/// </summary>
		/// <returns>目录解析结果。</returns>
		public string GetTable() {
			SetChapterUrls();

			var sb = new StringBuilder(100 * 1000);
			foreach (var it in _list) {
				sb.AppendLine(it.ToString());
			}

			return sb.ToString();
		}

		/// <summary>
		/// 测试第一章内容解析。
		/// </summary>
		/// <returns>第一章内容的解析结果。</returns>
		public async Task<string> GetChapterAsync(HttpClient client) {
			if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			var c = _list.FirstOrDefault();
			if (c == null)
				return "空";

			ChapterBody = await c.GetContentAsync(client);
			return c.GetBody(ChapterBody, Config.Chapter);
		}

		/// <summary>
		/// 获取所有章节内容。
		/// </summary>
		/// <param name="client">网络客户端。</param>
		/// <returns>所有章节内容。</returns>
		public async Task<string> GetChaptersAsync(HttpClient client) {
			if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			// 快速通道，直接返回已经下载的数据。
			if (!string.IsNullOrEmpty(Content)) return Content;

			SetChapterUrls();

			Downloading = true;
			TableContent = string.Empty;

			foreach (var it in _list) {
				if (!Downloading) break;

				var c = await it.GetContentAsync(client);
				await Task.Run(() => it.GetBody(c, Config.Chapter));

				TableContent += it.Title +
					(it.Ready() ? " 下载完成" : " 下载失败") +
					Environment.NewLine;
			}

			TableContent += "下载完毕";

			Content = string.Join(
				Environment.NewLine,
				_list.Select(it => it.GetChapter()));
			return Content;
		}

		/// <summary>
		/// 取消下载。
		/// </summary>
		public void Cancel() {
			Downloading = false;
		}

		/// <summary>
		/// 设置章节地址。
		/// </summary>
		private void SetChapterUrls() {
			if (string.IsNullOrEmpty(TableContent))
				throw new NullReferenceException("网页内容为空");

			var start = TableContent.IndexOf(Config.Table.Start);
			var end = TableContent.IndexOf(Config.Table.Stop);

			start += Config.Table.Start.Length;
			if (start >= end)
				throw new NullReferenceException("网页内容为空");

			var list = Regex.Matches(TableContent[start..end], Config.Table.Pattern);

			_list = new List<ChapterModel>(list.Count);
			foreach (Match it in list) {
				if (!it.Success || it.Groups.Count != 3) {
					continue;
				}

				_list.Add(new ChapterModel {
					Url = GetUri(it.Groups[1].Value),
					Title = it.Groups[2].Value
				});
			}
		}

		// TableUrl = http://www.xbiquge.la/28/28056/
		// uri = /28/28056/13639427.html
		private string GetUri(string uri) {
			var i = uri.LastIndexOf('/');
			if (i > 0) i += 1;
			else i = 0;
			return TableUrl + uri[i..];
		}
	}
}
