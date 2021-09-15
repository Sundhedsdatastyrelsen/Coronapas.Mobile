using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SSICPAS.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomConsentSwitch : ContentView
    {
        public CustomConsentSwitch()
        {
            InitializeComponent();
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(CustomConsentSwitchEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            
            if (propertyName == StateProperty.PropertyName)
            {
                UpdateSwitchState(State);
            }
        }

        private string _defaultOnText => "SETTINGS_SWITCH_ON".Translate();
        private string _defaultOffText => "SETTINGS_SWITCH_OFF".Translate();
        public static readonly BindableProperty OnTextProperty = BindableProperty.Create(propertyName: "OnText", returnType: typeof(string), declaringType: typeof(ContentView), defaultBindingMode: BindingMode.OneWay, propertyChanged: HandleTextPropertyChanged);
        public static readonly BindableProperty OffTextProperty = BindableProperty.Create(propertyName: "OffText", returnType: typeof(string), declaringType: typeof(ContentView) , defaultBindingMode: BindingMode.OneWay, propertyChanged: HandleTextPropertyChanged);

        public string OnText
        {
            get
            {
                return (string)base.GetValue(OnTextProperty);
            }
            set
            {
                if (this.OnText != value)
                {
                    base.SetValue(OnTextProperty, value);
                }     
            }
        }

        public string OffText
        {
            get
            {
                return (string)base.GetValue(OffTextProperty);
            }
            set
            {
                if (this.OffText != value)
                {
                    base.SetValue(OnTextProperty, value);
                }   
            }
        }

        private static void HandleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CustomConsentSwitch targetView;

            targetView = (CustomConsentSwitch)bindable;
            if (targetView != null)
                targetView.switchThumb.Text = (string)newValue;
        }

        private double valueX, valueY;
        private bool IsTurnX, IsTurnY;

        public void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            var x = e.TotalX;
            var y = e.TotalY;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    Debug.WriteLine("Started");
                    break;
                case GestureStatus.Running:
                    Debug.WriteLine("Running");

                    if ((x >= 5 || x <= -5) && !IsTurnX && !IsTurnY)
                    {
                        IsTurnX = true;
                    }

                    if ((y >= 5 || y <= -5) && !IsTurnY && !IsTurnX)
                    {
                        IsTurnY = true;
                    }

                    if (IsTurnX && !IsTurnY)
                    {
                        if (x <= valueX)
                        {
                            if (!State)
                            {
                                UpdateUI();
                            }
                        }

                        if (x >= valueX)
                        {
                            if (State)
                            {
                                UpdateUI();
                            }
                        }
                    }
                    OnSelectionChanged(new CustomConsentSwitchEventArgs(State));


                    break;
                case GestureStatus.Completed:
                    Debug.WriteLine("Completed");

                    valueX = x;
                    valueY = y;

                    IsTurnX = false;
                    IsTurnY = false;

                    break;
                case GestureStatus.Canceled:
                    Debug.WriteLine("Canceled");
                    break;


            }
        }

        public void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            State = !State;
        }

        private void UpdateSwitchState(bool state)
        {
            State = state;
            UpdateUI();
            OnSelectionChanged(new CustomConsentSwitchEventArgs(State));
        }

        private void UpdateUI()
        {
            if (State)
            {
                switchThumb.TranslateTo(switchThumb.X + 28, 0, 120);
                switchThumb.BorderColor = Color.FromHex("#3B37E2");
                switchTrack.BackgroundColor = Color.FromHex("#3B37E2");
                
                switchThumb.Text = string.IsNullOrEmpty(OnText) ? _defaultOnText : OnText;
                switchThumb.TextColor = Color.FromHex("#3B37E2");
            }
            else
            {
                switchThumb.TranslateTo(switchThumb.X, 0, 120);
                switchThumb.BorderColor = Color.FromHex("#C7D9D9");
                switchTrack.BackgroundColor = Color.FromHex("#DEEEEE");
                switchThumb.Text = string.IsNullOrEmpty(OffText) ? _defaultOffText : OffText;
                switchThumb.TextColor = Color.FromHex("#47526F");
            }
        }
        
        public static readonly BindableProperty StateProperty =
            BindableProperty.Create(nameof(State), typeof(bool), typeof(Grid), false, BindingMode.TwoWay);

        public bool State
        {
            get => (bool) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }


        void OnSwitchTapped(Object sender, EventArgs e)
        {
            State = !State;
        }

        public class CustomConsentSwitchEventArgs : EventArgs
        {
            public bool Selected { get; set; }

            public CustomConsentSwitchEventArgs(bool selected)
            {
                Selected = selected;
            }
        }
    }
}
