using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdService.Pages
{
    public partial class UserPage : Page
    {
        private Users _currentUser;

        public UserPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserAds();
        }

        private void LoadUserAds()
        {
            try
            {
                var userAds = Entities.GetContext().Ads
                    .Include(a => a.Statuses) 
                    .AsNoTracking()
                    .Where(a => a.Userid == _currentUser.Userid)
                    .ToList();
                ListAds.ItemsSource = userAds;

                UpdateProfit(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var addPage = new AddPage(null, _currentUser);
            addPage.DataSaved += LoadUserAds; 
            NavigationService.Navigate(addPage);
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var adsForRemoving = ListAds.SelectedItems.Cast<Ads>().ToList();

            if (adsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите объявления для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {adsForRemoving.Count} объявлений?", "Внимание",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var trackedAds = adsForRemoving.Select(ad => Entities.GetContext().Ads.Find(ad.AddId)).ToList();

                    Entities.GetContext().Ads.RemoveRange(trackedAds);
                    Entities.GetContext().SaveChanges();

                    MessageBox.Show("Объявления успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadUserAds(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ListAds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedAd = ListAds.SelectedItem as Ads;

            if (selectedAd != null)
            {
                var addPage = new AddPage(selectedAd, _currentUser);
                addPage.DataSaved += LoadUserAds;
                NavigationService.Navigate(addPage);
            }
            else
            {
                MessageBox.Show("Выберите объявление для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckBoxOnlyCompleted_Checked(object sender, RoutedEventArgs e)
        {
            UpdateAds();
        }

        private void CheckBoxOnlyCompleted_UnChecked(object sender, RoutedEventArgs e)
        {
            UpdateAds();
        }

        private void UpdateAds()
        {
            var currentAds = Entities.GetContext().Ads
                .Include(a => a.Statuses) 
                .AsNoTracking()
                .Where(a => a.Userid == _currentUser.Userid)
                .ToList();

            if (CheckBoxOnlyCompleted.IsChecked == true)
            {
                currentAds = currentAds
                    .Where(x => x.Statuses != null && x.Statuses.StatusName == "Завершенное")
                    .ToList();
            }

            ListAds.ItemsSource = currentAds;

            UpdateProfit(); 
        }

        private void UpdateProfit()
        {
            var completedAds = Entities.GetContext().Ads
                .Include(a => a.Statuses) 
                .Where(a => a.Userid == _currentUser.Userid && a.Statuses != null && a.Statuses.StatusName == "Завершенное")
                .ToList();

            decimal totalProfit = completedAds.Sum(a => a.AdPrice ?? 0); 

            TextBoxProfit.Text = totalProfit.ToString("C");
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                LoadUserAds();
            }
        }
    }
}