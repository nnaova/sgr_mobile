using System;
using System.Collections.Generic;
using System.Text;

namespace SGR_Mobile.Modèles
{
    public class Plat
    {
        // Id du plat
        public int id_plat { get; set; }

        // Nom du plat
        public string nom_plat { get; set; }

        // Type plat
        public string type_plat { get; set; }

        // Prix
        public decimal PU_carte { get; set; }

        // Id sous categorie
        public int id_sous_cat { get; set; }

    }
}
