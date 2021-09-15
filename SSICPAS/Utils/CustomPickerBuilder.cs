using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using SSICPAS.ViewModels;
using SSICPAS.Views.Elements;

namespace SSICPAS.Utils
{
    class CustomPickerBuilder
    {
        private IEnumerable<SelectionControl> _itemSource;
        private Action<ISelection> _onItemPickedAction;
        private string _title;
        IEnumerable<SelectionControl> _underItemSource;
        private Action<ISelection> _onUnderItemPickedAction;
        private Action _onEUPassportTypeChangeSpecificAction;

        public static CustomPickerBuilder Builder()
        {
            return new CustomPickerBuilder();
        }

        public CustomPickerBuilder WithItemSource(IEnumerable<SelectionControl> itemSource)
        {
            _itemSource = itemSource;
            return this;
        }

        public CustomPickerBuilder WithOnItemPickedAction(Action<ISelection> onItemPickedAction)
        {
            _onItemPickedAction = onItemPickedAction;
            return this;
        }

        public CustomPickerBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public CustomPickerBuilder WithUnderItemSource(IEnumerable<SelectionControl> underItemSource)
        {
            _underItemSource = underItemSource;
            return this;
        }

        public CustomPickerBuilder WithOnUnderItemPickedAction(Action<ISelection> onUnderItemPickedAction)
        {
            _onUnderItemPickedAction = onUnderItemPickedAction;
            return this;
        }

        public CustomPickerBuilder WithEUPassportTypeChangeSpecificAction(Action onEUPassportTypeChangeSpecificAction)
        {
            _onEUPassportTypeChangeSpecificAction = onEUPassportTypeChangeSpecificAction;
            return this;
        }

        public CustomPicker Build()
        {
            return new CustomPicker(
                _itemSource,
                _onItemPickedAction,
                _title,
                _underItemSource,
                _onUnderItemPickedAction,
                _onEUPassportTypeChangeSpecificAction);
        }

        public async Task Show()
        {
            await PopupNavigation.Instance.PushAsync(Build());
        }
    }
}