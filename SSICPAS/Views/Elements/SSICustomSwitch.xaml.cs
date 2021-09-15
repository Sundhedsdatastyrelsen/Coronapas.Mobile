using System;
using System.Diagnostics;
using SSICPAS.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SSICustomSwitch : ContentView
    {
        public SSICustomSwitch()
        {
            InitializeComponent();
            MoveToSelectionStart(false);
        }

        private const uint transitionDuration = 100;

        public static readonly BindableProperty SelectionStartActiveProperty =
            BindableProperty.Create(nameof(SelectionStartActive), typeof(bool), typeof(SSICustomSwitch), true,
                BindingMode.TwoWay, propertyChanged: OnSelectionStartActiveChanged);

        public bool SelectionStartActive {
            get
            {
                return (bool) GetValue(SelectionStartActiveProperty);
            }
            set
            {
                SetValue(SelectionStartActiveProperty, value);
            }
        }

        private double valueX, valueY;
        private bool IsTurnX, IsTurnY;

        static void OnSelectionStartActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SSICustomSwitch instance = bindable as SSICustomSwitch;
            bool newBool = (bool)newValue;
            if (newBool)
            {
                instance.MoveToSelectionStart(false);
            }
            else
            {
                instance.MoveToSelectionEnd(false);
            }
        }

        public void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            var x = e.TotalX; // TotalX Left/Right
            var y = e.TotalY; // TotalY Up/Down

            // StatusType
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    Debug.WriteLine("Started");
                    break;
                case GestureStatus.Running:
                    Debug.WriteLine("Running");

                    // Check that the movement is x or y
                    if ((x >= 5 || x <= -5) && !IsTurnX && !IsTurnY)
                    {
                        IsTurnX = true;
                    }

                    if ((y >= 5 || y <= -5) && !IsTurnY && !IsTurnX)
                    {
                        IsTurnY = true;
                    }

                    // X (Horizontal)
                    if (IsTurnX && !IsTurnY)
                    {
                        if (x <= valueX)
                        {
                            // Left
                            if (!SelectionStartActive)
                            {
                                SelectionStartActive = true;
                                MoveToSelectionStart();
                            }
                        }

                        if (x >= valueX)
                        {
                            // Right
                            if (SelectionStartActive)
                            {
                                SelectionStartActive = false;
                                MoveToSelectionEnd();
                            }
                        }
                    }


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

        void OnText1Tapped(object sender, EventArgs e)
        {
            if (!SelectionStartActive)
            {
                SelectionStartActive = true;
                MoveToSelectionStart();
            }
        }

        void OnText2Tapped(object sender, EventArgs e)
        {
            if (SelectionStartActive)
            {
                SelectionStartActive = false;
                MoveToSelectionEnd();
            }
        }

        private void MoveToSelectionStart(bool animated = true)
        {
            //make selection1 move to selection 2
            text2.TextColor = Color.FromHex("#47526F");
            text2.FontFamily = "IBMPlexSansRegular";
            text1.TextColor = Color.White;
            text1.FontFamily = "IBMPlexSansSemiBold";
            ReattachEffects();
            runningFrame.TranslateTo(runningFrame.X + 5, 0, animated ? transitionDuration : 1);
            var duration = TimeSpan.FromSeconds(0.2);
            if (animated) 
                Vibration.Vibrate(duration);
        }

        private void MoveToSelectionEnd(bool animated = true)
        {
            //make selection2 move to selection 1
            text2.TextColor = Color.White;
            text2.FontFamily = "IBMPlexSansSemiBold";
            text1.TextColor = Color.FromHex("#47526F");
            text1.FontFamily = "IBMPlexSansRegular";
            ReattachEffects();
            runningFrame.TranslateTo(runningFrame.X + 87, 0, animated ? transitionDuration : 1);
            var duration = TimeSpan.FromSeconds(0.2);
            if (animated)
                Vibration.Vibrate(duration);
        }

        private void ReattachEffects()
        {
            text1.Effects.Add(Effect.Resolve($"SSICPAS.{nameof(FontSizeLabelEffect)}"));
            text2.Effects.Add(Effect.Resolve($"SSICPAS.{nameof(FontSizeLabelEffect)}"));
        }
    }
}