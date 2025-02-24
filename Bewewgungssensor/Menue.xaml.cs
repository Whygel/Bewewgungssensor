using Bewewgungssensor.Datenbank;

namespace Bewewgungssensor;

public partial class Menue : ContentPage
{
    private readonly DatenbankService _database = new();
    int Score;
    bool GameFinished = false;
    string Spielername = string.Empty;
    public Menue(int iScore, bool iGameFinished, string iSpielername)
	{
		InitializeComponent();
        Score = iScore;
        GameFinished = iGameFinished;
        Spielername = iSpielername;
	}

    private  async void Button_Clicked(object sender, EventArgs e)
    {
        Spielername = TbPlayerName.Text?.Trim();
        await Navigation.PushAsync(new Spielfeld(TbPlayerName.Text));
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {

        if (GameFinished)
        {
            TbPlayerName.Text = Spielername;
            
            if(Spielername != null)
                await _database.AddHighscoreAsync(Spielername, Score);        
        }
        var highscores = await _database.GetHighscoresAsync();
        HighscoreCollectionView.ItemsSource = highscores;
        HighscoreCollectionView.ItemsSource = highscores;

    }
}