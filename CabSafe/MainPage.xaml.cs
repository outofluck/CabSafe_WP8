using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CabSafe.Resources;
using Microsoft.Phone.UserData;
using SQLite;
using System.IO;
using Windows.Storage;
using CabSafe.DataModel;

namespace CabSafe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            ContactResultsData.Items.Clear();
            Contacts cons = new Contacts();

            //Identify the method that runs after the asynchronous search completes.
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);

            //Start the asynchronous search.
            cons.SearchAsync(String.Empty, FilterKind.None, "");
        }

        void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            foreach (Contact con in e.Results)
            {
                ContactResultsData.Items.Add(con.CompleteName.FirstName + "(" + con.PhoneNumbers.FirstOrDefault());
            }
        }

        private void SubmitContacts_Click(object sender, RoutedEventArgs e)
        {
            String contactNumber = "";
            foreach (String str in ContactResultsData.SelectedItems)
            {
                str.Remove(str.Length - 1);
                contactNumber += str.Substring(str.IndexOf('(') + 1) + ";";
            }

            //Saving to Contact Num to SQLite

            if (!contactNumber.Equals(""))
            {
                InsertData(contactNumber);
            }

            //Transfer to new Window
            NavigationService.Navigate(new Uri("/Home.xaml", UriKind.Relative));
        }

        private async void InsertData(string contacts)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "contacts.db"), true);

            ContactNum contact = new ContactNum();
            {   
                contact.ContactNumber = contacts;
            };

            await conn.InsertAsync(contact);
        }

    }
}