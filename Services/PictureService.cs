using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BooruSharp.Booru;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SuccBot_master.Handlers;

namespace SuccBot.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;

        public PictureService(HttpClient http)
        {
            _http = http;
        }

        private readonly SocketGuildUser _user;

        public PictureService(SocketGuildUser user)
        {
            _user = user;
        }


        public async Task<Stream> GetPictureAsync(string url)
        {
            var resp = await _http.GetAsync(url);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<SocketGuildUser> GetUser(SocketCommandContext context, string message = null)
        {
            var user = context.Message.MentionedUsers.FirstOrDefault();

            if (message == null)
            {
                user = (SocketGuildUser)context.User;
            }
            if (user == null)
            {
                var users = context.Guild.Users.ToList();
                List<SocketGuildUser> userList = new List<SocketGuildUser>();
                foreach (SocketGuildUser u in users)
                {
                    if (u.Username.ToLower().Contains(message.ToLower()) || ((u.Nickname ?? "").ToLower().Contains(message.ToLower())))

                    {
                        userList.Add(u);
                    }
                }

                var maybeTheRightUser = userList.First();
                return maybeTheRightUser;
            }
            else
            {
                return (SocketGuildUser)user;
            }
        }

        public async Task<Stream> EditedImageAsync(SocketGuildUser user, string backgroundName, int x, int y)
        {
            var resp = await _http.GetAsync(user.GetAvatarUrl());
            await resp.Content.ReadAsStreamAsync();
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            string backgroundImagePath = $"Modules/PictureModule/images/{backgroundName}.jpg";

            var backgroundImageStream = new FileStream(backgroundImagePath, FileMode.Open);

            backgroundImageStream.Seek(0, SeekOrigin.Begin);

            var avatarInputStream = await GetPictureAsync(url);

            avatarInputStream.Seek(0, SeekOrigin.Begin);

            var avatarOutputStream = new MemoryStream();

            var outputStream = new MemoryStream();
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(avatarInputStream))

            {

                using (Image<Rgba32> outputImage = image.Clone(c => c.ConvertToAvatar(new Size(200, 200), 100)))
                {
                    outputImage.SaveAsPng(avatarOutputStream);
                    avatarOutputStream.Seek(0, SeekOrigin.Begin);
                }
            }

            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(backgroundImageStream))
            {
                image.Mutate(c => c.ApplyScalingWaterMark(avatarOutputStream, x, y));
                image.SaveAsPng(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            try
            {
                return outputStream;
            }
            finally
            {
                avatarInputStream.Dispose();
                avatarOutputStream.Dispose();
                backgroundImageStream.Dispose();
            }
        }

        public async Task<Embed> HousekiSearchAsync(string tag, SocketTextChannel channel, string title = "This makes me *rock* hard...")
        {
            var tags = $"houseki_no_kuni -bad_anatomy {tag}";
            return await GelbooruSearchAsync(tags, channel, title);
        }

        public async Task<Embed> NsfwAsync(string tag, SocketTextChannel channel, string title = "Oh my")
        {
            var tags = $"rating:explicit -bad_anatomy {tag}";
            if (!(channel.IsNsfw))
            {
                return await EmbedHandler.CreateErrorEmbed("Nsfw", "You must be in Nsfw channel to use this command");
            }
            return await GelbooruSearchAsync(tags, channel, title);
        }
        public async Task<Embed> GelbooruSearchAsync(string tag, SocketTextChannel channel, string title = null)
        {

            if (tag == null)
            {
                return await EmbedHandler.CreateErrorEmbed("Gelbooru command", "You forgot to add tags");
            }

            else
            {
                if (!(channel.IsNsfw))
                {
                    tag = $"-bad_anatomy {tag} rating:safe";
                }
                try
                {
                    Gelbooru booru = new Gelbooru();
                    var result = await booru.GetRandomImage(tag);
                    return await EmbedHandler.CreateBasicEmbed(title ?? "I hope y-you will like it:", "", result.fileUrl.ToString(), Color.Gold);
                }
                catch (Exception ex)
                {
                    return await EmbedHandler.CreateErrorEmbed(ex.Source, ex.Message);
                }
            }
        }
    }
}

