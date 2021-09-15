using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SSICPAS.Services;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Views.Elements
{
    public partial class CustomPicker : Rg.Plugins.Popup.Pages.PopupPage
    {
        public IEnumerable<SelectionControl> ItemSource { get; set; }
        public IEnumerable<SelectionControl> UnderItemSource { get; set; }
        private readonly Action<ISelection> OnItemPickedAction;
        private readonly Action<ISelection> OnUnderItemPickedAction;
        private readonly Action OnEUPassportTypeChangeSpecificAction;
        public bool IsDKSelected => 
            ItemSource.Any(item =>
                item.SelectedPassportType == Enums.PassportType.DK_LIMITED &&
                item.IsSelected);

        public bool ShouldShowSeparatorAndList => !IsDKSelected && UnderItemSource.Count() > 1;
        public SelectionControl PreviousSelected;

        private bool _shouldNotMarkSelectedFamilyMember;

        public CustomPicker(
            IEnumerable<SelectionControl> itemSource,
            Action<ISelection> onItemPickedAction,
            string title,
            IEnumerable<SelectionControl> underItemSource,
            Action<ISelection> onUnderItemPickedAction,
            Action onEUPassportTypeChangeSpecificAction)
        {
            InitializeComponent();
            _shouldNotMarkSelectedFamilyMember = false;
            PickerTitle.Text = title;
            AndroidTalkbackAccessibilityWorkaround = true;
            ItemSource = itemSource;
            UnderItemSource = underItemSource;
            OnItemPickedAction = onItemPickedAction;
            OnUnderItemPickedAction = onUnderItemPickedAction;
            OnEUPassportTypeChangeSpecificAction = onEUPassportTypeChangeSpecificAction;
            GenerateIconSelectionList(ItemSource);
            GenerateSelectionList(UnderItemSource);
            AutomationProperties.SetName(CloseButton, "PASSPORT_TYPE_CLOSE_BUTTON_ACCESSIBILITY".Translate());
            BindingContext = this;
            PreviousSelected = ItemSource.FirstOrDefault(x => x.IsSelected);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND, OnGoingToBackground);
        }

        private async void OnGoingToBackground(object obj)
        {
            PopupPage popup = PopupNavigation.Instance.PopupStack.
                FirstOrDefault(popup => popup.GetType() == typeof(CustomPicker));
            if (popup != null)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await PopupNavigation.Instance.RemovePageAsync(popup);
                });
            }
        }

        public async void ClosePicker(object sender, EventArgs e)
        {
            OnItemPickedAction?.Invoke(PreviousSelected);
            await PopupNavigation.Instance.PopAsync(true);
        }

        private void GenerateIconSelectionList(IEnumerable<SelectionControl> itemSource)
        {
            foreach (SelectionControl item in itemSource)
            {
                CustomIconPickerLayout view = new CustomIconPickerLayout(item);
                view.OnItemSelected += ItemPickedAction;
                if (item.SelectedPassportType == Enums.PassportType.DK_LIMITED)
                {
                    IconGridLayout.Children.Add(view, 0,1,0,1);
                }
                else
                {
                    IconGridLayout.Children.Add(view, 1,2,0,1);
                }
 
            }
        }

        private void GenerateSelectionList(IEnumerable<SelectionControl> itemSource, bool shouldMarkItemSelected = false)
        {
            foreach (SelectionControl item in itemSource)
            {
                CustomPickerLayout view = new CustomPickerLayout(item, IsDKSelected, shouldMarkItemSelected);
                view.OnItemSelected += UnderItemPickedAction;
                PickerStackLayout.Children.Add(view);
            }
        }

        private async void ItemPickedAction(ISelection obj)
        {
            SelectionControl selectionControl = obj as SelectionControl;
            ItemSource = ItemSource
                .Select(item =>
                {
                    item.IsSelected = item.SelectedPassportType == selectionControl.SelectedPassportType;
                    return item;
                });
            if (selectionControl?.SelectedPassportType != PassportType.UNIVERSAL_EU ||
                !ShouldShowSeparatorAndList)
            {
                await PopupNavigation.Instance.PopAsync();
                OnItemPickedAction?.Invoke(obj);
            }
            else
            {
                bool shouldShowEUAlert = PreviousSelected.SelectedPassportType != selectionControl.SelectedPassportType;

                IconGridLayout.Children.Clear();
                PickerStackLayout.Children.Clear();
                
                GenerateIconSelectionList(ItemSource);
                GenerateSelectionList(UnderItemSource, shouldShowEUAlert || _shouldNotMarkSelectedFamilyMember);

                OnPropertyChanged(nameof(IsDKSelected));
                OnPropertyChanged(nameof(ShouldShowSeparatorAndList));
                OnPropertyChanged(nameof(IconAndNameSeparator));
                OnPropertyChanged(nameof(PickerStackLayout));
                OnPropertyChanged(nameof(IconGridLayout));

                if (shouldShowEUAlert)
                {
                    OnEUPassportTypeChangeSpecificAction?.Invoke();
                    _shouldNotMarkSelectedFamilyMember = true;
                }
            }
            PreviousSelected = ItemSource.FirstOrDefault(x => x.IsSelected);
        }

        private async void UnderItemPickedAction(ISelection obj)
        {
            await PopupNavigation.Instance.PopAsync();
            OnUnderItemPickedAction?.Invoke(obj);
            if (PreviousSelected.SelectedPassportType != PassportType.UNIVERSAL_EU)
            {
                OnEUPassportTypeChangeSpecificAction?.Invoke();
            }
            
        }
        
        protected override bool OnBackButtonPressed()
        {
            // Overriden back button. Returning true means we do not want default behaviour.
            // App should behave the same as clicking the X button.
            ClosePicker(null, null);
            return true;
        }

        ~CustomPicker()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND);
        }
    }
}
