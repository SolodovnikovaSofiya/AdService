using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AdService.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : Page
    {
        public event Action DataSaved; 

        private Ads _currentAd = new Ads();
        private Users _currentUser;

        public AddPage(Ads selectedAd, Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            if (selectedAd != null)
                _currentAd = selectedAd;

            DataContext = _currentAd;

            CityComboBox.ItemsSource = Entities.GetContext().Cities.ToList();
            CategoryComboBox.ItemsSource = Entities.GetContext().Categories.ToList();
            TypeComboBox.ItemsSource = Entities.GetContext().Types.ToList();
            StatusComboBox.ItemsSource = Entities.GetContext().Statuses.ToList();

            if (_currentAd.CategoryId > 0)
                CategoryComboBox.SelectedValue = _currentAd.CategoryId;
            if (_currentAd.TypeId > 0)
                TypeComboBox.SelectedValue = _currentAd.TypeId;
            if (_currentAd.Statuid > 0)
                StatusComboBox.SelectedValue = _currentAd.Statuid;
            if (_currentAd.CityId > 0)
                CityComboBox.SelectedValue = _currentAd.CityId;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentAd.AdTitle))
                errors.AppendLine("Введите название!");

            if (!decimal.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
                errors.AppendLine("Укажите верную цену!");
            else
                _currentAd.AdPrice = price;

            if (CategoryComboBox.SelectedItem == null)
                errors.AppendLine("Выберите категорию!");
            else
                _currentAd.CategoryId = (int)CategoryComboBox.SelectedValue;

            if (TypeComboBox.SelectedItem == null)
                errors.AppendLine("Выберите тип!");
            else
                _currentAd.TypeId = (int)TypeComboBox.SelectedValue;

            if (StatusComboBox.SelectedItem == null)
                errors.AppendLine("Выберите статус!");
            else
                _currentAd.Statuid = (int)StatusComboBox.SelectedValue;

            if (CityComboBox.SelectedItem == null)
                errors.AppendLine("Выберите город!");
            else
                _currentAd.CityId = (int)CityComboBox.SelectedValue;

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentAd.AddId == 0)
            {
                _currentAd.AdDatePublication = DateTime.Now;
                _currentAd.Userid = _currentUser.Userid;
                Entities.GetContext().Ads.Add(_currentAd);
            }
            else
            {
                var existingAd = Entities.GetContext().Ads.Find(_currentAd.AddId);
                if (existingAd != null)
                {
                    existingAd.AdTitle = _currentAd.AdTitle;
                    existingAd.AdDescription = _currentAd.AdDescription;
                    existingAd.AdPrice = _currentAd.AdPrice;
                    existingAd.Photo = _currentAd.Photo;
                    existingAd.CategoryId = _currentAd.CategoryId;
                    existingAd.TypeId = _currentAd.TypeId;
                    existingAd.Statuid = _currentAd.Statuid;
                    existingAd.CityId = _currentAd.CityId;
                }
            }

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");

                DataSaved?.Invoke();

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}