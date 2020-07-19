using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TxtDownload {
	internal static class HttpClientHelper {
		/// <summary>
		/// 获取网页的字符串形式。
		/// </summary>
		/// <param name="client">网络客户端。</param>
		/// <param name="uri">网页的网址。</param>
		/// <returns>网页的字符串形式。</returns>
		public static async Task<string> GetStringAsync(HttpClient client, string uri, string charSet) {
			if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrEmpty(uri)) {
				throw new ArgumentException("网址为空", nameof(uri));
			}

			if (string.IsNullOrEmpty(charSet)) {
				throw new ArgumentException("字符集为空", nameof(charSet));
			}

			using var response = await client.GetAsync(uri);
			response.EnsureSuccessStatusCode();

			response.Content.Headers.ContentEncoding.Add("gzip");
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html") {
				CharSet = charSet
			};
			return await response.Content.ReadAsStringAsync();
		}
	}
}
