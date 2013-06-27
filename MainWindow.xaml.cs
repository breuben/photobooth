using System.Windows;

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
			string promptText = "Click the button on the remote to take a picture.\nDon't forget to smile!";
			string fontFamilyname = "Gabriola";
			double fontSize = 120;
			string imagePath = @"C:\Captures";
			string imageExtension = "*.jpg";
			int displaySeconds = 5;

			KioskWindow kioskWindow = new KioskWindow(promptText, fontFamilyname, fontSize, imagePath, imageExtension, displaySeconds);
			kioskWindow.Show();
		}
	}
}
