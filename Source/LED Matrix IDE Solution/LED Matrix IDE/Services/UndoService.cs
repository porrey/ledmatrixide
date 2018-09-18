using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedMatrixIde.Interfaces;
using Windows.UI.Xaml;

namespace LedMatrixIde.Services
{
	public class UndoTask : IUndoTask
	{
		public string Label { get; set; }
		public AsyncTask UndoAction { get; set; }
		public AsyncTask RedoAction { get; set; }

		public override string ToString()
		{
			return this.Label;
		}
	}

	public class UndoService : IUndoService
	{
		public event EventHandler<RoutedEventArgs> TaskAdded = null;

		protected Stack<IUndoTask> UndoStack { get; } = new Stack<IUndoTask>();
		protected Stack<IUndoTask> RedoStack { get; } = new Stack<IUndoTask>();

		public bool CanUndo => (this.UndoStack.Count() > 0);
		public bool CanRedo => (this.RedoStack.Count() > 0);

		public async Task AddUndoTask(AsyncTask undoAction, AsyncTask redoAction, string label = null)
		{
			await this.AddUndoTask(new UndoTask()
			{
				UndoAction = undoAction,
				RedoAction = redoAction,
				Label = label
			});
		}

		public Task AddUndoTask(IUndoTask undoTask)
		{
			this.UndoStack.Push(undoTask);
			this.OnTaskAdded();
			return Task.FromResult(0);
		}

		public async Task Redo()
		{
			if (this.RedoStack.Count() > 0)
			{
				IUndoTask undoTask = this.RedoStack.Pop();
				this.UndoStack.Push(undoTask);
				await undoTask.RedoAction.Invoke();
				this.OnTaskAdded();
			}
		}

		public async Task Undo()
		{
			if (this.UndoStack.Count() > 0)
			{
				IUndoTask undoTask = this.UndoStack.Pop();
				this.RedoStack.Push(undoTask);
				await undoTask.UndoAction.Invoke();
				this.OnTaskAdded();
			}
		}

		public Task Clear()
		{
			this.UndoStack.Clear();
			this.RedoStack.Clear();
			return Task.FromResult(0);
		}

		public void OnTaskAdded()
		{
			this.TaskAdded?.Invoke(this, new RoutedEventArgs());
		}
	}
}
