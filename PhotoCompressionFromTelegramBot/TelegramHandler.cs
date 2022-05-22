using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Enums;
using PhotoCompressionFromTelegramBot.BusinessLayer;
using System.Drawing;

namespace PhotoCompressionFromTelegramBot
{
    public class TelegramHandler
    {
        const string saveDataDir = @"C:\Users\azaro\Desktop\photo.png";
        const string outDataDir =  @"C:\Users\azaro\Desktop\photo_out.png";
        //Поменять на системные для сервака
        public ITelegramBotClient Bot { get; set; }
        public TelegramHandler(string token)
        {
            Bot = new TelegramBotClient(token);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null && message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    await botClient.SendTextMessageAsync(message.Chat, "Отправь мне фото и я заставлю его похудеть!");
                    return;
                }
                if (message.Photo is not null)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Сжимаю изображение");
                    double firstSize = Convert.ToDouble(message.Photo.Last().FileSize);
                    double secondSize;

                    await DownloadFile(saveDataDir, botClient, message.Photo.Last().FileId);

                    Image image = Compressor.compressImage(saveDataDir, 0);
                    image.Save(outDataDir);

                    SendFile(firstSize, message.Chat.Id);
                    return;
                }
                if (message.Document is not null)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Сжимаю изображение");

                    double firstSize = Convert.ToDouble(message.Document.FileSize);
                    Console.WriteLine(firstSize);

                    double secondSize;

                    await DownloadFile(saveDataDir, botClient, message.Document.FileId);

                    Image image = Compressor.compressImage(saveDataDir, 70);
                    image.Save(outDataDir);

                    SendFile(firstSize, message.Chat.Id);
                    return;
                }
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private async Task DownloadFile(string dataDir, ITelegramBotClient botClient, string fileId)
        {
            var fileInfo = await botClient.GetFileAsync(fileId);
            var filePath = fileInfo.FilePath;

            using (FileStream fileStream = System.IO.File.OpenWrite(dataDir))
            {
                await botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream);
            }
        }

        private async Task SendFile(double firstSize, long chatId)
        {
            double secondSize;
            using (var file = new FileStream(outDataDir,
                   FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                secondSize = file.Length;

                Console.WriteLine(secondSize);
                string size = Convert.ToString(100 - ((secondSize / firstSize) * 100));

                await Bot.SendPhotoAsync(
                    chatId: chatId,
                    photo: new InputOnlineFile(file),
                    caption: size);
            }
        }
    }
}
