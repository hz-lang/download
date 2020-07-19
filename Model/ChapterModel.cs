using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TxtDownload.ViewModel;

namespace TxtDownload.Model {
	internal sealed class ChapterModel {
		public string Url { get; set; }
		public string Title { get; set; }

		public string Content { get; private set; }
		public string Body { get; private set; }

		/// <summary>
		/// 获取网页内容。
		/// </summary>
		/// <param name="http">网络客户端。</param>
		/// <returns>网页内容。</returns>
		public async Task<string> GetContentAsync(HttpClient http) {
			if (http is null) {
				throw new ArgumentNullException(nameof(http));
			}

			if (string.IsNullOrEmpty(Url))
				throw new NullReferenceException("网址为空");

			Content = await HttpClientHelper.GetStringAsync(http, Url);
			return Content;
		}

		/// <summary>
		/// 获取解析后的章节内容。
		/// </summary>
		/// <param name="content">网页内容。</param>
		/// <param name="pattern">匹配模式。</param>
		/// <returns>解析后的章节内容。</returns>
		public string GetBody(string content, PagePatternViewModel pattern) {
			if (content is null) {
				throw new ArgumentNullException(nameof(content));
			}

			if (pattern is null) {
				throw new ArgumentNullException(nameof(pattern));
			}

			var start = content.IndexOf(pattern.Start);
			var end = content.IndexOf(pattern.Stop);

			var sb = new StringBuilder(content.Length);

			var list = Regex.Matches(content[start..end], pattern.Pattern);
			foreach (Match it in list) {
				if (it.Success && it.Groups.Count == 2) {
					var line = it.Groups[1].Value;
					if (!string.IsNullOrEmpty(line)) {
						sb.AppendLine(line);
					}
				}
			}

			Body = sb.ToString();
			return Body;
		}

		/// <summary>
		/// 获取章节的标题及内容。
		/// </summary>
		/// <returns>章节的标题及内容。</returns>
		public string GetChapter() {
			return Title + Environment.NewLine + Body;
		}

		/// <summary>
		/// 获取章节网址及标题。
		/// </summary>
		/// <returns>章节网址及标题。</returns>
		public override string ToString() {
			return Url + " " + Title;
		}

		/// <summary>
		/// 获取一个值，指示是否解析成功。
		/// </summary>
		/// <returns>true 表示解析成功。</returns>
		internal bool Ready() {
			return !string.IsNullOrEmpty(Body) && !Body.Equals(Content, StringComparison.Ordinal);
		}
	}
}
