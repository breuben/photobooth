using System.Windows;
using Photobooth.Properties;

namespace Photobooth
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			StartPhotoboothButton_Click(null, null);
		}

		private void StartPhotoboothButton_Click(object sender, RoutedEventArgs e)
		{
			string promptText = Settings.Default.PromptText;
			string fontFamilyName = Settings.Default.FontFamily;
			double fontSize = Settings.Default.FontSize;
			string imagePath = Settings.Default.ImagePath;
			string imageExtension = Settings.Default.ImageExtension;
			int displaySeconds = Settings.Default.DisplaySeconds;

			KioskWindow kioskWindow = new KioskWindow(promptText, fontFamilyName, fontSize, imagePath, imageExtension, displaySeconds);
			kioskWindow.Show();
		}
	}
}
