namespace SSICPAS.Models
{
    public class PinCodeBiometricsModel
    {
        public string PinCode { get; set; }
        public bool HasBiometrics { get; set; }
        public int Attempts { get; set; }
    }
}
