using Photobooth.Properties;

namespace Photobooth
{
	public class PhotoboothSettings
	{
		public string PromptText { get; set; }
		public string FontFamilyName { get; set; }
		public double FontSize { get; set; }
		public string ImagePath { get; set; }
		public string ImageExtension { get; set; }
		public int DisplaySeconds { get; set; }

		public static PhotoboothSettings Load()
		{
			return new PhotoboothSettings
			{
				PromptText = Settings.Default.PromptText,
				FontFamilyName = Settings.Default.FontFamily,
				FontSize = Settings.Default.FontSize,
				ImagePath = Settings.Default.ImagePath,
				ImageExtension = Settings.Default.ImageExtension,
				DisplaySeconds = Settings.Default.DisplaySeconds,
			};
		}

		public static void Save(PhotoboothSettings photoboothSettings)
		{
			Settings.Default.PromptText = photoboothSettings.PromptText;
			Settings.Default.FontFamily = photoboothSettings.FontFamilyName;
			Settings.Default.FontSize = photoboothSettings.FontSize;
			Settings.Default.ImagePath = photoboothSettings.ImagePath;
			Settings.Default.ImageExtension = photoboothSettings.ImageExtension;
			Settings.Default.DisplaySeconds = photoboothSettings.DisplaySeconds;

			Settings.Default.Save();
		}
	}

}
