using Moq;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.DecoderService;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.CoseModel;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.Services.WebServices;
using SSICPAS.Tests.TestMocks;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DCCVersion_1_3_0 = SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;

namespace SSICPAS.Tests.TokenDecryptionTest
{
    public class HcertTokenProcessorServiceTest
    {
        private HcertTokenProcessorService verifier;
        private Mock<ICertificationService> MockCertificationService { get; set; }

        public HcertTokenProcessorServiceTest()
        {
            IoCContainer.RegisterInterface<ILoggingService, MockLoggingService>();
            IoCContainer.RegisterInterface<IDateTimeService, MockDateTimeService>();
            
            MockCertificationService = new Mock<ICertificationService>();
            MockCertificationService.Setup(x => x.VerifyCoseSign1Object(It.IsAny<CoseSign1Object>()));
            
            verifier = new HcertTokenProcessorService(MockCertificationService.Object,
                IoCContainer.Resolve<ILoggingService>(),
                IoCContainer.Resolve<IDateTimeService>());

            var MockRatListService = new Mock<IRatListService>();
            MockRatListService.Setup(t => t.GetRatList()).Returns(Task.Run(async () =>
            {
                var assembly = typeof(RatListService).GetTypeInfo().Assembly;
                Stream ratlistStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.ratlist.json");
                using (var reader = new StreamReader(ratlistStream))
                    return await reader.ReadToEndAsync();
            }));
            MockRatListService.Setup(t => t.GetDCCValueSet()).Returns(Task.Run(async () =>
            {
                var assembly = typeof(RatListService).GetTypeInfo().Assembly;
                Stream valuesetsStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.valueset.json");
                using (var reader = new StreamReader(valuesetsStream))
                    return await reader.ReadToEndAsync();
            }));

            IDCCValueSetTranslator translator = new DCCValueSetTranslator(MockRatListService.Object);
            IDCCValueSetTranslator testmanufacturerTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(MockRatListService.Object);
            verifier.SetDCCValueSetTranslator(translator, testmanufacturerTranslator).Wait();
        }
        

