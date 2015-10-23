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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Xaml.Controls;


namespace TileEvents
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    partial class MainPage
    {
        private App viewModel;
        String a = null;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.StatusMessage = "Running ...";

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    this.viewModel.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }


                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    // Create a Tile with a TextButton on it.

                    IEnumerable<BandTile> tileslist = await
                       bandClient.TileManager.GetTilesAsync();

                    foreach (var t in tileslist)
                    {

                        if (t.Name == "Fan Control")
                        {
                            a = t.Name;
                        }
                    }

                    if (a != "Fan Control")
                    {
                        Guid myTileId = new Guid("12408A60-13EB-46C2-9D24-F14BF6A033C6");
                        Guid pageId = Guid.NewGuid();

                        BandTile myTile = new BandTile(myTileId)
                        {
                            Name = "Fan Control",
                            TileIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconLarge.png"),
                            SmallIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconSmall.png")
                        };

                        TextButton button = new TextButton() { ElementId = 1, Rect = new PageRect(0, 0, 50, 120) };
                        TextButton button1 = new TextButton() { ElementId = 2, Rect = new PageRect(50, 0, 50, 120) };
                        TextButton button2 = new TextButton() { ElementId = 3, Rect = new PageRect(100, 0, 50, 120) };
                        TextButton button3 = new TextButton() { ElementId = 4, Rect = new PageRect(150,0, 50, 120) };
                        TextButton button4= new TextButton() { ElementId = 5, Rect = new PageRect(200, 0, 50, 120) };
                        TextButton button5 = new TextButton() { ElementId = 6, Rect = new PageRect(250, 0, 50, 120) };

                        ScrollFlowPanel panel = new ScrollFlowPanel(button, button1, button2, button3, button4, button5) { 
                            Orientation = FlowPanelOrientation.Horizontal,
                            Rect = new PageRect(0, 0, 320, 120), 
                            };
                        myTile.PageLayouts.Add(new PageLayout(panel));


                        //FilledPanel panel1 = new FilledPanel(button1) { Rect = new PageRect(0, 0, 220, 150) };
                        //myTile.PageLayouts.Add(new PageLayout(panel1));

                        // Remove the Tile from the Band, if present. An application won't need to do this everytime it runs. 
                        // But in case you modify this sample code and run it again, let's make sure to start fresh.
                        await bandClient.TileManager.RemoveTileAsync(myTileId);

                        // Create the Tile on the Band.
                        await bandClient.TileManager.AddTileAsync(myTile);
                        await bandClient.TileManager.SetPagesAsync(myTileId, new PageData(new Guid("5F5FD06E-BD37-4B71-B36C-3ED9D721F200"), 0, new TextButtonData(1, "1"), new TextButtonData(2, "2"), new TextButtonData(3, "3"), new TextButtonData(4, "4"), new TextButtonData(5, "5"), new TextButtonData(6, "6")));
                        //await bandClient.TileManager.SetPagesAsync(myTileId, new PageData(new Guid("5F5FD06E-BD37-4B71-B36C-3ED9D721F200"), 1, new TextButtonData(2, "Click here2")));

                        // Subscribe to Tile events.
                        int buttonPressedCount = 0;

                        
                    }
                   
                        bandClient.TileManager.TileButtonPressed += TileManager_TileButtonPressed;
                    
                    TaskCompletionSource<bool> closePressed = new TaskCompletionSource<bool>();

                    bandClient.TileManager.TileClosed += (s, args) =>
                    {
                        closePressed.TrySetResult(true);
                    };

                    await bandClient.TileManager.StartReadingsAsync();

                    // Receive events until the Tile is closed.
                    this.viewModel.StatusMessage = "Check the Tile on your Band (it's the last Tile). Waiting for events ...";

                    await closePressed.Task;

                    // Stop listening for Tile events.
                  //  await bandClient.TileManager.StopReadingsAsync();

                    //this.viewModel.StatusMessage = "Done.";
                }
            }
            catch (Exception ex)
            {
                this.viewModel.StatusMessage = ex.ToString();
            }
        }


        void TileManager_TileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            //TextButton tb = (TextButton)sender;



            //tb.ElementId;
            //this.viewModel.StatusMessage = string.Format("TileButtonPressed =" + id+"  oo"+idd+"buttonPressedCount");
            var a = Dispatcher.RunAsync(  
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var idd = e.TileEvent.ElementId;
                    // var id = 1;
                    string url = null;
                    //  buttonPressedCount++;
                    this.viewModel.StatusMessage = string.Format("TileButtonPressed =" + idd + "  oo" + idd + "buttonPressedCount");
                    switch (idd)
                    {
                        case 1: url = "http://192.168.1.100:8000/automate/on";

                            break;
                        case 2: url = "http://192.168.1.100:8000/automate/off";

                            break;
                        case 3: url = "http://192.168.1.100:8000/automate/onf";


                            break;
                        case 4: url = "http://192.168.1.100:8000/automate/dance";

                            break;
                        case 5: url = "http://192.168.1.100:8000/automate/dim";


                            break;
                        case 6: url = "http://192.168.1.100:8000/automate/";

                            break;
                    }
                    get(url);
                }
            );
        }

        private async Task<BandIcon> LoadIcon(string uri)
        {
            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }




        //int buttonPressedCount = 0;


        public void get(String url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.1.100:8000/");
                var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("", "login")
            });
                try
                {
                    var result = client.GetAsync(url).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    this.viewModel.StatusMessage = resultContent;
                }
                catch (Exception e)
                {
                    this.viewModel.StatusMessage = "123";


                }


            }
        }


    }
}
