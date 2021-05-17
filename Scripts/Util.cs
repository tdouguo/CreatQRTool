using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Tool
{
    public static partial class Util
    {
        /// <summary>
        /// 获取连接后的路径。
        /// </summary>
        /// <param name="path">路径片段。</param>
        /// <returns>连接后的路径。</returns>
        public static string GetCombinePath(params string[] path)
        {
            if (path == null || path.Length < 1)
            {
                return null;
            }

            string combinePath = path[0];
            for (int i = 1; i < path.Length; i++)
            {
                combinePath = Path.Combine(combinePath, path[i]);
            }

            return GetRegularPath(combinePath);
        }

        /// <summary>
        /// 获取规范的路径。
        /// </summary>
        /// <param name="path">要规范的路径。</param>
        /// <returns>规范的路径。</returns>
        public static string GetRegularPath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace('\\', '/');
        }

        public static void CheckDir(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            CheckDir(path);
            File.WriteAllBytes(path, bytes);
        }
        
        
        /// <summary>
        /// Texture转Sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Sprite TextureToSprite(Texture texture)
        {
            Sprite sprite = null;
            if (texture)
            {
                Texture2D t2d = (Texture2D) texture;
                sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
            }

            return sprite;
        }

        /// <summary> 双线性插值法缩放图片，等比缩放 </summary>
        public static Texture2D ScaleTextureBilinear(Texture2D originalTexture, float scaleFactor)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor),
                Mathf.CeilToInt(originalTexture.height * scaleFactor));
            float scale = 1.0f / scaleFactor;
            int maxX = originalTexture.width - 1;
            int maxY = originalTexture.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scale;
                    float targetY = y * scale;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = originalTexture.GetPixel(x1, y1);
                    Color color2 = originalTexture.GetPixel(x2, y1);
                    Color color3 = originalTexture.GetPixel(x1, y2);
                    Color color4 = originalTexture.GetPixel(x2, y2);
                    Color color = new Color(
                        Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);
                }
            }

            newTexture.Apply();
            return newTexture;
        }

        /// <summary> 双线性插值法缩放图片为指定尺寸 </summary>
        public static Texture2D SizeTextureBilinear(Texture2D originalTexture, Vector2 size)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
            float scaleX = originalTexture.width / size.x;
            float scaleY = originalTexture.height / size.y;
            int maxX = originalTexture.width - 1;
            int maxY = originalTexture.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scaleX;
                    float targetY = y * scaleY;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = originalTexture.GetPixel(x1, y1);
                    Color color2 = originalTexture.GetPixel(x2, y1);
                    Color color3 = originalTexture.GetPixel(x1, y2);
                    Color color4 = originalTexture.GetPixel(x2, y2);
                    Color color = new Color(
                        Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);
                }
            }

            newTexture.Apply();
            return newTexture;
        }


        public static void SendGet(string url, Action<string> onCompleted)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            ao.completed += (_ao) =>
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    onCompleted?.Invoke(request.downloadHandler.text);
                }
                else
                {
                    onCompleted?.Invoke(string.Empty);
                }
            };
        }

        public static void RequestImage(string url, Action<Texture2D> callback = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                callback?.Invoke(null);
                return;
            }

            string localPath = GetCombinePath(Application.persistentDataPath, "qrcode", url.GetHashCode().ToString());
            try
            {
                if (File.Exists(localPath))
                {
                    url = "file://" + localPath;
                }

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                var ao = request.SendWebRequest();
                ao.completed += (_ao) =>
                {
                    if (string.IsNullOrEmpty(request.error))
                    {
                        if (!request.url.StartsWith("file://"))
                        {
                            WriteAllBytes(localPath, request.downloadHandler.data);
                        }

                        Texture2D texture2D = DownloadHandlerTexture.GetContent(request);
                        callback?.Invoke(texture2D);
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                };
            }
            catch (Exception e)
            {
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }

                callback?.Invoke(null);
            }
        }
    }
}