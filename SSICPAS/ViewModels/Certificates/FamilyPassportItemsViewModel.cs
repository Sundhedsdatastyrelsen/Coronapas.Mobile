using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.DK;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Models.Dtos;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;

namespace SSICPAS.ViewModels.Certificates
{
    public class FamilyPassportItemsViewModel
    {
        [JsonIgnore] private static int _selectedFamilyMember;
        [JsonIgnore] private static KeyValuePair<int, string>? _previousSelectedFamilyMember = null;
        public AdditionalDataViewModel AdditionalData { get; set; } = new AdditionalDataViewModel();
        public DKPassportsViewModel DkData { get; set; } = new DKPassportsViewModel();
        // The first item in the FamilyData list is always a Parent
        public List<EUPassportsViewModel> FamilyData { get; set; } = new List<EUPassportsViewModel>();

        [JsonIgnore] public PassportType SelectedPassportType { get; set; } = PassportType.DK_LIMITED;

        [JsonIgnore]
        public bool HasJobIDInProgress =>
            !string.IsNullOrEmpty(AdditionalData.JobId) && AdditionalData.JobStatus == PassportJobStatus.Inprogress;

        [JsonIgnore]
        public int SelectedFamilyMember
        {
            get => _selectedFamilyMember > FamilyData.Count - 1 ? 0 : _selectedFamilyMember;
            set
            {
                Interlocked.Exchange(ref _selectedFamilyMember, value);
                if (value < FamilyMembersNames.Count)
                {
                    _previousSelectedFamilyMember = new KeyValuePair<int, string>(value, FamilyMembersNames[value]);
                }
            }
        }

        public void AdjustSelectedFamilyMemberAfterFetch()
        {
            if (_previousSelectedFamilyMember != null)
            {
                int index =
                    FamilyMembersNames.FindIndex(name => name == _previousSelectedFamilyMember.Value.Value);
                if (index >= 0 && index < FamilyData.Count)
                {
                    var member = FamilyMembersNames[index];
                    SelectedFamilyMember = index;
                    return;
                }
            }

            SelectedFamilyMember = 0;
        }
        
        [JsonIgnore]
        public EUPassportsViewModel SelectedFamilyMemberPassport =>
            FamilyData.Any() ? FamilyData[SelectedFamilyMember] : new EUPassportsViewModel();

        [JsonIgnore]
        public List<string> FamilyMembersNames
        {
            get
            {
                if (FamilyData.Any() && FamilyData?[0]?.DkInfo != null)
                {
                    return FamilyMembersMandatoryInfoList.Select(member => member.FullName)
                        .ToList();
                }

                return ParentName == null ?
                    new List<string>() :
                    new List<string> { ParentName };
            }
        }

        [JsonIgnore]
        public string? ParentName =>
            FamilyData.Any() ? 
                ResolveFullName : 
                DkData?.DanishPassportWithInfo?.FullName ?? DkData?.DanishPassportNoInfo?.FullName;

        [JsonIgnore]
        private string ResolveFullName =>
            FamilyData[0]?.DkInfo?.FullName ??
            FamilyData[0]?.EuTestPassports
                .FirstOrDefault(pass => pass.FullName != null)?.FullName ??
            FamilyData[0]?.EuVaccinePassports
                .FirstOrDefault(pass => pass.FullName != null)?.FullName ??
            FamilyData[0]?.EuRecoveryPassports
                .FirstOrDefault(pass => pass.FullName != null)?.FullName ??
            DkData?.DanishPassportWithInfo?.FullName ??
            DkData?.DanishPassportNoInfo?.FullName;
        
        [JsonIgnore]
        public SinglePassportViewModel? SelectedMandatoryInfo =>
            SelectedFamilyMember < FamilyData.Count ? FamilyData[SelectedFamilyMember]?.DkInfo : null;

        [JsonIgnore]
        public List<SinglePassportViewModel> FamilyMembersMandatoryInfoList
        {
            get
            {
                if (FamilyData.Any() && FamilyData?[0]?.DkInfo != null)
                {
                    return FamilyData.Select(member => member.DkInfo)
                        .ToList();
                }
                
                return new List<SinglePassportViewModel> { DkData.DanishPassportWithInfo };
            }
        }
            

