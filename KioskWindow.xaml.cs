using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobooth
{
	public partial class KioskWindow
	{
		private readonly PhotoboothSettings _settings;
		private readonly TextBlock _promptTextblock;
		private FileSystemWatcher _imageMonitor;

		private Queue<string> imageQueue = new Queue<string>();

		public KioskWindow(PhotoboothSettings settings)
		{
			_settings = settings;
			FontFamily = new FontFamily(settings.FontFamilyName);
			FontSize = settings.FontSize;

			InitializeComponent();

			_promptTextblock = new TextBlock
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				TextWrapping = TextWrapping.NoWrap,
				Text = settings.PromptText
			};

			ShowPrompt();

			StartMonitoringImageDirectory();
		}

		private void KioskWindow_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
			}
		}

		private void StartMonitoringImageDirectory()
		{
			try
			{
				_imageMonitor = new FileSystemWatcher(_settings.ImagePath, _settings.ImageExtension);
				_imageMonitor.Created += ImageMonitorOnFileCreated;
				_imageMonitor.EnableRaisingEvents = true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

			Task queueWatcher = new Task(WatchImageQueue);
			queueWatcher.Start();
		}

		private void WatchImageQueue()
		{
			while (true)
			{
				while (imageQueue.Count > 0)
					ShowImageFromTopOfQueue();

				ShowPrompt();

				Thread.Sleep(100);
			}
		}

		private void ShowImageFromTopOfQueue()
		{
			string imagePath;
			lock (imageQueue)
			{
				imagePath = imageQueue.Dequeue();
			}

			ShowImage(imagePath);

			Thread.Sleep(_settings.DisplaySeconds * 1000);
		}

		private void ImageMonitorOnFileCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			lock (imageQueue)
			{
				imageQueue.Enqueue(fileSystemEventArgs.FullPath);
			}
		}

		private void ShowPrompt()
		{
			this.Dispatcher.Invoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				new Action(
					() =>
					{
						this.Content = _promptTextblock;
					}));
		}

		private void ShowImage(string imagePath)
		{
			this.Dispatcher.Invoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				new Action(
					() =>
					{
						try
						{
							this.Content = new Image
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
								Source = LoadBitmap(imagePath)
							};
						}
						catch (Exception)
						{
							// Probabably a threading exception, we can safely ignore, just keep chugging along
						}
					}));
		}

		private static BitmapImage LoadBitmap(string imagePath)
		{
			var bitmap = new BitmapImage();
			using (FileStream fs = new FileStream(imagePath, FileMode.Open))
			{
				bitmap.BeginInit();
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.StreamSource = fs;
				bitmap.EndInit();
			}

			bitmap.Freeze();
			return bitmap;
		}
	}
}
