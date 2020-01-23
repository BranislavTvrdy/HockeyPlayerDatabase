using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HockeyContext DbContext = new HockeyContext();
        public int NumberOfItems { get; set; }
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            BtnRemove.IsEnabled = false;
            BtnEdit.IsEnabled = false;
            BtnOpenClub.IsEnabled = false;
            //DataGridFilteredItems.ItemsSource = LoadCollectionData();
            #region InicializationDB
            DataGridFilteredItems.ItemsSource = null;
            List<Player> toDisplay = DbContext.GetPlayers().ToList();
            DataGridFilteredItems.ItemsSource = toDisplay;
            NumberOfItems = toDisplay.Count;
            Console.WriteLine("DB Loaded!");
            #endregion

        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            string whereCondition = "";
            bool isFiltering = false;
            //int krpId, yearOfBirthFrom, yearOfBirthTo;
            List<Player> filteredPlayers = DbContext.GetPlayers().ToList();
            if (!String.IsNullOrWhiteSpace(TextBoxKrp.Text) && Int32.TryParse(TextBoxKrp.Text, out _))
            {
                whereCondition = "player.Krp == " + TextBoxKrp.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.KrpId == int.Parse(TextBoxKrp.Text)).ToList();
                isFiltering = true;
            }
            if (!String.IsNullOrWhiteSpace(TextBoxFirstName.Text))
            {
                whereCondition = "player.FirstName == " + TextBoxFirstName.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.FirstName == TextBoxFirstName.Text).ToList();
                isFiltering = true;
            }

            if (!String.IsNullOrWhiteSpace(TextBoxLastName.Text))
            {
                whereCondition = "player.LastName == " + TextBoxLastName.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.LastName == TextBoxLastName.Text).ToList();
                isFiltering = true;
            }

            if (!String.IsNullOrWhiteSpace(TextBoxYearFrom.Text) 
                && Int32.TryParse(TextBoxYearFrom.Text, out _))
            {
                whereCondition = "player.YearOfBirth >= " + TextBoxYearFrom.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.YearOfBirth >= int.Parse(TextBoxYearFrom.Text)).ToList();
                isFiltering = true;
            }

            if (!String.IsNullOrWhiteSpace(TextBoxYearTo.Text)
                && Int32.TryParse(TextBoxYearTo.Text, out _))
            {
                whereCondition = "player.YearOfBirth <= " + TextBoxYearTo.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.YearOfBirth <= int.Parse(TextBoxYearFrom.Text)).ToList();
                isFiltering = true;
            }
            //Enum.TryParse(ComboBoxAgeCategory.SelectedItem.ToString(), out AgeCategory ageCategory)
            if (CheckBoxCadet.IsChecked != null && (bool) CheckBoxCadet.IsChecked)
            {
                whereCondition = "player.AgeCategory == " + AgeCategory.Cadet + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.AgeCategory == AgeCategory.Cadet).ToList();
                isFiltering = true;
            }
            if (CheckBoxJunior.IsChecked != null && (bool)CheckBoxJunior.IsChecked)
            {
                whereCondition = "player.AgeCategory == " + AgeCategory.Junior + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.AgeCategory == AgeCategory.Junior).ToList();
                isFiltering = true;
            }
            if (CheckBoxMidges.IsChecked != null && (bool)CheckBoxMidges.IsChecked)
            {
                whereCondition = "player.AgeCategory == " + AgeCategory.Midgest + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.AgeCategory == AgeCategory.Midgest).ToList();
                isFiltering = true;
            }
            if (CheckBoxSenior.IsChecked != null && (bool)CheckBoxSenior.IsChecked)
            {
                whereCondition = "player.AgeCategory == " + AgeCategory.Senior + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.AgeCategory == AgeCategory.Senior).ToList();
                isFiltering = true;
            }
            if (!String.IsNullOrWhiteSpace(TextBoxClub.Text))
            {
                whereCondition = "player.ClubId == " + TextBoxClub.Text + " && ";
                filteredPlayers = filteredPlayers.Where(p => p.ClubId == int.Parse(TextBoxClub.Text)).ToList();
                isFiltering = true;
            }

            if (isFiltering)
            {
                //TODO: make where condition out of filters

                DataGridFilteredItems.ItemsSource = null;
                DataGridFilteredItems.ItemsSource = filteredPlayers;
            }
            else
            {
                DataGridFilteredItems.ItemsSource = null;
                DataGridFilteredItems.ItemsSource = DbContext.GetPlayers().ToList();
                MessageBox.Show("No filters were chosen!", "Wrong filtering", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            TextBlockFilteredItems.Text = "Filtered items: " + filteredPlayers.Count + "/" + NumberOfItems;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddPlayer newPlayer = new AddPlayer
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            
            newPlayer.ShowDialog();
            
            newPlayer.Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you wish to remove selected player ?", "Player removal", MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
//            if (result == MessageBoxResult.Cancel)
//            {
//                this.Close();
//            }

            if (result == MessageBoxResult.OK)
            {
                //TODO: remove selected
                DbContext.Players.Remove((Player) DataGridFilteredItems.SelectedItem);
            }
            
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Player editingPlayer = (Player)DataGridFilteredItems.SelectedItem;
            EditPlayer newPlayer = new EditPlayer(editingPlayer)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            newPlayer.ShowDialog();

            newPlayer.Close();
        }

        private void BtnOpenClub_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you wish to close the application ?", "Closing application", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes )
            {
                this.Close();
            }
        }

        private void DataGridFilteredItems_OnSelected(object sender, RoutedEventArgs e)
        {
            BtnRemove.IsEnabled = true;
            BtnEdit.IsEnabled = true;
            BtnOpenClub.IsEnabled = true;
        }


        private void RowClickSelection(object sender, MouseButtonEventArgs e)
        {
            BtnRemove.IsEnabled = true;
            BtnEdit.IsEnabled = true;
            BtnOpenClub.IsEnabled = true;
        }

    }
}
