/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace HeartRate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    partial class MainPage
    {
        private App viewModel;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.StatusMessage = "Running ...";

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    this.viewModel.StatusMessage = "This app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    // Create the small and tile icons from writable bitmaps.
                    // Small icons are 24x24 pixels.
                    //WriteableBitmap smallIconBitmap = new WriteableBitmap(24, 24);
                    //BandIcon smallIcon = smallIconBitmap.ToBandIcon();
                    // Tile icons are 46x46 pixels for Microsoft Band 1, and 48x48 pixels
                    // for Microsoft Band 2.
                    //WriteableBitmap tileIconBitmap = new WriteableBitmap(46, 46);
                    //BandIcon tileIcon = tileIconBitmap.ToBandIcon();

                    // create a new Guid for the tile
                    Guid tileGuid = Guid.NewGuid();
                    // create a new tile with a new Guid
                /*    BandTile tile = new BandTile(tileGuid)
                    {
                        // enable badging (the count of unread messages)
                        IsBadgingEnabled = true,
                        // set the name
                        Name = "TileName",
                        // set the icons
                        SmallIcon = smallIcon,
                        TileIcon = tileIcon
                    };
                */    
                    // Create the small and tile icons from writable bitmaps.
                    // Small icons are 24x24 pixels.
                    WriteableBitmap smallIconBitmap = new WriteableBitmap(24, 24);
                    BandIcon smallIcon = smallIconBitmap.ToBandIcon();
                    // Tile icons are 46x46 pixels for Microsoft Band 1, and 48x48 pixels
                    // for Microsoft Band 2.
                    WriteableBitmap tileIconBitmap = new WriteableBitmap(46, 46);
                    BandIcon tileIcon = tileIconBitmap.ToBandIcon();

                    //WriteableBitmap smallIconBitmap = new WriteableBitmap(1, 1);
                    

                    // try #1
                    //var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconSmall.png", UriKind.Absolute));
                    //var fileStream = await imageFile.OpenAsync(FileAccessMode.Read);

                    // try #2
                    //var fileStream = File.OpenRead("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconSmall.png");

                    // try #3
                    //StorageFile fileStream = await StorageFile.GetFileFromApplicationUriAsync(new Uri("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconSmall.png", UriKind.Absolute));

                    //await smallIconBitmap.SetSourceAsync(fileStream);
                    //BandIcon smallIcon = smallIconBitmap.ToBandIcon();

                    //WriteableBitmap largeIconBitmap = new WriteableBitmap(1, 1);


                    // try #1
                    //var imageFile2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconLarge.png", UriKind.Absolute));
                    //var fileStream2 = await imageFile2.OpenAsync(FileAccessMode.Read);
                    
                    // try #2
                    //var fileStream2 = File.OpenRead("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconLarge.png");


                    // try #3
                    //StorageFile fileStream2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("C:/Users/Administrator/Downloads/Microsoft Band SDK and Samples for Windows/Samples/HeartRate/HeartRate.Shared/Assets/SampleTileIconLarge.png", UriKind.Absolute));


                    //await largeIconBitmap.SetSourceAsync(fileStream2);
                    //BandIcon largeIcon = largeIconBitmap.ToBandIcon();


                    BandTile tile = new BandTile(tileGuid)
                    {
                        // enable badging (the count of unread messages)
                        IsBadgingEnabled = true,
                        // set the name
                        Name = "My Tile",
                        // set the icons
                        TileIcon = tileIcon,
                        SmallIcon = smallIcon
                    };   

                    

                    // add the tile to the Band
                    await bandClient.TileManager.AddTileAsync(tile);


                    bool heartRateConsentGranted;

                    // Check whether the user has granted access to the HeartRate sensor.
                    if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() == UserConsent.Granted)
                    {
                        heartRateConsentGranted = true;
                    }
                    else
                    {
                        heartRateConsentGranted = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                    }

                    if (!heartRateConsentGranted)
                    {
                        this.viewModel.StatusMessage = "Access to the heart rate sensor is denied.";
                    }
                    else
                    {
                        //int samplesReceived = 0; // the number of HeartRate samples received
                        int myHeartRate = 0;
                        // Subscribe to HeartRate data.
                        //bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => { samplesReceived++; };
                        bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => { myHeartRate = args.SensorReading.HeartRate; };
                        await bandClient.SensorManager.HeartRate.StartReadingsAsync();

                        // Receive HeartRate data for a while, then stop the subscription.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await bandClient.SensorManager.HeartRate.StopReadingsAsync();

                        //this.viewModel.StatusMessage = string.Format("Done. {0} HeartRate samples were received!!!!", samplesReceived);
                        this.viewModel.StatusMessage = string.Format("Done. Your heartbeat is {0} . YAY!!!!", myHeartRate);
                    }
                }
            }
            catch (Exception ex)
            {
                this.viewModel.StatusMessage = ex.ToString();
            }
        }

    }
}
