using Newtonsoft.Json;
using SGR_Mobile.Modèles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SGR_Mobile.Vues
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Vérifie si les champs login et mdp sont vides ou nuls
            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtMdp.Text))
            {
                // Affiche une alerte avec le message "Veuillez remplir tous les champs" si l'un ou les deux champs sont vides
                await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
                return;
            }
            // Créer une instance de la classe User avec les valeurs entrées dans les champs login et mdp
            utilisateur uti = new utilisateur
            {
                login = txtUser.Text,
                mdp = txtMdp.Text,
            };
            // Définir l'adresse de la requête
            Uri RequestUri = new Uri("https://apisgr.alwaysdata.net/controllers/user/connexion.php");

            // Créer une nouvelle instance de HttpClient pour envoyer la requête
            var client = new HttpClient();

            // Convertir l'objet User en format JSON
            var json = JsonConvert.SerializeObject(uti);

            // Créer un contenu Http en utilisant l'objet JSON et le type de contenu "application/json"
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

            // Envoyer la requête POST et attendre une réponse
            var response = await client.PostAsync(RequestUri, contentJson);

            // Si la réponse a le code de statut "OK" (200)
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                if (uti.login == "admin")
                {

                    await Navigation.PushAsync(new NavAdmin());

                }
                else
                {
                    await Navigation.PushAsync(new Accueil());
                }

            }
            else
            {
                // Afficher une alerte indiquant que la connexion a échoué en raison d'un login ou mdp incorrect
                await DisplayAlert("Connexion", "Nom d'utilisateur ou mot de passe incorrect", "OK");

                // Effacer le contenu du champ mdp
                txtMdp.Text = string.Empty; // Ajout de cette ligne pour vider le champ txtMdp

            }


        }
    }
}
