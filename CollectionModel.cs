using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SZK
{
    public class CollectionModel : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private string _profileImagePath;
        private decimal _price;
        private string _status;
        private int _rating;
        private string _comment;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSold => Status == "Sprzedany";

        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ProfileImagePath
        {
            get => _profileImagePath;
            set => SetProperty(ref _profileImagePath, value);
        }
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsSold));
            }
        }

        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
