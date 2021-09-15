using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSICPAS.Core.CustomExceptions;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.CoseModel;
using SSICPAS.Core.Services.Model.DK;
using SSICPAS.Core.Services.Model.EuDCCModel;
using SSICPAS.Core.Services.Utils;

namespace SSICPAS.Core.Services.DecoderService
{
    public class HcertTokenProcessorService: ITokenProcessorService
    {
        private readonly ICertificationService _certificationService;
        private readonly ILoggingService _loggingService;
        private readonly IDateTimeService _dateTimeService;
        
        private IDCCValueSetTranslator _translator;
        private IDCCValueSetTranslator _testManufacturerTranslator;
        // provide new Contract to cache the custom Converters initialize with the curent translator
        private CustomContract _contract = new CustomContract();

        public HcertTokenProcessorService(
            ICertificationService certificationService, 
            ILoggingService loggingService,
            IDateTimeService dateTimeService)
        {
            _certificationService = certificationService;
            _loggingService = loggingService;
            _dateTimeService = dateTimeService;
        }

        public async Task SetDCCValueSetTranslator(IDCCValueSetTranslator translator, IDCCValueSetTranslator testmanufacturerTranslator)
        {
            _translator = translator;
            DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator = _translator;
            await DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator.InitValueSetAsync();

            _testManufacturerTranslator = testmanufacturerTranslator;
            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator = _testManufacturerTranslator;
            await DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator.InitValueSetAsync();
        }
        

        public async Task<TokenValidateResultModel> DecodePassportTokenToModel(string base45String)
        {
            TokenValidateResultModel resultModel = new TokenValidateResultModel();
            try
            {
                if (string.IsNullOrEmpty(base45String))
                {
                    return resultModel;
                }
                //Decode token to a cose sign 1 object
                CoseSign1Object coseSign1Object = DecodeToCOSEFlow(base45String);
#if !UNITTESTS && !DEBUG
                await _certificationService.VerifyCoseSign1Object(coseSign1Object);
#endif
                string jsonStringFromBytes = coseSign1Object.GetJson();

                Debug.Print($"{nameof(HcertTokenProcessorService)}.{nameof(DecodePassportTokenToModel)}: Decoded JSON string \n" +
                    $"{jsonStringFromBytes}");

                ITokenPayload decodedModel = MapToModelFromJson(jsonStringFromBytes, base45String.Substring(0, 3));
                DateTime? expiration = decodedModel.ExpiredDateTime();
                if (expiration != null)
                {
                    if (_dateTimeService.Now.CompareTo(expiration) >= 0)
                    {
                        resultModel.ValidationResult = TokenValidateResult.Expired;
                    }
                    else
                    {
                        resultModel.ValidationResult = TokenValidateResult.Valid;
                    }
                }
                else
                {
                    //Assume the token are invalid if no expiration date found
                    resultModel.ValidationResult = TokenValidateResult.Invalid;
                }

                resultModel.DecodedModel = decodedModel;
                resultModel.DecodedJson = jsonStringFromBytes;
                return resultModel;
            }
            catch (TokenDecodeException tokenDecodeException)
            {
#if APPSTORE
                _loggingService.LogException(LogSeverity.WARNING, tokenDecodeException);
#else
                _loggingService.LogException(LogSeverity.ERROR, tokenDecodeException);
#endif

                // If any exceptions are throw, assume it invalid
                return resultModel;
            }
            catch (Exception e)
            {
                Debug.Print($"Cannot decode or verify token with prefix {base45String.Substring(0, 3)}");
#if !APPSTORE && !APPSTOREBETA
                _loggingService.LogException(LogSeverity.WARNING, e);
#endif
                // If any exceptions are throw, assume it invalid
                return resultModel;
            }
            
        }

        private CoseSign1Object DecodeToCOSEFlow(string base45String)
        {
            // The app only expect passport with these prefix
            if (TokenTypeExtension.GetTokenType(base45String.Substring(0,3)) == TokenType.Unknown)
            {
                throw new InvalidDataException("The provided token is not a valid DK token or token based on hcert 1 specification");
            }
            //Remove the header
            base45String = base45String.Substring(3);
            if (base45String.StartsWith(":"))
            {
                base45String = base45String.Substring(1);
            }

            byte[] compressedBytesFromBase45Token;
            try
            {
                compressedBytesFromBase45Token = base45String.Base45Decode();
            }
            catch (Exception e)
            {
#if APPSTORE || APPSTOREBETA
                throw;
#else
                throw new TokenDecodeException("Failed to decode base 45 string", e);
#endif
            }

            byte[] decompressedSignedCOSEBytes;
            try
            {
                decompressedSignedCOSEBytes = ZlibCompressionUtils.Decompress(compressedBytesFromBase45Token);
            }
            catch (Exception e)
            {
                throw new TokenDecodeException("Failed to decompress byte string", e);
            }

            CoseSign1Object cborMessageFromCOSE;
            try
            {
                cborMessageFromCOSE = CoseSign1Object.Decode(decompressedSignedCOSEBytes);
            }
            catch (Exception e)
            {
                throw new TokenDecodeException("Failed to decode byte string to COSESign1 object", e);
            }
            return cborMessageFromCOSE;
        }

        public ITokenPayload MapToModelFromJson(string json, string tokenPrefix)
        {
            try
            {
                switch (TokenTypeExtension.GetTokenType(tokenPrefix))
                {
                    case TokenType.DK1:
                        return JsonConvert.DeserializeObject<DK1Payload>(json);
                    case TokenType.DK2:
                        return JsonConvert.DeserializeObject<DK2Payload>(json);
                    case TokenType.DK3:
                    case TokenType.HC1:
                        DefaultCWTPayload defaultPayload = JsonConvert.DeserializeObject<DefaultCWTPayload>(json);
                        return GetDigitalCovidModelV1ByVersion(defaultPayload, json);
                }
            }
            catch (Exception e)
            {
                throw new TokenDecodeException(
                    $"Error in deserialize CWT to model with prefix {tokenPrefix}", e);
            }

            throw new TokenDecodeException(
                $"The provided token is not a valid DK token or token based on hcert 1 specification, token prefix {tokenPrefix}");
        }

        private ITokenPayload GetDigitalCovidModelV1ByVersion(DefaultCWTPayload defaultCwtPayload, string json)
        {
            //override the current translator in case multiple translator exist 
            DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator = _translator;
            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator = _testManufacturerTranslator;


            switch (defaultCwtPayload.DCCPayloadData.euHcertV1Schema.Version)
            {
                case { } version
                    when version.StartsWith("1.0"):
                    return JsonConvert
                        .DeserializeObject<Model.EuDCCModel._1._0._x.DCCPayload>(json,
                            new JsonSerializerSettings {ContractResolver = _contract});
                case "1.1.0":
                case "1.2.0":
                case "1.2.1":
                case "1.3.0":
                    return JsonConvert
                        .DeserializeObject<Model.EuDCCModel._1._3._0.DCCPayload>(json,
                            new JsonSerializerSettings {ContractResolver = _contract});
            }

            var exception =
                new TokenDecodeException(
                    $"the provided DGC token version is not implemented, the provided version is: {defaultCwtPayload.DCCPayloadData.euHcertV1Schema.Version}");
            
            throw exception;
        }
    }
}