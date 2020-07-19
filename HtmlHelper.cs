using System;

namespace TxtDownload {
	internal static class HtmlHelper {
		/// <summary>
		/// 获取网页主体。
		/// </summary>
		/// <param name="content">网页的完整内容。</param>
		/// <returns>网页主体。</returns>
		public static string GetBody(string content) {
			if (string.IsNullOrEmpty(content)) {
				throw new ArgumentException("网页为空", nameof(content));
			}

			var start = content.IndexOf("<body>");
			var end = content.IndexOf("</body>");

			var body = content[(start + 6)..end];
			return body;
		}
	}
}