        [JsonIgnore]
        public Dictionary<SinglePassportViewModel, string> FamilyMembersCustodyDictionary =>
            FamilyMembersMandatoryInfoList
                .Zip(FamilyData.Select(
                        member => member.CustodyKey),
                    (model, s) => new {model, s})
                .ToDictionary(x => x.model, x => x.s);

        [JsonIgnore]
        public SinglePassportViewModel FamilyMembersGetParent =>
            FamilyMembersCustodyDictionary
                .FirstOrDefault(member => member.Value.ToLower() == "parent")
                .Key;
        
        [JsonIgnore]
        public List<SinglePassportViewModel> FamilyMembersGetChildrenList =>
            FamilyMembersCustodyDictionary
                .Where(pair => pair.Value.ToLower() != "parent")
                .Select(member => member.Key)
                .ToList();
        
        [JsonIgnore]
        public SinglePassportViewModel SelectedPassport
        {
            get
            {
                switch (SelectedPassportType, SelectedFamilyMember)
                {
                    case (PassportType.DK_FULL, _):
                        return DkData.DanishPassportWithInfo;
                    case (PassportType.DK_LIMITED, _):
                        return DkData.DanishPassportNoInfo;
                    case (PassportType.UNIVERSAL_EU, _):
                        return SelectedFamilyMemberPassport.EuTestPassports.FirstOrDefault()
                               ?? SelectedFamilyMemberPassport.EuRecoveryPassports.FirstOrDefault()
                               ?? SelectedFamilyMemberPassport.EuVaccinePassports
                                   .FirstOrDefault();
                    default:
                        return DkData.DanishPassportNoInfo;
                }
            }
        }

        [JsonIgnore] public bool IsDKPassportValid => DkData.AreAllPassportsValid;

        [JsonIgnore]
        public bool IsAnyEuPassportValid =>
            FamilyData.Any(pass => pass.IsAnyPassportValid);

        [JsonIgnore]
        public bool IsAllAvailablePassportValid =>
            DkData.AreAllAvailablePassportsValid &&
            IsAnyEuPassportValid;

        [JsonIgnore]
        public bool IsAnyPassportValid =>
            DkData.IsAnyPassportValid ||
            IsAnyEuPassportValid;

        [JsonIgnore]
        public bool IsPassportValid => SelectedPassportType == PassportType.UNIVERSAL_EU
            ? IsAnyEuPassportValid
            : SelectedPassport?.IsValid ?? true;

        [JsonIgnore]
        public bool ShouldPrefetchNewPassport =>
            DkData.ShouldFetchNewPassport ||
            FamilyData.Any(pass => pass.ShouldFetchNewPassport);

        [JsonIgnore]
        public bool IsAnyPassportAvailable =>
            DkData.IsAnyPassportAvailable ||
            IsAnyEuPassportAvailable;

        [JsonIgnore]
        public bool IsMoreThanOneEuPassportAvailable =>
            SelectedFamilyMemberPassport.IsMoreThanOneEuPassportAvailable;

        [JsonIgnore]
        public bool IsAnyEuPassportAvailable =>
            FamilyData.Any(pass => pass.IsAnyPassportAvailable);

        [JsonIgnore]
        public bool IsAnyEuPassportForSelectedMemberAvailable =>
            SelectedFamilyMemberPassport?.IsAnyPassportAvailable ?? false;

        [JsonIgnore]
        public bool IsEuVaccinePassportForSelectedMemberAvailable =>
            SelectedFamilyMemberPassport?.IsAnyVaccinePassportAvailable ?? false;

        [JsonIgnore]
        public bool IsEuTestPassportForSelectedMemberAvailable =>
            SelectedFamilyMemberPassport?.IsAnyTestPassportAvailable ?? false;

        [JsonIgnore]
        public bool IsEuRecoveryPassportForSelectedMemberAvailable =>
            SelectedFamilyMemberPassport?.IsAnyRecoveryPassportAvailable ?? false;
        
