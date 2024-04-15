using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SZK
{
    public partial class CollectionDetailsPage : ContentPage
    {
        private ObservableCollection<CollectionModel> items = new ObservableCollection<CollectionModel>();
        private string collectionsFilePath;

        public CollectionDetailsPage(string collectionName)
        {
            InitializeComponent();
            collectionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SZK", $"{collectionName}.txt");
            CollectionNameLabel.Text = $"Elementy kolekcji: {collectionName}";
            LoadItems();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();

            var sortedItems = items.OrderBy(item => item.IsSold).ToList();
            items.Clear();
            foreach (var item in sortedItems)
            {
                items.Add(item);
            }

        }

        private void OnAddItemClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddItemPage(items, newItem =>
            {
                SaveItems();
            }));
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as CollectionModel;
            if (selectedItem != null)
            {
                Navigation.PushAsync(new EditItemPage(selectedItem, items, SaveItems));
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private async void OnSummaryButtonClicked(object sender, EventArgs e)
        {
            int totalCount = items.Count;
            int soldCount = items.Count(item => item.Status == "Sprzedany");
            int forSaleCount = items.Count(item => item.Status == "Na sprzeda¿");

            await DisplayAlert("Podsumowanie Kolekcji",
                $"Ca³kowita liczba przedmiotów: {totalCount}\n" +
                $"Liczba przedmiotów sprzedanych: {soldCount}\n" +
                $"Przedmioty na sprzeda¿: {forSaleCount}",
                "OK");
        }

        private void LoadItems()
        {
            items.Clear();
            if (File.Exists(collectionsFilePath))
            {
                var lines = File.ReadAllLines(collectionsFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(new string[] { "||" }, StringSplitOptions.None);
                    if (parts.Length >= 7)
                    {
                        items.Add(new CollectionModel
                        {
                            Id = Guid.Parse(parts[0]),
                            Name = parts[1],
                            ProfileImagePath = parts[2],
                            Price = decimal.TryParse(parts[3], out decimal price) ? price : 0,
                            Status = parts[4],
                            Rating = int.TryParse(parts[5], out int rating) ? rating : 0,
                            Comment = parts[6]
                        });
                    }
                }
            }
            ItemsList.ItemsSource = items;
        }


        private void SaveItems()
        {
            var lines = items.Select(item =>
                $"{item.Id}||{item.Name}||{item.ProfileImagePath}||{item.Price}||{item.Status}||{item.Rating}||{item.Comment}"
            ).ToArray();

            File.WriteAllLines(collectionsFilePath, lines);
        }

    }
}