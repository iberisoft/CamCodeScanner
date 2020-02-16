using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace CamCodeScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Filter.ItemsSource = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenDevice((FilterInfo)Filter.SelectedItem);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CloseDevice();

            Properties.Settings.Default.Save();
        }

        VideoCaptureDevice m_Device;

        private void OpenDevice(FilterInfo filter)
        {
            if (m_Device != null)
            {
                CloseDevice();
            }

            m_Device = new VideoCaptureDevice(filter.MonikerString);
            m_Device.NewFrame += Device_NewFrame;
            m_Device.VideoSourceError += Device_VideoSourceError;
            m_Device.Start();
        }

        private void CloseDevice()
        {
            if (m_Device == null)
            {
                return;
            }

            m_Device.NewFrame -= Device_NewFrame;
            m_Device.VideoSourceError -= Device_VideoSourceError;
            m_Device.SignalToStop();
            m_Device = null;
        }

        private void Device_NewFrame(object sender, NewFrameEventArgs e)
        {
            Dispatcher.Invoke(() => ShowFrame(e.Frame));
        }

        private void Device_VideoSourceError(object sender, VideoSourceErrorEventArgs e)
        {
            Dispatcher.Invoke(() => ShowError(e.Description));
        }

        private void ShowFrame(Bitmap frame)
        {
            frame = (Bitmap)frame.Clone();
            if (Viewport.Image != null)
            {
                Viewport.Image.Dispose();
            }
            Viewport.Image = frame;
            ShowCode(frame);
        }

        CodeReader m_CodeReader = new CodeReader();
        long m_ReadTime = Stopwatch.GetTimestamp();

        private void ShowCode(Bitmap frame)
        {
            var code = m_CodeReader.Decode(frame);
            if (code != null && (double)(Stopwatch.GetTimestamp() - m_ReadTime) / Stopwatch.Frequency > 1)
            {
                MessageBox.Show(code, Title, MessageBoxButton.OK, MessageBoxImage.Information);
                m_ReadTime = Stopwatch.GetTimestamp();
            }
        }

        private void ShowError(string description)
        {
            MessageBox.Show(description, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
