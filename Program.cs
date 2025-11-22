using System;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.Enums.SafetyEnums;
using VkNet.Enums; // для DocMessageType
using System.IO; // для File.ReadAllBytesAsync
using VkNet.Enums;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;
using System.Net;
using VkNet.Enums.StringEnums;
using Newtonsoft.Json.Linq;
using System.Xml;
// using VkNet.Model.RequestParams;

// using VkNet.Model.GroupUpdate;

class Program
{
    static async Task Main(string[] args)
    {
        
        var xmlDoc = new XmlDocument();
                string Config()
        {
            string dockerPath = "config.xml";
            if (File.Exists(dockerPath))
            {
                Console.WriteLine($"✅ Использую Docker-путь: {dockerPath}");
                return dockerPath;
            }
            string macPath = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/config.xml";
            if (File.Exists(macPath))
            {
                Console.WriteLine($"✅ Использую Mac-путь: {macPath}");
                return macPath;
            }

            Console.WriteLine("⚠️ Файл не найден ни по одному пути!");
            return string.Empty; // или можно вернуть дефолтный путь
        }
        xmlDoc.Load(Config());
        string accessToken = xmlDoc.SelectSingleNode("/Configuration/BotSettings/Token")?.InnerText;
        string? groupIdStr = xmlDoc.SelectSingleNode("/Configuration/BotSettings/groupId")?.InnerText;
        if (!ulong.TryParse(groupIdStr, out ulong groupId))
        {
            Console.WriteLine("Ошибка: неверный groupId в config.xml");
            return;
        }

        if (string.IsNullOrEmpty(accessToken) || groupId == null)
        {
            Console.WriteLine("Ошибка: не переданы VK_TOKEN или GROUP_ID!");
            return;
        }


        var bot = new VkApi();
        await bot.AuthorizeAsync(new ApiAuthParams { AccessToken = accessToken });

        Console.WriteLine("=== Бот запущен ===");
        Console.WriteLine($"Группа ID: {groupId}");

        // Проверим сам токен
        var currentUser = await bot.Users.GetAsync(new long[] { });
        if (currentUser.Any())
        {
            Console.WriteLine($"Авторизован как пользователь: {currentUser.First().FirstName} {currentUser.First().LastName}");
        }
        else
        {
            //достаем имя групы и айдишник которая запусается
            var group = await bot.Groups.GetByIdAsync(null, null, null);
            if (group != null)
            {
                foreach (var g in group)
                {
                    Console.WriteLine($"Авторизован как группа: {g.Name} (ID: {g.Id})");
                }
            }
        }

        // 1️⃣ создаём обычную клавиатуру (не inline, чтобы она была внизу чата)
        var mainKeyboard = new MessageKeyboard
        {
            Inline = false,
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "🎨 Кисти",
                            Payload = "{\"cmd\":\"show_brushes\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "📱 Инструкция iOS: PNG или JPG",
                            Payload = "{\"cmd\":\"instruction_ios\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "🤖 Инструкция Android: PNG или JPG",
                            Payload = "{\"cmd\":\"instruction_android\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "💻 Инструкция ПК: PNG или JPG",
                            Payload = "{\"cmd\":\"instruction_pc\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                }
            }
        };



