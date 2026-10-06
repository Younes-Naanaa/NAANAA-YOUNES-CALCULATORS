using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NAANAA_YOUNES_CALCULATORS
{
    // Délégué : référence vers une méthode qui prend deux double et renvoie un double
    public delegate double CalculDelegate(double x, double y);

    public class Calculatrice
    {
        public double Addition(double a, double b)
        {
            return a + b;
        }

        public double Soustraction(double a, double b)
        {
            return a - b;
        }

        public double Multiplication(double a, double b)
        {
            return a * b;
        }

        public double Division(double a, double b)
        {
            if (b != 0)
                return a / b;
            else
                throw new DivideByZeroException("Division par zéro.");
        }
    }

    public partial class MainWindow : Window
    {
        // Création d'une instance de la classe Calculatrice
        private Calculatrice calculatrice = new Calculatrice();

        private CalculDelegate operation;   // délégué de l'opération en cours (null si aucune)
        private string symbole = "";        // + − × ÷
        private double premierNombre = 0;
        private bool nouvelleSaisie = true;

        public MainWindow()
        {
            InitializeComponent();
            Reinitialiser();
        }

        // Un seul gestionnaire pour toutes les touches
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string touche = ((Button)sender).Content.ToString();

            switch (touche)
            {
                case "+":
                    ChoisirOperation(calculatrice.Addition, touche);
                    break;
                case "−":
                    ChoisirOperation(calculatrice.Soustraction, touche);
                    break;
                case "×":
                    ChoisirOperation(calculatrice.Multiplication, touche);
                    break;
                case "÷":
                    ChoisirOperation(calculatrice.Division, touche);
                    break;
                case "=":
                    Calculer();
                    break;
                case "C":
                    Reinitialiser();
                    break;
                case ".":
                    AjouterPoint();
                    break;
                default:
                    AjouterChiffre(touche);
                    break;
            }
        }

        private void AjouterChiffre(string chiffre)
        {
            if (nouvelleSaisie || DisplayText.Text == "0")
                DisplayText.Text = chiffre;
            else if (DisplayText.Text.Length < 12)
                DisplayText.Text += chiffre;

            nouvelleSaisie = false;
        }

        private void AjouterPoint()
        {
            if (nouvelleSaisie)
            {
                DisplayText.Text = "0.";
                nouvelleSaisie = false;
            }
            else if (!DisplayText.Text.Contains("."))
            {
                DisplayText.Text += ".";
            }
        }

        private void ChoisirOperation(CalculDelegate nouvelleOperation, string nouveauSymbole)
        {
            // Enchaînement : 2 + 3 + ... calcule d'abord 2 + 3
            if (operation != null && !nouvelleSaisie)
            {
                if (!Calculer())
                    return;   // erreur : on s'arrête
            }

            premierNombre = LireAffichage();
            operation = nouvelleOperation;
            symbole = nouveauSymbole;
            OperationText.Text = $"{Formater(premierNombre)} {symbole}";
            nouvelleSaisie = true;
        }

        // Renvoie false s'il y a eu une erreur
        private bool Calculer()
        {
            if (operation == null)
                return true;

            double second = LireAffichage();

            try
            {
                // Utilisation du délégué pour effectuer l'opération
                double resultat = operation(premierNombre, second);

                OperationText.Text = $"{Formater(premierNombre)} {symbole} {Formater(second)}";
                DisplayText.Text = Formater(resultat);
                operation = null;
                nouvelleSaisie = true;
                return true;
            }
            catch (DivideByZeroException)
            {
                Reinitialiser();
                DisplayText.Text = "Erreur";
                return false;
            }
        }

        // Tout supprimer : écran, opération en cours, calcul précédent
        private void Reinitialiser()
        {
            premierNombre = 0;
            operation = null;
            symbole = "";
            nouvelleSaisie = true;
            DisplayText.Text = "0";
            OperationText.Text = "";
        }

        private double LireAffichage()
        {
            double valeur;
            if (double.TryParse(DisplayText.Text, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out valeur))
                return valeur;
            else
                return 0;   // par exemple si l'écran affiche "Erreur"
        }

        // Évite les affichages du type 0.30000000000000004
        private string Formater(double valeur)
        {
            return valeur.ToString("0.##########", CultureInfo.InvariantCulture);
        }
    }
}