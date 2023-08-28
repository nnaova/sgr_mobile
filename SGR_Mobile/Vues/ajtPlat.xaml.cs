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
    public partial class ajoutPlat : ContentPage
    {
        public ajoutPlat()
        {
            InitializeComponent();

            typePlatPicker.ItemsSource = new List<string>
            {
                "mise en bouche",
                "entrée",
                "plat",
                "dessert"
            };
        }

        private async void validerAddPlat(object sender, EventArgs e)
        {
            string nom_plat = nomPlatEntry.Text;
            string type_plat = typePlatPicker.SelectedItem as string;
            double PU_carte;

            string prixText = prixUnitaire.Text.Replace(',', '.'); // Remplacer la virgule par un point

            if (double.TryParse(prixText, NumberStyles.Float, CultureInfo.InvariantCulture, out PU_carte))
            {
                string id_sous_cat = "1";

                var requestData = new
                {
                    nom_plat,
                    type_plat,
                    PU_carte,
                    id_sous_cat
                };

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string apiUrl = "https://apisgr.alwaysdata.net/controllers/plat/create.php";

                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            await DisplayAlert("succès", "les données ont été envoyées avec succès", "OK");
                        }
                        else
                        {
                            await DisplayAlert("erreur", "Une erreur s'est produite lors de l'envoi des données.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("erreur", $"une exception s'est produite : {ex.Message}", "OK");
                    }
                }
            }
            else if (int.TryParse(prixUnitaire.Text, out int PU_carteEntier))
            {
                PU_carte = PU_carteEntier;

                string id_sous_cat = "1";

                var requestData = new
                {
                    nom_plat,
                    type_plat,
                    PU_carte,
                    id_sous_cat
                };

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string apiUrl = "https://apisgr.alwaysdata.net/controllers/plat/create.php";

                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            await DisplayAlert("succès", "les données ont été envoyées avec succès", "OK");
                        }
                        else
                        {
                            await DisplayAlert("erreur", "Une erreur s'est produite lors de l'envoi des données.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("erreur", $"une exception s'est produite : {ex.Message}", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("erreur", "Le prix doit être un nombre valide.", "OK");
            }
        }
    }
}
