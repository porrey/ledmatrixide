using Windows.UI.Xaml;

namespace CodeBuilder
{
	public class BuildEventArgs : RoutedEventArgs
	{
		public enum BuildEventType
		{
			Information,
			Warning,
			Error
		}

		public BuildEventArgs(BuildEventType eventType, string message)
		{
			this.EventType = eventType;
			this.Message = message;
		}

		public string Message { get; protected set; }
		public BuildEventType EventType { get; protected set; }
	}
}
