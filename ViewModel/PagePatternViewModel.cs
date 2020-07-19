namespace TxtDownload.ViewModel {
	internal sealed class PagePatternViewModel : NotifyPropertyChanged {
		private string _start;
		public string Start {
			get { return _start; }
			set { _start = value; OnChanged(); }
		}

		private string _stop;
		public string Stop {
			get { return _stop; }
			set { _stop = value; OnChanged(); }
		}

		private string _pattern;
		public string Pattern {
			get { return _pattern; }
			set { _pattern = value; OnChanged(); }
		}
	}
}
