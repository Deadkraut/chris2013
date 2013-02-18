

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoverFlowDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {

        private StorageFolder folder;
        public MainPage()
        {
            this.InitializeComponent();
        }

        //Ordner wird nach Bildern durchsucht. Diese werden direkt in BitMapImages umgewandelt und dem Programm übergeben. Dateipfade funktionieren nicht.
        async public void getFolder()
        {
            var comics = new ObservableCollection<BitmapImage>();
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");
            folderPicker.FileTypeFilter.Add(".jps");
            folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                IReadOnlyList<StorageFile> fileItems = await folder.GetFilesAsync();
                foreach (StorageFile file in fileItems)
                {

                    Windows.Storage.Streams.IRandomAccessStreamWithContentType myStream = await file.OpenReadAsync();
                    var selectedPic = new BitmapImage();
                    selectedPic.SetSource(myStream);
                    comics.Add(selectedPic);
                }
                CoverFlowControl.ItemsSource = comics;
            }
            
        }

        async public void getFile()
        {
            var comics = new ObservableCollection<BitmapImage>();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jps");

            
            IReadOnlyList<StorageFile> fileItems = await openPicker.PickMultipleFilesAsync();
            if (fileItems.Count() > 0)
            {
                foreach (StorageFile file in fileItems)
                {
                    Windows.Storage.Streams.IRandomAccessStreamWithContentType myStream = await file.OpenReadAsync();
                    var selectedPic = new BitmapImage();
                    selectedPic.SetSource(myStream);
                    comics.Add(selectedPic);
                }
                CoverFlowControl.ItemsSource = comics;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        /// 
        private void loadContent(object sender, RoutedEventArgs e)
	    {
            getFolder();
	    }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var comics = new ObservableCollection<string>();
            comics.Add("Assets/HS Logo.jpg");
            CoverFlowControl.ItemsSource = comics;
        }
        private void loadPhoto(object sender, RoutedEventArgs e)
        {
            getFile();
        }

    }
}
