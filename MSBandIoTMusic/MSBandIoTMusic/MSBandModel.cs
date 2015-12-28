using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles.Pages;
using Windows.UI;
using Microsoft.Band.Tiles;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band.Notifications;

namespace MSBandIoTMusic
{

    //Accelerometer 
    //Provides X, Y, and Z acceleration in g units. 1 g = 9.81 meters per second squared

    //Gyroscope
    //Provides X, Y, and Z angular velocity in degrees per second(°/sec) units

    //Distance
    //Provides the total distance in centimeters, current speed in centimeters per second(cm/s), current pace in milliseconds per meter(ms/m), and the current


    public class MSBandModel
    {
        public IBandClient bandClient;
        string bandName;

        public event EventHandler<int> BandHeartRateReceived;
        public event EventHandler<double> BandSkinTemperatureReceived;
        public event EventHandler<long> BandCaloriesReceived;
        public event EventHandler<long> BandTotalStepsReceived;
        public event EventHandler<UVIndexLevel> BandUVIndexLevelReceived;
        public event EventHandler<IBandDistanceReading> BandBandDistanceReceived;
        public event EventHandler<BandContactState> BandWornStateReceived;
        public event EventHandler<IBandAccelerometerReading> BandAccelerometerReceived;
        public event EventHandler<IBandAccelerometerReading> BandGyroscopeReceived;

        public async Task<bool> StartListening()
        {
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();
            if (pairedBands.Any())
            {
                var band = pairedBands.FirstOrDefault();
                if (band != null)
                {
                    bandName = band.Name;
                    bandClient = await BandClientManager.Instance.ConnectAsync(band);
                    var consent = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.HeartRate;
                        sensor.ReadingChanged += SensorReadingChanged;
                        await sensor.StartReadingsAsync();
                    }

                    consent = await bandClient.SensorManager.SkinTemperature.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.SkinTemperature;
                        sensor.ReadingChanged += Sensor_ReadingChanged; ;
                        await sensor.StartReadingsAsync();
                    }

                    consent = await bandClient.SensorManager.Calories.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.Calories;
                        sensor.ReadingChanged += Sensor_ReadingCaloriesChanged;
                        await sensor.StartReadingsAsync();
                    }

                    consent = await bandClient.SensorManager.Pedometer.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.Pedometer;
                        sensor.ReadingChanged += Sensor_ReadingPedometerChanged; ;
                        await sensor.StartReadingsAsync();
                    }

                    //consent = await bandClient.SensorManager.UV.RequestUserConsentAsync();
                    //if (consent)
                    //{
                    //    var sensor = bandClient.SensorManager.UV;
                    //    sensor.ReadingChanged += Sensor_ReadingUVChanged;
                    //    await sensor.StartReadingsAsync();
                    //}

