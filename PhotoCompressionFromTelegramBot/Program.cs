using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Enums;
using System.Drawing.Imaging;
using System.Drawing;

namespace PhotoCompressionFromTelegramBot
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