        [Test]
        public async Task TestDecode_CanDecode_EC256()
        {
            //This test does not validate the signature, it just test the decoding part
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH:VEIOOW%I04W-ODGJLSLL56M+QI6M8SA3/-2E%5TR5VVBFVA*T5OGO%9BA180JC6IAT92K1WBDVA K%KIO4KPK6PK6F$B7$KN+R$FK8+S:RA39K9-UOH2ATP8$JHG4TNOVCTA KK.S-DI.SSW*P*WEZ-S-YNWCK+7B5KDG/BWLG1JAF.7BPKR-S E1CSQ6U7SSQY%SVJ55M8K7PMK5%NARK4519VMAWL6QH0 L6PK6RK43:4W0BAJ2RK4LOEZK4PSEN$K1RS$15SBCL20*W0VTQ8OI+*PA KZ*U0I1-I0*OC6H05TMBDKDII-GGUJKXGGEC8.-B97U3-SY$NKLACIQ 52564L64W5A 4F4DR+7C218UBR: KF N04C:A3*Y8QZ8 .K/6UW6N8:JEWE$JDBLEH-BL1H6TK-CI:ULOPD6LF20HFJC3DAYJDPKDUDBQEAJJKHHGEC8ZI9$JAQJKS-KX2MYII*GICZG9$G5/BUZ4RD9$XLM.300SPV4HHFO87Y3MM2MS-8+QB*CSO$I-HB+FQUE9KS6D0Q0BOH2EA9DJJDWQB HO4VSNRD$1K:ZJJ4WXXRGAT*405+5C3";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            //because this token is expired
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_CanDecode_RSA2048()
        {
            //This test does not validate the signature, it just test the decoding part
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH:VEIOOW%I04W-ODGJLSLL56M+QI6M8SA3/-2E%5TR5VVBFVA*T5OGO%9BA180JC6IAT92K1WBDVA K%KIO4KPK6PK6F$B7$KN+R$FK8+S:RA39K9-UOH2ATP8$JHG4TNOVCTA KK.S-DI.SSW*P*WEZ-S-YNWCK+7B5KDG/BWLG1JAF.7BPKR-S E1CSQ6U7SSQY%SVJ55M8K7PMK5%NARK4519VMAWL6QH0 L6PK6RK43:4W0BAJ2RK4LOEZK4PSEN$K1RS$15SBCL20*W0VTQ8OI+*PA KZ*U0I1-I0*OC6H05TMBDKDII-GGUJKXGGEC8.-B97U3-SY$NKLACIQ 52564L64W5A 4F4DR+7C218UBR: KF N04C:A3*Y8QZ8 .K/6UW6N8:JEWE$JDBLEH-BL1H6TK-CI:ULOPD6LF20HFJC3DAYJDPKDUDBQEAJJKHHGEC8ZI9$JAQJKS-KX2MYII*GICZG9$G5/BUZ4RD9$XLM.300SPV4HHFO87Y3MM2MS-8+QB*CSO$I-HB+FQUE9KS6D0Q0BOH2EA9DJJDWQB HO4VSNRD$1K:ZJJ4WXXRGAT*405+5C3";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            //because this token is expired
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        
        [Test]
        public async Task TestDecode_CanDecode_RSA3072()
        {
            //This test does not validate the signature, it just test the decoding part
            string prefixCompressedCose =
                "HC1:NCF*90*C0/3WUWGVLK6796/9R5M5/GWMBH479CKM603XK2F3O8J0.42F3O%I+-4/IC6TAY50.FK6ZK7:EDOLFVC*70B$D% D3IA4W5646946846.966KCN9E%961A69L6QW6B46XJCCWENF6OF63W5KF60A6WJCT3ETB8WJC0FDU56:KEPH7M/ESDD746IG77TA$96T476:6/Q6M*8CR63Y8R46WX8F46VL6/G8SF6DR64S8+96D7AQ$D.UDRYA 96NF6L/5SW6Y57+EDB.D+Y9V09HM9HC8 QE*KE0ECKQEPD09WEQDD+Q6TW6FA7C46TPCBEC8ZKW.CNWE.Y92OAGY82+8UB8-R7/0A1OA1C9K09UIAW.CE$E7%E7WE KEVKER EB39W4N*6K3/D5$CMPCG/DA8DBB85IAAY8WY8I3DA8D0EC*KE: CZ CO/EZKEZ96446C56GVC*JC1A6NA73W5KF6TF6FBBF9GJ/98:FGYB25F$4VPO9$GO7ECN:S5T3TDHU*0ZLB5CR WLYXPN:BHRSAUEJP7:6F3D2MD80QGSQ7*F8TX1OVMW7AIJ1RG4DOLJ 983U9NR+XG.A1L8CPXHB1AD8DPU3OY94WAX.B.UD9ZSPIG7TV2NMFKKE9L:+H5XQN60ULCD.9CK9:5P-+SWCJ Q6+JCVMT:2IK2HW4884UAJ1:HA+FN /CJ%TI-O1PR.IH-R6GJ5EDW7UTAIOC5RO4JZT0Y1G/W57M49JUX178X010T$B1*OG1.N/9R5*A L9:S8JR2WU4:72U7K8SI*AAS1JNLJHZUWSNB3UC/MM%CBDH$N88UM0.VQPBLML6BKE9F0S7FXQJADXOED/H%.5+:7K%5ZBK$HUH/LX074%9+RMDRDH7Q:G3E4J$LK+6I5/IJ+0U58RSLSBW.AKVZT ANOLQZW720F-%4WZV*SOHX52J0JM3F3GPFPHZKSAB6KKGV9$RJ00G:O08Z4GP3FGEU$OCOSE3HY VD05.FK 7L893VZH0RG*Q36YF7B25AR650M+8G6B$6CLOE+EFV/OE:0O*JP26X48H1";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            //because this token is expired
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_Faulty_COSE()
        {
            string prefixCompressedCose =
                "HC1:NCFC:MQ.NAP2H432EEY.G61FXR4.N2IMJ1*GK7US4F47VYVJ+7TF:8YLE8S4V-80GF4+5$XSW27N*J6D2S9IK*D/G9RP2E$SIIO6MHANJBV4HXK7FAS+86T5.%VIWB+KM6VM8DFG:NDN7VEQXXNBIU%JK2SH.-A%5E*LFK$QPR5:1O$56ESL+8W:WCM33RET+.L0UC TMFCB77IO0IL4HS6N:F79-1.ZT.QGM5AUC1+I3RMRBPBSFFWWA+S9S%5IZS/731QABSR5NKF2NO62Y+6 $HWZEN/RP 4 EB29UV0Z0IRKQ6TB-W28VAG2JCMHCCA9ALK4RHZHDCO-%1GF9M0LC5GU3P:40510H*9S:M%0K3QLIF57+I0*UZMAV 2TJ761OY RF10IMKE06Z$Q3V960IQXVM+9*UR9WH$DC18BH0GLBJC$4Z-FVX1SCVD-Q -3MTGC8AP1QO:6GXECZBV*1%78BMKEX2XNI7%6QGO2X8DGB8DETFM0JQA/2B+5YMBX$S2/52X0E-O/8RR*8PW7T:TYYRLWFZ62KTVDJTLTQ3BR$4E65NL-5+UP%8M14AS0C88NM%AQZFF+FCGWM3U42963UC27$Q8Y7S3VH19P$BO%*1R4K5.UT1";
            
            
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_Faulty_CBOR()
        {
            string prefixCompressedCose =
                "HC1:NCFY/LEJMBK2U13P7E/%OFQ80441MDQ7CS*N3-F6YNYJELP2QSD:S5WGKL-O9Y32UFDR90I8B5BP2O9MHJFDNH1S-17O20B2USN%-1VWHVXIOOGBTS/VLNO8VCF+6R2NOXA3VMSF0GG8KV%NRUA*XBP*VMAN$7Q007%TD6+7YQ2+L07TDT7EW-CT%G$$LW%JSEO3UO7+U2KSTXALORM9AA80GF55OO$WIO$S17LB36:3NF:02G69*MDELMX2+3BZ*QD KYDPDIQI6WTOJ5/B7Q3JK385F:UD0XA$-2CTJ8$ODS3LOQCW7ODPF6FM 7+*0B$1LNLY.0F/TKEN:ANX*8RBSI+V49EX$DR%2PZCHL20EA98P2P6Y0R$YC/90RZ0WLO0LFP6CORJU7HP MQJ53LRZ4IK4Q4V09MCZYB/NMG7JE:IA20%93$2F.8RLVMS$KWEE:HC-GJI.IY.MIF47G49+UBWAVQHFWR*QNXRLDYAN-VZLHMNDH57XZNZPNEET-UT7MUN.DV*NT02+5GO6ONAWGTLU4G6A6PW7P7V0WU/PVW.P2DW:/EJWVC-M7SCM5OM2UJ1S*HEP0403MAGWMIED8UD:LPCQT+6S.M";
                
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_KidHeaderInUnProtectedHeader()
        {
            string prefixCompressedCose =
                "HC1:NCFOXNRTS3DH3ZSUZK+.V0ETD%6TNLAI6LR5OGINCJMKB:X9RFCR/GKQCAQCCV4*XUA2PSGH.+HIMIBRU SITK292W7*RBT1ON1XVHWVHE 9HOP+MMBT16Y5TU1AT1SU96IAXPMMCGCNNG.8 .GYE9/MVEK0WLI+J53J9OUUMK9WLIK*L5R1ZP3YXL.T1 R1VW5G H1R5GQ3GYBKUBFX9KS5W0SRZJ3T9NYJQP59$H5NI5K1*TB3:U-1VVS1UU15%HVLI7VHAZJ7UGBL04U2YO9XO9*E6FTIPPAAMI PQVW5/O16%HAT1Z%P WUQRENS431TN$IK.G47HB%0WT072HFHN/SNP.0FVV/SN7Y431TCRVH/M$%2DU2O3J$NNP5SLAFG.CILFFDA6LFCD9KWNHPA%8L+5I9JAX.B-QFDIAS.C4-9NKE$JDVPLW1KD0K8KES/F-1JF.K+W002CT+C-Z1+168+F HTH/L5IQ1PML/3I2T5N6P U2R4$V7WJBFCRK8SH-M508%I4ANV%72M8PV-7 %4L BD$BL-DDIV$UP4D8GZS9RCS1U81A6Y0-XP-MG";
                
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            //because this token is expired
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        
        [Test]
        public async Task TestDecode_Unknown_Prefix()
        {
            string prefixCompressedCose =
                "HC2:NCFC:M3B6+J2 53WGEY-A9S5Q34P466EDQ*NVTFI1SG%RRDIL8D6KO9BI0-EN0O/:BUTNB7FI1S512:-KC4UFJ94KGZ$HDMLONGYH8%T2B%J*P7D346K7NJLDSIPBVD5GJEU 9TI2IF5S$PV7II45H/PB9:1VHB.0H9XSI4TA:0I9M/0RTQB+$69BB+%0P%2G*P$JAJXP2-Q/PRAHK-N1 8JN/9BZD/IJ*D4C.8/KBPV3-QE6.C-QRPX8+03RXTQ94L+LHOHIS8MP51VF%EQA6I0MGX4EU41-2WLK1199 SO$P4/HI$ 9AG9HLB$HMYHKJYE-82 R5B8GP7ESMN%/B HRA73J67DXH.0BP6FO/BLBMJI8CIE4A55K5-$GQYQLM1961.T7%P4TZG:B9A9NACD* 35E3S30M8AR31V/P8T2CJPM.CH9QZO4L39N6WCE6-:03ERAEWXU8K:RLZCXK0:LHT$A+59F*TM90H0Q$CPB6I3AG6/HWDNSZE8/RO741DFA55Z%5UGJQ8N47F0ORS0Q8CR3P7J:0$3G-C9Q0KQ04YM6N9WJYSC:R%4K5/FOY6L:B$5GN2LD-L:1O 20WFWYUNUWN9OST9W:IHK.P7$VZYIQ90GD790";
                
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        
        [Test]
        public async Task TestDecode_FaultyBase45()
        {
            string prefixCompressedCose =
                "HC1:NCFC:MC9Q.P2 53QMU+7JLHI.12U6BS/EV-RH%FY7S3HJS-T7PVDTGUAH00S-AS/6S 6FU+J+5DNFBFN0MV38+O2$Q*R94KI8OKNYU/321O1KJT*JLO9ADCHKHLF.JC-E VU*$UXPUK/E*SUQ.R1%0QT14CB8SB$-LN:NMCIKGBA/V/+VQ2VDY48VMI2RMSOYX4PLJ5H9YMR0H7S.RNRN FQ IFZ.8M:EVA1EN84XCD$5GDARF5Z%H/F3%HP+K8R7CDEP6VLUMK20JBZ281OCAD:09/C80/BH519XPA9T2IIQZGWEECP4-GJRY23$MAF4WR86AU+TJ9LM060+DOQWC%%FQ:CC+CEES8G65*DHERUUNFICFVGO S/PAKF7-$G/0RLM1961.T7487F-GZLH VNACD$%3H8I530SH8LOJC4EUUKC0A:*6/AWH:QP*6YRUGOHD:7G9P0GVSY47%HPL3GEAMV8DKHK.2N7H 2B%ZAD BDIGD 71G6*-0LT5SAGJA33QFP7G%Q7OBUL*53:V5YEE1F%DVD2F .16Y5DV7KN7-%BQ4CMTUNHV8-KN8UP8T/QIWM6JOM8LV1VT.JU5L1L08BXF/$UWKO3H6I3B0:AUI76 T740=====";
                
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_FaultyCompressor()
        {
            string prefixCompressedCose =
                "HC1:C/A-H96N11UV/4UC-NIMUZT3V6IS TN0ULQCZO8ZXITMV85GO8EG JL2L51S*D56XJWAV/ NO 5OQU *NG1S/.F+5GE:RU5O1EU$%7T.F%BFVV70GAYUH 583JFJOT*HUTT03M0G F*C4YTLL$24KQ8$AJKLBVQZ 1-7TE1GTU44IVF%RI5G+P2HTQI4F4TA63WHKLYF00O3DMDCM7-P24X0GFEPGUCT8G5HKEE3J9%G9SCOYGU8GUN-S$KK6OFP80H56%889T1T-G%BRM7DQ68P00 HE7O7.W3YKIK10XEPL1PEQDF-BWBB%-9W4B7W91*J$IA7Z6ND9XQ4M09NT1PC43/RWG5C.2TSB:66UG3WX8.0I RPGWE2Z9+*GH08%0R$-2S838 A2UTK45IB9+B7 9SFOKXDI3Y9+NF:$4YV6*PPEYR0-45-P5OR9.NKVK XTFWVV5E IMLSKRG6TZUX/U%D78HVTZJX11J:AVKDTVULAW0PSP9W%*I6MDRIR+44K5GT*G0CHMDHS7B5GBIHAH%3X*A0:4ZJGIHH1 GIOR+SIX5VD0PZAUH:3 EK P8.T6LM4RFL2ANMWM9CU$UE8+IC54OS01+HYS1VFE:237Q2Z:NO:MU2";
                
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_NoopCompressor()
        {
            string prefixCompressedCose =
                "HC1:RRQT 9GO0+$F%47N23.G2O609CKIA03XK4F35KC:CE2F3EFG2LU/IC6TAY50.FKP1LLWEYWEYPCZ$E7ZKPEDMHG7ECMUDQF60R6BB87M8BL6-A6HPCTB8IECDJCD7AW.CXJD7%E7WE KEVKEZ$EI3DA8D0EC*KE: CZ CGVC*JC1A6NA73W5KF6TF6$PC1ECFNGGPCEVCD8FI-AIPCZEDIEC EDM-DKPCG/DZKE/34QEDA/DOCCQF6$PCEEC5JD%96IA7B46646WX6JECFWEB1A-:8$966469L6OF6WX6F$DP9EJY8L/5M/5546.96VF6YPC4$C+70AVCPQEX47B46IL6646I*6UPCZ$ETB8RPC24EQ DPF6BW5E%6Z*83W5746JPCIEC6JD846Y968464W5Z57HWE/TEEOL2ECY$D9Q31ECOPC..DBWE-3EB441$CKWEDZCQ-A1$C..D734FM6K/EN9E%961A69L6QW6B46GPC8%E% D3IA4W5646946846.96SPC3$C.UDRYA 96NF6L/5SW6Z57LQE+CEJPC+EDQDD+Q6TW6FA7C46IPC34E/IC3UA*VDFWECM8KF6 590G6A*8746C562VCCWENF6OF63W56L6*96ZPC24EOD0P/EH.EY$5Y$5XPCZ CJAG8VCOPCPVC..49A61TAOF6LA7WW68463:6QF6A46UF6+Q6RF6//6SF6H%6NF6SF6646.Q627B5JQ0H3/.HJ2S%3U7SKVGH835.R7JL24/N$5TJZ2V32NCUPSOAWNNJSY$23RS+5TT4M862*O6IPDVZ8L1P0FQR6KHFC9Z7VDF";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        [Test]
        public async Task TestDecode_DK1()
        {
            string prefixCompressedCose =
                "DK1:NCFJ70S90-7WF.9.60+O4W79F%GZM9NTVO K*6B0XK1JCYL92F3Q7HLTO2F3LTGCJ4J6BH*6O-CVW6RIC7W54VC+/61R6YICCW5.CCC-C3R6M-CNF6LPCRW6%ZCFBBG20FV39CFHL6C0N:VB6JPW*0OGFBNF3U3AL8S4WHCAT RC+BZA1.WFFJG5:F0ASALDNSPO-AP79RBF76D-*P50L/Q4+INTJV0UO.*KY77UFJ*ZJ33GHJFGRKY0W4*9T3B-HNKO3:GQZFCPXRD4J BEJ0BL.2:G8/*1QK5P24W74VUB//U.1S7CDJ*121UL58*8CW-Q%L2BHQEBA1-1.BR1LA2DI PERSA6LUH6OXCOPO6K9F7670B57NAC4J$VU/.NQ/BSLFR 8.+NV9O *EO-K81I8F5JBS*TFH8U/KNYYIQWHTUMA9MJLIS$3.KE1O1SVT1:MHP1UX4T+FL9CVF2D039FTTHL4GCWAP-3D4JCJKJCYMDR1T*TWG5I243VI5.5RE0.0";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        } 
        
        [Test]
        public async Task TestDecode_DK2()
        {
            string prefixCompressedCose =
                "DK2:NCFM80P80-7WF.9.60+O4W79F%GZM9NTVO K.7BH:K1JCYL92F3Q7HLTO2F3LTGCJ4J6BH-C*966VCL%69W5LPC6L6.Q6OA7BW5MA7Q47X57W47HOC1A64S6F56XED* CAEC++9F$DB.D1$C7WEN341$CLVCLQE7/D2VC7WEV1A H85LE09D1A6 473W5NW6A46FBBX20PAM5K0Y.FH8FWO1NWEA5G3OAB4IZD8V5T1-H711X460HC0K9N44CERJN1UZP OIGNV*%FDC4G4BJ34LXDI5KZIEB+1A69$8I$91YEKNBPAKBIBP+D3.WU$ZNP-MH*T*QP+IT 28:6MO1TSSRDQQD:E102WZ3ADRA02.1A ARD*C0/D631F/UC*I -QZ6VNIV%HMGQ39FK4$8BIF918P$N/B7JUA0*AMBR.ZTK$PA:8UYTL*SQMV46VHNH9TV9V5TZ5:XI7T1U7W1E1GPDRVBWXE53B/EFNRS2H5:HU9UT84IQWCF0JA4KG9L3YCG72F J2P2AOL:BJ+3L4VHL/79VG.T6 $GWKGZAAYH39P7PJC/MQ*FDF B7+IX2B0$4V2NVLD/2";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(result.ValidationResult, TokenValidateResult.Invalid);
        }
        
        [Test]
        public async Task TestDCC1_0_x_isValid()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCF8Y5BM75VOO10*EH6092014MACWI62L3/5ARJ4/0 RVERH-W4WR8+.P4UQWD9M:I8T16F7WQSW$5VSSY*9LMG TJWIVUZ1G0EGLH491O11THC88RTCVTCQ43N%F2BOFS8QV4CO$BTQA5SKMMH5CP5N5BXS82AI3TJ:6QFC6R5U1SXWAQ41U.9P:S1EN7-QZS6E.GM00C66B9PUNCGG84QM.118$2+IOBK2:D3CQ6ADMR3IRERMPDHARZTEVJ8Z9J.369+I912034P9E%E8XR4L10VQ7HMRYYJI9FD:JG34%T4FLBBZ8I 0X439FGBL4WQQHK3/FH/NJ4AOUCLBE2H5N*%0-LQ5*QR11%BENUTGFG3N0XHU7WE/+PKXHVYR59FIQ9-N9KXKKR6D/3A5TDILGYJ8CIWL5 O9I3L 5CS$H3:LCXBVO3OBQ +F-NU*MLLKPCB2D74P9F::OT-7.%BVP1RBSOFD$$0:$A3EWGOCP$S0.4I:7EENZ/EHL5//BMP4N515MAI38LSQLO7MVUV6U8%L9YV32WXC95DV*AF6AWBXG7AMPA8Q2F25CP UA9J44TRLVI%Q$FH";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_0_x_isInvalid()
        {
            // dob is a string instead of date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH3%QIOO6+I$MEBY9GJL1QCQ2EK$4C:GPF6R:5SVBHABVCN395R4IL95OD6%28%%BPHQOGO3IRIHPQT69YLLYP-HQLTQV*OOGOBR7L*K1UPH65%PDGZK*9DJZIM-1/24BD7WOOCU6O8QGU68ORJSPZHQ5D6ZSPQGO7HO0SOGIR1TQWNQ01RL1RE+QLTQV0Q.NRI+Q%MPCSOKAB795Z6NC8P$WA3AA9EPBDSM+QFE4IWMEK81:6G16IFNPCL694F$9DK4LC6DQ4939HHM 55ZIJFVA.QO5VA81K0ECM8C%P12MHSKE MCTPI8%MIMIBDSUES$8RZ6N*8PBL3C7GKGS$0AY6F*DK8%MRIA:EKT*QR$M% OP*B9YOB*16CQJN9TW5F/94O5 9E6UEDTUD1VVY95CQ-8EDS9%PP%.P3Y9UM97H98$QJEQF69AKPOK9799.MP06IVBCMTE*TMOSO4QFS*43:OP-7%DJK7G-FB:HHUH6OP6C268M5D5K S4J6V+PS%4CXQ4*:E43K79SUPNDXLG+4QPQTPNM%VIXGFZBN:BO50$A485";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_wasValidIn1_0_x_ShouldBeValidIn1_1_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH-R6IOO6+I0NEXX8GJLMYSI-2+QI6M8SA3/-2E%5VR5VVB9ZILAPIZI.EJJ14B2MZ8DC8COVD9VC/MJK.A+ C/8DXED%JCC8C62KXJAUYCOS2QW6%PQRZMPK9I+0MCIKYJGCC:H3J1D1I3-*TW CXBDW33+ CD8CQ8C0EC%*TGHD1KT0NDPST7KDQHDN8TSVD2NDB*S6ECX%LBZI+PB/VSQOL9DLKWCZ3EBKD8IIGDB0D48UJ06J9UBSVAXCIF4LEIIPBJ7OICWK%5BBS22T9UF5LDCPF5RBQ746B46JZ0V-OEA7IB6$C94JB2E9Z3E8AE-QD+PB.QCD-H/8O3BEQ8L9VN.6A4JBLHLM7A$JD IBCLCK5MJMS68H36DH.K:Z28AL**I3DN3F7MHFEVV%*4HBTSCNT 4C%C47TO*47*KB*KYQTYQTNS4.$S:D81GD1QDIH2KCMR5D VOF37T$GVNR1+NGIMP-MHJRW699-9F%PY5K2BAV/1$:SFLSC J3ABMX6 S6+AUMBWYHNWLN/64-5LYQ3Q:RS:LURJ-$RI%JE30DQ1.4";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_wasInvalidIn1_0_x_ShouldBeValidIn1_1_0()
        {
            // dob is a string instead of date
            string prefixCompressedCose =
                "HC1:NCF5W1P8QPO0%20%M315WM:26+6OVC3CS:/3$.CQ76 YRF%C6KR6HEF*NAAN*WMO5OZ+PQQSF%5VLV$R28*EMR3VSG4P3-/KHB8F1MOSP:5M%TEZVTQTVCWLEK9RHT+N9W9VRQVHEIK6SX-CW:QO6R-VPVKM++EFNK WC ZJCFJUX3LPLN9J6AEOI6BBLX/4UU4V50JQ3YIKBZPSS0CKKJLUH00$$JWF1J6IL8G%F8TM3S04J1HV11. E$NLNZ1RLKE/8GUEYB0GS8.1K%P9 VHV58I001:2.W13C0:I3NC4PB3I/O-J1GCCW-CY13N LVZ9.HM7ED ZQ6 JUBG3W1EH24WUE:VNMN.J2O9PU:K0QFBRA6IGMMV9H0$:QFYA*P9EIAFP9LFI82B* D$GJDILPJCWA8Q6Q7LA:2B*SI-*UH6AU:7X9SMFD657O0I+EFQ1IG71 :G.2I5U2ML7/JMTBAN$1C84.C0 5PPQ4$T5J:JOLF7/KC9W 2SF5B6FQ1ZP4SPRDR.SELYV9/FFND.OD.TJE+H*BW4 2+4BP0ESPC/ VESCYSN. GFU7G:UUWCQETUXM//T *GI-H";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }

        [Test]
        public async Task TestDCC1_3_0_wasValidIn1_0_x_ShouldBeValidIn1_2_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH-R6IOO6+I2LEUA38WA$FE%I6AT4V22F/8X*G3M9JUPY0BX/KR96R/S09T./0LWTKD33236J3TA3M*4VV2 73-E3GG396B-43O058YIB73A*G3W19UEBY5:PI0EGSP4*2DN43U*0CEBQ/GXQFY73CIBC:G 7376BXBJBAJ UNFMJCRN0H3PQN*E33H3OA70M3FMJIJN523.K5QZ4A+2XEN QT QTHC31M3+E32R44$28A9H0D3ZCL4JMYAZ+S-A5$XKX6T2YC 35H/ITX8GL2-LH/CJTK96L6SR9MU9RFGJA6Q3QR$P2OIC0JVLA8J3ET3:H3A+2+33U SAAUOT3TPTO4UBZIC0JKQTL*QDKBO.AI9BVYTOCFOPS4IJCOT0$89NT2V457U8+9W2KQ-7LF9-DF07U$B97JJ1D7WKP/HLIJLKNF8JFHJP7NVQ7ACQJFG34S8XUFJ.F5CASDRQVV*ISI%PV7LR:7KCJYFOP U5VTV+K+/HQ9F05E4G5.L6L496FAH6QAEF%UQ8.NK6WA5NYTQV2P KJ/O6VR94NUK$7$*03VJ*NH";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_wasInvalidIn1_0_x_ShouldBeValidIn1_2_0()
        {
            // dob is a string instead of date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH3%QIOO6+I8NE/N0GJL*SC..4K$4C:GPF6R:5SVBHABVCN395R4IL95OD6%28%%BPHQOGO3IRIHPQT69YLLYP-HQLTQV*OOGOBR7L*K1UPH65%PDGZK*9DJZIM-1/24BD7WOOCU6O8QGU68ORJSPZHQ5D6ZSPQGO7HO0SOGIR1TQWNQ01RL1RE+QLTQV0Q.NRI+Q%MPCSOKAB795Z6NC8P$WA3AA9EPBDSM+QFE4IWMEK81:6G16IFNPCL694F$9DK4LC6DQ4939HHM 55ZIJFVA.QO5VA81K0ECM8C%P12MHSKE MCTPI8%MIMIBDSUES$8RZ6N*8PBL3C7GKGS$0AY6F*DK8%MRIA:EKT*QR$M% OP*B9YOB*16CQJN9TW5F/94O5 9E6UEDTUD1VVY95CQ-8EDS9%PP%.P3Y9UM97H98$QP3RF69AKPOK9799.MP06I+79HGDB55385:*SGNQ-CUFI9N9L.D6531AFUR1CH$U5C5$BQ6FU-RV-PT*2II-RQGU8-VM*R4.PKPCZPG/5W5 2IDSPKU6:96G7C+I$00B%2H2";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_wasValidIn1_0_x_ShouldBeValidIn1_3_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH-R6IOO6+I2LEUA38WA$FE%I6AT4V22F/8X*G3M9JUPY0BX/KR96R/S09T./0LWTKD33236J3TA3M*4VV2 73-E3GG396B-43O058YIB73A*G3W19UEBY5:PI0EGSP4*2DN43U*0CEBQ/GXQFY73CIBC:G 7376BXBJBAJ UNFMJCRN0H3PQN*E33H3OA70M3FMJIJN523.K5QZ4A+2XEN QT QTHC31M3+E32R44$28A9H0D3ZCL4JMYAZ+S-A5$XKX6T2YC 35H/ITX8GL2-LH/CJTK96L6SR9MU9RFGJA6Q3QR$P2OIC0JVLA8J3ET3:H3A+2+33U SAAUOT3TPTO4UBZIC0JKQTL*QDKBO.AI9BVYTOCFOPS4IJCOT0$89NT2V457U8+9W2KQ-7LF9-DF07U$B97JJ1D7WKP/HLIJLKNF8JFHJP7NVQ7ACQJFG34S8XUFJ.F5CASDRQVV*ISI%PV7LR:7KCJYFOP U5VTV+K+/HQ9F05E4G5.L6L496FAH6QAEF%UQ8.NK6WA5NYTQV2P KJ/O6VR94NUK$7$*03VJ*NH";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_wasInvalidIn1_0_x_ShouldBeValidIn1_3_0()
        {
            // dob is a string instead of date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AH3%QIOO6+I8NE/N0GJL*SC..4K$4C:GPF6R:5SVBHABVCN395R4IL95OD6%28%%BPHQOGO3IRIHPQT69YLLYP-HQLTQV*OOGOBR7L*K1UPH65%PDGZK*9DJZIM-1/24BD7WOOCU6O8QGU68ORJSPZHQ5D6ZSPQGO7HO0SOGIR1TQWNQ01RL1RE+QLTQV0Q.NRI+Q%MPCSOKAB795Z6NC8P$WA3AA9EPBDSM+QFE4IWMEK81:6G16IFNPCL694F$9DK4LC6DQ4939HHM 55ZIJFVA.QO5VA81K0ECM8C%P12MHSKE MCTPI8%MIMIBDSUES$8RZ6N*8PBL3C7GKGS$0AY6F*DK8%MRIA:EKT*QR$M% OP*B9YOB*16CQJN9TW5F/94O5 9E6UEDTUD1VVY95CQ-8EDS9%PP%.P3Y9UM97H98$QP3RF69AKPOK9799.MP06I+79HGDB55385:*SGNQ-CUFI9N9L.D6531AFUR1CH$U5C5$BQ6FU-RV-PT*2II-RQGU8-VM*R4.PKPCZPG/5W5 2IDSPKU6:96G7C+I$00B%2H2";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }

        [Test]
        public async Task TestDCC1_3_0_TestedUserHasNoDRField_ShouldBeValidIn1_2_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AHBP1IOO6+I7TU%188WATGE91GAT4V22F/8X*G3M9BM9Z0BFU2P4JY73JC3KD34LT7A3523*BBXSJ$IJGX8.+IIYC6Q0ZIJPKJ+LJ%2TK/IS/SR4DKJ5QWCB4DN57E-4LXKV85HZ0T+0K%I17JLXKB6J57TJK57ALT$I/+GDG6Z$U*C2OQ1:PIGEGEV4*2DN43U*0CEBQ/GXQFY73CIBC:GUC7QHBN83GG3NQN%976FNXEB.FJN83HB3EG3CAJTA3ANBXEBGM5J%44$28A9H0D3ZCL4JMYAZ+S-A5$XKX6T2YC 35H/ITX8GL2TK96L6SR9MU9DV5 R1JNI:E4I+C7*4M:KCY07LPMIH-O9XZQSH9R$FXQGDVBK*RZP3:*DG1W7SGT$7S%RMSG2UQYI96GGLXK6*K$X4FUTD14//EF.712U0$89NT2V457U8+9W2KQ-7LF9-DF07U$B97JJ1D7WKP/HLIJLRKF8JFHJP7NVDEBU1J6+2FBKBSH%9BP/KOTN93N$3WSYF5 M8.N/1F2FV NIB6OS0F6+5-VRG.3BWE 2RBNTTN7B%C56DM-QZG1+JRX*PV38*0LXY5ADWGWJ9Y8%O5$ G*00WS552";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }
        
        [Test]
        public async Task TestDCC1_3_0_TestedUserHasNoDRField_ShouldBeValidIn1_3_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AHBP1IOO6+IAUULZ78WAAFE6TFAT4V22F/8X*G3M9BM9Z0BFU2P4JY73JC3KD34LT7A3523*BBXSJ$IJGX8.+IIYC6Q0ZIJPKJ+LJ%2TK/IS/SR4DKJ5QWCB4DN57E-4LXKV85HZ0T+0K%I17JLXKB6J57TJK57ALT$I/+GDG6Z$U*C2OQ1:PIGEGEV4*2DN43U*0CEBQ/GXQFY73CIBC:GUC7QHBN83GG3NQN%976FNXEB.FJN83HB3EG3CAJTA3ANBXEBGM5J%44$28A9H0D3ZCL4JMYAZ+S-A5$XKX6T2YC 35H/ITX8GL2TK96L6SR9MU9DV5 R1JNI:E4I+C7*4M:KCY07LPMIH-O9XZQSH9R$FXQGDVBK*RZP3:*DG1W7SGT$7S%RMSG2UQYI96GGLXK6*K$X4FUTD14//EF.712U0$89NT2V457U8+9W2KQ-7LF9-DF07U$B97JJ1D7WKP/HLIJLKNF8JFHJP7NVDEBU1J6+2FBKBSH:9H2K71IE8-P- S/YAL$BBJAFBQIS0*.N*WSQVN4CPQLIZ2EU H$4U9WD.ZT$ 654CVVTM7I18KSIC3HN$W5QA2NC669WFT3:%QQAPZ206UUI2";
            var result =  await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
        }

        [Test]
        public async Task TestDCC1_3_0_DK3MyPageTokenWithTests_ShouldBeValidIn1_3_0()
        {
            string prefixCompressedCose =
                "DK3:NCF$/LW08BK2/53YFE-$A-0S147KE0.ZMF4KJ-F%1WOLOPEUEQF+I8OWAAUAKP7FXNF.M3FDV-G0*HUXT046GD1GB86/0FENTEEQ75R*AIZJLMG579+N2K5H$PNW+9OC13F2M4TRAQ3ASKBNQG5:FDVDNU4WLDLDCET*R+K3R NL%5:0TTMQSYOO9M/SK43C0NJH9U+VPHCQ/.1EQG0LRBSE8KD48U5NA.00CDL-LO8 KT 24*RPS00VTM Q9$1TFI1F00VHRVFX11 OM9DI./6FF5+$9:MU6E6VAR1U7VZF:7I:0I+:6RIH X39XV:O42B1EKKIBGAA00GOZ:HI60ZVN478CQU*XD4FQ%MB*JFP72D7AR90788.G2ZDOZN2AF0LVJ89R6USOC2J714M3F1G0.TM1I+7EXA2%VM5Y5TC5NI9UNGBBK-67AT69ETOM5KAA:368%UQOH/Q4AH2%LILZ1.*9K71N2NDEPBH8+JL-.VM246G69D12GG6K6Q5UAPPR949MC.6QP.6KW9/JA3A462O+416P37+GO58A*2VIIY43EDB8F6P56XXC4SP/87HHOXEFL3199RGQ11J8U.3 :IM 9URG3QN*953/I8XG.N53D62LR-BK937XO1E923667Q8V$0Z.F VDLO0STC0ZDQ/70PAW+N1T5T.J37WEQDK.LL$616M5Z69/AFSR QQSX4%N541IU.MH.389LQPAAFE.2TALNFSNZ/QN2T3L9HFBZG3CYKE4M:MFUT6PVC:L5%Y8CMIBTO/R2J3O9GWP7R3OUQPG57F+4I*/HP4UU9OM1SFAUH5CTWVL3DH$VN0OYCT*7HVM2L1KX7IU1RE2UCESF67QQEC29O/FA1VA3MD-CU%VO/LF7N";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
            Assert.True(result.DecodedModel is DCCVersion_1_3_0.DCCPayload);
        }

        [Test]
        public async Task TestDCC1_3_0_DK3MyPageTokenWithTests_XDUIsPresentForEachTest()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "DK3:NCF$/LW08BK2/53YFE-$A-0S147KE0.ZMF4KJ-F%1WOLOPEUEQF+I8OWAAUAKP7FXNF.M3FDV-G0*HUXT046GD1GB86/0FENTEEQ75R*AIZJLMG579+N2K5H$PNW+9OC13F2M4TRAQ3ASKBNQG5:FDVDNU4WLDLDCET*R+K3R NL%5:0TTMQSYOO9M/SK43C0NJH9U+VPHCQ/.1EQG0LRBSE8KD48U5NA.00CDL-LO8 KT 24*RPS00VTM Q9$1TFI1F00VHRVFX11 OM9DI./6FF5+$9:MU6E6VAR1U7VZF:7I:0I+:6RIH X39XV:O42B1EKKIBGAA00GOZ:HI60ZVN478CQU*XD4FQ%MB*JFP72D7AR90788.G2ZDOZN2AF0LVJ89R6USOC2J714M3F1G0.TM1I+7EXA2%VM5Y5TC5NI9UNGBBK-67AT69ETOM5KAA:368%UQOH/Q4AH2%LILZ1.*9K71N2NDEPBH8+JL-.VM246G69D12GG6K6Q5UAPPR949MC.6QP.6KW9/JA3A462O+416P37+GO58A*2VIIY43EDB8F6P56XXC4SP/87HHOXEFL3199RGQ11J8U.3 :IM 9URG3QN*953/I8XG.N53D62LR-BK937XO1E923667Q8V$0Z.F VDLO0STC0ZDQ/70PAW+N1T5T.J37WEQDK.LL$616M5Z69/AFSR QQSX4%N541IU.MH.389LQPAAFE.2TALNFSNZ/QN2T3L9HFBZG3CYKE4M:MFUT6PVC:L5%Y8CMIBTO/R2J3O9GWP7R3OUQPG57F+4I*/HP4UU9OM1SFAUH5CTWVL3DH$VN0OYCT*7HVM2L1KX7IU1RE2UCESF67QQEC29O/FA1VA3MD-CU%VO/LF7N";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            var resultDCC = result.DecodedModel as DCCVersion_1_3_0.DCCPayload;

            foreach (var test in resultDCC.DCCPayloadData.DCC.Tests) {
                Assert.NotNull(test.CBSDefinedExpirationTime);
            }
        }

        [Test]
        public async Task TestDCC1_3_0_DK3MyPageTokenwithVaccines_ShouldBeValidIn1_3_0()
        {
            string prefixCompressedCose =
                "DK3:NCFOXN%TSMAHN-H9QCGDSB5QPN9OO3:D4K1CNDC+NEM/C3K9RXRUXI.I565TR1BF/8X*G3M9CXP5+AZW4Z*AK.GNNVR*G0C7PHBO335KN QB9E3PQN:436J3/FJSA3NQN%234NJT53IMJJKBUA3OJJ8E3FIN$3HSZ4ZI00T9UKP0T9WC5PF6846A$Q$76QW6%V98T5WBI$E9$UPV3Q.GUQ$9WC5R7ACB97C968ELZ5$DP6PP5IL*PP:+P*.1D9R+Q6646G%63ZMZE9KZ56DE/.QC$Q3J62:6QW6G0A++9-G9+E93ZM$96PZ6+Q6X46+E5+DP:Q67ZMA$6BVUARI6IAHM9*7VIFT+F3423BNBO13BBAJ.AU53Q57HI7JON O7QHBNOJPLN*6B3SJ2SJ 73 IJ523L83$974E5EJOQQOGIUVB3VZ37JD:$8-BHUV0Y 3HD0X1LVS139H$ QHJP7NVXCB$ZAI984LT+LJ/9TL4T.B9NVPLEE:*P.B9C9Q4*17$PB/9BL5GFEJ/U1C9P8QRA9YKUEB9UM97H98$QP3R8BHGLVQQ94KTNRMEGLDZHN4BICFAC9B37/CW9KBU-RDVV44RRYT3/9EQ12+52OTW1OXC5Q71PJB-Y790OHS6Z:C5H10ES+DEFAST86+IE:00KJUG3";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);
            Assert.True(result.DecodedModel is DCCVersion_1_3_0.DCCPayload);
        }

        [Test]
        public async Task TestDCC1_3_0_DK3MyPageTokenWithVaccines_XDUIsPresentForEachTest()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "DK3:NCFOXN%TSMAHN-H9QCGDSB5QPN9OO3:D4K1CNDC+NEM/C3K9RXRUXI.I565TR1BF/8X*G3M9CXP5+AZW4Z*AK.GNNVR*G0C7PHBO335KN QB9E3PQN:436J3/FJSA3NQN%234NJT53IMJJKBUA3OJJ8E3FIN$3HSZ4ZI00T9UKP0T9WC5PF6846A$Q$76QW6%V98T5WBI$E9$UPV3Q.GUQ$9WC5R7ACB97C968ELZ5$DP6PP5IL*PP:+P*.1D9R+Q6646G%63ZMZE9KZ56DE/.QC$Q3J62:6QW6G0A++9-G9+E93ZM$96PZ6+Q6X46+E5+DP:Q67ZMA$6BVUARI6IAHM9*7VIFT+F3423BNBO13BBAJ.AU53Q57HI7JON O7QHBNOJPLN*6B3SJ2SJ 73 IJ523L83$974E5EJOQQOGIUVB3VZ37JD:$8-BHUV0Y 3HD0X1LVS139H$ QHJP7NVXCB$ZAI984LT+LJ/9TL4T.B9NVPLEE:*P.B9C9Q4*17$PB/9BL5GFEJ/U1C9P8QRA9YKUEB9UM97H98$QP3R8BHGLVQQ94KTNRMEGLDZHN4BICFAC9B37/CW9KBU-RDVV44RRYT3/9EQ12+52OTW1OXC5Q71PJB-Y790OHS6Z:C5H10ES+DEFAST86+IE:00KJUG3";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            var resultDCC = result.DecodedModel as DCCVersion_1_3_0.DCCPayload;

            foreach (var vaccine in resultDCC.DCCPayloadData.DCC.Vaccinations)
            {
                Assert.NotNull(vaccine.CBSDefinedExpirationTime);
            }
        }

        [Test]
        public async Task TestDCC1_3_0_HC1HasNoXDUField_ShouldBeValidIn1_3_0()
        {
            // dob is a proper date
            string prefixCompressedCose =
                "HC1:NCFOXN%TS3DH3ZSUZK+.V0ETD%65NL-AHBP1IOO6+IAUULZ78WAAFE6TFAT4V22F/8X*G3M9BM9Z0BFU2P4JY73JC3KD34LT7A3523*BBXSJ$IJGX8.+IIYC6Q0ZIJPKJ+LJ%2TK/IS/SR4DKJ5QWCB4DN57E-4LXKV85HZ0T+0K%I17JLXKB6J57TJK57ALT$I/+GDG6Z$U*C2OQ1:PIGEGEV4*2DN43U*0CEBQ/GXQFY73CIBC:GUC7QHBN83GG3NQN%976FNXEB.FJN83HB3EG3CAJTA3ANBXEBGM5J%44$28A9H0D3ZCL4JMYAZ+S-A5$XKX6T2YC 35H/ITX8GL2TK96L6SR9MU9DV5 R1JNI:E4I+C7*4M:KCY07LPMIH-O9XZQSH9R$FXQGDVBK*RZP3:*DG1W7SGT$7S%RMSG2UQYI96GGLXK6*K$X4FUTD14//EF.712U0$89NT2V457U8+9W2KQ-7LF9-DF07U$B97JJ1D7WKP/HLIJLKNF8JFHJP7NVDEBU1J6+2FBKBSH:9H2K71IE8-P- S/YAL$BBJAFBQIS0*.N*WSQVN4CPQLIZ2EU H$4U9WD.ZT$ 654CVVTM7I18KSIC3HN$W5QA2NC669WFT3:%QQAPZ206UUI2";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            Assert.AreNotEqual(TokenValidateResult.Invalid, result.ValidationResult);

            var resultDCC = result.DecodedModel as DCCVersion_1_3_0.DCCPayload;
            Assert.Null(resultDCC.DCCPayloadData.DCC.Tests[0].CBSDefinedExpirationTime);
        }
    }
}