using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;

namespace SnackManagerWPF
{
    // --- MODÈLES ---
    public class Plat
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public string Categorie { get; set; }
        public string? ImageNom { get; set; }
    }

    public class Commande
    {
        public int Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal MontantTotal { get; set; }
        public string DetailsArticles { get; set; }
    }

    public class LignePanier
    {
        public Plat LePlat { get; set; }
        public bool AvecFrites { get; set; }
        public bool AvecBoisson { get; set; }
        public int Quantite { get; set; }

        public string NomPlat
        {
            get
            {
                // On combine la catégorie et le nom (ex: "PIZZA Dinde")
                // .ToLower() et char.Upper permettent de rendre ça plus joli (ex: Pizza Dinde)
                string categorie = LePlat.Categorie;
                string nom = LePlat.Nom;

                string nomComplet = $"{categorie} {nom}";

                if (AvecFrites) nomComplet += " + 🍟";
                if (AvecBoisson) nomComplet += " + 🥤";
                return nomComplet;
            }
        }

        public decimal TotalLigne
        {
            get
            {
                decimal prixUnitaire = LePlat.Prix;
                if (AvecFrites) prixUnitaire += 5;
                if (AvecBoisson) prixUnitaire += 5;
                return prixUnitaire * Quantite;
            }
        }
    }

    // --- BASE DE DONNÉES ---
    public class AppDbContext : DbContext
    {
        public DbSet<Plat> Plats { get; set; }
        public DbSet<Commande> Commandes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=snack.db");
        }
    }

    // --- FENÊTRE PRINCIPALE ---
    public partial class MainWindow : Window
    {
        private ObservableCollection<LignePanier> panier = new ObservableCollection<LignePanier>();

        public MainWindow()
        {
            InitializeComponent();
            InitialiserBaseDeDonnees();
            ChargerMenu();
            if (GridPanier != null) GridPanier.ItemsSource = panier;
        }

        private void InitialiserBaseDeDonnees()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                if (!context.Plats.Any())
                {
                    var listePlats = new List<Plat>
        {
            // --- SANDWICH ---
            new Plat { Categorie = "SANDWICH", Nom = "Thon", Prix = 15, ImageNom = "Thon.jpg" },
            new Plat { Categorie = "SANDWICH", Nom = "Dinde", Prix = 20, ImageNom = "sandwich_dinde.png" },
            new Plat { Categorie = "SANDWICH", Nom = "Viande hachée", Prix = 25, ImageNom = "sandwich_viande.png" },
            new Plat { Categorie = "SANDWICH", Nom = "Nuggets", Prix = 28, ImageNom = "sandwich_nuggets.png" },
            new Plat { Categorie = "SANDWICH", Nom = "Cordon bleu", Prix = 29, ImageNom = "sandwich_cordonbleu.png" },
            new Plat { Categorie = "SANDWICH", Nom = "Fruit de mer", Prix = 45, ImageNom = "sandwich_mer.png" },
            new Plat { Categorie = "SANDWICH", Nom = "Spécial JM", Prix = 30, ImageNom = "sandwich_jm.png" },

            // --- TACOS ---
            new Plat { Categorie = "TACOS", Nom = "Dinde", Prix = 30, ImageNom = "tacos_dinde.png" },
            new Plat { Categorie = "TACOS", Nom = "Nuggets", Prix = 34, ImageNom = "tacos_nuggets.png" },
            new Plat { Categorie = "TACOS", Nom = "Viande hachée", Prix = 35, ImageNom = "tacos_viande.png" },
            new Plat { Categorie = "TACOS", Nom = "Cordon bleu", Prix = 35, ImageNom = "tacos_cordonbleu.png" },
            new Plat { Categorie = "TACOS", Nom = "Fruit de mer", Prix = 45, ImageNom = "tacos_mer.png" },
            new Plat { Categorie = "TACOS", Nom = "Spécial JM", Prix = 38, ImageNom = "tacos_jm.png" },

            // --- BURGERS ---
            new Plat { Categorie = "BURGERS", Nom = "Chesse burger", Prix = 25, ImageNom = "burger.jpg" },
            new Plat { Categorie = "BURGERS", Nom = "Big burger", Prix = 35, ImageNom = "burger.jpg" },
            new Plat { Categorie = "BURGERS", Nom = "Spécial JM", Prix = 40, ImageNom = "burger.jpg" },

            // --- PIZZA ---
            new Plat { Categorie = "PIZZA", Nom = "Margarita", Prix = 32, ImageNom = "pizza_4fromages.png" },
            new Plat { Categorie = "PIZZA", Nom = "Thon", Prix = 37, ImageNom = "pizza_thon.png" },
            new Plat { Categorie = "PIZZA", Nom = "Dinde", Prix = 40, ImageNom = "pizza_din.png" },
            new Plat { Categorie = "PIZZA", Nom = "Viande hachée", Prix = 42, ImageNom = "pizza_viande.png" },
            new Plat { Categorie = "PIZZA", Nom = "4 Saisons", Prix = 45, ImageNom = "pizza_4saisons.png" },
            new Plat { Categorie = "PIZZA", Nom = "Fruit de mer", Prix = 50, ImageNom = "pizza_mer.png" },

            // --- PANZAROTTI ---
            new Plat { Categorie = "PANZAROTTI", Nom = "Fromage", Prix = 32, ImageNom = "panzarotti.png" },
            new Plat { Categorie = "PANZAROTTI", Nom = "Dinde", Prix = 40, ImageNom = "panzarotti.png" },
            new Plat { Categorie = "PANZAROTTI", Nom = "Viande hachée", Prix = 42, ImageNom = "panzarotti.png" },
            new Plat { Categorie = "PANZAROTTI", Nom = "Fruit de mer", Prix = 50, ImageNom = "panzarotti.png" },
            new Plat { Categorie = "PANZAROTTI", Nom = "Spécial JM", Prix = 48, ImageNom = "panzarotti.png" },

            // --- WRAP ---
            new Plat { Categorie = "WRAP", Nom = "Dinde", Prix = 32, ImageNom = "wrap_dinde.png" },
            new Plat { Categorie = "WRAP", Nom = "Nuggets", Prix = 36, ImageNom = "wrap_nugg.png" },
            new Plat { Categorie = "WRAP", Nom = "Viande hachée", Prix = 37, ImageNom = "wrap_vind.png" },
            new Plat { Categorie = "WRAP", Nom = "Spécial JM", Prix = 45, ImageNom = "wrap_spec.png" },

            // --- JM GOLD ---
            new Plat { Categorie = "JM GOLD", Nom = "Dinde", Prix = 35, ImageNom = "sandwich_dinde.png" },
            new Plat { Categorie = "JM GOLD", Nom = "Nuggets", Prix = 38, ImageNom = "sandwich_nuggets.png" },
            new Plat { Categorie = "JM GOLD", Nom = "Viande hachée", Prix = 40, ImageNom = "sandwich_viande.png" },
            new Plat { Categorie = "JM GOLD", Nom = "Spécial JM", Prix = 45, ImageNom = "sandwich_jm.png" },

            // --- POUTINE ---
            new Plat { Categorie = "POUTINE", Nom = "Dinde", Prix = 30, ImageNom = "poutine.png" },
            new Plat { Categorie = "POUTINE", Nom = "Nuggets", Prix = 35, ImageNom = "poutine.png" },
            new Plat { Categorie = "POUTINE", Nom = "Cordon bleu", Prix = 35, ImageNom = "poutine.png" },
            new Plat { Categorie = "POUTINE", Nom = "Viande hachée", Prix = 37, ImageNom = "poutine.png" },
            new Plat { Categorie = "POUTINE", Nom = "Fruit de mer", Prix = 45, ImageNom = "poutine.png" },
            new Plat { Categorie = "POUTINE", Nom = "Spécial JM", Prix = 35, ImageNom = "poutine.png" },

            // --- SALADES ---
            new Plat { Categorie = "SALADES", Nom = "César", Prix = 30, ImageNom = "salade_cesar.png" },
            new Plat { Categorie = "SALADES", Nom = "Protéine", Prix = 35, ImageNom = "salade_pro.png" },
            new Plat { Categorie = "SALADES", Nom = "Prise mass", Prix = 38, ImageNom = "salade_pri.png" },
            new Plat { Categorie = "SALADES", Nom = "Spécial JM", Prix = 40, ImageNom = "salade_spec.png" },

            // --- JUS ---
            new Plat { Categorie = "JUS", Nom = "Mojito", Prix = 8, ImageNom = "jus_mojito.jpg" },
            new Plat { Categorie = "JUS", Nom = "Orange", Prix = 15, ImageNom = "jus_orange.jpg" },
            new Plat { Categorie = "JUS", Nom = "Citron", Prix = 15, ImageNom = "jus_citron.jpg" },
            new Plat { Categorie = "JUS", Nom = "Banane", Prix = 15, ImageNom = "jus_banane.jpg" },
            new Plat { Categorie = "JUS", Nom = "Avocat", Prix = 18, ImageNom = "jus_avocat.jpg" },
            new Plat { Categorie = "JUS", Nom = "Cocktail", Prix = 20, ImageNom = "jus_cocktail.jpg" },

            // --- DESSERT ---
            new Plat { Categorie = "DESSERT", Nom = "Flan", Prix = 15, ImageNom = "dessert_flan.jpg" },
            new Plat { Categorie = "DESSERT", Nom = "Tiramissu", Prix = 20, ImageNom = "dessert_tiramisu.jpg" },

            // --- SUPPLÉMENT ---
            new Plat { Categorie = "SUPPLÉMENT", Nom = "Frite", Prix = 5, ImageNom = "supp_frite.jpg" },
            new Plat { Categorie = "SUPPLÉMENT", Nom = "Double fromage", Prix = 2, ImageNom = "supp_fromage.jpg" },
            new Plat { Categorie = "SUPPLÉMENT", Nom = "Double sauce", Prix = 2, ImageNom = "supp_sauce.jpg" },
            new Plat { Categorie = "SUPPLÉMENT", Nom = "oeufs", Prix = 1, ImageNom = "supp_oeuf.jpg" }
        };
                    context.Plats.AddRange(listePlats);
                    context.SaveChanges();
                }
            }
        }

        private void ChargerMenu()
        {
            List<Plat> tousLesPlats;
            using (var context = new AppDbContext())
            {
                tousLesPlats = context.Plats.ToList();
            }

            var categories = tousLesPlats.Select(p => p.Categorie).Distinct().ToList();
            if (TabMenu != null) TabMenu.Items.Clear();

            foreach (var cat in categories)
            {
                TabItem onglet = new TabItem { Header = cat.ToUpper(), Tag = cat };

                ScrollViewer scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden, Padding = new Thickness(0, 15, 0, 0) };
                WrapPanel panelBoutons = new WrapPanel { Orientation = Orientation.Horizontal };
                var platsDeLaCat = tousLesPlats.Where(p => p.Categorie == cat).ToList();

                foreach (var plat in platsDeLaCat)
                {
                    // --- DESIGN DE LA CARTE PRODUIT ---
                    Border cardBorder = new Border
                    {
                        Background = Brushes.White,
                        CornerRadius = new CornerRadius(16),
                        Width = 185,
                        Height = 210,
                        Margin = new Thickness(0, 0, 15, 15),
                        Effect = new System.Windows.Media.Effects.DropShadowEffect { Color = Colors.Black, Direction = 270, BlurRadius = 10, ShadowDepth = 3, Opacity = 0.08 }
                    };

                    Grid cardGrid = new Grid();
                    cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.3, GridUnitType.Star) });
                    cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    Border imageContainer = new Border { CornerRadius = new CornerRadius(16, 16, 0, 0), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9FAFB")), ClipToBounds = true };
                    Image img = new Image { Stretch = Stretch.Uniform, Margin = new Thickness(8) };
                    try
                    {
                        if (!string.IsNullOrEmpty(plat.ImageNom))
                        {
                            string cheminImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", plat.ImageNom);
                            if (System.IO.File.Exists(cheminImage)) img.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(cheminImage));
                        }
                    }
                    catch { }
                    imageContainer.Child = img;
                    cardGrid.Children.Add(imageContainer);
                    Grid.SetRow(imageContainer, 0);

                    StackPanel infoContainer = new StackPanel { Margin = new Thickness(12, 8, 12, 8), VerticalAlignment = VerticalAlignment.Center };
                    TextBlock txtNom = new TextBlock { Text = plat.Nom, FontWeight = FontWeights.SemiBold, FontSize = 13, TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")) };
                    DockPanel prixPanel = new DockPanel { Margin = new Thickness(0, 6, 0, 0) };
                    TextBlock txtPrix = new TextBlock { Text = $"{plat.Prix} DH", FontWeight = FontWeights.Bold, FontSize = 15, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#059669")), VerticalAlignment = VerticalAlignment.Center };
                    Border btnPlus = new Border { Width = 24, Height = 24, CornerRadius = new CornerRadius(12), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6")), HorizontalAlignment = HorizontalAlignment.Right };
                    btnPlus.Child = new TextBlock { Text = "+", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.Gray, FontWeight = FontWeights.Bold };

                    DockPanel.SetDock(txtPrix, Dock.Left);
                    DockPanel.SetDock(btnPlus, Dock.Right);
                    prixPanel.Children.Add(txtPrix);
                    prixPanel.Children.Add(btnPlus);
                    infoContainer.Children.Add(txtNom);
                    infoContainer.Children.Add(prixPanel);
                    cardGrid.Children.Add(infoContainer);
                    Grid.SetRow(infoContainer, 1);
                    cardBorder.Child = cardGrid;

                    Button btn = new Button { Content = cardBorder, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Cursor = Cursors.Hand, Tag = plat };

                    // --------------------------------------------------------------------------------
                    // CLIC SUR UN PLAT (Avec gestion de la quantité)
                    // --------------------------------------------------------------------------------
                    btn.Click += (sender, e) =>
                    {
                        Button button = (Button)sender;
                        Plat platSelectionne = (Plat)button.Tag;

                        if (platSelectionne.Categorie != "JUS" && platSelectionne.Categorie != "DESSERT")
                        {
                            OptionsWindow fenetreOptions = new OptionsWindow(platSelectionne.Nom);
                            this.Opacity = 0.8;
                            bool? resultat = fenetreOptions.ShowDialog();
                            this.Opacity = 1;

                            if (resultat == true)
                            {
                                int qte = fenetreOptions.QuantiteSelectionnee; // On récupère la quantité
                                AjouterAuPanier(platSelectionne, fenetreOptions.AvecFrites, fenetreOptions.AvecBoisson, qte);
                            }
                        }
                        else
                        {
                            // Jus et Dessert = Quantité 1 par défaut
                            AjouterAuPanier(platSelectionne, false, false, 1);
                        }
                    };
                    panelBoutons.Children.Add(btn);
                }
                scroll.Content = panelBoutons;
                onglet.Content = scroll;
                TabMenu.Items.Add(onglet);
            }
            if (TabMenu.Items.Count > 0) TabMenu.SelectedIndex = 0;
        }

        // --------------------------------------------------------------------------------
        // AJOUTER AU PANIER (Modifié pour accepter la quantité)
        // --------------------------------------------------------------------------------
        private void AjouterAuPanier(Plat p, bool frites, bool boisson, int quantiteAjoutee)
        {
            var ligneExistante = panier.FirstOrDefault(l => l.LePlat.Id == p.Id && l.AvecFrites == frites && l.AvecBoisson == boisson);

            if (ligneExistante != null)
            {
                ligneExistante.Quantite += quantiteAjoutee; // On ajoute la quantité choisie
                GridPanier.Items.Refresh();
            }
            else
            {
                panier.Add(new LignePanier
                {
                    LePlat = p,
                    Quantite = quantiteAjoutee, // On définit la quantité initiale 
                    AvecFrites = frites,
                    AvecBoisson = boisson
                });
            }
            MettreAJourTotal();
        }

        private void BtnRetirer_Click(object sender, RoutedEventArgs e)
        {
            if (GridPanier.SelectedItem is LignePanier selectedItem)
            {
                if (selectedItem.Quantite > 1)
                {
                    selectedItem.Quantite--;
                    GridPanier.Items.Refresh();
                }
                else panier.Remove(selectedItem);
                MettreAJourTotal();
            }
            else MessageBox.Show("Veuillez sélectionner un article à retirer.");
        }

        private void MettreAJourTotal()
        {
            decimal total = panier.Sum(l => l.TotalLigne);
            if (TxtTotal != null) TxtTotal.Text = $"{total:0.00} DH";
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            if (panier.Count == 0) return;
            decimal total = panier.Sum(l => l.TotalLigne);
            StringBuilder details = new StringBuilder();
            foreach (var item in panier) details.AppendLine($"{item.Quantite}x {item.NomPlat} ({item.TotalLigne} DH)");

            var articlesPourTicket = panier.ToList();
            DateTime dateTicket = DateTime.Now;
            int idCommandeGenere = 0;

            using (var context = new AppDbContext())
            {
                Commande nouvelleCommande = new Commande { DateCommande = dateTicket, MontantTotal = total, DetailsArticles = details.ToString() };
                context.Commandes.Add(nouvelleCommande);
                context.SaveChanges();
                idCommandeGenere = nouvelleCommande.Id;
            }

            MessageBoxResult resultat = MessageBox.Show("Commande enregistrée ! Imprimer le ticket ?", "Succès", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultat == MessageBoxResult.Yes) ImprimerTicket(articlesPourTicket, total, idCommandeGenere, dateTicket);

            panier.Clear();
            MettreAJourTotal();
        }

        private void ImprimerTicket(List<LignePanier> articles, decimal total, int numeroCommande, DateTime date)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument();

                // --- CORRECTION DU CENTRAGE ---
                // 1. On enlève les marges par défaut qui poussent le texte à droite
                doc.PagePadding = new Thickness(0);

                // 2. On force la largeur du document à la largeur imprimable exacte
                doc.PageWidth = printDialog.PrintableAreaWidth;
                doc.ColumnWidth = printDialog.PrintableAreaWidth;

                // 3. Police adaptée aux tickets (Monospace aligne mieux les chiffres)
                doc.FontFamily = new FontFamily("Consolas");
                doc.FontSize = 11; // Un peu plus petit pour éviter que ça dépasse
                                   // ------------------------------

                // 1. En-tête (Nom du Snack)
                Paragraph header = new Paragraph(new Run("🍟 JM FOOD 🍔"));
                header.TextAlignment = TextAlignment.Center;
                header.FontSize = 16;
                header.FontWeight = FontWeights.Bold;
                header.Margin = new Thickness(0, 5, 0, 5); // Espacement haut/bas seulement
                doc.Blocks.Add(header);

                // 2. Infos de la commande
                Paragraph infos = new Paragraph();
                infos.Inlines.Add(new Run($"CDE N° : {numeroCommande}\n")); // "Commande" abrégé pour gagner de la place
                infos.Inlines.Add(new Run($"{date:dd/MM/yyyy HH:mm}\n"));
                infos.Inlines.Add(new Run("--------------------------------\n"));
                infos.TextAlignment = TextAlignment.Center;
                infos.Margin = new Thickness(0);
                doc.Blocks.Add(infos);

                // 3. Liste des articles
                Paragraph corps = new Paragraph();
                foreach (var item in articles)
                {
                    // On affiche le détail article par article
                    string ligne = $"{item.Quantite}x {item.NomPlat}";
                    string prix = $"{item.TotalLigne:0.00}";

                    // Astuce : Mettre le prix sur une nouvelle ligne si le nom est long
                    corps.Inlines.Add(new Run(ligne + "\n"));
                    corps.Inlines.Add(new Run(">> " + prix + " DH\n"));
                }
                corps.Inlines.Add(new Run("--------------------------------\n"));
                corps.TextAlignment = TextAlignment.Center;
                corps.Margin = new Thickness(0);
                doc.Blocks.Add(corps);

                // 4. Total
                Paragraph footer = new Paragraph();
                footer.Inlines.Add(new Run($"TOTAL : {total:0.00} DH"));
                footer.FontSize = 16;
                footer.FontWeight = FontWeights.Bold;
                footer.TextAlignment = TextAlignment.Center;
                footer.Margin = new Thickness(0, 10, 0, 10);
                doc.Blocks.Add(footer);

                // 5. Message de fin
                Paragraph fin = new Paragraph(new Run("Merci et à bientôt !"));
                fin.TextAlignment = TextAlignment.Center;
                fin.FontSize = 10;
                fin.Margin = new Thickness(0);
                doc.Blocks.Add(fin);

                // 6. Contact
                Paragraph contact = new Paragraph();
                contact.Inlines.Add(new Run("\n📞 07 12 78 45 41\n")); // J'ai formaté le numéro
                contact.Inlines.Add(new Run("Instagram : jm_food_48"));
                contact.TextAlignment = TextAlignment.Center;
                contact.FontSize = 10;
                contact.Margin = new Thickness(0, 0, 0, 10); // Marge en bas pour couper le papier proprement
                doc.Blocks.Add(contact);

                // 7. Impression
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Ticket Snack");
            }
        }

        // ============================================================
        // GESTION SÉCURISÉE DU BOUTON ADMIN (PIN 1234)
        // ============================================================
        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            const string CODE_SECRET = "221125";

            Window fenetreAuth = new Window
            {
                Title = "Sécurité Admin",
                Width = 300,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                Background = Brushes.WhiteSmoke
            };

            StackPanel stack = new StackPanel { Margin = new Thickness(20) };
            TextBlock label = new TextBlock { Text = "Veuillez entrer le code PIN :", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) };
            PasswordBox passwordBox = new PasswordBox { Padding = new Thickness(5), FontSize = 16, HorizontalContentAlignment = HorizontalAlignment.Center };

            passwordBox.Loaded += (s, arg) => passwordBox.Focus();

            Button btnValider = new Button
            {
                Content = "Valider",
                Height = 35,
                Margin = new Thickness(0, 15, 0, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                IsDefault = true
            };

            btnValider.Click += (s, args) =>
            {
                if (passwordBox.Password == CODE_SECRET)
                {
                    fenetreAuth.DialogResult = true;
                    fenetreAuth.Close();
                }
                else
                {
                    MessageBox.Show("Code PIN incorrect !", "Accès Refusé", MessageBoxButton.OK, MessageBoxImage.Warning);
                    passwordBox.Clear();
                    passwordBox.Focus();
                }
            };

            stack.Children.Add(label);
            stack.Children.Add(passwordBox);
            stack.Children.Add(btnValider);
            fenetreAuth.Content = stack;

            if (fenetreAuth.ShowDialog() == true)
            {
                // Ici on ouvre l'historique
                HistoriqueWindow fenetreHistorique = new HistoriqueWindow();
                fenetreHistorique.Show();
            }
        }
    }
}