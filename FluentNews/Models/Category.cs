using System.Xml.Linq;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
using System.ComponentModel;
using System;
using FluentNews.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using FluentNews.Views;

namespace FluentNews.Models
{
    public class Category : INotifyPropertyChanged
    {
        public string Name
        {
            get; set;
        }
        public int Id
        {
            get; set;
        }
        public Uri IconUri
        {
            get; set;
        }
        public String Url
        {
            get; set;
        }

        public TappedEventHandler eventHandler
        {
            get; set;
        }

        public Visibility HasUrl
        {
            get; set;
        }

        public Category()
        {
        }

        public static Category FromItem(NextcloudNewsFeed feed)
        {
            var newItem = new Category();
            newItem.Name = feed.title;
            newItem.Id = feed.id;
            newItem.Url = feed.url;
            newItem.IconUri = new Uri(feed.faviconLink != null && feed.faviconLink != "" ? feed.faviconLink : "ms-appx:///Assets/icons8-rss-48.png");
            newItem.HasUrl = feed.url != null && feed.url != "" ? Visibility.Visible : Visibility.Collapsed;
            newItem.eventHandler = new TappedEventHandler(bTapped_Tapped);
            return newItem;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async static void bTapped_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Category;
            await Launcher.LaunchUriAsync(new Uri(item.Url));
        }
    }
}
