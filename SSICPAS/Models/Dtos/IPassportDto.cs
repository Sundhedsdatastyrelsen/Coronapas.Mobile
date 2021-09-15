using System.Collections.Generic;

namespace SSICPAS.Models.Dtos
{
    public interface IPassportDto
    {
        public string DkInfo { get; set; }

        public string CustodyKey { get; set; }

        public string Dkmin { get; set; }

        public string Dkmax { get; set; }

        public List<string> EuroTest { get; set; }

        public List<string> EuroVaccine { get; set; }

        public List<string> EuroRecovery { get; set; }
    }
}