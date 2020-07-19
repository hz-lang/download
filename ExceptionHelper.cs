using System;

namespace TxtDownload {
	internal static class ExceptionHelper {
		public static string GetError(this Exception e) {
			return e.Message + Environment.NewLine + e.StackTrace;
		}
	}
}
