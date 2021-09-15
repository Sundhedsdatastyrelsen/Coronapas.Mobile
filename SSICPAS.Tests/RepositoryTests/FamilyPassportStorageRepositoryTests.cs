using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Repositories;
using SSICPAS.Tests.TestMocks;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Certificates;
using static SSICPAS.Data.SecureStorageKeys;

namespace SSICPAS.Tests.RepositoryTests
{
    public class FamilyPassportStorageRepositoryTests
    {
        [SetUp]
        public void SetUp()
        {
            IoCContainer.RegisterSingleton<IDateTimeService, MockDateTimeService>();
            ((MockDateTimeService)IoCContainer.Resolve<IDateTimeService>()).Now = new DateTime(2021,06,14);
            
            IoCContainer
                .RegisterSingleton<ISecureStorageService<AdditionalDataViewModel>,
                    MockSecureStorageService<AdditionalDataViewModel>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<EUPassportsViewModel>,
                    MockSecureStorageService<EUPassportsViewModel>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<DKPassportsViewModel>,
                    MockSecureStorageService<DKPassportsViewModel>>();
            IoCContainer.RegisterSingleton<IFamilyPassportStorageRepository, FamilyPassportStorageRepository>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<PassportItemsViewModel>,
                    MockSecureStorageServiceThrowingOnTooLargeValue<PassportItemsViewModel>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<string>,
                    MockSecureStorageService<string>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<int>,
                    MockSecureStorageService<int>>();
            IoCContainer.RegisterSingleton<IPassportStorageRepository, PassportStorageRepository>();
        }

