using System.ComponentModel;
using FluentNews.Models;
using FluentNews.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.System;
using static System.Net.Mime.MediaTypeNames;

namespace FluentNews.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }


    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        ArticlesListView.ItemsSource = ViewModel.Items;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var item = e.Parameter as Category;

        EmptyListText.Visibility = Visibility.Collapsed;
        LoadingProcessProgressRing.IsActive = true;

        if (item != null)
        {
            ViewModel.SelectedItem = item;
            ViewModel.LoadFeedItemsByFeedIdAsync(item.Id, item.Name != "Unread articles");
        } else {
            ViewModel.SelectedItem = new Category();
            ViewModel.LoadFeedItemsByFeedIdAsync(0, false);
        }
        EmptyListText.Visibility = ViewModel.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        LoadingProcessProgressRing.IsActive = false;

    }

    private async void ItemTapped(object sender, TappedRoutedEventArgs e)
    {
        var item = ((FrameworkElement)e.OriginalSource).DataContext as ItemModel;
        await Launcher.LaunchUriAsync(new Uri(item?.url));
        if(item.unread)
            ViewModel.toggleReadStatus(item);
    }

    private void MenuFlyoutItem_Tapped(object sender, RoutedEventArgs e)
    {
        var item = ((FrameworkElement)e.OriginalSource).DataContext as ItemModel;
        var pkg = new Windows.ApplicationModel.DataTransfer.DataPackage();
        pkg.SetText(item.url ?? "");
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pkg);
    }

    private void MenuFlyoutItem_Tapped_1(object sender, RoutedEventArgs e)
    {
        var item = ((FrameworkElement)e.OriginalSource).DataContext as ItemModel;
        if (item.unread)
            ViewModel.toggleReadStatus(item);
        else
            ViewModel.toggleReadStatus(item);
    }

    private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        MenuFlyout myFlyout = new MenuFlyout();
        MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Copy url" };
        MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "Toggle read" };
        firstItem.Click += new RoutedEventHandler(MenuFlyoutItem_Tapped);
        secondItem.Click += new RoutedEventHandler(MenuFlyoutItem_Tapped_1);
        myFlyout.Items.Add(firstItem);
        myFlyout.Items.Add(secondItem);
        FrameworkElement senderElement = sender as FrameworkElement;
        myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
    }

    private void rc_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
    {
        ViewModel.RefreshCompletionDeferral1 = args.GetDeferral();
        ViewModel.LoadFeedItemsByFeedIdAsync(ViewModel.SelectedItem.Id, ViewModel.SelectedItem.Name != "Unread articles");
        if (ViewModel.RefreshCompletionDeferral1 != null)
        {
            ViewModel.RefreshCompletionDeferral1.Complete();
            ViewModel.RefreshCompletionDeferral1.Dispose();
            ViewModel.RefreshCompletionDeferral1 = null;
        }
    }

}
