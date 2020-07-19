namespace TxtDownload.ViewModel {
	internal sealed class ConfigViewModel : NotifyPropertyChanged {
		public ConfigViewModel() {
			// <dt>《颤栗高空》正文</dt>
			// <dd><a href ="545037108.html">第1章 这也太高了吧？</a></dd>
			// ...
			// </dl>
			_table = new PagePatternViewModel {
				Start = "正文</dt>",
				Stop = "</dl>",
				Pattern = "<a href =\"([^\"]+?)\">([^<]+?)</a>"
			};

			// <div id="content"><br /><br />　　“咦？”<br /><br />
			// ...
			// <script>chaptererror();</script>
			_chapter = new PagePatternViewModel {
				Start = "<div id=\"content\"><br /><br />",
				Stop = "<script>chaptererror();</script>",
				Pattern = ">(?:&nbsp;)*([^<]+?)<"
			};
		}

		private PagePatternViewModel _table;
		public PagePatternViewModel Table {
			get { return _table; }
			set { _table = value; OnChanged(); }
		}

		private PagePatternViewModel _chapter;
		public PagePatternViewModel Chapter {
			get { return _chapter; }
			set { _chapter = value; OnChanged(); }
		}
	}
}
