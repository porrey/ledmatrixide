using System;
using System.Threading.Tasks;

namespace LedMatrixIde.Interfaces
{
	public delegate Task AsyncTask();

	public interface IUndoTask
	{
		AsyncTask UndoAction { get; set; }
		AsyncTask RedoAction { get; set; }
	}

	public interface IUndoService
	{
		bool CanUndo { get; }
		bool CanRedo { get; }
		Task AddUndoTask(AsyncTask undoAction, AsyncTask redoAction);
		Task AddUndoTask(IUndoTask undoTask);
		Task Redo();
		Task Undo();
		Task Clear();
	}
}
