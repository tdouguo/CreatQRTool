using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace Tool
{
    public class CreatQRTool
    {
        /// <summary>
        /// 定义方法生成二维码 
        /// </summary>
        /// <param name="textForEncoding">需要生产二维码的字符串</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns></returns>       
        private static Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }

        static Texture2D rqcodeTexture2d;

        public static Texture2D CreatQRCode(string text)
        {
            if (rqcodeTexture2d == null)
            {
                rqcodeTexture2d = new Texture2D(256, 256);
            }

            rqcodeTexture2d.ClearRequestedMipmapLevel();
            //二维码写入图片    
            var color32 = Encode(text, rqcodeTexture2d.width, rqcodeTexture2d.height);
            rqcodeTexture2d.SetPixels32(color32);
            rqcodeTexture2d.Apply();
            return rqcodeTexture2d;
        }
        
        public static void AutoGetQR(string text, int width, int height, Action<Sprite> onCompleted)
        {
            Texture2D texture2D =CreatQRCode(text);
            if (texture2D)
            {
                Sprite sprite = Util.TextureToSprite(texture2D);
                onCompleted?.Invoke(sprite);
                return;
            }
            RequestAPI(text, width, height, (sprite) =>
            {
                if (sprite)
                {
                    onCompleted?.Invoke(sprite);
                }
                else
                {
                    RequestCLI(text, (_sprite) => { onCompleted?.Invoke(_sprite); });
                }
            });
        }


        public static void RequestAPI(string text, int width, int height, Action<Sprite> onCompleted)
        {
            string url = $"https://api.qrserver.com/v1/create-qr-code/?size={width}×{height}&data={text}";
            ReqeustNetImg(url, (texture) =>
            {
                if (texture)
                {
                    Sprite sprite = Util.TextureToSprite(texture);
                    onCompleted?.Invoke(sprite);
                }
                else
                {
                    onCompleted?.Invoke(null);
                }
            });
        }

        public static void RequestCLI(string text, Action<Sprite> onCompleted)
        {
            try
            {
                string url = $"https://cli.im/api/qrcode/code?text={text}";
                ReqeustNetText(url, (netText) =>
                {
                    //获取'src=" //' 后所有的数据
                    string s = netText.Substring(netText.IndexOf("<img src=") + 12,
                        netText.Length - (netText.IndexOf("<img src=") + 12));
                    //截取src="" 内部的链接地址，不包括'//'
                    string result = s.Substring(0, s.IndexOf("\""));

                    ReqeustNetImg(result, (texture) =>
                    {
                        Sprite sprite = Util.TextureToSprite(texture);
                        onCompleted?.Invoke(sprite);
                    });
                });
            }
            catch (Exception e)
            {
                Debug.LogError("CLI QRCode error " + e.Message);
                onCompleted?.Invoke(null);
            }
        }

        private static void ReqeustNetText(string url, Action<string> onCompleted)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            var ao = request.SendWebRequest();
            ao.completed += (_ao) =>
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    onCompleted?.Invoke(request.downloadHandler.text);
                }
                else
                {
                    onCompleted?.Invoke(null);
                }
            };
        }

        private static void ReqeustNetImg(string url, Action<Texture> onCompleted)
        {
            Util.RequestImage(url, onCompleted);
        }
    }
}
