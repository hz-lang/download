using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TxtDownload.ViewModel {
	internal abstract class NotifyPropertyChanged : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
