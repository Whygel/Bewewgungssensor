using Bewewgungssensor.Datenbank;

namespace Bewewgungssensor;

public partial class Menue : ContentPage
{
    private readonly DatenbankService _database = new();
    public Menue()
	{
		InitializeComponent();
	}

    private  async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Spielfeld(TbPlayerName.Text));
        //await Shell.Current.GoToAsync("//Spielfeld");
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        string spielername = "Max";  // Beispiel: Den Namen aus Eingabefeld holen
        int score = new Random().Next(1000);  // Beispielwert

        await _database.AddHighscoreAsync(spielername, score);

        var highscores = await _database.GetHighscoresAsync();
        HighscoreListView.ItemsSource = highscores;
    }
}