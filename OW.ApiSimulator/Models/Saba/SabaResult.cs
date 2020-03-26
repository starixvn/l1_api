namespace OW.ApiSimulator.Models.Saba
{
    public abstract class SabaResult
    {
        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string Seq { get; set; }
    }
}