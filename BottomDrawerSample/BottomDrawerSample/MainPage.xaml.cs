using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BottomDrawerSample
{
    public partial class MainPage : ContentPage
    {

        public ICommand OnItemSelected { get; set; }
        public MainPage()
        {
            InitializeComponent();
            OnItemSelected = new Command(async (obj) => ExecuteItemSelected(obj));
        }

        private async void ExecuteItemSelected(object o)
        {
            await DisplayAlert("Done!", "Ready", "OK");
        }


        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await BottomDrawer.OpenDrawer();

        }
    }
}