        var brushKeyboard = new MessageKeyboard
        {
            Inline = true,
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "🦊 Fur Кисти",
                            Payload = "{\"cmd\":\"brush1\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "🖋 Кисти #2",
                            Payload = "{\"cmd\":\"brush2\"}"
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "✒️ Кисти #3",
                            Payload = "{\"cmd\":\"brush3\"}"
                        },
                        Color = KeyboardButtonColor.Negative
                    }
                }
            }
        };

        var brushKeyboard2 = new MessageKeyboard
        {
            Inline = true,
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "🖌 Кисти #4",
                            Payload = "{\"cmd\":\"brush1\"}"
                        },
                        Color = KeyboardButtonColor.Positive
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "🖋 Кисти #5",
                            Payload = "{\"cmd\":\"brush2\"}"
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Callback,
                            Label = "✒️ Кисти #6",
                            Payload = "{\"cmd\":\"brush3\"}"
                        },
                        Color = KeyboardButtonColor.Negative
                    }
                }
            }
        };

        string basePath = AppContext.BaseDirectory;

        var longPollServer = await bot.Groups.GetLongPollServerAsync(groupId);

        while (true)
        {
            try
            {
                var updates = await bot.Groups.GetBotsLongPollHistoryAsync(
                    new BotsLongPollHistoryParams
                    {
                        Server = longPollServer.Server,
                        Key = longPollServer.Key,
                        Ts = longPollServer.Ts,
                        Wait = 25
                    });
                    
                foreach (var update in updates.Updates)
                {
                    switch (update.Instance)
                    {
                        //ловим колбек
                        case MessageEvent messageEvent:

                            Console.WriteLine($"EventId: {messageEvent.EventId}");
                            Console.WriteLine($"UserId: {messageEvent.UserId}");
                            Console.WriteLine($"PeerId: {messageEvent.PeerId}");
                            Console.WriteLine($"ConversationMessageId: {messageEvent.ConversationMessageId}");
                            Console.WriteLine($"Payload: {messageEvent.Payload}");
                            Console.WriteLine($"Raw: {messageEvent?.GetType().FullName}");

                            //Обробатываем колбек
                            if (messageEvent.Payload.Contains("{\"cmd\":\"Instruction-ios\"}"))
                            {
                                // 1️⃣ обрабатываем callback "крутилку"
                                await bot.Messages.SendMessageEventAnswerAsync(
                                    messageEvent.EventId,
                                    messageEvent.UserId.Value,
                                    messageEvent.PeerId.Value,
                                    eventData: new EventData
                                    {
                                        Type = MessageEventType.ShowSnackbar,
                                        Text = "Приносим свои извинения, раздел находится в разработке."
                                    }
                                );
                                // 🧩 Теперь редактируем сообщение, например, меняем текст
                                // await bot.Messages.EditAsync(new MessageEditParams
                                // {
                                //     PeerId = messageEvent.PeerId.Value,
                                //     ConversationMessageId = messageEvent.ConversationMessageId.Value,
                                //     Message = "Вы выбрали 🖌 Кисть 1",
                                //     // можно также передать новую клавиатуру
                                //     Keyboard = brushKeyboard2
                                // });
                            }

                            //Обробатываем колбек
                            if (messageEvent.Payload.Contains("{\"cmd\":\"brush1\"}"))
                            {
                                // 1️⃣ обрабатываем callback "крутилку"
                                await bot.Messages.SendMessageEventAnswerAsync(
                                    messageEvent.EventId,
                                    messageEvent.UserId.Value,
                                    messageEvent.PeerId.Value,
                                    eventData: new EventData
                                    {
                                        Type = MessageEventType.ShowSnackbar,
                                        Text = "Fur Brush Set активирован. ✅"
                                    }
                                );

                                long? groupId1 = 69383700; // ID вашей группы

                                // 1️⃣ Получаем URL для загрузки фото
                                var uploadServer = await bot.Photo.GetMessagesUploadServerAsync(groupId1);

                                using var httpClient = new HttpClient();
                                using var form = new MultipartFormDataContent();
                                using var fileStream = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/Fur.PNG");

                                form.Add(new StreamContent(fileStream), "photo", "Fur.PNG");

                                // 2️⃣ Загружаем фото на сервер VK
                                var response = await httpClient.PostAsync(uploadServer.UploadUrl, form);
                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                // 3️⃣ Сохраняем фото на серверах VK
                                var savedPhotos = await bot.Photo.SaveMessagesPhotoAsync(jsonResponse);
                                var photo = savedPhotos.FirstOrDefault();

                                // 4️⃣ Отправляем сообщение с фото
                                await bot.Messages.SendAsync(new MessagesSendParams
                                {
                                    PeerId = messageEvent.PeerId.Value,
                                    Message = "🖌 🦊 Fur Brush Set",
                                    Attachments = new List<MediaAttachment> { photo },
                                    RandomId = new Random().Next()
                                });

                                // 1️⃣ Получаем сервер для загрузки документов
                                var uploadServer1 = await bot.Docs.GetMessagesUploadServerAsync(messageEvent.PeerId.Value);

                                // 2️⃣ Готовим запрос
                                using var httpClient1 = new HttpClient();
                                using var form1 = new MultipartFormDataContent();
                                using var fileStream1 = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Brush/FurBrushSet.zip");

                                // ⚠️ ВАЖНО: поле должно называться "file", иначе VK не примет
                                form1.Add(new StreamContent(fileStream1), "file", "FurBrushSet.zip");

                                // 3️⃣ Загружаем ZIP на сервер VK
                                var uploadResponse1 = await httpClient1.PostAsync(uploadServer1.UploadUrl, form1);
                                string uploadJson1 = await uploadResponse1.Content.ReadAsStringAsync();

                                // 4️⃣ Сохраняем документ на серверах VK
                                var savedDoc = await bot.Docs.SaveAsync(uploadJson1, "FurBrushSet", "zip");
                                // var document = savedDoc.Doc;
                                var document = savedDoc.FirstOrDefault()?.Instance as MediaAttachment;
                                
                                if (document != null)
                                {
                                    // 4. Отправляем пользователю документ
                                    await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                    {
                                        PeerId = messageEvent.PeerId.Value,
                                        Message = "✅ Вот ваши кисти Fur Brush Set 📎",
                                        Attachments = new List<MediaAttachment> { document },
                                        // Attachments = new List<VkNet.Model.Attachments.MediaAttachment> { document },
                                        RandomId = new Random().Next()
                                    });
                                }


                                // 1. Берём сервер для загрузки документа в ЛС
                                // var uploadServer = await bot.Docs.GetMessagesUploadServerAsync(message.PeerId.Value);

                                // // 2. Заливаем файл на сервер ВК
                                // using var wc = new WebClient();
                                // var responseJson = Encoding.ASCII.GetString(
                                //     wc.UploadFile(uploadServer.UploadUrl, tempPath)
                                // );

                                // // 3. Сохраняем документ в ВК
                                // var savedDocs = await bot.Docs.SaveAsync(responseJson, "Обработанное изображение");
                                // var document = savedDocs.FirstOrDefault()?.Instance as MediaAttachment;

                                // if (document != null)
                                // {
                                //     // 4. Отправляем пользователю документ
                                //     await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                //     {
                                //         PeerId = message.PeerId.Value,
                                //         Message = "✅ Вот твой обработанный jpeg 📎",
                                //         Attachments = new List<MediaAttachment> { document },
                                //         // Attachments = new List<VkNet.Model.Attachments.MediaAttachment> { document },
                                //         RandomId = new Random().Next()
                                //     });
                                // }

                                // var uploadServer1 = await bot.Docs.GetMessagesUploadServerAsync(groupId1);
                                // using var httpClient1 = new HttpClient();
                                // using var form1 = new MultipartFormDataContent();
                                // using var fileStream1 = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Brush/FurBrushSet.zip");

                                // form1.Add(new StreamContent(fileStream1), "Brush", "FurBrushSet.zip");

                                // 🧩 Теперь редактируем сообщение, например, меняем текст
                                // await bot.Messages.EditAsync(new MessageEditParams
                                // {
                                //     PeerId = messageEvent.PeerId.Value,
                                //     ConversationMessageId = messageEvent.ConversationMessageId.Value,
                                //     Message = "Вы выбрали 🖌 Кисть 1",
                                //     // можно также передать новую клавиатуру
                                //     Keyboard = brushKeyboard2
                                // });
                            }

                            if (messageEvent.Payload.Contains("{\"cmd\":\"brush2\"}"))
                            {
                                // 1️⃣ обрабатываем callback "крутилку"
                                await bot.Messages.SendMessageEventAnswerAsync(
                                    messageEvent.EventId,
                                    messageEvent.UserId.Value,
                                    messageEvent.PeerId.Value,
                                    eventData: new EventData
                                    {
                                        Type = MessageEventType.ShowSnackbar,
                                        Text = "Этот набор кистей скоро будет доступен. ⛔️"
                                    }
                                );
                                // 🧩 Теперь редактируем сообщение, например, меняем текст
                                // await bot.Messages.EditAsync(new MessageEditParams
                                // {
                                //     PeerId = messageEvent.PeerId.Value,
                                //     ConversationMessageId = messageEvent.ConversationMessageId.Value,
                                //     Message = "Вы выбрали 🖌 Кисть 1",
                                //     // можно также передать новую клавиатуру
                                //     Keyboard = brushKeyboard2
                                // });
                            }
                            
                            if (messageEvent.Payload.Contains("{\"cmd\":\"brush3\"}"))
                            {
                                // 1️⃣ обрабатываем callback "крутилку"
                                await bot.Messages.SendMessageEventAnswerAsync(
                                    messageEvent.EventId,
                                    messageEvent.UserId.Value,
                                    messageEvent.PeerId.Value,
                                    eventData: new EventData
                                    {
                                        Type = MessageEventType.ShowSnackbar,
                                        Text = "Этот набор кистей скоро будет доступен. ⛔️"
                                    }
                                );
                                 // 🧩 Теперь редактируем сообщение, например, меняем текст
                                // await bot.Messages.EditAsync(new MessageEditParams
                                // {
                                //     PeerId = messageEvent.PeerId.Value,
                                //     ConversationMessageId = messageEvent.ConversationMessageId.Value,
                                //     Message = "Вы выбрали 🖌 Кисть 1",
                                //     // можно также передать новую клавиатуру
                                //     Keyboard = brushKeyboard2
                                // });
                            }
                            break;

                        case MessageNew messageNew:
                            var message = messageNew.Message;
                            var text = message.Text?.ToLower() ?? "";
                            var Id = messageNew.Message.ChatId;
                            Console.WriteLine(text);

                            if (message.Payload != null && message.Payload.Contains("\"cmd\":\"show_brushes\""))
                            {
                                await bot.Messages.SendAsync(new MessagesSendParams
                                {
                                    PeerId = message.PeerId.Value,
                                    Message = "выбери кисти 👇",
                                    Keyboard = brushKeyboard,
                                    RandomId = new Random().Next()
                                });
                                break;
                            }

                            // if (message.Payload != null && message.Payload.Contains("\"cmd\":\"Instruction\""))
                            // {
                            //     long? groupId1 = 69383700; // ID вашей группы без минуса

                            //     // 1️⃣ Получаем сервер загрузки для сообщений
                            //     var uploadServer = await bot.Photo.GetMessagesUploadServerAsync(groupId1);

                            //     // 2️⃣ Загружаем фото на сервер VK через WebClient
                            //     using var stream = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/logo.PNG");
                            //     using var client = new WebClient();

                            //     byte[] responseBytes = client.UploadData(uploadServer.UploadUrl, ReadStream(stream));

                            //     // 3️⃣ Преобразуем ответ в строку (JSON)
                            //     string jsonResponse = Encoding.UTF8.GetString(responseBytes);

                            //     // 4️⃣ Сохраняем фото через SaveMessagesPhotoAsync
                            //     var savedPhotos = await bot.Photo.SaveMessagesPhotoAsync(jsonResponse);
                            //     var photo = savedPhotos.FirstOrDefault();

                            //     // 5️⃣ Отправляем фото пользователю
                            //     await bot.Messages.SendAsync(new MessagesSendParams
                            //     {
                            //         PeerId = message.PeerId.Value,
                            //         Message = "📘 Инструкция",
                            //         Attachments = new List<MediaAttachment> { photo },
                            //         RandomId = new Random().Next()
                            //     });

                            //     break;
                            // }

                            // // Метод для чтения потока в массив байт
                            // byte[] ReadStream(Stream stream)
                            // {
                            //     using var ms = new MemoryStream();
                            //     stream.CopyTo(ms);
                            //     return ms.ToArray();
                            // }

                            if (message.Payload != null && message.Payload.Contains("\"cmd\":\"instruction_ios\""))
                            {
                                long? groupId1 = 69383700; // ID вашей группы

                                // 1️⃣ Получаем URL для загрузки фото
                                var uploadServer = await bot.Photo.GetMessagesUploadServerAsync(groupId1);

                                using var httpClient = new HttpClient();
                                using var form = new MultipartFormDataContent();
                                using var fileStream = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/Instruction_ios.PNG");

                                form.Add(new StreamContent(fileStream), "photo", "Instruction_ios.PNG");

                                // 2️⃣ Загружаем фото на сервер VK
                                var response = await httpClient.PostAsync(uploadServer.UploadUrl, form);
                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                // 3️⃣ Сохраняем фото на серверах VK
                                var savedPhotos = await bot.Photo.SaveMessagesPhotoAsync(jsonResponse);
                                var photo = savedPhotos.FirstOrDefault();

                                // 4️⃣ Отправляем сообщение с фото
                                await bot.Messages.SendAsync(new MessagesSendParams
                                {
                                    PeerId = message.PeerId.Value,
                                    Message = "📘 Инструкция",
                                    Attachments = new List<MediaAttachment> { photo },
                                    RandomId = new Random().Next()
                                });

                                break;
                            }

                            if (message.Payload != null && message.Payload.Contains("\"cmd\":\"instruction_android\""))
                            {
                                long? groupId1 = 69383700; // ID вашей группы

                                // 1️⃣ Получаем URL для загрузки фото
                                var uploadServer = await bot.Photo.GetMessagesUploadServerAsync(groupId1);

                                using var httpClient = new HttpClient();
                                using var form = new MultipartFormDataContent();
                                using var fileStream = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/Instruction_Android.PNG");

                                form.Add(new StreamContent(fileStream), "photo", "Instruction_Android.PNG");

                                // 2️⃣ Загружаем фото на сервер VK
                                var response = await httpClient.PostAsync(uploadServer.UploadUrl, form);
                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                // 3️⃣ Сохраняем фото на серверах VK
                                var savedPhotos = await bot.Photo.SaveMessagesPhotoAsync(jsonResponse);
                                var photo = savedPhotos.FirstOrDefault();

                                // 4️⃣ Отправляем сообщение с фото
                                await bot.Messages.SendAsync(new MessagesSendParams
                                {
                                    PeerId = message.PeerId.Value,
                                    Message = "📘 Инструкция",
                                    Attachments = new List<MediaAttachment> { photo },
                                    RandomId = new Random().Next()
                                });

                                break;
                            }

                            if (message.Payload != null && message.Payload.Contains("\"cmd\":\"instruction_pc\""))
                            {
                                long? groupId1 = 69383700; // ID вашей группы

                                // 1️⃣ Получаем URL для загрузки фото
                                var uploadServer = await bot.Photo.GetMessagesUploadServerAsync(groupId1);

                                using var httpClient = new HttpClient();
                                using var form = new MultipartFormDataContent();
                                using var fileStream = File.OpenRead("/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/Instruction_PK.PNG");

                                form.Add(new StreamContent(fileStream), "photo", "Instruction_PK.PNG");

                                // 2️⃣ Загружаем фото на сервер VK
                                var response = await httpClient.PostAsync(uploadServer.UploadUrl, form);
                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                // 3️⃣ Сохраняем фото на серверах VK
                                var savedPhotos = await bot.Photo.SaveMessagesPhotoAsync(jsonResponse);
                                var photo = savedPhotos.FirstOrDefault();

                                // 4️⃣ Отправляем сообщение с фото
                                await bot.Messages.SendAsync(new MessagesSendParams
                                {
                                    PeerId = message.PeerId.Value,
                                    Message = "📘 Инструкция",
                                    Attachments = new List<MediaAttachment> { photo },
                                    RandomId = new Random().Next()
                                });

                                break;
                            }




                            // 2️⃣ Проверяем обычный текст
                            switch (text)
                            {
                                case "1":
                                    {
                                        // 1️⃣ Отправляем сообщение с кнопкой
                                        var sentMessageId = await bot.Messages.SendAsync(new MessagesSendParams
                                        {
                                            PeerId = message.PeerId.Value,
                                            Message = "Keyboard off",
                                            Keyboard = null,
                                            // FromGroup = true,
                                            // Payload = "{\"cmd\":\"show_brushes\"}",
                                            RandomId = new Random().Next()
                                        });
                                        break;
                                    }
                                case "начать":
                                    await bot.Messages.SendAsync(new MessagesSendParams
                                    {
                                        PeerId = message.PeerId.Value,
                                        Message = "Кисти:",
                                        Keyboard = mainKeyboard,
                                        RandomId = new Random().Next()
                                    });
                                    break;

                                case "Привет":
                                    {
                                        //             // 1️⃣ Отправляем сообщение с кнопкой
                                        var sentMessageId = await bot.Messages.SendAsync(new MessagesSendParams
                                        {
                                            PeerId = message.PeerId.Value,
                                            Message = "Привет! 👋",
                                            // Keyboard = keyboard,
                                            // FromGroup = true,
                                            Payload = "{\"cmd\":\"show_brushes\"}",
                                            RandomId = new Random().Next()
                                        });
                                        break;
                                    }
                            }

                            // Обработка вложений (фото, документы)
                            if (message.Attachments != null && message.Attachments.Count > 0)
                            {
                                foreach (var attachment in message.Attachments)
                                {
                                    switch (attachment.Instance)
                                    {
                                        case VkNet.Model.Document doc when doc.Ext == "jpg":
                                            Console.WriteLine($"Получен изображение: {doc.Title}");
                                            // await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                            // {
                                            //     PeerId = message.PeerId.Value,
                                            //     Message = "Принял jpg, но работаю только с PNG. Извините, мы работаем над этим.",
                                            //     RandomId = new Random().Next()
                                            // });
                                            try
                                            {
                                                // Скачиваем изображение
                                                using (var httpClient = new HttpClient())
                                                {
                                                    var imageData = await httpClient.GetByteArrayAsync(doc.Uri);

                                                    // Конвертируем в черно-белое
                                                    using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageData))
                                                    {
                                                        image.Mutate(x =>
                                                        {
                                                            x.Grayscale(); // сначала переводим в ЧБ
                                                            // x.BinaryThreshold(0.5f); // 0.5f = порог уберает серый фон, можно поиграться 0.3..0.7
                                                        });

                                                        // === ВСТАВКА НАЛОЖЕНИЯ ===
                                                        string OverlayPath = Path.Combine(basePath, "Overlay", $"2.PNG");
                                                        string pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/2.PNG";

                                                        using (var overlay = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(GetPath(OverlayPath, pathMacOS)))
                                                        {
                                                            // ⚡ Подгоняем размер оверлея под размер основного изображения
                                                            // overlay.Mutate(o => o.Resize(image.Width, image.Height));
                                                            overlay.Mutate(o => o.Resize(
                                                            new ResizeOptions
                                                            {
                                                                Size = new Size(image.Width, image.Height),
                                                                Mode = ResizeMode.Stretch,      // вписывает целиком, добавляя пустые поля (сохраняется пропорция)
                                                                Position = AnchorPositionMode.Center
                                                            }));

                                                            // Наложим оверлей поверх обработанного изображения
                                                            //альфа чб оконтовка 
                                                            image.Mutate(x =>
                                                        {
                                                            x.DrawImage(overlay, new Point(0, 0), 1f); // 1f = прозрачность
                                                        });

                                                            // Лого для штампа

                                                            OverlayPath = Path.Combine(basePath, "Overlay", $"logo.PNG");
                                                            pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/logo.PNG";
                                                            using var watermark = SixLabors.ImageSharp.Image.Load<Rgba32>(GetPath(OverlayPath, pathMacOS));

                                                            // Преобразуем логотип: черный → белый, белый → черный
                                                            watermark.Mutate(ctx =>
                                                            {
                                                                for (int y = 0; y < watermark.Height; y++)
                                                                {
                                                                    for (int x = 0; x < watermark.Width; x++)
                                                                    {
                                                                        var pixel = watermark[x, y];
                                                                        byte brightness = (byte)(255 - (pixel.R + pixel.G + pixel.B) / 3);

                                                                        // Уменьшаем альфу для большей прозрачности текста
                                                                        byte alpha = (byte)(pixel.A * 0.05f); // 0.2 = 20% непрозрачности

                                                                        watermark[x, y] = new Rgba32(brightness, brightness, brightness, alpha);
                                                                    }
                                                                }

                                                                // Подгоняем размер логотипа под ширину изображения
                                                                ctx.Resize(new ResizeOptions
                                                                {
                                                                    Size = new Size(image.Width, watermark.Height * image.Width / watermark.Width),
                                                                    Mode = ResizeMode.Stretch
                                                                });
                                                            });

                                                            // Наложение внизу по центру с отступом 4px
                                                            int posX = (image.Width - watermark.Width) / 2;
                                                            int posY = image.Height - watermark.Height - 4;

                                                            image.Mutate(ctx =>
                                                            {
                                                                ctx.DrawImage(watermark, new Point(posX, posY), 1f); // opacity = 1f, альфа уже учтена в пикселях
                                                            });
                                                        }
                                                        // Сохраняем временный файл
                                                        string tempPath = $"bw_{DateTime.Now.Ticks}.jpg";
                                                        await image.SaveAsPngAsync(tempPath);

                                                        Console.WriteLine($"Файл сохранён: {tempPath}");

                                                        // ================= ЗАГРУЗКА В ВК =================

                                                        // 1. Берём сервер для загрузки документа в ЛС
                                                        var uploadServer = await bot.Docs.GetMessagesUploadServerAsync(message.PeerId.Value);

                                                        // 2. Заливаем файл на сервер ВК
                                                        using var wc = new WebClient();
                                                        var responseJson = Encoding.ASCII.GetString(
                                                            wc.UploadFile(uploadServer.UploadUrl, tempPath)
                                                        );

                                                        // 3. Сохраняем документ в ВК
                                                        var savedDocs = await bot.Docs.SaveAsync(responseJson, "Обработанное изображение");
                                                        var document = savedDocs.FirstOrDefault()?.Instance as MediaAttachment;

                                                        if (document != null)
                                                        {
                                                            // 4. Отправляем пользователю документ
                                                            await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                            {
                                                                PeerId = message.PeerId.Value,
                                                                Message = "✅ Вот твой обработанный jpg 📎",
                                                                Attachments = new List<MediaAttachment> { document },
                                                                // Attachments = new List<VkNet.Model.Attachments.MediaAttachment> { document },
                                                                RandomId = new Random().Next()
                                                            });
                                                        }

                                                        // 5. Удаляем временный файл
                                                        File.Delete(tempPath);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Ошибка: {ex.Message}");
                                                await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                {
                                                    PeerId = message.PeerId.Value,
                                                    Message = "❌ Ошибка при обработке PNG",
                                                    RandomId = new Random().Next()
                                                });
                                            }
                                            break;

                                        case VkNet.Model.Document doc when doc.Ext == "jpeg":
                                            Console.WriteLine($"Получен изображение: {doc.Title}");
                                            // await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                            // {
                                            //     PeerId = message.PeerId.Value,
                                            //     Message = "Принял jpeg, но работаю только с PNG. Извините, мы работаем над этим.",
                                            //     RandomId = new Random().Next()
                                            // });

                                            try
                                            {
                                                // Скачиваем изображение
                                                using (var httpClient = new HttpClient())
                                                {
                                                    var imageData = await httpClient.GetByteArrayAsync(doc.Uri);

                                                    // Конвертируем в черно-белое
                                                    using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageData))
                                                    {
                                                        image.Mutate(x =>
                                                        {
                                                            x.Grayscale(); // сначала переводим в ЧБ
                                                            // x.BinaryThreshold(0.5f); // 0.5f = порог уберает серый фон, можно поиграться 0.3..0.7
                                                        });

                                                        // === ВСТАВКА НАЛОЖЕНИЯ ===
                                                        string OverlayPath = Path.Combine(basePath, "Overlay", $"2.PNG");
                                                        string pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/2.PNG";

                                                        using (var overlay = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(GetPath(OverlayPath, pathMacOS)))
                                                        {
                                                            // ⚡ Подгоняем размер оверлея под размер основного изображения
                                                            // overlay.Mutate(o => o.Resize(image.Width, image.Height));
                                                            overlay.Mutate(o => o.Resize(
                                                            new ResizeOptions
                                                            {
                                                                Size = new Size(image.Width, image.Height),
                                                                Mode = ResizeMode.Stretch,      // вписывает целиком, добавляя пустые поля (сохраняется пропорция)
                                                                Position = AnchorPositionMode.Center
                                                            }));

                                                            // Наложим оверлей поверх обработанного изображения
                                                            //альфа чб оконтовка 
                                                            image.Mutate(x =>
                                                        {
                                                            x.DrawImage(overlay, new Point(0, 0), 1f); // 1f = прозрачность
                                                        });

                                                            // Лого для штампа

                                                            OverlayPath = Path.Combine(basePath, "Overlay", $"logo.PNG");
                                                            pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/logo.PNG";
                                                            using var watermark = SixLabors.ImageSharp.Image.Load<Rgba32>(GetPath(OverlayPath, pathMacOS));

                                                            // Преобразуем логотип: черный → белый, белый → черный
                                                            watermark.Mutate(ctx =>
                                                            {
                                                                for (int y = 0; y < watermark.Height; y++)
                                                                {
                                                                    for (int x = 0; x < watermark.Width; x++)
                                                                    {
                                                                        var pixel = watermark[x, y];
                                                                        byte brightness = (byte)(255 - (pixel.R + pixel.G + pixel.B) / 3);

                                                                        // Уменьшаем альфу для большей прозрачности текста
                                                                        byte alpha = (byte)(pixel.A * 0.05f); // 0.2 = 20% непрозрачности

                                                                        watermark[x, y] = new Rgba32(brightness, brightness, brightness, alpha);
                                                                    }
                                                                }

                                                                // Подгоняем размер логотипа под ширину изображения
                                                                ctx.Resize(new ResizeOptions
                                                                {
                                                                    Size = new Size(image.Width, watermark.Height * image.Width / watermark.Width),
                                                                    Mode = ResizeMode.Stretch
                                                                });
                                                            });

                                                            // Наложение внизу по центру с отступом 4px
                                                            int posX = (image.Width - watermark.Width) / 2;
                                                            int posY = image.Height - watermark.Height - 4;

                                                            image.Mutate(ctx =>
                                                            {
                                                                ctx.DrawImage(watermark, new Point(posX, posY), 1f); // opacity = 1f, альфа уже учтена в пикселях
                                                            });
                                                        }
                                                        // Сохраняем временный файл
                                                        string tempPath = $"bw_{DateTime.Now.Ticks}.jpeg";
                                                        await image.SaveAsPngAsync(tempPath);

                                                        Console.WriteLine($"Файл сохранён: {tempPath}");

                                                        // ================= ЗАГРУЗКА В ВК =================

                                                        // 1. Берём сервер для загрузки документа в ЛС
                                                        var uploadServer = await bot.Docs.GetMessagesUploadServerAsync(message.PeerId.Value);

                                                        // 2. Заливаем файл на сервер ВК
                                                        using var wc = new WebClient();
                                                        var responseJson = Encoding.ASCII.GetString(
                                                            wc.UploadFile(uploadServer.UploadUrl, tempPath)
                                                        );

                                                        // 3. Сохраняем документ в ВК
                                                        var savedDocs = await bot.Docs.SaveAsync(responseJson, "Обработанное изображение");
                                                        var document = savedDocs.FirstOrDefault()?.Instance as MediaAttachment;

                                                        if (document != null)
                                                        {
                                                            // 4. Отправляем пользователю документ
                                                            await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                            {
                                                                PeerId = message.PeerId.Value,
                                                                Message = "✅ Вот твой обработанный jpeg 📎",
                                                                Attachments = new List<MediaAttachment> { document },
                                                                // Attachments = new List<VkNet.Model.Attachments.MediaAttachment> { document },
                                                                RandomId = new Random().Next()
                                                            });
                                                        }

                                                        // 5. Удаляем временный файл
                                                        File.Delete(tempPath);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Ошибка: {ex.Message}");
                                                await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                {
                                                    PeerId = message.PeerId.Value,
                                                    Message = "❌ Ошибка при обработке jpeg",
                                                    RandomId = new Random().Next()
                                                });
                                            }

                                            break;

                                        case VkNet.Model.Document doc when doc.Ext == "png":
                                            Console.WriteLine($"Получен PNG изображение: {doc.Title}");

                                            try
                                            {
                                                // Скачиваем изображение
                                                using (var httpClient = new HttpClient())
                                                {
                                                    var imageData = await httpClient.GetByteArrayAsync(doc.Uri);

                                                    // Конвертируем в черно-белое
                                                    using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageData))
                                                    {
                                                        image.Mutate(x =>
                                                        {
                                                            x.Grayscale(); // сначала переводим в ЧБ
                                                            // x.BinaryThreshold(0.5f); // 0.5f = порог уберает серый фон, можно поиграться 0.3..0.7
                                                        });

                                                        // === ВСТАВКА НАЛОЖЕНИЯ ===
                                                        string OverlayPath = Path.Combine(basePath, "Overlay", $"2.PNG");
                                                        string pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/2.PNG";

                                                        using (var overlay = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(GetPath(OverlayPath, pathMacOS)))
                                                        {
                                                            // ⚡ Подгоняем размер оверлея под размер основного изображения
                                                            // overlay.Mutate(o => o.Resize(image.Width, image.Height));
                                                            overlay.Mutate(o => o.Resize(
                                                            new ResizeOptions
                                                            {
                                                                Size = new Size(image.Width, image.Height),
                                                                Mode = ResizeMode.Stretch,      // вписывает целиком, добавляя пустые поля (сохраняется пропорция)
                                                                Position = AnchorPositionMode.Center
                                                            }));

                                                            // Наложим оверлей поверх обработанного изображения
                                                            //альфа чб оконтовка 
                                                            image.Mutate(x =>
                                                        {
                                                            x.DrawImage(overlay, new Point(0, 0), 1f); // 1f = прозрачность
                                                        });

                                                            // Лого для штампа

                                                            OverlayPath = Path.Combine(basePath, "Overlay", $"logo.PNG");
                                                            pathMacOS = "/Users/vladislavfurazkin/Desktop/vk_bot/vk_bot_img/Overlay/logo.PNG";
                                                            using var watermark = SixLabors.ImageSharp.Image.Load<Rgba32>(GetPath(OverlayPath, pathMacOS));

                                                            // Преобразуем логотип: черный → белый, белый → черный
                                                            watermark.Mutate(ctx =>
                                                            {
                                                                for (int y = 0; y < watermark.Height; y++)
                                                                {
                                                                    for (int x = 0; x < watermark.Width; x++)
                                                                    {
                                                                        var pixel = watermark[x, y];
                                                                        byte brightness = (byte)(255 - (pixel.R + pixel.G + pixel.B) / 3);

                                                                        // Уменьшаем альфу для большей прозрачности текста
                                                                        byte alpha = (byte)(pixel.A * 0.05f); // 0.2 = 20% непрозрачности

                                                                        watermark[x, y] = new Rgba32(brightness, brightness, brightness, alpha);
                                                                    }
                                                                }

                                                                // Подгоняем размер логотипа под ширину изображения
                                                                ctx.Resize(new ResizeOptions
                                                                {
                                                                    Size = new Size(image.Width, watermark.Height * image.Width / watermark.Width),
                                                                    Mode = ResizeMode.Stretch
                                                                });
                                                            });

                                                            // Наложение внизу по центру с отступом 4px
                                                            int posX = (image.Width - watermark.Width) / 2;
                                                            int posY = image.Height - watermark.Height - 4;

                                                            image.Mutate(ctx =>
                                                            {
                                                                ctx.DrawImage(watermark, new Point(posX, posY), 1f); // opacity = 1f, альфа уже учтена в пикселях
                                                            });
                                                        }
                                                        // Сохраняем временный файл
                                                        string tempPath = $"bw_{DateTime.Now.Ticks}.png";
                                                        await image.SaveAsPngAsync(tempPath);

                                                        Console.WriteLine($"Файл сохранён: {tempPath}");

                                                        // ================= ЗАГРУЗКА В ВК =================

                                                        // 1. Берём сервер для загрузки документа в ЛС
                                                        var uploadServer = await bot.Docs.GetMessagesUploadServerAsync(message.PeerId.Value);

                                                        // 2. Заливаем файл на сервер ВК
                                                        using var wc = new WebClient();
                                                        var responseJson = Encoding.ASCII.GetString(
                                                            wc.UploadFile(uploadServer.UploadUrl, tempPath)
                                                        );

                                                        // 3. Сохраняем документ в ВК
                                                        var savedDocs = await bot.Docs.SaveAsync(responseJson, "Обработанное изображение");
                                                        var document = savedDocs.FirstOrDefault()?.Instance as MediaAttachment;

                                                        if (document != null)
                                                        {
                                                            // 4. Отправляем пользователю документ
                                                            await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                            {
                                                                PeerId = message.PeerId.Value,
                                                                Message = "✅ Вот твой обработанный PNG 📎",
                                                                Attachments = new List<MediaAttachment> { document },
                                                                // Attachments = new List<VkNet.Model.Attachments.MediaAttachment> { document },
                                                                RandomId = new Random().Next()
                                                            });
                                                        }

                                                        // 5. Удаляем временный файл
                                                        File.Delete(tempPath);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Ошибка: {ex.Message}");
                                                await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                                {
                                                    PeerId = message.PeerId.Value,
                                                    Message = "❌ Ошибка при обработке PNG",
                                                    RandomId = new Random().Next()
                                                });
                                            }
                                            break;
                                        // case VkNet.Model.Document doc when doc.Ext == "png":
                                        //     Console.WriteLine($"Получен изображение: {doc.Title}");
                                        //     await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                        //     {
                                        //         PeerId = message.PeerId.Value,
                                        //         Message = "Получил твое изображение! png",
                                        //         RandomId = new Random().Next()
                                        //     });
                                        //     break;

                                        case VkNet.Model.Document doc:
                                            Console.WriteLine($"Получен документ: {doc.Title}");
                                            await bot.Messages.SendAsync(new VkNet.Model.MessagesSendParams
                                            {
                                                PeerId = message.PeerId.Value,
                                                Message = "Получил твой файл!",
                                                RandomId = new Random().Next()
                                            });
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }

                longPollServer.Ts = updates.Ts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await Task.Delay(5000);
                longPollServer = await bot.Groups.GetLongPollServerAsync(groupId);
            }
        }
    }

    private static readonly Dictionary<long, long> SentMessages = new();

    static string GetPath(string dockerPath, string macPath)
    {
        if (File.Exists(dockerPath))
        {
            Console.WriteLine($"✅ Использую Docker-путь: {dockerPath}");
            return dockerPath;
        }

        if (File.Exists(macPath))
        {
            Console.WriteLine($"✅ Использую Mac-путь: {macPath}");
            return macPath;
        }

        Console.WriteLine("⚠️ Файл не найден ни по одному пути!");
        return string.Empty; // или можно вернуть дефолтный путь
    }
}