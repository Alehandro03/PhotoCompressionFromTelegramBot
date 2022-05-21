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
    public class TelegramHandler
    {
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
                    await botClient.SendTextMessageAsync(message.Chat, "Ща все буит");

                    var fileId = message.Photo.Last().FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;
                    double firstSize = Convert.ToDouble(fileInfo.FileSize);
                    Console.WriteLine(firstSize);

                    double secondSize = 0;
                    var dataDir = @"C:\Users\azaro\Desktop\photo.jpeg";

                    //string destinationFilePath = $"../downloaded.file";
                    await using FileStream fileStream = System.IO.File.OpenWrite(dataDir);
                    await botClient.DownloadFileAsync(
                        filePath: filePath,
                        destination: fileStream);
                    fileStream.Close();

                    using (Image image = Image.Load(dataDir))
                    {
                        JpegOptions options = new JpegOptions();
                        options.CompressionType = JpegCompressionMode.Progressive;
                        image.Save(@"C:\Users\azaro\Desktop\photo_out.jpg", options);
                    }


                    using (var file = new FileStream(@"C:\Users\azaro\Desktop\photo_out.jpg", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        secondSize = file.Length;
                        string size = Convert.ToString(100 - ((secondSize / firstSize) * 100));
                        await Bot.SendPhotoAsync(
                            chatId: message.Chat.Id,
                            photo: new InputOnlineFile(file),
                            caption: size);
                    }
                    return;
                }
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}
