
namespace Bewewgungssensor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Menue(0,false));
        }

    }

}
