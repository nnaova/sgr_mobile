using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SGR_Mobile.Vues;

namespace SGR_Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Définit la page principale de l'application comme une instance de NavigationPage avec MainPage comme page racine
            MainPage = MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
