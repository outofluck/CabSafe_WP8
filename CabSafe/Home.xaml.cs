using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SQLite;
using System.IO;
using Windows.Storage;
using Microsoft.Phone.Tasks;
using CabSafe.DataModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;

namespace CabSafe
{
    public partial class Home : PhoneApplicationPage
    {
        String _latitude, _longitude;

        //get the concatenated contact information
        String contact;
        public Home()
        {
            InitializeComponent();
        }

        private async void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "contacts.db"), true);

            var query = conn.Table<ContactNum>();
            var result = await query.ToListAsync();
            foreach (var contactNum in result)
            {
                contact += contactNum.ContactNumber;
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {

            //Check for the user agreement in use his position. If not, method returns.
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                     maximumAge: TimeSpan.FromMinutes(5),
                     timeout: TimeSpan.FromSeconds(10)
                    );

                //With this 2 lines of code, the app is able to write on a Text Label the Latitude and the Longitude, given by {{Icode|geoposition}}
                _latitude = geoposition.Coordinate.Latitude.ToString("0.00");
                _longitude = geoposition.Coordinate.Longitude.ToString("0.00");
            }
            //If an error is catch 2 are the main causes: the first is that you forgot to include ID_CAP_LOCATION in your app manifest. 
            //The second is that the user doesn't turned on the Location Services
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    MessageBox.Show("location is disabled in phone settings.");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }

            //////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////

            MessageBox.Show(_latitude);
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.To = contact;
            smsComposeTask.Body = "Hello,\n I am currently in a taxi with the following details:\n Plate#:" + PlateNumberTextBox.Text
                                    + "\n Taxi Name:" + TaxiNameTextBox.Text + "\n Driver's Name:" + DriversNameTextBox.Text
                                    + "\n Message:" + MessageTextBox.Text;



            String referenceNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            TaxiInformation taxiInfo = new TaxiInformation();
            taxiInfo.PlateNum = PlateNumberTextBox.Text;
            taxiInfo.SenderName = TaxiNameTextBox.Text;
            taxiInfo.DriverName = TaxiNameTextBox.Text;
            taxiInfo.OtherMessage = MessageTextBox.Text;
            taxiInfo.referenceCode = referenceNo;
            taxiInfo.Latitude = "";
            taxiInfo.Longitude = "";
            InserData(taxiInfo);

            // smsComposeTask.Show();
        }

        private async void InserData(TaxiInformation taxiInfo)
        {
            try
            {
                await App.MobileService.GetTable<TaxiInformation>().InsertAsync(taxiInfo);
                MessageBox.Show("Record Saved");
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }
}