                    consent = await bandClient.SensorManager.Distance.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.Distance;
                        sensor.ReadingChanged += Sensor_ReadingDistanceChanged; ;
                        await sensor.StartReadingsAsync();
                    }

                    consent = await bandClient.SensorManager.Contact.RequestUserConsentAsync();
                    if (consent)
                    {
                        var sensor = bandClient.SensorManager.Contact;
                        sensor.ReadingChanged += Sensor_ReadingBandWornChanged;
                        await sensor.StartReadingsAsync();
                    }


                    return consent;
                }
            }
            return false;
        }

        private void Sensor_ReadingGyroscopeChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            Debug.WriteLine("BandGyroscopeReceived: " + " " + e.SensorReading.AccelerationX + " " + e.SensorReading.AccelerationY + " " + e.SensorReading.AccelerationZ);
            if (BandGyroscopeReceived != null)
            {
                BandGyroscopeReceived(this, e.SensorReading);
            }
        }

        private void Sensor_ReadingAccelerometerChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            Debug.WriteLine("BandAccelerometerReceived: " + " " + e.SensorReading.AccelerationX + " " + e.SensorReading.AccelerationY + " " + e.SensorReading.AccelerationZ);
            if (BandAccelerometerReceived != null)
            {
                BandAccelerometerReceived(this, e.SensorReading);
            }
        }

        private void Sensor_ReadingBandWornChanged(object sender, BandSensorReadingEventArgs<IBandContactReading> e)
        {
            Debug.WriteLine("BandWornReceived: " + e.SensorReading.State);
            if (BandWornStateReceived != null)
            {
                BandWornStateReceived(this, e.SensorReading.State);
            }
        }

        private void Sensor_ReadingDistanceChanged(object sender, BandSensorReadingEventArgs<IBandDistanceReading> e)
        {
            Debug.WriteLine("BandBandDistanceReceived: " + e.SensorReading);
            if (BandBandDistanceReceived != null)
            {
                BandBandDistanceReceived(this, e.SensorReading);
            }
        }

        private void Sensor_ReadingUVChanged(object sender, BandSensorReadingEventArgs<IBandUVReading> e)
        {
            Debug.WriteLine("UVIndexLevel: " + e.SensorReading.IndexLevel);
            if (BandUVIndexLevelReceived != null)
            {
                BandUVIndexLevelReceived(this, e.SensorReading.IndexLevel);
            }
        }

        private void Sensor_ReadingPedometerChanged(object sender, BandSensorReadingEventArgs<IBandPedometerReading> e)
        {
            Debug.WriteLine("TotalSteps: " + e.SensorReading.TotalSteps);
            if (BandTotalStepsReceived != null)
            {
                BandTotalStepsReceived(this, e.SensorReading.TotalSteps);
            }
        }

        private void Sensor_ReadingCaloriesChanged(object sender, BandSensorReadingEventArgs<IBandCaloriesReading> e)
        {
            Debug.WriteLine("Calories: " + e.SensorReading.Calories);
            if (BandCaloriesReceived != null)
            {
                BandCaloriesReceived(this, e.SensorReading.Calories);
            }
        }

        private void SensorReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            Debug.WriteLine("HeartRate: " + e.SensorReading.HeartRate);
            if (BandHeartRateReceived != null)
            {
                BandHeartRateReceived(this, e.SensorReading.HeartRate);
            }
        }

        private void Sensor_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> e)
        {
            Debug.WriteLine("SkinTemperature: " + e.SensorReading.Temperature);
            if (BandSkinTemperatureReceived != null)
            {
                BandSkinTemperatureReceived(this, e.SensorReading.Temperature);
            }
        }


        public async Task StopListening()
        {
            if (bandClient != null)
            {
                var sensor = bandClient.SensorManager.HeartRate;
                sensor.ReadingChanged -= SensorReadingChanged;
                await sensor.StopReadingsAsync();

                var sensor1 = bandClient.SensorManager.SkinTemperature;
                sensor1.ReadingChanged -= Sensor_ReadingChanged;
                await sensor1.StopReadingsAsync();


                var sensor2 = bandClient.SensorManager.Calories;
                sensor2.ReadingChanged -= Sensor_ReadingCaloriesChanged;
                await sensor2.StopReadingsAsync();

                bandClient.Dispose();
                bandClient = null;
            }
        }

        public string TileID = "D781F673-6D05-4D69-BCFF-EA7E706C3488";

        public string barcodePageID = "D781F673-6D05-4D69-BCFF-EA7E706C3488";

        public async void CreatTile()
        {
            // Now we'll create the Tile.
            Guid myTileId = new Guid(TileID);
            BandTile myTile = new BandTile(myTileId)
            {
                Name = "My Tile",
                TileIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconLarge.png"),
                SmallIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconSmall.png")
            };

            // We'll create a Tile that looks like this:
            // +--------------------+
            // | MY CARD            | 
            // | |||||||||||||||||  | 
            // | 123456789          |
            // +--------------------+
            // First, we'll prepare the layout for the Tile page described above.
            TextBlock myCardTextBlock = new TextBlock()
            {
                Color = Colors.Blue.ToBandColor(),
                ElementId = 1, // the Id of the TextBlock element; we'll use it later to set its text to "MY CARD"
                Rect = new PageRect(0, 0, 200, 25)
            };
            Barcode barcode = new Barcode(BarcodeType.Code39)
            {
                ElementId = 2, // the Id of the Barcode element; we'll use it later to set its barcode value to be rendered
                Rect = new PageRect(0, 0, 250, 50)
            };
            TextBlock digitsTextBlock = new TextBlock()
            {
                ElementId = 3, // the Id of the TextBlock element; we'll use it later to set its text to "123456789"
                Rect = new PageRect(0, 0, 200, 25)
            };
            FlowPanel panel = new FlowPanel(myCardTextBlock, barcode, digitsTextBlock)
            {
                Orientation = FlowPanelOrientation.Vertical,
                Rect = new PageRect(0, 0, 250, 100)
            };
            myTile.PageLayouts.Add(new PageLayout(panel));


            TextButton button = new TextButton() { ElementId = 1, Rect = new PageRect(10, 10, 200, 90) };
            FilledPanel btnpanel = new FilledPanel(button) { Rect = new PageRect(0, 0, 220, 150) };
            myTile.PageLayouts.Add(new PageLayout(btnpanel));


            // Remove the Tile from the Band, if present. An application won't need to do this everytime it runs. 
            // But in case you modify this sample code and run it again, let's make sure to start fresh.
            await bandClient.TileManager.RemoveTileAsync(myTile.TileId);

            // Create the Tile on the Band.
            await bandClient.TileManager.AddTileAsync(myTile);

            // And create the page with the specified texts and values.
            PageData page = new PageData(
                Guid.NewGuid(), // the Id for the page
                0, // the index of the layout to be used; we have only one layout in this sample app, but up to 5 layouts can be registered for a Tile
                new TextBlockData(myCardTextBlock.ElementId.Value, "MY CARD"),
                new BarcodeData(barcode.BarcodeType, barcode.ElementId.Value, "123456789"),
                new TextBlockData(digitsTextBlock.ElementId.Value, "123456789"));

            PageData btnpage = new PageData(new Guid("5F5FD06E-BD37-4B71-B36C-3ED9D721F200"), 1, new TextButtonData(1, "Clear Meg"));

            await bandClient.TileManager.SetPagesAsync(myTile.TileId, page);
            await bandClient.TileManager.SetPagesAsync(myTileId, btnpage);

            bandClient.TileManager.TileButtonPressed += async (s, args) =>
            {
                ClearMmeg();
                await bandClient.TileManager.SetPagesAsync(myTile.TileId, page);
                await bandClient.TileManager.SetPagesAsync(myTileId, btnpage);
            };

            await bandClient.TileManager.StartReadingsAsync();

        }

        public async void ClearMmeg()
        {
            await bandClient.TileManager.RemovePagesAsync(new Guid(TileID));
        }

        public async void PushMeg()
        {
            await bandClient.NotificationManager.SendMessageAsync(new Guid(TileID), "Microsoft Band", "Hello World !", DateTimeOffset.Now, MessageFlags.ShowDialog);
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

    }
}