using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGR_Mobile.Modèles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SGR_Mobile.Vues
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccueilAdmin : ContentPage
    {
        private HttpClient _httpClient;
        public AccueilAdmin()
        {
            InitializeComponent();
            lstPlat.RefreshCommand = new Command(() => {
                RefreshContacts();
                lstPlat.IsRefreshing = false;
            });
        }

        private const string ApiUrl = "https://apisgr.alwaysdata.net/controllers/plat/readAll.php";
        private List<Plat> allPlat;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshContacts();
        }

        private async void RefreshContacts()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Envoyer une requête GET à l'API pour récupérer les données de contact
                    var response = await client.GetAsync(ApiUrl);

                    // Si la requête a réussi, désérialiser les données JSON en objets Contact
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var PlatLists = JsonConvert.DeserializeObject<List<List<Plat>>>(content);

                        // Concaténer les listes de contacts en une seule liste
                        allPlat = PlatLists.SelectMany(cl => cl).ToList();

                        // Afficher les données de contact dans la ListView
                        lstPlat.ItemsSource = allPlat;

                        // Associer le gestionnaire d'événements au bouton "M" pour chaque élément de la liste

                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible de récupérer les contacts", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Impossible de récupérer les contacts : {ex.Message}", "OK");
                }
            }
        }
        private void ajoutPlat(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ajoutPlat());
        }
        private async void ModifierPlat_Clicked(object sender, EventArgs e)
        {
            // Obtenez le plat à modifier à partir de l'élément sélectionné dans la ListView
            Plat plat = (Plat)((Button)sender).BindingContext;

            // Passez les informations du plat à la page de modification
            await Navigation.PushAsync(new modifPlat(plat));
        }
        private async void SupprimerPlat_Clicked(Object sender, EventArgs e)
        {
            // Récupérer le plat sélectionné depuis le paramètre de la commande
            Plat plat = (Plat)((Button)sender).CommandParameter;

            bool confirmation = await DisplayAlert("Confirmation", "Êtes-vous sûr de vouloir supprimer ce plat ?", "Oui", "Non");

            if (confirmation)
            {
                // Créer un objet JSON avec l'ID du plat
                JObject requestBody = new JObject();
                requestBody["id_plat"] = plat.id_plat;

                // Convertir l'objet JSON en chaîne
                string json = requestBody.ToString();

                // Créer le contenu de la requête DELETE avec la chaîne JSON contenant l'ID du plat
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // Effectuer la requête DELETE à l'API pour supprimer le plat
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://apisgr.alwaysdata.net/controllers/plat/delete.php"; // Remplacez l'URL de l'API et l'identifiant du plat
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                    request.Content = content;

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        // Afficher une confirmation ou effectuer d'autres actions nécessaires
                        await DisplayAlert("Succès", "Le plat a été supprimé avec succès.", "OK");

                        //réactualisation de la liste des plats
                        RefreshContacts();
                    }
                    else
                    {
                        // Afficher un message d'erreur en cas d'échec de la suppression
                        await DisplayAlert("Erreur", "Une erreur s'est produite lors de la suppression du plat.", "OK");
                    }
                }
            }
        }

    }
}
