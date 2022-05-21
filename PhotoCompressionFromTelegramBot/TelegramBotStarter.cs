using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.InputFiles;
using Aspose.Imaging;
using Aspose.Imaging.ImageOptions;
using Telegram.Bot.Types.Enums;
using Aspose.Imaging.FileFormats.Jpeg;


namespace PhotoCompressionFromTelegramBot.TGLayer
{
    internal class TelegramBotStarter
    {
        readonly string token;
        readonly TelegramHandler handler;

        public TelegramBotStarter()
        {
            token = Environment.GetEnvironmentVariable("TG_TOKEN");
            handler = new TelegramHandler(token);
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
