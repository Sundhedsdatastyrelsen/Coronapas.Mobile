using System.Collections.Generic;
using System.Linq;
using SSICPAS.ViewModels.Base;

namespace SSICPAS.Models.Validation
{
    public class ValidatableObject<T> : BaseBindableObject
    {
        private readonly List<IValidationRule<T>> _validationRules;
        private List<string> _errors;
        private T _value;
        private bool _isValid;

        public List<IValidationRule<T>> ValidationRules => _validationRules;
        
        public List<string> Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
            }
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }
        
        public ValidatableObject()
        {
            _isValid = true;
            _errors = new List<string>();
            _validationRules = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            var errors = _validationRules.Where(v => !v.Check(Value)).Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return IsValid;
        }
    }
}