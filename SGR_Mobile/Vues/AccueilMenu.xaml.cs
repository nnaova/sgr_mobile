using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGR_Mobile.Modèles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SGR_Mobile.Vues
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccueilMenu : ContentPage
    {
        private HttpClient _httpClient;
        public AccueilMenu()
        {
            InitializeComponent();
            lstMenu.RefreshCommand = new Command(() =>
            {
                RefreshContacts();
                lstMenu.IsRefreshing = false;
            });
        }
        private const string ApiUrl = "https://apisgr.alwaysdata.net/controllers/menu/readAll.php";
        private List<Modèles.Menu> allMenu;

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
                        var MenuLists = JsonConvert.DeserializeObject<List<List<Modèles.Menu>>>(content);

                        // Concaténer les listes de contacts en une seule liste
                        allMenu = MenuLists.SelectMany(cl => cl).ToList();

                        // Afficher les données de contact dans la ListView
                        lstMenu.ItemsSource = allMenu;


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
        private async void OnMenuSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Modèles.Menu selectedMenu = e.SelectedItem as Modèles.Menu;

                // Vérifie si la page DetailContact correspondante est déjà affichée
                if (Navigation.NavigationStack.LastOrDefault()?.BindingContext is DetailMenu detailPage &&
                    detailPage.BindingContext is Modèles.Menu detailContact &&
                    detailContact.id_menu == selectedMenu.id_menu)
                {
                    // Ne fait rien si la page est déjà affichée
                    return;
                }

                await Navigation.PushAsync(new DetailMenu(selectedMenu));

                lstMenu.SelectedItem = null;
            }
        }
        private async void OnCreateMenuClicked(object sender, EventArgs e)
        {
            // Ouvrir une fenêtre avec un formulaire pour créer un menu
            var nomMenu = await DisplayPromptAsync("Créer un menu", "Entrez le nom du menu");
            var prixMenu = await DisplayPromptAsync("Créer un menu", "Entrez le prix du menu");
            var descriptionMenu = await DisplayPromptAsync("Créer un menu", "Entrez la description du menu");

            // Créer un nouvel objet Menu avec les données saisies
            var newMenu = new Modèles.Menu
            {
                nom_menu = nomMenu,
                PU = Convert.ToDouble(prixMenu.Replace(',', '.'), CultureInfo.InvariantCulture),
                description = descriptionMenu,
                date_menu = DateTime.Now // Utilisez la date appropriée
            };

            // Envoyer la requête POST à l'API pour créer le menu
            using (var client = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(newMenu);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://apisgr.alwaysdata.net/controllers/menu/create.php", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Actualiser la liste des menus
                        RefreshContacts();
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible de créer le menu", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Impossible de créer le menu : {ex.Message}", "OK");
                }
            }

        }




        private async void OnDeleteMenusClicked(object sender, EventArgs e)
        {
            // Récupérer le plat sélectionné depuis le paramètre de la commande
            Modèles.Menu menu = (Modèles.Menu)((Button)sender).CommandParameter;

            bool confirmation = await DisplayAlert("Confirmation", "Êtes-vous sûr de vouloir supprimer ce plat ?", "Oui", "Non");

            if (confirmation)
            {
                // Créer un objet JSON avec l'ID du plat
                JObject requestBody = new JObject();
                requestBody["id_menu"] = menu.id_menu;

                // Convertir l'objet JSON en chaîne
                string json = requestBody.ToString();

                // Créer le contenu de la requête DELETE avec la chaîne JSON contenant l'ID du plat
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // Effectuer la requête DELETE à l'API pour supprimer le plat
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://apisgr.alwaysdata.net/controllers/menu/delete.php"; // Remplacez l'URL de l'API et l'identifiant du plat
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