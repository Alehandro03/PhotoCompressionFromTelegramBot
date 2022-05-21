using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.InputFiles;
//using Aspose.Imaging;
//using Aspose.Imaging.ImageOptions;
using Telegram.Bot.Types.Enums;
using Aspose.Imaging.FileFormats.Jpeg;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.IO;

namespace PhotoCompressionFromTelegramBot
{

    class Program
    {
        static string token = Environment.GetEnvironmentVariable("TG_TOKEN");

        static ITelegramBotClient bot = new TelegramBotClient(token);
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
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
                    double firstSize = Convert.ToDouble(message.Photo[message.Photo.Length - 1].FileSize);
                    Console.WriteLine(firstSize);

                    double secondSize = 0;
                    var dataDir = @"C:\Users\azaro\Desktop\photo.png";

                    //string destinationFilePath = $"../downloaded.file";
                    await using FileStream fileStream = System.IO.File.OpenWrite(dataDir);
                    await botClient.DownloadFileAsync(
                        filePath: filePath,
                        destination: fileStream);
                    fileStream.Close();

                    //using (Image image = Image.Load(dataDir))
                    //{
                    //    JpegOptions options = new JpegOptions();
                    //    options.CompressionType = JpegCompressionMode.Progressive;
                    //    image.Save(@"C:\Users\azaro\Desktop\photo_out.jpg", options);
                    //}


                    Image image = compressImage(dataDir, message.Photo.Last().Width, message.Photo.Last().Height, 70);
                    image.Save(@"C:\Users\azaro\Desktop\photo_out.png");


                    using (var file = new FileStream(@"C:\Users\azaro\Desktop\photo_out.png", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        secondSize = file.Length;
                        Console.WriteLine(secondSize);
                        string size = Convert.ToString(100 - ((secondSize / firstSize) * 100));
                        await bot.SendPhotoAsync(
                            chatId: message.Chat.Id,
                            photo: new InputOnlineFile(file),
                            caption: size);
                    }
                    return;
                }
                if (message.Document is not null)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Ща все буит");

                    var fileId = message.Document.FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;
                    double firstSize = Convert.ToDouble(message.Document.FileSize);
                    Console.WriteLine(firstSize);

                    double secondSize = 0;
                    var dataDir = @"C:\Users\azaro\Desktop\photo.png";

                    //string destinationFilePath = $"../downloaded.file";
                    await using FileStream fileStream = System.IO.File.OpenWrite(dataDir);
                    await botClient.DownloadFileAsync(
                        filePath: filePath,
                        destination: fileStream);
                    fileStream.Close();

                    //using (Image image = Image.Load(dataDir))
                    //{
                    //    JpegOptions options = new JpegOptions();
                    //    options.CompressionType = JpegCompressionMode.Progressive;
                    //    image.Save(@"C:\Users\azaro\Desktop\photo_out.jpg", options);
                    //}


                    Image image = compressImage(dataDir, 100, 100, 70);
                    image.Save(@"C:\Users\azaro\Desktop\photo_out.png");


                    using (var file = new FileStream(@"C:\Users\azaro\Desktop\photo_out.png", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        secondSize = file.Length;
                        Console.WriteLine(secondSize);
                        string size = Convert.ToString(100 - ((secondSize / firstSize) * 100));
                        await bot.SendPhotoAsync(
                            chatId: message.Chat.Id,
                            photo: new InputOnlineFile(file),
                            caption: size);
                    }
                    return;
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private static Image compressImage(string fileName, int newWidth, int newHeight,
                            int newQuality)   // set quality to 1-100, eg 50
        {
            using (Image image = Image.FromFile(fileName))
            using (Image memImage = new Bitmap(image))
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                MemoryStream memStream = new MemoryStream();
                memImage.Save(memStream, myImageCodecInfo, myEncoderParameters);
                Image newImage = Image.FromStream(memStream);
                ImageAttributes imageAttributes = new ImageAttributes();
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode =
                      System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;  //**
                    g.DrawImage(newImage, new Rectangle(Point.Empty, newImage.Size), 0, 0,
                      newImage.Width, newImage.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                return newImage;
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
