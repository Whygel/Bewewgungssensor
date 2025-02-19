using Microsoft.Maui.Devices.Sensors;

namespace Bewewgungssensor
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            ToggleAccelerometer();
        }


        public void ToggleAccelerometer()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // Turn on accelerometer
                    Accelerometer.Default.ReadingChanged += Default_ReadingChanged; ;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off accelerometer
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Default_ReadingChanged;
                }
            }
        }

        private void Default_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            Label1.Text = "Geht";
            AccelLabel.TextColor = Colors.Green;
            AccelLabel.Text = $"Accel: {e.Reading}";
        }
    }
}
