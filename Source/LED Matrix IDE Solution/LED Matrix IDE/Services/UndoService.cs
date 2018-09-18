using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedMatrixIde.Interfaces;

namespace LedMatrixIde.Services
{
	public class UndoTask : IUndoTask
	{
		public AsyncTask UndoAction { get; set; }
		public AsyncTask RedoAction { get; set; }
	}

	public class UndoService : IUndoService
	{
		protected Stack<IUndoTask> UndoStack { get; } = new Stack<IUndoTask>();
		protected Stack<IUndoTask> RedoStack { get; } = new Stack<IUndoTask>();

		public bool CanUndo => true;// (this.UndoStack.Count() > 0);
		public bool CanRedo => true;// (this.RedoStack.Count() > 0);

		public async Task AddUndoTask(AsyncTask undoAction, AsyncTask redoAction)
		{
			await this.AddUndoTask(new UndoTask() { UndoAction = undoAction, RedoAction = redoAction });
		}

		public Task AddUndoTask(IUndoTask undoTask)
		{
			this.UndoStack.Push(undoTask);
			return Task.FromResult(0);
		}

		public async Task Redo()
		{
			if (this.RedoStack.Count() > 0)
			{
				IUndoTask undoTask = this.RedoStack.Pop();
				this.UndoStack.Push(undoTask);
				await undoTask.RedoAction.Invoke();
			}
		}

		public async Task Undo()
		{
			if (this.UndoStack.Count() > 0)
			{
				IUndoTask undoTask = this.UndoStack.Pop();
				this.RedoStack.Push(undoTask);
				await undoTask.UndoAction.Invoke();
			}
		}

		public Task Clear()
		{
			this.UndoStack.Clear();
			this.RedoStack.Clear();
			return Task.FromResult(0);
		}
	}
}
