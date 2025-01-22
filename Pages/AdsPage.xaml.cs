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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdService.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdsPage.xaml
    /// </summary>
    public partial class AdsPage : Page
    {
        public AdsPage()
        {
            InitializeComponent();
            var currentAds = Entities.GetContext().Ads.ToList();
            ListAds.ItemsSource = currentAds;
            ComboBoxCity.ItemsSource = Entities.GetContext().Cities.ToList();
            ComboBoxCategory.ItemsSource = Entities.GetContext().Categories.ToList();
            ComboBoxType.ItemsSource = Entities.GetContext().Types.ToList();
            ComboBoxStatus.ItemsSource = Entities.GetContext().Statuses.ToList();
            UpdateAds();
        }

        private void UpdateAds()
        {
            var currentAds = Entities.GetContext().Ads.ToList();
            string searchText = TextBoxAds.Text?.ToLower() ?? string.Empty;
            currentAds = currentAds.Where(a => a.AdTitle.ToLower().Contains(searchText)).ToList();

            var selectedCity = (ComboBoxCity.SelectedItem as Cities)?.CityId;
            var selectedCategory = (ComboBoxCategory.SelectedItem as Categories)?.CategoryId;
            var selectedType = (ComboBoxType.SelectedItem as Types)?.TypeId;
            var selectedStatus = (ComboBoxStatus.SelectedItem as Statuses)?.StatusId;

            if (selectedCity.HasValue)
            {
                currentAds = currentAds.Where(a => a.CityId == selectedCity.Value).ToList();
            }

            if (selectedCategory.HasValue)
            {
                currentAds = currentAds.Where(a => a.CategoryId == selectedCategory.Value).ToList();
            }

            if (selectedType.HasValue)
            {
                currentAds = currentAds.Where(a => a.TypeId == selectedType.Value).ToList();
            }

            if (selectedStatus.HasValue)
            {
                currentAds = currentAds.Where(a => a.Statuid == selectedStatus.Value).ToList();
            }

            ListAds.ItemsSource = currentAds;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAds.Text = string.Empty;
            ComboBoxCity.SelectedIndex = -1;
            ComboBoxCategory.SelectedIndex = -1;
            ComboBoxType.SelectedIndex = -1;
            ComboBoxStatus.SelectedIndex = -1;
        }

        private void ComboBoxCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void TextBoxAds_TextChanged(object sender, TextChangedEventArgs e)
        { 
            UpdateAds();
        }

        private void Btn_auth(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }
    }
}
