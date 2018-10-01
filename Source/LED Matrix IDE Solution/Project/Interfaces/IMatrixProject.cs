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
using Matrix;
using Newtonsoft.Json;
using Windows.UI;

namespace Project
{
	public interface IMatrixProject
	{
		[JsonIgnore]
		IColorMatrix ColorMatrix { get; set; }

		[JsonProperty(PropertyName = "name")]
		string Name { get; set; }

		[JsonProperty(PropertyName = "backgroundColor")]
		Color BackgroundColor { get; set; }

		[JsonProperty(PropertyName = "pixelOutputColumns")]
		uint PixelOutputColumns { get; set; }

		[JsonProperty(PropertyName = "maskOutputColumns")]
		uint MaskOutputColumns { get; set; }

		[JsonProperty(PropertyName = "useRandomSand")]
		bool UseRandomSand { get; set; }

		[JsonProperty(PropertyName = "randomSandCount")]
		uint RandomSandCount { get; set; }

		[JsonProperty(PropertyName = "accelerometerScaling")]
		uint AccelerometerScaling { get; set; }

		[JsonProperty(PropertyName = "elasticity")]
		uint Elasticity { get; set; }

		[JsonProperty(PropertyName = "sortParticles")]
		bool SortParticles { get; set; }
	}
}
