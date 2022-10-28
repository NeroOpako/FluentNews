using System.ComponentModel;
using FluentNews.Core.Models;
using Windows.UI.Text;

namespace FluentNews.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        private bool _unread;
        private FontWeight _weight;

        public int id
        {
            get; set;
        }
        public string title
        {
            get; set;
        }
        public string body
        {
            get; set;
        }
        public string author
        {
            get; set;
        }
        public string favicon
        {
            get; set;
        }
        public string url
        {
            get; set;
        }
        public FontWeight weight
        {
            get => _weight;
            set
            {
                _weight = value; OnPropertyChanged("weight");
            }
        }
        public bool unread
        {
            get => _unread;
            set
            {
                _unread = value;
                OnPropertyChanged("unread");
            }
        }

        public ItemModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ItemModel FromItem(NextcloudNewsItem backendItem)
        {
            var newItem = new ItemModel();

            newItem.id = backendItem.id;
            newItem.title = backendItem.title;
            newItem.author = backendItem.author;
            newItem.body = backendItem.body;
            newItem.unread = backendItem.unread;
            newItem.url = backendItem.url;
            return newItem;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
