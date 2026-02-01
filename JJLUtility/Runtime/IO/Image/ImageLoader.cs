using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace JJLUtility.IO
{
    public partial class ImageLoader : SingletonBehavior<ImageLoader>
    {
#if JJLUTILITY_IMAGELOADER_CACHE
#if UNITY_EDITOR
        private Dictionary<string, int> _textureCache = new Dictionary<string, int>();
        [SerializeField]
        private List<Texture2D> textureCacheList = new List<Texture2D>();
        private Dictionary<string, int> _spriteCache = new Dictionary<string, int>();
        [SerializeField]
        private List<Sprite> spriteCacheList = new List<Sprite>();
        private Dictionary<string, int> _materialCache = new Dictionary<string, int>();
        [SerializeField]
        private List<Material> materialCacheList = new List<Material>();
#else
        private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
        private Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
#endif //UNITY_EDITOR
#endif //JJLUTILITY_IMAGELOADER_CACHE

        private void Start()
        {
            var path0 = Path.Combine(Application.streamingAssetsPath, "4bit.bmp");
            LoadTexture(path0);
            var path1 = Path.Combine(Application.streamingAssetsPath, "8bit.bmp");
            LoadTexture(path1);
            var path2 = Path.Combine(Application.streamingAssetsPath, "8bitRLE.bmp");
            LoadTexture(path2);
            var path3 = Path.Combine(Application.streamingAssetsPath, "16bitR5G6B5.bmp");
            LoadTexture(path3);
            var path4 = Path.Combine(Application.streamingAssetsPath, "16bitA1R5G5B5.bmp");
            LoadTexture(path4);
            var path5 = Path.Combine(Application.streamingAssetsPath, "16bitX1R5G5B5.bmp");
            LoadTexture(path5);
            var path6 = Path.Combine(Application.streamingAssetsPath, "24bit.bmp");
            LoadTexture(path6);
            var path7 = Path.Combine(Application.streamingAssetsPath, "32bitA8R8G8B8.bmp");
            LoadTexture(path7);
            var path8 = Path.Combine(Application.streamingAssetsPath, "32bitX8R8G8B8.bmp");
            LoadTexture(path8);
        }

        public static Texture2D LoadTexture(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                Debugger.LogError($"Image path is empty: {filepath}", Instance, nameof(ImageLoader));
                return null;
            }

            if (!File.Exists(filepath))
            {
                Debugger.LogError($"Image file not found: {filepath}", Instance, nameof(ImageLoader));
                return null;
            }

#if JJLUTILITY_IMAGELOADER_CACHE
            if (Instance._textureCache.ContainsKey(filepath))
            {
#if UNITY_EDITOR
                return Instance.textureCacheList[Instance._textureCache[filepath]];
#else
        return _textureCache[filepath];
#endif //UNITY_EDITOR
            }
#endif //JGLUTILITY_IMAGELOADER_CACHE

            
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string extension = Path.GetExtension(filepath).ToLower();

            Texture2D texture = null;
            switch (extension)
            {
                case ".jpg" or ".jpeg" or ".png":
                    byte[] imageData = File.ReadAllBytes(filepath);
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    break;
                case ".bmp":
                    BMPFile bmpFile = LoadBMPFile(filepath);
                    if (bmpFile == null)
                    {
                        Debugger.LogError($"Unsupported image extension: {filepath}", Instance, nameof(ImageLoader));
                        return null;
                    }
                    texture = new Texture2D(bmpFile.InfoHeader.Width, bmpFile.InfoHeader.Height);
                    texture.SetPixels32(bmpFile.Pixels);
                    texture.Apply();
                    break;
                default:
                    Debugger.LogError($"Unsupported image extension: {filepath}", Instance, nameof(ImageLoader));
                    return null;
            }
            
            texture.name = filename;
            
#if JJLUTILITY_IMAGELOADER_CACHE
#if UNITY_EDITOR
            Instance.textureCacheList.Add(texture);
            Instance._textureCache.Add(filepath, Instance.textureCacheList.Count - 1);
#else
            Instance._textureCache.Add(filepath, texture);
#endif //UNITY_EDITOR
#endif //JGLUTILITY_IMAGELOADER_CACHE
            
            return texture;
        }
    }
}