using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SZK
{
    public partial class EditItemPage : ContentPage
    {
        private ObservableCollection<CollectionModel> items;
        private CollectionModel itemToEdit;
        private Action onSaveCallback;
        private string selectedImagePath;

        public EditItemPage(CollectionModel itemToEdit, ObservableCollection<CollectionModel> items, Action onSaveCallback)
        {
            InitializeComponent();
            this.itemToEdit = itemToEdit;
            this.items = items;
            this.onSaveCallback = onSaveCallback;

            ItemNameEntry.Text = itemToEdit.Name;
            PriceEntry.Text = itemToEdit.Price > 0 ? itemToEdit.Price.ToString(CultureInfo.InvariantCulture) : "";
            StatusPicker.SelectedItem = itemToEdit.Status;
            RatingSlider.Value = itemToEdit.Rating;
            CommentEntry.Text = itemToEdit.Comment;

            ProfileImage.Source = string.IsNullOrWhiteSpace(itemToEdit.ProfileImagePath)
                                  ? "default_image_placeholder.png"
                                  : ImageSource.FromFile(itemToEdit.ProfileImagePath);
            selectedImagePath = itemToEdit.ProfileImagePath;
        }

        private async void OnChangeImageButtonClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                selectedImagePath = result.FullPath;
                ProfileImage.Source = ImageSource.FromFile(selectedImagePath);
            }
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            itemToEdit.Name = ItemNameEntry.Text;

            if (decimal.TryParse(PriceEntry.Text, out decimal price))
            {
                itemToEdit.Price = price;
            }
            else
            {
                itemToEdit.Price = 0; 
            }

            itemToEdit.Status = StatusPicker.SelectedItem?.ToString(); 

            itemToEdit.Rating = (int)RatingSlider.Value; 

            itemToEdit.Comment = CommentEntry.Text; 

            if (!string.IsNullOrWhiteSpace(selectedImagePath))
            {
                itemToEdit.ProfileImagePath = selectedImagePath;
            }

            onSaveCallback?.Invoke(); 
            await Navigation.PopAsync(); 
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz usun¹æ ten element?", "Tak", "Nie");
            if (isConfirmed)
            {
                items.Remove(itemToEdit);
                onSaveCallback?.Invoke(); 
                await Navigation.PopAsync(); 
            }
        }
    }
}
