using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;


namespace PhotoCompressionFromTelegramBot
{
    internal class TelegramBotStarter
    {
        readonly string token;
        readonly TelegramHandler handler;
        const string downloadPathVariableName = "DOWNLOAD_PATH";
        const string downloadResizePathVariableName = "DOWNLOAD_RESIZE_PATH";

        public TelegramBotStarter()
        {
            token = Environment.GetEnvironmentVariable("TG_TOKEN");
            handler = new TelegramHandler(
                token,
                Environment.GetEnvironmentVariable(downloadPathVariableName),
                Environment.GetEnvironmentVariable(downloadResizePathVariableName));
        }

        public void StartBot()
        {
            Console.WriteLine("Запущен бот " + handler.Bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            handler.Bot.StartReceiving(
                handler.HandleUpdateAsync,
                handler.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
