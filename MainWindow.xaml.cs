using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NAANAA_YOUNES_CALCULATORS
{
    public partial class MainWindow : Window
    {
        // Variables de la classe (champs) : elles gardent leur valeur entre deux clics
        double premierNombre = 0;     // le nombre avant l'opérateur
        string operateur = "";        // l'opérateur choisi (+, −, ×, ÷)
        bool nouveauNombre = true;    // true = le prochain chiffre commence un nouveau nombre

        // Constructeur : appelé automatiquement à la création de la fenêtre
        public MainWindow()
        {
            InitializeComponent();
        }

        // Méthode appelée par TOUS les boutons (Click="Button_Click")
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // On récupère le bouton cliqué et le texte écrit dessus
            Button bouton = (Button)sender;
            string touche = bouton.Content.ToString();

            // Si "Erreur" est affiché, on repart de zéro avant de traiter la touche
            if (DisplayText.Text == "Erreur")
            {
                Effacer();
            }

            if (touche == "C")
            {
                Effacer();
            }
            else if (touche == "=")
            {
                ClicEgal();
            }
            else if (touche == "+" || touche == "−" || touche == "×" || touche == "÷")
            {
                ClicOperateur(touche);
            }
            else if (touche == "sin" || touche == "cos" || touche == "tan")   // AJOUT
            {
                ClicFonction(touche);
            }
            else
            {
                ClicChiffre(touche);   // 0 à 9 et "."
            }
        }

        // Touche chiffre ou point
        void ClicChiffre(string touche)
        {
            if (nouveauNombre || DisplayText.Text == "0")
            {
                if (touche == ".")
                {
                    DisplayText.Text = "0.";
                }
                else
                {
                    DisplayText.Text = touche;
                }
                nouveauNombre = false;
            }
            else
            {
                // Un seul point autorisé par nombre
                if (touche == "." && DisplayText.Text.Contains("."))
                {
                    return;
                }
                DisplayText.Text = DisplayText.Text + touche;
            }
        }

        // Touche opérateur
        void ClicOperateur(string touche)
        {
            // Conversion texte -> nombre (cf. diapo « Chaîne -> Nombre »)
            double nombreAffiche = double.Parse(DisplayText.Text, CultureInfo.InvariantCulture);

            // Si un calcul est déjà en attente (ex : 2 + 3 +), on le termine d'abord
            if (operateur != "" && nouveauNombre == false)
            {
                // Division par 0 : on affiche une erreur et on s'arrête
                if (operateur == "÷" && nombreAffiche == 0)
                {
                    AfficherErreur();
                    return;
                }

                nombreAffiche = Calculer(premierNombre, nombreAffiche, operateur);
                DisplayText.Text = nombreAffiche.ToString(CultureInfo.InvariantCulture);
            }

            premierNombre = nombreAffiche;
            operateur = touche;
            OperationText.Text = DisplayText.Text + " " + touche;
            nouveauNombre = true;
        }

        // AJOUT : touches sin, cos, tan (l'angle affiché est en degrés)
        void ClicFonction(string touche)
        {
            double angle = double.Parse(DisplayText.Text, CultureInfo.InvariantCulture);

            // Math.Sin, Math.Cos et Math.Tan travaillent en radians : on convertit
            double radian = angle * Math.PI / 180;
            double resultat = 0;

            if (touche == "sin")
            {
                resultat = Math.Sin(radian);
            }
            else if (touche == "cos")
            {
                resultat = Math.Cos(radian);
            }
            else if (touche == "tan")
            {
                // tan n'existe pas quand cos = 0 (90°, 270°...)
                if (Math.Abs(Math.Cos(radian)) < 0.0000000001)
                {
                    AfficherErreur();
                    return;
                }
                resultat = Math.Tan(radian);
            }

            // On arrondit pour éviter 0.000000000000000012 au lieu de 0
            resultat = Math.Round(resultat, 8);

            // On n'écrase pas le petit texte s'il y a un calcul en cours (ex : 5 + sin 30)
            if (operateur == "")
            {
                OperationText.Text = touche + "(" + DisplayText.Text + ")";
            }

            DisplayText.Text = resultat.ToString(CultureInfo.InvariantCulture);
            nouveauNombre = true;
        }

        // Touche =
        void ClicEgal()
        {
            if (operateur == "")
            {
                return;   // rien à calculer
            }

            double deuxiemeNombre = double.Parse(DisplayText.Text, CultureInfo.InvariantCulture);

            // Division par 0 : on affiche une erreur et on s'arrête
            if (operateur == "÷" && deuxiemeNombre == 0)
            {
                AfficherErreur();
                return;
            }

            double resultat = Calculer(premierNombre, deuxiemeNombre, operateur);

            OperationText.Text = OperationText.Text + " " + DisplayText.Text + " =";
            DisplayText.Text = resultat.ToString(CultureInfo.InvariantCulture);

            operateur = "";
            nouveauNombre = true;
        }

        // Fonction qui fait le calcul (cf. diapo « Functions » : elle renvoie une valeur)
        double Calculer(double a, double b, string op)
        {
            double resultat = 0;

            if (op == "+")
            {
                resultat = a + b;
            }
            else if (op == "−")
            {
                resultat = a - b;
            }
            else if (op == "×")
            {
                resultat = a * b;
            }
            else if (op == "÷")
            {
                resultat = a / b;
            }

            return resultat;
        }

        // Affiche "Erreur" et remet tout à zéro
        void AfficherErreur()
        {
            Effacer();
            DisplayText.Text = "Erreur";
        }

        // Touche C : tout remettre à zéro
        void Effacer()
        {
            DisplayText.Text = "0";
            OperationText.Text = "";
            premierNombre = 0;
            operateur = "";
            nouveauNombre = true;
        }
    }
}