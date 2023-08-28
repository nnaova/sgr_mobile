using Newtonsoft.Json;
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
    public partial class modifPlat : ContentPage
    {
        private Plat plat;
        public modifPlat(Plat plat)
        {
            this.plat = plat;

            // Créez votre formulaire de modification avec les champs nécessaires
            Entry nomPlatEntry = new Entry { Placeholder = "Nom du plat", Text = plat.nom_plat };
            Picker typePlatPicker = new Picker
            {
                Title = "Type de plat",
                ItemsSource = new List<string> { "mise en bouche", "entrée", "plat", "dessert" },
                SelectedItem = plat.type_plat
            };
            Entry prixUnitaireEntry = new Entry { Placeholder = "Prix unitaire", Text = plat.PU_carte.ToString() };
            Button submitButton = new Button { Text = "Modifier" };
            submitButton.Clicked += async (s, args) =>
            {
                // Récupérer les nouvelles valeurs des champs
                string nouveauNomPlat = nomPlatEntry.Text;
                string nouveauTypePlat = typePlatPicker.SelectedItem.ToString();
                decimal nouveauPrixUnitaire;

                string prixText = prixUnitaireEntry.Text.Replace(',', '.'); // Remplacer la virgule par un point

                if (decimal.TryParse(prixText, NumberStyles.Float, CultureInfo.InvariantCulture, out nouveauPrixUnitaire))
                {
                    // Mettez à jour les propriétés du plat avec les nouvelles valeurs
                    plat.nom_plat = nouveauNomPlat;
                    plat.type_plat = nouveauTypePlat;
                    plat.PU_carte = nouveauPrixUnitaire;

                    // Effectuez la requête PUT vers votre API pour mettre à jour le plat
                    HttpClient client = new HttpClient();
                    string apiUrl = "https://apisgr.alwaysdata.net/controllers/plat/update.php";
                    string jsonData = JsonConvert.SerializeObject(plat);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Affichez une confirmation ou effectuez d'autres actions nécessaires
                        await DisplayAlert("Succès", "Le plat a été modifié avec succès.", "OK");

                        // Retournez à la page précédente après la modification
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // Affichez un message d'erreur en cas d'échec de la mise à jour
                        await DisplayAlert("Erreur", "La modification du plat a échoué.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Erreur", "Le prix doit être un nombre valide.", "OK");
                }

                // Retournez à la page précédente après la modification
                await Navigation.PopAsync();
            };

            StackLayout formLayout = new StackLayout
            {
                Children = { nomPlatEntry, typePlatPicker, prixUnitaireEntry, submitButton }
            };

            Content = formLayout;
        }

    }
}