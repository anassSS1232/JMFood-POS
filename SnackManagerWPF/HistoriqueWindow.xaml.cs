using System;
using System.Linq;
using System.Collections.Generic; // Nécessaire pour List<>
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace SnackManagerWPF
{
    public partial class HistoriqueWindow : Window
    {
        public HistoriqueWindow()
        {
            InitializeComponent();

            // Par défaut, on sélectionne aujourd'hui
            SelecteurDate.SelectedDate = DateTime.Today;
        }

        // Se déclenche quand on change la date
        private void SelecteurDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelecteurDate.SelectedDate.HasValue)
            {
                ChargerDonnees(SelecteurDate.SelectedDate.Value);
            }
        }

        // Bouton "Voir Tout"
        private void BtnToutAfficher_Click(object sender, RoutedEventArgs e)
        {
            // On désélectionne la date visuellement
            SelecteurDate.SelectedDate = null;

            using (var context = new AppDbContext())
            {
                // On récupère TOUT sans filtre
                var toutesLesCommandes = context.Commandes
                                                .OrderByDescending(c => c.DateCommande)
                                                .ToList();

                MettreAJourInterface(toutesLesCommandes);
            }
        }

        // Fonction principale de chargement avec correction du filtre date
        private void ChargerDonnees(DateTime dateChoisie)
        {
            using (var context = new AppDbContext())
            {
                // CORRECTION ICI :
                // On définit le début (00h00) et la fin (00h00 le lendemain)
                // pour attraper toutes les commandes de la journée.
                DateTime debutJournee = dateChoisie.Date;
                DateTime finJournee = debutJournee.AddDays(1);

                var listeDuJour = context.Commandes
                                         .Where(c => c.DateCommande >= debutJournee && c.DateCommande < finJournee)
                                         .OrderByDescending(c => c.DateCommande)
                                         .ToList();

                MettreAJourInterface(listeDuJour);
            }
        }

        // Met à jour le tableau et les cartes
        private void MettreAJourInterface(List<Commande> liste)
        {
            // 1. Remplir le tableau
            GrilleHistorique.ItemsSource = liste;

            // 2. Mise à jour des cartes statistiques
            if (TxtNombre != null)
                TxtNombre.Text = liste.Count.ToString();

            if (TxtTotalJour != null)
            {
                decimal total = liste.Sum(c => c.MontantTotal);
                TxtTotalJour.Text = $"{total:N2} DH";
            }
        }
    }
}