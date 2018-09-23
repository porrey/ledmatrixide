using Matrix;

namespace ImageManager
{
	public static class ActivePixelCountDecorator
	{
		public static uint GetPixelCount(this IColorMatrix colorMatrix, ColorItem.ColorItemType itemType)
		{
			uint returnValue = 0;

			for (uint row = 0; row < colorMatrix.Height; row++)
			{
				for (uint column = 0; column < colorMatrix.Width; column++)
				{
					if (colorMatrix.ColorItems[row, column].ItemType == itemType)
					{
						returnValue++;
					}
				}
			}

			return returnValue;
		}
	}
}
