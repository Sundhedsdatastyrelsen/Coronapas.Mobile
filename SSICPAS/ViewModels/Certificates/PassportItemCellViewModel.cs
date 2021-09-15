using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Certificates;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SSICPAS.Services.Translator;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportItemCellViewModel : BaseViewModel, IComparable<PassportItemCellViewModel>
    {
        public enum PassportItemType
        {
            Vaccine,
            Test,
            Recovery
        }

        private static readonly IDateTimeService _dateTimeService = IoCContainer.Resolve<IDateTimeService>();
        private static int _isCommandInProgress; // 0 = false, other values = true. (original C convention) 
        private DateTime _date;
        private string _doseLabelText;
        private string _headerLabelText;
        private string _durationLabelFirstSpanText;
        private string _durationLabelSecondSpanText;
        private string _iconImageSource;
        private FamilyPassportItemsViewModel _passportItemsViewModel;
        private SinglePassportViewModel _selectedPassportViewModel;
        private string _subHeaderLabelText;
        private ICommand _tapCommand;
        

        public PassportItemType Type { get; set; }

        public string HeaderLabelText
        {
            get => _headerLabelText;
            set
            {
                _headerLabelText = value;
                RaisePropertyChanged(() => HeaderLabelText);
            }
        }

        public string SubHeaderLabelText
        {
            get => _subHeaderLabelText;
            set
            {
                _subHeaderLabelText = value;
                RaisePropertyChanged(() => SubHeaderLabelText);
            }
        }

        public string DurationLabelFirstSpanText
        {
            get => _durationLabelFirstSpanText;
            set
            {
                _durationLabelFirstSpanText = value;
                RaisePropertyChanged(() => DurationLabelFirstSpanText);
            }
        }

        public string DurationLabelSecondSpanText
        {
            get => _durationLabelSecondSpanText;
            set
            {
                _durationLabelSecondSpanText = value;
                RaisePropertyChanged(() => DurationLabelSecondSpanText);
            }
        }

        public string DoseLabelText
        {
            get => _doseLabelText;
            set
            {
                _doseLabelText = value;
                RaisePropertyChanged(() => DoseLabelText);
            }
        }

        public string AccessibilityText
        {
            get => _headerLabelText + _subHeaderLabelText + _durationLabelFirstSpanText + _durationLabelSecondSpanText;
        }

        public string IconImageSource
        {
            get => _iconImageSource;
            set
            {
                _iconImageSource = value;
                RaisePropertyChanged(() => IconImageSource);
            }
        }

        public ICommand TapCommand
        {
            get => _tapCommand;
            set
            {
                _tapCommand = value;
                RaisePropertyChanged(() => TapCommand);
            }
        }

        public DateTime Date
        {
            get => _date;
            set => _date = value;
        }

        static PassportItemCellViewModel() => _isCommandInProgress = 0;

        public int CompareTo(PassportItemCellViewModel compareCell)
        {
            return compareCell == null ? 1 : Date.CompareTo(compareCell.Date);
        }

        public static void TimeSpanToDateParts (DateTime d1, DateTime d2, out int years, out int months, out int days)
        {
            if (d1 < d2)
            {
                var d3 = d2;
                d2 = d1;
                d1 = d3;
            }
            var span = d1 - d2;
            months = 12 * (d1.Year - d2.Year) + (d1.Month - d2.Month);
            if (d1.CompareTo(d2.AddMonths(months).AddMilliseconds(-500)) <= 0)

            {
                --months;
            }
            years = months / 12;
            months -= years * 12;
            if (months == 0 && years == 0)
            {
                days = span.Days;
            }
            else
            {
                var md1 = new DateTime(d1.Year, d1.Month, d1.Day);
                var md2 = new DateTime(d2.Year, d2.Month, d2.Day);               
                {
                    days = (int)(md1.AddMonths(-months) - md2).TotalDays;
                }
            }
        }
            public static PassportItemCellViewModel CreateVaccineItemCellViewModel(
            SinglePassportViewModel passportViewModel, FamilyPassportItemsViewModel passportItemsViewModel)
        {
            
            DateTime vaccinationDate = passportViewModel.PassportData.VaccinationDate.Value;
            TimeSpan timeSinceVaccination = _dateTimeService.Now - vaccinationDate;

            int days = 365;
            int months = 12;
            int years = 20;
            TimeSpanToDateParts(vaccinationDate, _dateTimeService.Now, out years, out months, out days);

            int monthsSinceVaccination = (months);
            int daysSinceVaccination = (days);

            string durationString = (monthsSinceVaccination > 0 ? $"{monthsSinceVaccination} " +
                    (monthsSinceVaccination == 1 ? "INTERNATIONAL_RECOVERY_MONTH_TEXT".Translate() + " ": "INTERNATIONAL_RECOVERY_MONTHS_TEXT".Translate() + " ") : "")
                + $"{daysSinceVaccination} " + (daysSinceVaccination == 1 ? "INTERNATIONAL_RECOVERY_DAY_TEXT".Translate() : "INTERNATIONAL_RECOVERY_DAYS_TEXT".Translate())
                + " ";

            return new PassportItemCellViewModel
            {
                _selectedPassportViewModel = passportViewModel,
                _passportItemsViewModel = passportItemsViewModel,
                Type = PassportItemType.Vaccine,
                HeaderLabelText = "INTERNATIONAL_INFO_VACCINE_HEADER_TEXT".Translate(),
                SubHeaderLabelText = passportViewModel.PassportData.MarketingAuthorizationHolder,
                DurationLabelFirstSpanText = durationString,
                DurationLabelSecondSpanText = "INTERNATIONAL_INFO_VACCINE_TIME_SINCE_VACCINATION_TEXT".Translate(),
                DoseLabelText = $"{passportViewModel.PassportData.DoseNumber} {"INTERNATIONAL_INFO_VACCINE_DOSE_OF_TEXT".Translate()} {passportViewModel.PassportData.TotalNumberOfDose}",
                IconImageSource = (string)Application.Current.Resources["Covid19VaccineIcon"],
                TapCommand = CreateInterlockedCommand(async () =>
                {
                    await Device.InvokeOnMainThreadAsync(async () => await _navigationService.PushPage(
                        new PassportInfoModalView(passportItemsViewModel, EuPassportType.VACCINE, passportViewModel),
                        true,
                        PageNavigationStyle.PushModallySheetPageIOS));
                })
            };
        }



        public static PassportItemCellViewModel CreateTestItemCellViewModel(SinglePassportViewModel passportViewModel,
            FamilyPassportItemsViewModel passportItemsViewModel)
        {
            DateTime testDate = passportViewModel.PassportData.SampleCollectedTime.Value;
            TimeSpan timeSinceTest = _dateTimeService.Now - testDate;

            string durationString = " " + (timeSinceTest.TotalHours > 0 ? $"{(int)timeSinceTest.TotalHours} {"INTERNATIONAL_SAMPLE_CONDUCTED_TIME_HOURS_TEXT".Translate()} " : "")
                + $"{timeSinceTest.Minutes} {"INTERNATIONAL_SAMPLE_CONDUCTED_TIME_MINUTES_TEXT".Translate()}" + " ";
            
            return new PassportItemCellViewModel
            {
                
                _selectedPassportViewModel = passportViewModel,
                _passportItemsViewModel = passportItemsViewModel,
                Type = PassportItemType.Test,
                HeaderLabelText = "INTERNATIONAL_NEGATIVE_TEST_HEADER_TEXT".Translate(),
                SubHeaderLabelText = DCCValueSetTranslator.ToLocale(passportViewModel.PassportData.TypeOfTest, DCCValueSetEnum.TypeOfTest, true) ,
                DurationLabelFirstSpanText = durationString,
                DurationLabelSecondSpanText = "INTERNATIONAL_NEGATIVE_TEST_TIME_SINCE_TEST_TEXT".Translate(),
                DoseLabelText = "",
                IconImageSource = (string)Application.Current.Resources["Covid19NegativeTestIcon"],
                TapCommand = CreateInterlockedCommand(async () =>
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await _navigationService.PushPage(
                            new PassportInfoModalView(passportItemsViewModel, EuPassportType.TEST, passportViewModel),
                            true,
                            PageNavigationStyle.PushModallySheetPageIOS);
                    });
                })
            };
        }

        public static PassportItemCellViewModel CreateRecoveryItemCellViewModel(
            SinglePassportViewModel passportViewModel, FamilyPassportItemsViewModel passportItemsViewModel)
        {
            string durationString = $" {passportViewModel.PassportData.RecoveryValidTo.Value.ToLocaleDateFormat(true)}";

            return new PassportItemCellViewModel
            {
                _selectedPassportViewModel = passportViewModel,
                _passportItemsViewModel = passportItemsViewModel,
                Type = PassportItemType.Recovery,
                HeaderLabelText = "INTERNATIONAL_RECOVERY_HEADER_TEXT".Translate(),
                SubHeaderLabelText = "INTERNATIONAL_INFO_CERTIFICATE_PASSPORT_HEADER_TEXT".Translate(),
                DurationLabelFirstSpanText = "INTERNATIONAL_RECOVERY_VALID_UNTIL_TEXT".Translate(),
                DurationLabelSecondSpanText = durationString,
                DoseLabelText = "",
                IconImageSource = (string)Application.Current.Resources["Covid19RecoveryIcon"],
                TapCommand = CreateInterlockedCommand(async () =>
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await _navigationService.PushPage(
                            new PassportInfoModalView(passportItemsViewModel, EuPassportType.RECOVERY, passportViewModel),
                            true,
                            PageNavigationStyle.PushModallySheetPageIOS);
                    });
                })
            };
        }

        private static ICommand CreateInterlockedCommand(Func<Task> func) => new Command(async () =>
        {
            if (Interlocked.CompareExchange(ref _isCommandInProgress, 1, 0) != 0)
                return;

            try
            {
                await Task.Run(func);
            }
            finally
            {
                Interlocked.Exchange(ref _isCommandInProgress, 0);
            }
        });
    }
}