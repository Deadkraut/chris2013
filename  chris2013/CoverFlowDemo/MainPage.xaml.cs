/*
 Help: http://stackoverflow.com/questions/12641223/thread-sleep-replacement-in-net-for-windows-store
 */

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
using System.Diagnostics;
//Ist für die Tastatur
using Windows.UI.Core;
using Windows.System;
//Wird für die Nutzung der Maus benötigt
using Windows.Devices.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoverFlowDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {

        private StorageFolder folder;
        private Boolean pause;
        private Boolean holding;
        public MainPage()
        {
            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown; // Wird benötigt damit die Tasten in der App gegen und runter gedrückt sind
            this.InitializeComponent();
        }

        private void MainPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey.Equals(VirtualKey.P) || args.VirtualKey.Equals(VirtualKey.Pause))
            {
                if (pause)
                    pause = true;
                else
                    pause = false;
            }
            if (args.VirtualKey.Equals(VirtualKey.Left)) //Bewegt die Bilder nach Links
            {
                CoverFlowControl.PreviousItem();
            }
            if (args.VirtualKey.Equals(VirtualKey.Right)) //Bewegt die Bilder nach Rechts
            {
                CoverFlowControl.NextItem();
            }

            if (args.VirtualKey.Equals(VirtualKey.R)) //Rotiert nach Rechts
            {
                
                CoverFlowControl.SelectedCoverItem.ZRotation -= 90;
            }
            if (args.VirtualKey.Equals(VirtualKey.L)) //Rotiert nach Rechts
            {

                CoverFlowControl.SelectedCoverItem.ZRotation += 90;
            }


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

        private void deleteContent(object sender, RoutedEventArgs e)
        {
            var comics = new ObservableCollection<string>();
            comics.Add("Assets/HS Logo.jpg");
            CoverFlowControl.ItemsSource = comics;
        }


        // This is the Methode for the Diashow
        private void startDiashow(object sender, RoutedEventArgs e)
        {
            startDiashow();
        }

        async private void startDiashow()
        {
            pause = false;
            int items = CoverFlowControl.Items.Count() - 1;
            for (int i = 0; i <= items; i++)
            {
                if (pause == false)
                {
                    DateTime start = DateTime.Now;
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5)); // Do something after 2 Seconds
                    DateTime end = DateTime.Now;
                    if (end.Subtract(start).Seconds == 5 && pause == false) // At this Place we need the same Nummber as in the Threads
                    {
                        Debug.WriteLine("Pause = false");
                        CoverFlowControl.NextItem();
                    }
                }

            }  

        }

        private void pauseDiashow(object sender, RoutedEventArgs e)
        {
            pause = true;
        }

        //Aktuelles Bild Vergrößern.
        private void zoomIn(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.SelectedCoverItem.Scale += 0.1;
        }

        //Aktuelles Bild verkleinern, ohne über die optimale Größe zu gehen.
        private void zoomOut(object sender, RoutedEventArgs e)
        {
            //var msg = new MessageDialog(CoverFlowControl.SelectedCoverItem.Scale.ToString());
            //msg.ShowAsync();
            
            if(CoverFlowControl.SelectedCoverItem.Scale > 1)
                CoverFlowControl.SelectedCoverItem.Scale -= 0.5;

            if(CoverFlowControl.SelectedCoverItem.Scale < 1)
                CoverFlowControl.SelectedCoverItem.Scale = 1.0;

        }

        //Funktion für Drehen und Zoomen mit Fingern.
       private void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var image = sender as Windows.UI.Xaml.Controls.Image;
            if (image != null)
            {
                Double scale = CoverFlowControl.SelectedCoverItem.Scale * e.Delta.Scale;
                if (scale >= 1.0)
                    CoverFlowControl.SelectedCoverItem.Scale = scale;

                CoverFlowControl.SelectedCoverItem.ZRotation += e.Delta.Rotation;
            }
        }
        //This Events are for the Fingerpressed Event
        
        private void ImagePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Pressed");
            holding = true;
            
        }

        private void ImagePointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Released");
            holding = false;
        }
        
        private void GridPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (holding)
            {
                pause = true;
                Debug.WriteLine("pause = true");
            }
        }

        private void rotateRight(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.SelectedCoverItem.ZRotation -= 90;
        }

        private void rotateLeft(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.SelectedCoverItem.ZRotation += 90;
        }

    }
}
