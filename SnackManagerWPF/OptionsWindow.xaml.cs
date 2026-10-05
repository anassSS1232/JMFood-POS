using System.Windows;

namespace SnackManagerWPF
{
    public partial class OptionsWindow : Window
    {
        public bool AvecFrites { get; private set; }
        public bool AvecBoisson { get; private set; }

        // NOUVEAU : Propriété pour stocker la quantité
        public int QuantiteSelectionnee { get; private set; } = 1;

        public OptionsWindow(string nomPlat)
        {
            InitializeComponent();
            TxtTitrePlat.Text = nomPlat;
            MettreAJourAffichage();
        }

        // Action bouton MOINS
        private void BtnMoins_Click(object sender, RoutedEventArgs e)
        {
            if (QuantiteSelectionnee > 1)
            {
                QuantiteSelectionnee--;
                MettreAJourAffichage();
            }
        }

        // Action bouton PLUS
        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            QuantiteSelectionnee++;
            MettreAJourAffichage();
        }

        private void MettreAJourAffichage()
        {
            if (TxtQuantite != null)
                TxtQuantite.Text = QuantiteSelectionnee.ToString();
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            AvecFrites = ChkFrites.IsChecked == true;
            AvecBoisson = ChkBoisson.IsChecked == true;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}