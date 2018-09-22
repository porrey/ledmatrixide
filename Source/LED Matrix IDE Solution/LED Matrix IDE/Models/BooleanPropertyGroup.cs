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

namespace LedMatrixIde.Models
{
	public delegate void SetPropertyAction(bool value);

	/// <summary>
	/// Groups a set of boolean properties together so that only
	/// one can be true at a time.
	/// </summary>
	public class BooleanPropertyGroup
	{
		/// <summary>
		/// Creates an instance of BooleanPropertyGroup.
		/// </summary>
		/// <param name="items">An array of actions each with as
		/// an assigned key value.</param>
		public BooleanPropertyGroup(params (string, SetPropertyAction)[] items)
		{
			this.Items = items;
		}

		/// <summary>
		/// Gets/sets the items that are grouped together.
		/// </summary>
		protected (string, SetPropertyAction)[] Items { get; set; }

		/// <summary>
		/// Set the value of all properties to false
		/// except for the one specified by the key name.
		/// </summary>
		/// <param name="itemKey">The item key that will not be reset.</param>
		/// <param name="value">The current value of the property identified
		/// by the key.</param>
		public void SetItem(string itemKey, bool value)
		{
			if (value)
			{
				foreach ((string key, SetPropertyAction action) in this.Items)
				{
					if (key != itemKey)
					{
						action.Invoke(false);
					}
				}
			}
		}
	}
}
