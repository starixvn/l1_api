namespace L1.Features.PartnerCommunicators
{
    public static class SubjectFormatter
    {
        public static string FormatSubject(string prefix, string subject)
        {
            return $"{prefix}.{subject}";
        }
    }
}