        private async Task<PassportsViewModel> ProcessDKTokensToDKPassportsViewModel(
            GetPassportDto inputPassportDto)
        {
            ITokenProcessorService tokenProcessorServiceLocal = IoCContainer.Resolve<ITokenProcessorService>();
            DCCValueSetTranslator translator = new DCCValueSetTranslator(IoCContainer.Resolve<IRatListService>());
            DigitalCovidValueSetTestAndTestManufacturerNameTranslator ratListTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(IoCContainer.Resolve<IRatListService>());
            await tokenProcessorServiceLocal.SetDCCValueSetTranslator(translator, ratListTranslator);

            DKPassportsViewModel dKPassportsViewModel = new DKPassportsViewModel();

            Task<TokenValidateResultModel> decodeDkMin =
                tokenProcessorServiceLocal.DecodePassportTokenToModel(inputPassportDto.Dkmin ?? string.Empty);
            Task<TokenValidateResultModel> decodeDkMax =
                tokenProcessorServiceLocal.DecodePassportTokenToModel(inputPassportDto.Dkmax ?? string.Empty);

            await Task.WhenAll(
                decodeDkMin,
                decodeDkMax);

            if (decodeDkMin.Result.ValidationResult != TokenValidateResult.Invalid &&
                decodeDkMin.Result.DecodedModel != null)
            {
                var dkMinHcertModel = decodeDkMin.Result.DecodedModel;
                dKPassportsViewModel.DanishPassportNoInfo = new SinglePassportViewModel
                {
                    PassportData = new PassportData(inputPassportDto.Dkmin, (DK1Payload)dkMinHcertModel)
                };
            }

            if (decodeDkMax.Result.ValidationResult != TokenValidateResult.Invalid &&
                decodeDkMax.Result.DecodedModel != null)
            {
                var dkMaxHcertModel = decodeDkMax.Result.DecodedModel;

                dKPassportsViewModel.DanishPassportWithInfo = new SinglePassportViewModel
                {
                    PassportData = new PassportData(inputPassportDto.Dkmax, (DK2Payload)dkMaxHcertModel)
                };
            }

            return dKPassportsViewModel;
        }

        private List<Task<TokenValidateResultModel>> DecodePassportTokenToModelEuroRecovery(
            ITokenProcessorService euTokenProcessorService, IPassportDto dto)
        {
            return dto.EuroRecovery
                .Select(token => euTokenProcessorService.DecodePassportTokenToModel(token ?? string.Empty)).ToList();
        }

        private List<Task<TokenValidateResultModel>> DecodePassportTokenToModelEuroVaccine(
            ITokenProcessorService euTokenProcessorService, IPassportDto dto)
        {
            return dto.EuroVaccine
                .Select(token => euTokenProcessorService.DecodePassportTokenToModel(token ?? string.Empty)).ToList();
        }

        private List<Task<TokenValidateResultModel>> DecodePassportTokenToModelEuroTest(
            ITokenProcessorService euTokenProcessorService, IPassportDto dto)
        {
            return dto.EuroTest
                .Select(token => euTokenProcessorService.DecodePassportTokenToModel(token ?? string.Empty)).ToList();
        }

        private async Task<PassportsViewModel> ProcessEUTokensToEUPassportsViewModel(IPassportDto inputPassportDto)
        {
            ITokenProcessorService euTokenProcessorService = IoCContainer.Resolve<ITokenProcessorService>();
            DCCValueSetTranslator translator = new DCCValueSetTranslator(IoCContainer.Resolve<IRatListService>());
            DigitalCovidValueSetTestAndTestManufacturerNameTranslator ratListTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(IoCContainer.Resolve<IRatListService>());
            await euTokenProcessorService.SetDCCValueSetTranslator(translator, ratListTranslator);

            EUPassportsViewModel eUPassportsViewModel = new EUPassportsViewModel();

            Task<TokenValidateResultModel> decodeDkInfo =
                euTokenProcessorService.DecodePassportTokenToModel(inputPassportDto.DkInfo ?? string.Empty);

            await Task.WhenAll(decodeDkInfo);

            eUPassportsViewModel.DkInfo = DecodeAndCreateSinglePassportViewModels(
                    new List<Task<TokenValidateResultModel>> { decodeDkInfo },
                    new List<string> { inputPassportDto.DkInfo })
                .FirstOrDefault();

            List<Task<TokenValidateResultModel>> decodeEuVaccineTasks =
                DecodePassportTokenToModelEuroVaccine(euTokenProcessorService, inputPassportDto) ??
                new List<Task<TokenValidateResultModel>>();
            List<Task<TokenValidateResultModel>> decodeEuTestTasks =
                DecodePassportTokenToModelEuroTest(euTokenProcessorService, inputPassportDto) ??
                new List<Task<TokenValidateResultModel>>();
            List<Task<TokenValidateResultModel>> decodeEuRecoveryTasks =
                DecodePassportTokenToModelEuroRecovery(euTokenProcessorService, inputPassportDto) ??
                new List<Task<TokenValidateResultModel>>();

            IEnumerable<Task> decodingEuPassportTokensToModelsTasks =
                decodeEuVaccineTasks
                    .Concat(decodeEuTestTasks)
                    .Concat(decodeEuRecoveryTasks);

            await Task.WhenAll(decodingEuPassportTokensToModelsTasks);

            eUPassportsViewModel.EuVaccinePassports = DecodeAndCreateSinglePassportViewModels(
                decodeEuVaccineTasks,
                inputPassportDto.EuroVaccine);

            eUPassportsViewModel.EuTestPassports = DecodeAndCreateSinglePassportViewModels(
                decodeEuTestTasks,
                inputPassportDto.EuroTest);

            eUPassportsViewModel.EuRecoveryPassports = DecodeAndCreateSinglePassportViewModels(
                decodeEuRecoveryTasks,
                inputPassportDto.EuroRecovery);

            eUPassportsViewModel.CustodyKey = inputPassportDto.CustodyKey;
            
            return eUPassportsViewModel;
        }

