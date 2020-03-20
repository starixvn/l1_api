namespace OW.ApiSimulator.Models.OW
{
    public class NotifyCustomerLoginCall : OWCall
    {
        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }
    }
}
