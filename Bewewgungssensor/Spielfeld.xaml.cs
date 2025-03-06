namespace Bewewgungssensor;
using Microsoft.Maui.Dispatching;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Devices.Sensors;
using System.Text.RegularExpressions;
using Microsoft.Maui.Layouts;

public partial class Spielfeld : ContentPage
{
	IDispatcherTimer _timer;
    int ClockNumber = 0;
	int CountDown = 4;
	bool GameHasStart = false;
	bool GameOver = false;
    string Spielername;
	private const double Speed = 10;
    int Score  = 0;
    public Spielfeld(string iSpielername)
	{
		InitializeComponent();
		_timer = Dispatcher.CreateTimer();
        Spielername = iSpielername;
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
        {
            _timer.Stop();
            GameOver = true;
            _timer.Tick -= _timer_Tick;
        }
			
	}
    List<Border> ListOfObstacle = new List<Border>();
    bool SavedInitialPosition = false;

    double SpielfeldWidth;
    double SpielfeldHeight;

    private async void GameLoop()
    {
        SpielfeldWidth = SpielfeldLayout.Width;
        SpielfeldHeight = SpielfeldLayout.Height;

        MainThread.BeginInvokeOnMainThread(() =>
        {
           // LbSoielfeld.Text = SpielfeldLayout.Height.ToString() + "|" + SpielfeldLayout.Width.ToString();
            SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(SpielfeldWidth / 2, SpielfeldLayout.Height/2, Spieler.Width, Spieler.Height));
        });

        int CountObstacle = 3;
        ToggleAccelerometer();

        for (int i = 0; i< CountObstacle; i++)
        {
            {
                CreateObstacle();
            }
        }
        await Task.Run(async () =>
        {
            while (!GameOver)
            {
                Task.Delay(10).Wait();
                GameHasStart = true;
                CalculateMovement();
                HandleCollision();
            }
           
        });
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PushAsync(new Menue(Score, true, Spielername));
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
            HeightRequest = 50,
        };
        ListOfObstacle.Add(newObstacle);

        obstacleNewPosition(newObstacle);         
       
    }

    double newPosX;
    double newPosY;
    private void CalculateMovement()
    {

        if (AccelerationZ < 0)
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
        if ((newPosX + Spieler.Width > SpielfeldWidth || newPosX < 0) && (newPosY + Spieler.Height > SpielfeldHeight || newPosY < 0))
            return;

        if (newPosX + Spieler.Width > SpielfeldWidth || newPosX < 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(Spieler.X, newPosY, Spieler.Width, Spieler.Height));
            });
            return;
        }

        if ((int)newPosY + Spieler.Height > (int)SpielfeldHeight || newPosY < 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(newPosX, Spieler.Y, Spieler.Width, Spieler.Height));
            });
            return;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(newPosX, newPosY, Spieler.Width, Spieler.Height));
        });
    }
    void HandleCollision()
    {
        if (ListOfObstacle != null)
        {

            foreach (var obstacle in ListOfObstacle)
            {
                if (CheckCollision(obstacle))
                {
                    Score += 100;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (Score >= 1000)
                            TbScore.Text = $"{Score}";
                        else
                            TbScore.Text = $"0{Score}";
                        SpielfeldLayout.Remove(obstacle);
                        obstacleNewPosition(obstacle);
                    });                                   
                }
            }
        }
    }

    private void obstacleNewPosition(Border iBorder)
    {
        if (iBorder.Parent != null)
        {
            ((AbsoluteLayout)iBorder.Parent).Children.Remove(iBorder);
        }
        x = RandomX.Next(0, (int)SpielfeldWidth - (int)iBorder.Width - 10);
        y = RandomY.Next(0, (int)SpielfeldHeight - (int)iBorder.Height - 10);



        MainThread.BeginInvokeOnMainThread(() =>
        {
            AbsoluteLayout.SetLayoutBounds(iBorder, new Rect(x, y, iBorder.Width, iBorder.Height));
            AbsoluteLayout.SetLayoutFlags(iBorder, AbsoluteLayoutFlags.None);
            SpielfeldLayout.Add(iBorder);
        });
    }

    private bool CheckCollision(Border obstacle)
    {
        var playerBounds = AbsoluteLayout.GetLayoutBounds(Spieler);
        var obstacleBounds = AbsoluteLayout.GetLayoutBounds(obstacle);
        if(Spieler.X >= obstacle.X & Spieler.X + Spieler.Width <= obstacle.X+ obstacle.Width& 
            Spieler.Y >= obstacle.Y & Spieler.Y + Spieler.Height <= obstacle.Y + obstacle.Height)
        {
            return  true;
        }
        else
        {
            return false;
        }
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
                // Turn on Magnetometer
                Magnetometer.Default.ReadingChanged += MagnetometerChanged;
                Magnetometer.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off Magnetometer
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
            //InitialNeigungX = e.Reading.Acceleration.X;
            //InitialNeigungY = e.Reading.Acceleration.Y; 
            SavedInitialPosition = true;
            return;
        }
       // Koordinaten = e.Reading.ToString();
        //NeigungX = e.Reading.Acceleration.X;
        //NeigungY = e.Reading.Acceleration.Y;
        AccelerationZ = e.Reading.Acceleration.Z;
    }
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += _timer_Tick;
        _timer.Start();
        // SpielfeldLayout.SetLayoutBounds(Spieler, new Rect(0.5, 0.5, Width, Height));
        SavedInitialPosition = false;
    }
    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        HanddleCloseing();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        HanddleCloseing();
    }

    void HanddleCloseing()
    {
        Accelerometer.Default.Stop();
        Accelerometer.Default.ReadingChanged -= Default_ReadingChanged;

        Magnetometer.Default.Stop();
        Magnetometer.Default.ReadingChanged -= MagnetometerChanged;
       
    }
}