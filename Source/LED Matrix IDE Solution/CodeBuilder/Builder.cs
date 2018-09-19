using System;
using System.Text;
using System.Threading.Tasks;
using ImageConverter;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI;

namespace CodeBuilder
{
	public class Builder : IBuilder
	{
		public event EventHandler<BuildEventArgs> BuildEvent = null;

		public async Task<bool> Build(StorageFolder folder, string projectName, ColorMatrix colorMatrix, ColorMatrix grainMatrix)
		{
			bool returnValue = false;

			// ***
			// *** Get the .cpp code and format it.
			// ***
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, "Reading code template.");
			ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
			ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("CodeBuilder/Code");
			ResourceCandidate resourceValue = resourceMap.GetValue("cpp", resourceContext);
			string codeTemplate = resourceValue.ValueAsString;
			string code = String.Format(codeTemplate, projectName);

			// ***
			// *** Write the .cpp file.
			// ***
			string cppFileName = $"{projectName}-image.cpp";
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing .cpp code file '{cppFileName}'.");
			StorageFile cppFile = await folder.CreateFileAsync(cppFileName, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(cppFile, code, Windows.Storage.Streams.UnicodeEncoding.Utf8);

			// ***
			// *** Generate the header file (.h)
			// ***
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating image code.");
			string imageCode = this.CreateImageCode(colorMatrix, 12);
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating mask code.");
			string maskCode = this.CreateMaskCode(colorMatrix, 24);
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating grain code.");
			string grainCode = this.CreateGrainCode(grainMatrix);
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, "Generating header file code.");
			string headerCode = this.CreateHeaderFile(grainCode, imageCode, maskCode);

			// ***
			// *** Write the header file (.h)
			// ***
			string hFileName = $"{projectName}-image.h";
			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, $"Writing .h code file '{hFileName}'.");
			StorageFile hFile = await folder.CreateFileAsync(hFileName, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(hFile, headerCode, Windows.Storage.Streams.UnicodeEncoding.Utf8);

			this.OnBuildEvent(BuildEventArgs.BuildEventType.Information, $"Completed.");
			returnValue = true;

			return returnValue;
		}

		private string CreateImageCode(ColorMatrix colorMatrix, uint colorsPerRow)
		{
			StringBuilder returnValue = new StringBuilder();

			returnValue.AppendLine($"#define IMAGE_WIDTH  {colorMatrix.Width}");
			returnValue.AppendLine($"#define IMAGE_HEIGHT {colorMatrix.Height}");
			returnValue.AppendLine();
			returnValue.AppendLine("const uint32_t image_color[IMAGE_HEIGHT][IMAGE_WIDTH] =");
			returnValue.AppendLine("{");

			int i = 0;

			for (uint row = 0; row < colorMatrix.Height; row++)
			{
				for (uint column = 0; column < colorMatrix.Width; column++)
				{
					Color color = colorMatrix.Colors[row, column];

					if (i == 0)
					{
						returnValue.Append("\t");
					}

					if (color.A == 0)
					{
						returnValue.Append("0x00000000");
					}
					else
					{
						returnValue.Append($"{this.ColorToHex(color)}");
					}

					if (i < ((colorMatrix.Height * colorMatrix.Width) - 1))
					{
						returnValue.Append(", ");
					}

					if ((i + 1) % colorsPerRow == 0)
					{
						returnValue.AppendLine();
						returnValue.Append("\t");
					}

					i++;
				}
			}

			returnValue.AppendLine();
			returnValue.Append("};");

			return returnValue.ToString();
		}

		private string CreateMaskCode(ColorMatrix colorMatrix, uint colorsPerRow)
		{
			StringBuilder returnValue = new StringBuilder();

			returnValue.AppendLine("const uint8_t image_mask[IMAGE_HEIGHT][IMAGE_WIDTH] =");
			returnValue.AppendLine("{");

			int i = 0;

			for (uint row = 0; row < colorMatrix.Height; row++)
			{
				for (uint column = 0; column < colorMatrix.Width; column++)
				{
					Color color = colorMatrix.Colors[row, column];

					if (i == 0)
					{
						returnValue.Append("\t");
					}

					if (color.A == 0 || color == Colors.Black)
					{
						returnValue.Append($"0");
					}
					else
					{
						returnValue.Append($"1");
					}

					if (i < ((colorMatrix.Height * colorMatrix.Width) - 1))
					{
						returnValue.Append(", ");
					}

					if ((i + 1) % colorsPerRow == 0)
					{
						returnValue.AppendLine();
						returnValue.Append("\t");
					}

					i++;
				}
			}

			returnValue.AppendLine();
			returnValue.Append("};");

			return returnValue.ToString();
		}

		private string CreateGrainCode(ColorMatrix grainMatrix)
		{
			StringBuilder returnValue = new StringBuilder();

			uint grainCount = grainMatrix != null ? this.GetActivePixelCount(grainMatrix) : 0;

			returnValue.AppendLine("// ***");
			returnValue.AppendLine("// *** Define the number of grains of sand on matrix.");
			returnValue.AppendLine("// ***");
			returnValue.AppendLine($"#define NUM_GRAINS {grainCount}");
			returnValue.AppendLine();
			returnValue.AppendLine("// ***");
			returnValue.AppendLine("// *** Defines the initial position and color");
			returnValue.AppendLine("// *** of each grain (x, y, color).");
			returnValue.AppendLine("// ***");
			returnValue.AppendLine("const uint32_t grains[NUM_GRAINS][3] =");
			returnValue.AppendLine("{");

			uint index = 0;

			if (grainMatrix != null)
			{
				for (uint row = 0; row < grainMatrix.Height; row++)
				{
					for (uint column = 0; column < grainMatrix.Width; column++)
					{
						Color color = grainMatrix.Colors[row, column];
						returnValue.Append($"\t{column}, {row}, {this.ColorToHex(color)}");
						index++;

						if (index < grainCount)
						{
							returnValue.AppendLine(",");
						}
					}
				}
			}
			else
			{
				this.OnBuildEvent(BuildEventArgs.BuildEventType.Warning, "No sand grains have been defined.");
			}

			returnValue.AppendLine();
			returnValue.Append("};");

			return returnValue.ToString();
		}

		private string CreateHeaderFile(string grain, string image, string mask)
		{
			StringBuilder returnValue = new StringBuilder();

			returnValue.AppendLine(grain);
			returnValue.AppendLine();
			returnValue.AppendLine(image);
			returnValue.AppendLine();
			returnValue.AppendLine(mask);

			return returnValue.ToString();
		}

		private string ColorToHex(Color color)
		{
			return $"0x{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		private uint GetActivePixelCount(ColorMatrix colorMatrix)
		{
			uint returnValue = 0;

			for (uint row = 0; row < colorMatrix.Height; row++)
			{
				for (uint column = 0; column < colorMatrix.Width; column++)
				{
					Color color = colorMatrix.Colors[row, column];

					if (color.A == 0 || color == Colors.Black)
					{
						returnValue++;
					}
				}
			}

			return returnValue;
		}

		protected void OnBuildEvent(BuildEventArgs.BuildEventType eventType, string message)
		{
			this.BuildEvent?.Invoke(this, new BuildEventArgs(eventType, message));
		}
	}
}
