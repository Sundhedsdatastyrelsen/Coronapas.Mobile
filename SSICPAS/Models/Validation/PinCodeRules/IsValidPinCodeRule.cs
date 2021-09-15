using System;

namespace SSICPAS.Models.Validation.PinCodeRules
{
    public class IsValidPinCodeRule<T>: IValidationRule<T>
    {
        public IsValidPinCodeRule()
        {
            ValidationMessage = string.Empty;
        }

        public string ValidationMessage { get; set; }

        public static string ValidationSequence = "Sequence";
        public static string ValidationConsecutive = "Consecutive";

        public bool Check(T value)
        {
            bool isValid = false;
            try
            {
                char[] charArray = value.ToString().ToCharArray();
                int fValue = Convert.ToInt32(charArray[0]);
                int lValue = Convert.ToInt32(charArray[3]);
                if (fValue > lValue)
                {
                    Array.Reverse(charArray);
                }
                // Check if a number is sequence number
                if (!(charArray[0] == charArray[1] &&
                      charArray[1] == charArray[2] &&
                      charArray[2] == charArray[3]))
                {
                    isValid = true;
                }
                ValidationMessage = ValidationSequence;
                // Check if a number is consecutive
                if (isValid)
                {
                    int firstValue = Convert.ToInt32(charArray[0]);
                    for (int i = 0; i < charArray.Length; i++)
                    {
                        if (Convert.ToInt32(charArray[i]) - i != firstValue)
                        {
                            isValid = true;
                            break;
                        }
                        else
                        {
                            isValid = false;
                            ValidationMessage = ValidationConsecutive;
                        }
                    }

                }
            } catch
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
