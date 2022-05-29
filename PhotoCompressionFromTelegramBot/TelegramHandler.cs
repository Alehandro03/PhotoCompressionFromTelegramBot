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
        readonly string saveDataDir;
        readonly string outDataDir;
        
        public ITelegramBotClient Bot { get; set; }
        public TelegramHandler(string token, string savePath, string saveOutPath)
        {
            Bot = new TelegramBotClient(token);
            saveDataDir = savePath;
            outDataDir = saveOutPath;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message!.Text != null && message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat!, "Добро пожаловать на борт, добрый путник!");
                    await botClient.SendTextMessageAsync(message.Chat!, "Отправь мне фото и я заставлю его похудеть!");
                    return;
                }
                if ((message.Photo is not null && message.Photo.Last().FileSize > 20971520) ||
                    (message.Document is not null && message.Document.FileSize > 20971520))
                {
                    await botClient.SendTextMessageAsync(message.Chat!, "Файл слишком большой");
                    return;
                }
                if (message.Photo is not null)
                {
                    await botClient.SendTextMessageAsync(message.Chat!, "Сжимаю изображение");
                    double firstSize = Convert.ToDouble(message.Photo.Last().FileSize);

                    string savePath = saveDataDir + message.MessageId;
                    await DownloadFile(savePath, botClient, message.Photo.Last().FileId);

                    Image image = Compressor.compressImage(savePath, 0);
                    string outSavePath = outDataDir + message.MessageId;
                    image.Save(outSavePath);

                    await SendFile(firstSize, message.Chat.Id, outSavePath);
                    await CleanUp(savePath, outSavePath);
                    return;
                }
                if (message.Document is not null)
                {
                    await botClient.SendTextMessageAsync(message.Chat!, "Сжимаю изображение");

                    double firstSize = Convert.ToDouble(message.Document.FileSize);
                    Console.WriteLine(firstSize);

                    string savePath = saveDataDir + message.MessageId + ".png";
                    await DownloadFile(savePath, botClient, message.Document.FileId);

                    Image image = Compressor.compressImage(savePath, 70);
                    string outSavePath = outDataDir + message.MessageId + ".png";
                    image.Save(outSavePath);

                    await SendFile(firstSize, message.Chat.Id, outSavePath);
                    await CleanUp(savePath, outSavePath);
                    return;
                }
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient,
            Exception exception, CancellationToken cancellationToken)
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
                    filePath: filePath!,
                    destination: fileStream);
            }
        }

        private async Task SendFile(double firstSize, long chatId, string path)
        {
            double secondSize;
            using (var file = new FileStream(path,
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

        private async Task CleanUp(string savePath, string outPath)
        {
            System.IO.File.Delete(savePath);
            System.IO.File.Delete(outPath);
        }
    }
}
