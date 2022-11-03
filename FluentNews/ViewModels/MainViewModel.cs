using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentNews.Contracts.Services;
using FluentNews.Core.Services;
using FluentNews.Models;
using FluentNews.Services;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Core;
using static FluentNews.ViewModels.ShellViewModel;

namespace FluentNews.ViewModels;

public class MainViewModel : ObservableRecipient,INotifyPropertyChanged
{
    private ObservableCollection<ItemModel> _list;
    public delegate void SelectedItemChangedHandler(object sender, Category selectedItem);
    public delegate void VisualStateChangedEventHandler(object sender, Visibility selectedItem);
    public delegate void BooleanChangedEventHandler(object sender, Boolean enabled);


    public event SelectedItemChangedHandler SelectedItemChanged;
    public event VisualStateChangedEventHandler VisibilityChanged;
    public event BooleanChangedEventHandler BooleanChanged;

    private Category _selectedItem = null;
    private Boolean _buttonMarkReadEnabled;
    Windows.Storage.ApplicationDataContainer localSettings;


    public Boolean ButtonMarkReadVisibility
    {
        get
        {
            return _buttonMarkReadEnabled;
        }
        set
        {
            _buttonMarkReadEnabled = value; BooleanChanged?.Invoke(this, _buttonMarkReadEnabled); OnPropertyChanged("ButtonMarkReadVisibility");
        }
    }

    public ObservableCollection<ItemModel> Items
    {
        get
        {
            return _list;
        }
    }
    public Category SelectedItem
    {
        get
        {
            return _selectedItem;
        }
        set
        {
            _selectedItem = value; SelectedItemChanged?.Invoke(this, _selectedItem);
        }
    }

    public enum Result
    {
        NoError,
        ConnectionError,
    };


    //public event RefreshEventHandler RefreshComplete;



    public Deferral RefreshCompletionDeferral1
    {
        get;
        set;
    }

    public async void RemoveRead()
    {
        try
        {
            var backendItems = await NextcloudNewsInterface.getInstance().getItems();

            foreach (var item in backendItems.items)
            {
                var existingItem = (from ItemModel selitem in _list where selitem.id == item.id && item.unread == false select selitem).FirstOrDefault<ItemModel>();
                if (existingItem != null)
                {
                    _list.Remove(existingItem);
                }
            }
        }
        catch
        {
            //DJ: Calling the RefreshComplete callback here is valid, since erroring out here means the refresh can not continue.
            //TODO: Catch the right exception
            //RefreshComplete?.Invoke(this, (ShellViewModel.Result)Result.ConnectionError);
        }
    }

    public async void LoadFeedItemsByFeedIdAsync(int id, bool showAll)
    {
        var errorCondition = Result.NoError;
        try
        {
            NextcloudNewsInterface.getInstance().invalidateCache();
            var backendItems = await NextcloudNewsInterface.getInstance().getItems(showAll, -1, 0, id);
            _list.Clear();
            _buttonMarkReadEnabled = false;
            foreach (var item in backendItems.items)
            {
                var existingItem = (from ItemModel selitem in _list where selitem.id == item.id select selitem).FirstOrDefault<ItemModel>();
                if (existingItem == null)
                {
                    var newItem = ItemModel.FromItem(item);
                    if (newItem.unread)
                        newItem.weight = FontWeights.Bold;
                    else
                        newItem.weight = FontWeights.Normal;
                    _list.Add(newItem);
                    _buttonMarkReadEnabled = true;
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
            //RefreshComplete?.Invoke(this, (ShellViewModel.Result)errorCondition);
        }

    }

    public ItemModel GetItemById(int id)
    {
        return (from ItemModel item in _list where item.id == id select item).FirstOrDefault<ItemModel>();
    }

    public async void toggleReadStatus(ItemModel item)
    {
        var rawItem = (await NextcloudNewsInterface.getInstance().getItems()).getForId(item.id);
        item.unread = !item.unread;
        item.weight = item.unread ? FontWeights.Bold : FontWeights.Normal;
        NextcloudNewsInterface.getInstance().toggleItemRead(rawItem);
    }


    public MainViewModel()
    {
        
        _list = new ObservableCollection<ItemModel>();

        localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        NextcloudNewsInterface.getInstance((string)localSettings.Values["host"], (string)localSettings.Values["username"], (string)localSettings.Values["password"]);

    }
}