        private List<SinglePassportViewModel> DecodeAndCreateSinglePassportViewModels(
            List<Task<TokenValidateResultModel>> decodeEuTasks,
            List<string> tokens)
        {
            List<SinglePassportViewModel> listOfSinglePassportViewModels = new List<SinglePassportViewModel>();

            IEnumerable<bool> mapDecodeTasksAndTokens =
                decodeEuTasks.Zip(tokens, (task, token) =>
                {
                    if (task.Result.ValidationResult != TokenValidateResult.Invalid &&
                        task.Result.DecodedModel != null)
                    {
                        listOfSinglePassportViewModels
                            .Add(
                                new SinglePassportViewModel
                                {
                                    PassportData = new PassportData(
                                        token,
                                        task.Result.DecodedModel,
                                        task.Result.DecodedJson)
                                });
                        return true;
                    }

                    return false;
                })
                .ToList();

            if (!mapDecodeTasksAndTokens.Any() && mapDecodeTasksAndTokens.Contains(false))
            {
                Debug.Print("Nothing or false inside completedDecodeTasksAndLocalPassportDTOsEuRecovery");
            }

            return listOfSinglePassportViewModels;
        }

        public async Task ProcessPassportToken(GetPassportDto passportDto)
        {
            AdditionalData.JobStatus = passportDto.Status;

            if (!string.IsNullOrEmpty(passportDto.JobId))
            {
                AdditionalData.JobId = passportDto.JobId;
                return;
            }

            AdditionalData.LanguageSelection = LocaleService.Current.GetLanguage();

            Task<PassportsViewModel> processDKTask = ProcessDKTokensToDKPassportsViewModel(passportDto);
            Task<PassportsViewModel> processEUTask = ProcessEUTokensToEUPassportsViewModel(passportDto);

            IEnumerable<Task<PassportsViewModel>> childrenEUTasks =
                passportDto.Children
                    .Select(child =>
                        ProcessEUTokensToEUPassportsViewModel(child));

            await Task.WhenAll(processDKTask, processEUTask);
            await Task.WhenAll(childrenEUTasks);

            DkData = (DKPassportsViewModel)processDKTask.Result;
            List<EUPassportsViewModel> familyList =
                childrenEUTasks.Select(task => (EUPassportsViewModel)task.Result)
                    .OrderBy(child =>
                        child.DkInfo?.FullName ??
                        child.EuRecoveryPassports.FirstOrDefault()?.FullName ??
                        child.EuTestPassports.FirstOrDefault()?.FullName ??
                        child.EuVaccinePassports.FirstOrDefault()?.FullName ??
                        child.CustodyKey)
                    .ToList();
            familyList.Insert(0, (EUPassportsViewModel)processEUTask.Result);
            FamilyData = familyList;

            IPreferencesService preferenceService = IoCContainer.Resolve<IPreferencesService>();
            if (!preferenceService.GetUserPreferenceAsBoolean(PreferencesKeys.EXIST_VALID_DK_PASSPORTS) &&
                DkData.DanishPassportNoInfo != null && DkData.DanishPassportWithInfo != null)
            {
                preferenceService.SetUserPreference(PreferencesKeys.EXIST_VALID_DK_PASSPORTS, true);
            }
        }
    }
}