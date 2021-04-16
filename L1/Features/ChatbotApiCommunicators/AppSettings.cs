using L1.Features.ChatbotApiCommunicators;

namespace L1.Features.TelegramCommunicators
{
    public partial interface IAppSettings
    {
        ChatbotApiSettings ChatbotApiSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public ChatbotApiSettings ChatbotApiSettings { get; set; }
    }
}