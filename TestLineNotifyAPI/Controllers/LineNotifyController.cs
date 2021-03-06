﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestLineNotifyAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestLineNotifyAPI.Controllers
{
    [Route("api/[controller]")]
    public class LineNotifyController : Controller
    {
        private readonly string _notifyUrl;

        public LineNotifyController(IConfiguration config)
        {
            //TODO: 請先在 appsettings.json 中填入 Line Notify 服務的識別碼、密鑰、成功轉跳頁面等資訊
            var lineConfig = config.GetSection("LineNotify");

            _notifyUrl = lineConfig.GetValue<string>("notifyUrl");
        }

        // GET: api/LineNotify/SendMessage?target=PoyChang&message=HelloWorld
        /// <summary>傳送文字訊息</summary>
        /// <param name="token">令牌</param>
        /// <param name="message">訊息</param>
        [HttpGet]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage(string token, string message)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_notifyUrl);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("message", message)
                });

                await client.PostAsync("", form);
            }

            return new EmptyResult();
        }

        // POST: api/LineNotify/SendMessage
        /// <summary>傳送文字訊息</summary>
        /// <param name="msg">訊息</param>
        [HttpPost]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody]MessageModel msg)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_notifyUrl);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + msg.Token);

                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("message", msg.Message)
                });

                await client.PostAsync("", form);
            }

            return new EmptyResult();
        }

        // POST: api/LineNotify/SendWithSticker
        /// <summary>傳送官方貼圖</summary>
        /// <param name="msg">訊息</param>
        [HttpPost]
        [Route("SendWithSticker")]
        public async Task<IActionResult> SendWithSticker([FromBody]MessageModel msg)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 60);
                client.BaseAddress = new Uri(_notifyUrl);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + msg.Token);
                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("message", msg.Message),
                    new KeyValuePair<string, string>("stickerPackageId", msg.StickerPackageId),
                    new KeyValuePair<string, string>("stickerId", msg.StickerId)
                });

                await client.PostAsync("", form);
            }
            return new EmptyResult();
        }

        // POST: api/LineNotify/SendWithPicture
        /// <summary>傳送圖片檔案</summary>
        /// <param name="msg">訊息</param>
        [HttpPost]
        [Route("SendWithPicture")]
        public async Task<IActionResult> SendWithPicture([FromBody]MessageModel msg)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 60);
                client.BaseAddress = new Uri(_notifyUrl);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + msg.Token);

                var form = new MultipartFormDataContent
                {
                    {new StringContent(msg.Message), "message"},
                    {new ByteArrayContent(await new HttpClient().GetByteArrayAsync(msg.FileUri)), "imageFile", msg.Filename}
                };

                await client.PostAsync("", form);
            }

            return new EmptyResult();
        }
    }
}