        [Test]
        public async Task SaveFamilyPassportStorage_ShouldHaveCorrectData()
        {
            // Given
            IFamilyPassportStorageRepository storage = IoCContainer.Resolve<IFamilyPassportStorageRepository>();
            await storage.DeleteFamilyPassportFromSecureStorage();
            
            var additionalStorage = IoCContainer.Resolve<ISecureStorageService<AdditionalDataViewModel>>();
            var euStorage = IoCContainer.Resolve<ISecureStorageService<EUPassportsViewModel>>();
            var dkStorage = IoCContainer.Resolve<ISecureStorageService<DKPassportsViewModel>>();

            var familyData = new List<EUPassportsViewModel>
            {
                // Parent
                new EUPassportsViewModel
                {
                    CustodyKey = "parent",
                    EuRecoveryPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier3"
                            }
                        }
                    }
                },
                // Child
                new EUPassportsViewModel
                {
                    CustodyKey = "Child-1",
                    EuRecoveryPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier4"
                            }
                        }
                    }
                }
            };
            var myData = new EUPassportsViewModel
            {
                EuRecoveryPassports = new List<SinglePassportViewModel>
                {
                    new SinglePassportViewModel
                    {
                        PassportData = new PassportData
                        {
                            CertificateIdentifier = "CertificateIdentifier2"
                        }
                    }
                }
            };
            var dkData = new DKPassportsViewModel
            {
                DanishPassportNoInfo = new SinglePassportViewModel
                {
                    PassportData = new PassportData
                    {
                        CertificateIdentifier = "CertificateIdentifier1"
                    }
                },
                DanishPassportWithInfo = new SinglePassportViewModel
                {
                    PassportData = new PassportData
                    {
                        CertificateIdentifier = "CertificateIdentifier1_1"
                    }
                }
            };
            var additionalData = new AdditionalDataViewModel
            {
                JobId = "JobId",
                LanguageSelection = LanguageSelection.Danish,
                JobStatus = PassportJobStatus.Done
            };
            var family = new FamilyPassportItemsViewModel
            {
                AdditionalData = additionalData,
                DkData = dkData,
                SelectedPassportType = PassportType.DK_FULL,
                FamilyData = familyData
            };

            // When 1 - Saving data
            await storage.SaveFamilyPassportToSecureStorage(family);

            // Then 1
            Assert.True(await additionalStorage.TryHasValue(PASSPORT_DATA.AdditionalData()));
            Assert.True(await euStorage.TryHasValue(PASSPORT_DATA.Child(0)));
            Assert.True(await euStorage.TryHasValue(PASSPORT_DATA.ParentEU()));
            Assert.True(await dkStorage.TryHasValue(PASSPORT_DATA.ParentDK()));

            // When 2 - Reading directly from storage
            var additionalDataViewModel =
                await additionalStorage.TryGetSecureStorageAsync(PASSPORT_DATA.AdditionalData());
            var parentDataViewModel = await euStorage.TryGetSecureStorageAsync(PASSPORT_DATA.ParentEU());
            var childDataViewModel = await euStorage.TryGetSecureStorageAsync(PASSPORT_DATA.Child(0));

            // Then 2
            Assert.That(additionalDataViewModel.JobId, Is.EqualTo("JobId"));
            Assert.That(additionalDataViewModel.JobStatus, Is.EqualTo(PassportJobStatus.Done));
            Assert.That(additionalDataViewModel.LanguageSelection, Is.EqualTo(LanguageSelection.Danish));
            Assert.That(parentDataViewModel.EuRecoveryPassports[0].PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier3"));
            Assert.That(childDataViewModel.EuRecoveryPassports[0].PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier4"));
            Assert.That(parentDataViewModel.CustodyKey, Is.EqualTo("parent"));
            Assert.That(childDataViewModel.CustodyKey, Is.EqualTo("Child-1"));

            // When 3 - Getting data with repository method
            var familyPassportFromSecureStorage = await storage.GetFamilyPassportFromSecureStorage();

            //Then
            Assert.That(familyPassportFromSecureStorage.AdditionalData.JobId, Is.EqualTo("JobId"));
            Assert.That(familyPassportFromSecureStorage.AdditionalData.JobStatus, Is.EqualTo(PassportJobStatus.Done));
            Assert.That(familyPassportFromSecureStorage.AdditionalData.LanguageSelection,
                Is.EqualTo(LanguageSelection.Danish));
            Assert.That(
                familyPassportFromSecureStorage.DkData.DanishPassportNoInfo.PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier1"));
            Assert.That(
                familyPassportFromSecureStorage.DkData.DanishPassportWithInfo.PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier1_1"));
            Assert.That(
                familyPassportFromSecureStorage.FamilyData[0].EuRecoveryPassports[0].PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier3"));
            Assert.That(
                familyPassportFromSecureStorage.FamilyData[1].EuRecoveryPassports[0].PassportData.CertificateIdentifier,
                Is.EqualTo("CertificateIdentifier4"));

            // When 4 - Data deletion
            await storage.DeleteFamilyPassportFromSecureStorage();

            // Then 4
            Assert.False(await additionalStorage.TryHasValue(PASSPORT_DATA.AdditionalData()));
            Assert.False(await euStorage.TryHasValue(PASSPORT_DATA.Child(0)));
            Assert.False(await euStorage.TryHasValue(PASSPORT_DATA.ParentEU()));
            Assert.False(await dkStorage.TryHasValue(PASSPORT_DATA.ParentDK()));
        }

        [Test]
        public async Task Migrate_ShouldHaveCorrectData()
        {
            // Given
            var storage =
                IoCContainer.Resolve<ISecureStorageService<PassportItemsViewModel>>();
            var oldPassport = IoCContainer.Resolve<IPassportStorageRepository>();
            
            var familyPassport = IoCContainer.Resolve<IFamilyPassportStorageRepository>();
            var passportItemsViewModel = new PassportItemsViewModel
            {
                DKPassportsViewModel = new DKPassportsViewModel
                {
                    DanishPassportNoInfo = new SinglePassportViewModel
                    {
                        PassportData = new PassportData
                        {
                            CertificateIdentifier = "CertificateIdentifier1"
                        }
                    },
                    DanishPassportWithInfo = new SinglePassportViewModel
                    {
                        PassportData = new PassportData
                        {
                            CertificateIdentifier = "CertificateIdentifier2"
                        }
                    }
                },

                MyPageViewModel = new EUPassportsViewModel
                {
                    EuRecoveryPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier3"
                            }
                        }
                    },
                    EuTestPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier4"
                            }
                        }
                    },
                    EuVaccinePassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier5"
                            }
                        }
                    }
                },

                EUPassportsViewModel = new EUPassportsViewModel
                {
                    EuRecoveryPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier6"
                            }
                        }
                    },
                    EuTestPassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier7"
                            }
                        }
                    },
                    EuVaccinePassports = new List<SinglePassportViewModel>
                    {
                        new SinglePassportViewModel
                        {
                            PassportData = new PassportData
                            {
                                CertificateIdentifier = "CertificateIdentifier8"
                            }
                        }
                    }
                },
                LanguageSelection = LanguageSelection.English,
                JobId = "JobId",
                JobStatus = PassportJobStatus.Done
            };

            // Then 0 - Check there is nothing in the storage
            Assert.False(await storage.TryHasValue(PASSPORT_DATA));

            // When 1 - Write old data into storage
            await oldPassport.SavePassportToSecureStorage(passportItemsViewModel);
            // Then 1 - Check there are data in old format in storage
            Assert.True(await storage.TryHasValue(PASSPORT_DATA));

            // When 2 - Migrate old data into new storage and new format
            await familyPassport.MigrateData();

            // Then 2 - Check there is no data in the old storage
            Assert.False(await storage.TryHasValue(PASSPORT_DATA));

            // When 3 - Get data from the new storage
            var family = await familyPassport.GetFamilyPassportFromSecureStorage();

            // Then 3 - Check the data in the new storage are the same as in old object
            Assert.AreEqual(family.AdditionalData.JobId, passportItemsViewModel.JobId);
            Assert.AreEqual(family.AdditionalData.JobStatus, passportItemsViewModel.JobStatus);
            Assert.AreEqual(family.AdditionalData.LanguageSelection, passportItemsViewModel.LanguageSelection);
            Assert.AreEqual(family.DkData.DanishPassportNoInfo.PassportData.CertificateIdentifier,
                passportItemsViewModel.DKPassportsViewModel.DanishPassportNoInfo.PassportData.CertificateIdentifier);
            Assert.AreEqual(family.DkData.DanishPassportWithInfo.PassportData.CertificateIdentifier,
                passportItemsViewModel.DKPassportsViewModel.DanishPassportWithInfo.PassportData.CertificateIdentifier);

            Assert.AreEqual(family.FamilyData[0].EuRecoveryPassports[0].PassportData.CertificateIdentifier,
                passportItemsViewModel.EUPassportsViewModel.EuRecoveryPassports[0].PassportData.CertificateIdentifier);
            Assert.AreEqual(family.FamilyData[0].EuTestPassports[0].PassportData.CertificateIdentifier,
                passportItemsViewModel.EUPassportsViewModel.EuTestPassports[0].PassportData.CertificateIdentifier);
            Assert.AreEqual(family.FamilyData[0].EuVaccinePassports[0].PassportData.CertificateIdentifier,
                passportItemsViewModel.EUPassportsViewModel.EuVaccinePassports[0].PassportData.CertificateIdentifier);
        }
    }
}