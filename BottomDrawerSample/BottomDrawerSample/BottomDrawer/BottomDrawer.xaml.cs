using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BottomDrawerSample.BottomDrawer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomDrawer : ContentView
    {
        private bool _isBackdropTapEnabled = true;
        private uint duration = 100;
        private double openY = (Device.RuntimePlatform == "Android") ? 20 : 60;
        private double lastPanY = 0;

        public BottomDrawer()
        {
            InitializeComponent();
            this.IsVisible = false;
        }


        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (_isBackdropTapEnabled)
            {
                await CloseDrawer();
            }
        }
        async void PanGestureRecognizer_OnPanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                _isBackdropTapEnabled = false;
                lastPanY = e.TotalY;
                //Debug.WriteLine($"Running: {e.TotalY}");
                if (e.TotalY > 0)
                {
                    BottomToolbar.TranslationY = openY + e.TotalY;
                }

            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                //Debug.WriteLine($"Completed: {e.TotalY}");
                if (lastPanY < 110)
                {
                    await OpenDrawer();
                }
                else
                {
                    await CloseDrawer();
                }
                _isBackdropTapEnabled = true;
            }
        }

        public async Task OpenDrawer()
        {
            this.IsVisible = true;
            await Task.WhenAll
            (
                Backdrop.FadeTo(1, length: duration),
                BottomToolbar.TranslateTo(0, openY, length: duration, easing: Easing.SinIn)
            );
            Backdrop.InputTransparent = false;
        }

        public async Task CloseDrawer()
        {
            await Task.WhenAll
            (
                Backdrop.FadeTo(0, length: duration),
                BottomToolbar.TranslateTo(0, 260, length: duration, easing: Easing.SinIn)
            );
            Backdrop.InputTransparent = true;
            this.IsVisible = false;
        }


        public static readonly BindableProperty ListItemsProperty =
            BindableProperty.Create("ListItems", typeof(IEnumerable<string>), typeof(BottomDrawer), null, BindingMode.Default, null, OnListItemsChangedPropertyHandler);

        private static void OnListItemsChangedPropertyHandler(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is BottomDrawer control)) return;
            if (!(newvalue is IEnumerable<string> list)) return;

            control.TableSection.Clear();
            foreach (var option in list)
            {
                var textCell = new TextCell()
                {
                    Text = option
                };

                textCell.Tapped += OnItemTappedHandler;
                control.TableSection.Add(textCell);
            }
        }

        private static async void OnItemTappedHandler(object sender, EventArgs e)
        {
            if (!(sender is TextCell item)) return;

            BottomDrawer control = GetBottomDrawer(item);
            control?.OnItemTappedCommand?.Execute(item.Text);
            if (control != null) await control.CloseDrawer();
        }

        private static BottomDrawer GetBottomDrawer(TextCell item)
        {
            var parent = item.Parent;
            if (parent == null) return null;
            while (parent != null)
            {
                if (parent.GetType() == typeof(BottomDrawer)) return parent as BottomDrawer;

                if (parent.Parent == null) break;
                parent = parent.Parent;
            }

            return null;
        }

        public IEnumerable<string> ListItems
        {
            get => (IEnumerable<string>)GetValue(ListItemsProperty);
            set
            {
                SetValue(ListItemsProperty, value);
                OnPropertyChanged("ListItems");
            }
        }


        public static readonly BindableProperty OnItemTappedCommandProperty =
            BindableProperty.Create("OnItemTappedCommand", typeof(ICommand), typeof(BottomDrawer), null, BindingMode.Default, null, null);

        public ICommand OnItemTappedCommand
        {
            get => (ICommand)GetValue(OnItemTappedCommandProperty);
            set
            {
                SetValue(OnItemTappedCommandProperty, value);
                OnPropertyChanged("OnItemTappedCommand");
            }
        }


        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title", typeof(string), typeof(BottomDrawer), null, BindingMode.Default, null, null);

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                OnPropertyChanged("Title");
            }
        }


    }
}