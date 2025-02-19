namespace Bewewgungssensor;

public partial class Menue : ContentPage
{
	public Menue()
	{
		InitializeComponent();
	}

    private  async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Spielfeld(TbPlayerName.Text));
        //await Shell.Current.GoToAsync("//Spielfeld");
    }
}