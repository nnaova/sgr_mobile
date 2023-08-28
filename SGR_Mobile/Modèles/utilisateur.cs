using System;
using System.Collections.Generic;
using System.Text;

namespace SGR_Mobile.Modèles
{
    public class utilisateur
    {
        // Identifiant de l'utilisateur
        public int id_user { get; set; }

        // Nom de l'utilisateur
        public string login { get; set; }

        // role de l'utilisateur
        public string role { get; set; }

        // mdp de l'utilisateur
        public string mdp { get; set; }

    }
}
