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

    class Program
    {

        static void Main(string[] args)
        {
            TelegramBotStarter telegramBotStarter = new TelegramBotStarter();
            telegramBotStarter.StartBot();
        }
    }
}
