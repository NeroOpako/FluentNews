using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using FluentNews.Contracts.Services;
using FluentNews.Core.Services;
using FluentNews.Models;
using FluentNews.Views;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Navigation;

namespace FluentNews.ViewModels;

public class ShellViewModel : ObservableRecipient
{
    private bool _isBackEnabled;
    private Category _selected;

    Windows.Storage.ApplicationDataContainer localSettings;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    public Category Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

     private ObservableCollection<Category> _categories;

    public delegate void RefreshEventHandler(object sender, Result resultCondition);
   

    
    public ObservableCollection<Category> Categories
    {
        get
        {
            return _categories;
        }
    }

    
    public event RefreshEventHandler RefreshComplete;


    public enum Result
    {
        NoError,
        ConnectionError,
    };

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        _categories = new ObservableCollection<Category>();
        initCategory();

        localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        NextcloudNewsInterface.getInstance((string)localSettings.Values["host"], (string)localSettings.Values["username"], (string)localSettings.Values["password"]);
    }

    private void initCategory()
    {
        _categories.Clear();
        var category1 = new Category();
        var category2 = new Category();

        category1.Name = "Unread articles";
        category2.Name = "All articles";
        category1.Id = category2.Id = 0;

        category1.IconUri = new Uri("ms-appx:///Assets/icons8-rss-48.png");
        category2.IconUri = new Uri("ms-appx:///Assets/tm.png");

        category2.HasUrl = category1.HasUrl = Microsoft.UI.Xaml.Visibility.Collapsed;

        _categories.Add(category1);
        _categories.Add(category2);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
    }

    public async void Refresh()
    {
        var errorCondition = Result.NoError;
        initCategory();
        try
        {
            var feeds = await NextcloudNewsInterface.getInstance().getFeeds();
            foreach (var item in feeds.feeds)
            {
                var existingItem = (from Category selitem in _categories where selitem.Id == item.id select selitem).FirstOrDefault<Category>();
                if (existingItem == null)
                {
                    _categories.Add(Category.FromItem(item));
                }
            }
        }
        catch
        {
            //TODO: Catch the right exception
            errorCondition = Result.ConnectionError;
        }
        finally
        {
            RefreshComplete?.Invoke(this, (ShellViewModel.Result)errorCondition);
        }
    }
}
