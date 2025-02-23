namespace Bewewgungssensor;
using Microsoft.Maui.Dispatching;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Devices.Sensors;
using System.Text.RegularExpressions;
using Microsoft.Maui.Layouts;
using GameplayKit;

public partial class Spielfeld : ContentPage
{
	IDispatcherTimer _timer;
    int ClockNumber = 0;
	int CountDown = 3;
	bool GameHasStart = false;
	bool GameOver = false;

	private const double Speed = 10;
    int Score  = 0;
    public Spielfeld(string Spielername)
	{
		InitializeComponent();
		_timer = Dispatcher.CreateTimer();
        
    }
	private void _timer_Tick(object? sender, EventArgs e)
	{
		if (CountDown != 0)
		{
			CountDown--;
			LbTimer.Text = CountDown.ToString();
		}
		else
		{
            if (!GameHasStart)
            {
                GameLoop();
            }

        }

        if (GameHasStart)
        {
            LbTimer.Text = String.Empty;
            ClockNumber += 1;
            LbTimer.Text = ClockNumber.ToString();
        }


        if (ClockNumber == 60)
			GameOver = true;
	}

    double SpielerX;
    double SpielerY;
    List<Border> ListOfObstacle = new List<Border>();

    bool SavedInitialPosition = false;

    private async void GameLoop()
    {

        double SpielerX = SpielfeldLayout.Width;
        double SpielerY = SpielfeldLayout.Height;
    
        int CountObstacle = 3;
        ToggleAccelerometer();

        for (int i = 0; i <= CountObstacle; i++)
        {
            {
                CreateObstacle();
            }
        }
        await Task.Run(async () =>
        {
            while (!GameOver)
            {
                GameHasStart = true;
                await Task.Delay(500); // Verhindert das Blockieren des Threads
                CalculateMovement();
            }
        });
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PushAsync(new Menue());
        });
    }

    Random RandomX = new Random();
    Random RandomY = new Random();

    int x;
    int y;


    private void CreateObstacle()
    {
        Border newObstacle = new Border ()
        {
            BackgroundColor = Colors.Red,
            WidthRequest = 50,
            HeightRequest = 50
        };
        ListOfObstacle.Add(newObstacle);

        foreach (Border obstacle in ListOfObstacle)
        {
            if (obstacle.Parent != null)
            {
                ((AbsoluteLayout)obstacle.Parent).Children.Remove(obstacle);
            }
            x = RandomX.Next(0, (int)SpielfeldLayout.Height);
            y = RandomY.Next(0, (int)SpielfeldLayout.Width);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                AbsoluteLayout.SetLayoutBounds(obstacle, new Rect(x, y, obstacle.Width, obstacle.Height));
                AbsoluteLayout.SetLayoutFlags(obstacle, AbsoluteLayoutFlags.None);
                SpielfeldLayout.Add(obstacle);
            });           
        }
    }

    double newPosX;
    double newPosY;
    private void CalculateMovement()
    {
        if(AccelerationZ < 0)
        {
            newPosY = Spieler.Y - Speed;
        }
        if (AccelerationZ > 0)
        {
            newPosY = Spieler.Y + Speed;
        }

        if(MagnetometerX > 0)
        {
            newPosX = Spieler.X +Speed;
        }
        if(MagnetometerX < 0)
        {
            newPosX = Spieler.X - Speed;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(newPosX ,newPosY, Spieler.Width, Spieler.Height));
        });


       foreach(var obstacle in ListOfObstacle)
        {
            if (CheckCollision(obstacle))
            {
                Score += 100;
                TbScore.Text = $"Score: {score}";
                return;
            }
        }

        // Kolliosinhandlung 


    }

    private bool CheckCollision(Border obstacle)
    {
        var playerBounds = AbsoluteLayout.GetLayoutBounds(Spieler);
        var obstacleBounds = AbsoluteLayout.GetLayoutBounds(obstacle);

        return (Math.Abs(playerBounds.X - obstacleBounds.X) < 0.1) && (Math.Abs(playerBounds.Y - obstacleBounds.Y) < 0.1);
    }

    public void ToggleAccelerometer()
    {
        if (Accelerometer.Default.IsSupported)
        {
            if (!Accelerometer.Default.IsMonitoring)
            {
                // Turn on accelerometer
                Accelerometer.Default.ReadingChanged += Default_ReadingChanged; ;
                Accelerometer.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off accelerometer
                Accelerometer.Default.Stop();
                Accelerometer.Default.ReadingChanged -= Default_ReadingChanged;
            }
        }

        if(Magnetometer.Default.IsSupported)
        {
            if (!Magnetometer.Default.IsMonitoring)
            {
                // Turn on accelerometer
                Magnetometer.Default.ReadingChanged += MagnetometerChanged;
                Magnetometer.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off accelerometer
                Magnetometer.Default.Stop();
                Magnetometer.Default.ReadingChanged -= MagnetometerChanged;
            }
        }
    }

    private void MagnetometerChanged(object? sender, MagnetometerChangedEventArgs e)
    {
        MagnetometerX = e.Reading.MagneticField.X;
    }

    float AccelerationZ;
    float MagnetometerX;
    private  void Default_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        if(!SavedInitialPosition)
        {
            InitialNeigungX = e.Reading.Acceleration.X;
            InitialNeigungY = e.Reading.Acceleration.Y; 
            SavedInitialPosition = true;

           
            return;
        }
       // Koordinaten = e.Reading.ToString();
        NeigungX = e.Reading.Acceleration.X;
        NeigungY = e.Reading.Acceleration.Y;
        AccelerationZ = e.Reading.Acceleration.Z;

      

    }
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += _timer_Tick;
        _timer.Start();
        SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(0.5, 0.5, Width, Height));
        SavedInitialPosition = false;

    }
    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        Accelerometer.Default.Stop();
        Accelerometer.Default.ReadingChanged -= Default_ReadingChanged;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PushAsync(new Menue());
        });
    }
}