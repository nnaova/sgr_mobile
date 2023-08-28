using System;
using System.Collections.Generic;
using System.Text;

namespace SGR_Mobile.Modèles
{
    public class Menu
    {
        // Id du plat
        public int id_menu { get; set; }

        // Nom du plat
        public string nom_menu { get; set; }

        // Type plat
        public double PU { get; set; }

        // Prix
        public string description { get; set; }

        // Id sous categorie
        public DateTime date_menu { get; set; }

    }
}