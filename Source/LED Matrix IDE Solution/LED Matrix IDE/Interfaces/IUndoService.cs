using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LedMatrixIde.Interfaces
{
	public delegate Task AsyncTask();

	public interface IUndoTask
	{
		string Label { get; set; }
		AsyncTask UndoAction { get; set; }
		AsyncTask RedoAction { get; set; }
	}

	public interface IUndoService
	{
		event EventHandler<RoutedEventArgs> TaskAdded;
		bool CanUndo { get; }
		bool CanRedo { get; }
		Task AddUndoTask(AsyncTask undoAction, AsyncTask redoAction, string label = null);
		Task AddUndoTask(IUndoTask undoTask);
		Task Redo();
		Task Undo();
		Task Clear();
	}
}
