﻿// Copyright © 2018 Daniel Porrey. All Rights Reserved.
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
using Project;

namespace CodeBuilder.Decorators
{
	public static class ProjectDecorator
	{
		public static string CppFileName(this IMatrixProject project) => $"{project.Name}-image.cpp";
		public static string HeaderFileName(this IMatrixProject project) => $"{project.Name}-image.h";
		public static string MakeFileName(this IMatrixProject project) => $"{project.Name}-make.txt";
		public static string InstructionsFileName(this IMatrixProject project) => $"{project.Name}-instructions.txt";
	}
}
