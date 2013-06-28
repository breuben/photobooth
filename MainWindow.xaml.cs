using System;
using System.Windows;

namespace Photobooth
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			var settings = PhotoboothSettings.Load();

			PopulateFormWithSettings(settings);
		}

		private void PopulateFormWithSettings(PhotoboothSettings settings)
		{
			CaptureDirectory.Text = settings.ImagePath;
			CaptureExtension.Text = settings.ImageExtension;
			PreviewDuration.Text = settings.DisplaySeconds.ToString();
			PromptFont.Text = settings.FontFamilyName;
			PromptFontSize.Text = settings.FontSize.ToString();
			PromptText.AppendText(settings.PromptText);
		}

		private PhotoboothSettings ReadSettingsFromForm()
		{
			return new PhotoboothSettings
			{
				ImagePath = CaptureDirectory.Text,
				ImageExtension = CaptureExtension.Text,
				DisplaySeconds = Convert.ToInt32(PreviewDuration.Text),
				FontFamilyName = PromptFont.Text,
				FontSize = Convert.ToInt32(PromptFontSize.Text),
				PromptText = PromptText.Text
			};
		}

		private void EnterKioskMode_Click(object sender, RoutedEventArgs e)
		{
			PhotoboothSettings settings = ReadSettingsFromForm();
			PhotoboothSettings.Save(settings);

			KioskWindow kioskWindow = new KioskWindow(settings);
			kioskWindow.Show();
		}
	}
}
