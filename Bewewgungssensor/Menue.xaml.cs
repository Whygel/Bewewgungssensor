using Bewewgungssensor.Datenbank;

namespace Bewewgungssensor;

public partial class Menue : ContentPage
{
    private readonly DatenbankService _database = new();
    int Score;
    bool GameFinished = false;
    string Spielername = string.Empty;
    public Menue(int iScore, bool iGameFinished)
	{
		InitializeComponent();
        Score = iScore;
        GameFinished = iGameFinished;
	}

    private  async void Button_Clicked(object sender, EventArgs e)
    {
        Spielername = TbPlayerName.Text;
        await Navigation.PushAsync(new Spielfeld(TbPlayerName.Text));
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (GameFinished)
        {
            TbPlayerName.Text = Spielername;
            await _database.AddHighscoreAsync(Spielername, Score);        
        }
        var highscores = await _database.GetHighscoresAsync();
        HighscoreListView.ItemsSource = highscores;
        HighscoreListView.ItemsSource = highscores;

    }
}