using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace SZK
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> collections = new ObservableCollection<string>();
        private string collectionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SZK", "collections.txt");
        private string selectedCollection;

        public MainPage()
        {
            InitializeComponent();
            CollectionsList.ItemsSource = collections;
            LoadCollections();

            Debug.WriteLine($"Ścieżka do danych aplikacji: {collectionsFilePath}");
        }

        private async void OnAddCollectionClicked(object sender, EventArgs e)
        {
            string collectionName = await DisplayPromptAsync("Nowa Kolekcja", "Podaj nazwę kolekcji:");
            if (!string.IsNullOrWhiteSpace(collectionName))
            {
                collections.Add(collectionName);
                SaveCollections();
            }
        }

        private async void OnEditCollectionClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Wybierz kolekcję do edycji:", "Anuluj", null, collections.ToArray());
            if (action != null && action != "Anuluj")
            {
                string newCollectionName = await DisplayPromptAsync("Edytuj Kolekcję", "Zmień nazwę kolekcji:", initialValue: action);
                if (!string.IsNullOrWhiteSpace(newCollectionName))
                {
                    int index = collections.IndexOf(action);
                    if (index != -1)
                    {
                        RenameCollectionFile(action, newCollectionName);

                        collections[index] = newCollectionName;
                        SaveCollections();
                    }
                }
            }
        }

        private void DeleteCollectionFile(string collectionName)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SZK", $"{collectionName}.txt");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        private async void OnDeleteCollectionClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Wybierz kolekcję do usunięcia:", "Anuluj", null, collections.ToArray());
            if (action != null && action != "Anuluj")
            {
                bool isConfirmed = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć kolekcję: {action}?", "Tak", "Nie");
                if (isConfirmed)
                {
                    DeleteCollectionFile(action);

                    collections.Remove(action);
                    SaveCollections();
                }
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string collectionName)
            {
                selectedCollection = collectionName;
                await Navigation.PushAsync(new CollectionDetailsPage(selectedCollection));
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private void LoadCollections()
        {
            collections.Clear();
            if (File.Exists(collectionsFilePath))
            {
                var lines = File.ReadAllLines(collectionsFilePath);
                foreach (var line in lines)
                {
                    collections.Add(line);
                }
            }
        }

        private void RenameCollectionFile(string oldName, string newName)
        {
            var oldFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SZK", $"{oldName}.txt");
            var newFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SZK", $"{newName}.txt");

            if (File.Exists(oldFilePath))
            {
                File.Move(oldFilePath, newFilePath);
            }
        }


        private void SaveCollections()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(collectionsFilePath));
            File.WriteAllLines(collectionsFilePath, collections);
        }
    }
}
