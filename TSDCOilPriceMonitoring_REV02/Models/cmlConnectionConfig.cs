namespace TSDCOilPriceMonitoring_REV02.Models
{
    public class cmlConnectionConfig
    {
        public string tServerDB { get; set; }
        public string tNameDB { get; set; }
        public string tUser { get; set; }
        public string tPassword { get; set; }
        public bool bIntegratedSecurity { get; set; }
        public bool bEncrypt { get; set; }
        public bool bTrustServerCertificate { get; set; }
    }
}
