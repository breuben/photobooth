using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobooth
{
	public partial class KioskWindow
	{
		public string PromptText { get; set; }
		public string ImagePath { get; set; }
		public string ImageExtension { get; set; }
		public int DisplaySeconds { get; set; }

		private TextBlock promptTextblock;
		private FileSystemWatcher imageMonitor;

		public KioskWindow(string promptText, string fontFamilyName, double fontSize, string imagePath, string imageExtension, int displaySeconds)
		{
			PromptText = promptText;
			FontFamily = new FontFamily(fontFamilyName);
			FontSize = fontSize;
			ImagePath = imagePath;
			ImageExtension = imageExtension;
			DisplaySeconds = displaySeconds;

			InitializeComponent();

			promptTextblock = new TextBlock
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				TextWrapping = TextWrapping.NoWrap,
				Text = PromptText
			};

			showPrompt();
			startMonitoring();
		}

		private void startMonitoring()
		{
			try
			{
				imageMonitor = new FileSystemWatcher(ImagePath, ImageExtension);
				imageMonitor.Created += ImageMonitorOnCreated;
				imageMonitor.EnableRaisingEvents = true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void ImageMonitorOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			dispatchShowImage(fileSystemEventArgs.FullPath);

			Thread.Sleep(DisplaySeconds * 1000);

			dispatchShowPrompt();
		}

		private void dispatchShowPrompt()
		{
			this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(showPrompt));
		}

		private void showPrompt()
		{
			this.Content = promptTextblock;
		}

		private void dispatchShowImage(string imagePath)
		{
			this.Dispatcher.Invoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				new Action(
				delegate()
				{
					showImage(imagePath);
				}));
		}

		private void showImage(string imagePath)
		{
			try
			{
				var photo = new Image
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Source = new BitmapImage(new Uri(imagePath))
				};

				this.Content = photo;
			}
			catch (Exception)
			{
				// Probabably a threading exception, we can safely ignore, just keep chugging along
			}
		}
	}
}
