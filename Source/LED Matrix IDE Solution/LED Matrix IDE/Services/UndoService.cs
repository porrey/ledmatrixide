// Copyright © 2018 Daniel Porrey. All Rights Reserved.
//
// This file is part of the LED Matrix IDE Solution project.
// 
// The LED Matrix IDE Solution is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// The LED Matrix IDE Solution is distributed in the hope that it will
// be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with the LED Matrix IDE Solution. If not, 
// see http://www.gnu.org/licenses/.
//
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
