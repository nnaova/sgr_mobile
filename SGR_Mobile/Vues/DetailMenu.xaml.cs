using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SGR_Mobile.Modèles;
using Newtonsoft.Json.Linq;

namespace SGR_Mobile.Vues
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailMenu : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();
        private Modèles.Menu menu;
        public DetailMenu(Modèles.Menu menu)
        {
            InitializeComponent();
            this.menu = menu;
        }
        protected override void OnAppearing()
        {
            int id = menu.id_menu;
            string idAsString = id.ToString();
            double PU = menu.PU;
            string PUAsString = PU.ToString();
            base.OnAppearing();
            RefreshContacts();
            lblnom_menu.Text = menu.nom_menu;
            lblPU.Text = "Prix : " + PUAsString + "€";
            lbldescription.Text = menu.description;
        }

        private const string ApiUrl = "https://apisgr.alwaysdata.net/controllers/menu_contient_plat/readAll.php";
        private const string DeleteApiUrl = "https://apisgr.alwaysdata.net/controllers/menu_contient_plat/delete.php";
        private List<MCP> allMCP;


        private async void RefreshContacts()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Envoyer une requête GET à l'API pour récupérer les données de contact
                    var response = await client.GetAsync(ApiUrl + "?id_menu=" + menu.id_menu);

                    // Si la requête a réussi, désérialiser les données JSON en objets Contact
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var MCPLists = JsonConvert.DeserializeObject<List<MCP>>(content);

                        // Afficher les données de contact dans la ListView
                        allMCP = MCPLists;
                        lstMCP.ItemsSource = allMCP;
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible de récupérer les plats", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Impossible de récupérer les plats : {ex.Message}", "OK");
                }
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var mcp = button.BindingContext as MCP;

            // Confirmation de la suppression
            var result = await DisplayAlert("Confirmation", "Voulez-vous vraiment supprimer ce plat ?", "Oui", "Non");
            if (result)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var data = new { id_menu = menu.id_menu, id_plat = mcp.id_plat };
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(DeleteApiUrl),
                            Content = content
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            // Suppression réussie, actualisation de la liste
                            allMCP.Remove(mcp);
                            lstMCP.ItemsSource = null;
                            lstMCP.ItemsSource = allMCP;
                        }
                        else
                        {
                            await DisplayAlert("Erreur", "La suppression a échoué", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Erreur", $"La suppression a échoué : {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://apisgr.alwaysdata.net/controllers/plat/readAll.php");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonArray = JArray.Parse(jsonString);

                var plats = jsonArray[0].ToObject<List<Plat>>();

                var platNames = plats.Select(p => p.nom_plat).ToArray();
                var selectedPlat = await DisplayActionSheet("Sélectionner un plat", "Annuler", null, platNames);

                if (!string.IsNullOrEmpty(selectedPlat) && selectedPlat != "Annuler")
                {
                    var platId = plats.First(p => p.nom_plat == selectedPlat).id_plat;

                    var formData = new Dictionary<string, string>
            {
                { "id_menu", menu.id_menu.ToString() },
                { "id_plat", platId.ToString() }
            };
                    var content = new FormUrlEncodedContent(formData);
                    var createResponse = await client.PostAsync("https://apisgr.alwaysdata.net/controllers/menu_contient_plat/create.php", content);

                    if (createResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Succès", "Le plat a été ajouté au menu", "OK");
                        RefreshContacts(); // Mettre à jour l'affichage des plats après l'ajout
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible d'ajouter le plat au menu", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de récupérer les plats disponibles", "OK");
            }
        }

    }
}