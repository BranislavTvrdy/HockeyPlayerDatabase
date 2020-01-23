using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.MainApp
{
    /// <summary>
    /// Interaction logic for AddPlayer.xaml
    /// </summary>
    public partial class AddPlayer : Window
    {
        public int Krp { get; private set; }
        public string TitleBefore { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int YearOfBirth { get; private set; }
        public AgeCategory? AgeCategory { get; private set; }
        public string Club { get; private set; }




        public AddPlayer()
        {
            InitializeComponent();
            ComboBoxAgeCategory.ItemsSource = Enum.GetValues(typeof(Interfaces.AgeCategory)).Cast<Interfaces.AgeCategory>().ToList<Interfaces.AgeCategory>();
        }

        private void BtnPlayerOk_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextBoxKrp.Text)
                && Int32.TryParse(TextBoxKrp.Text, out var krpId)
                && !String.IsNullOrWhiteSpace(TextBoxTitleBefore.Text)
                && !String.IsNullOrWhiteSpace(TextBoxFirstName.Text)
                && !String.IsNullOrWhiteSpace(TextBoxLastName.Text)
                && !String.IsNullOrWhiteSpace(TextBoxYearOfBirth.Text)
                && Int32.TryParse(TextBoxYearOfBirth.Text, out var yearOfBirth)
                && !String.IsNullOrWhiteSpace(ComboBoxAgeCategory.SelectedItem.ToString())
                && Enum.TryParse(ComboBoxAgeCategory.SelectedItem.ToString(), out AgeCategory ageCategory)
                && !String.IsNullOrWhiteSpace(ComboBoxClub.SelectedItem.ToString())
            )
            {
                Krp = krpId;
                TitleBefore = TitleBefore;
                FirstName = TextBoxFirstName.Text;
                LastName = TextBoxLastName.Text;
                YearOfBirth = yearOfBirth;
                AgeCategory = ageCategory;
                Club = ComboBoxClub.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Error in entry! Please check your form...", "Wrong entry", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPlayerCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
