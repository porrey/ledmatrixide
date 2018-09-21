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
using System.Threading.Tasks;
using ImageManager;
using LedMatrixControl;
using LedMatrixIde.Interfaces;

namespace LedMatrixIde.Services
{
	public class PixelEventService : IPixelEventService
	{
		public event EventHandler<PixelSelectedEventArgs> PixelSelected = null;
		public event EventHandler<PixelChangedEventArgs> PixelChanged = null;

		public Task PublishPixelSelectedEvent(PixelSelectedEventArgs e)
		{
			if (this.PixelSelected != null)
			{
				this.PixelSelected.Invoke(this, e);
			}

			return Task.FromResult(0);
		}

		public Task PublishPixelChangedEvent(PixelChangedEventArgs e)
		{
			if (this.PixelChanged != null)
			{
				this.PixelChanged.Invoke(this, e);
			}

			return Task.FromResult(0);
		}
	}
}
