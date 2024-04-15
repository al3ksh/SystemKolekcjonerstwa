using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SZK
{
    public partial class AddItemPage : ContentPage
    {
        private ObservableCollection<CollectionModel> items;
        private Action<CollectionModel> onItemAdded;
        private string selectedImagePath;

        public AddItemPage(ObservableCollection<CollectionModel> items, Action<CollectionModel> onItemAdded)
        {
            InitializeComponent();
            this.items = items;
            this.onItemAdded = onItemAdded;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string itemName = ItemNameEntry.Text;

            if (string.IsNullOrWhiteSpace(itemName))
            {
                await DisplayAlert("B³¹d", "Nazwa elementu jest wymagana.", "OK");
                return;
            }

            var newItem = new CollectionModel
            {
                Id = Guid.NewGuid(),
                Name = itemName,
                ProfileImagePath = selectedImagePath
            };

            if (decimal.TryParse(PriceEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                newItem.Price = price;
            }

            if (StatusPicker.SelectedItem != null)
            {
                newItem.Status = StatusPicker.SelectedItem.ToString();
            }

            newItem.Rating = (int)RatingSlider.Value;

            newItem.Comment = CommentEntry.Text;

            items.Add(newItem);
            onItemAdded?.Invoke(newItem);
            await Navigation.PopAsync();
        }

        private async void OnPickImageButtonClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                selectedImagePath = result.FullPath;
                ItemImagePreview.Source = ImageSource.FromFile(selectedImagePath);
            }
        }
    }
}
