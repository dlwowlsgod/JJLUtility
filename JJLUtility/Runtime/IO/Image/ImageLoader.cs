using UnityEngine;
using System.Collections.Generic;

namespace JJLUtility.IO
{
    public partial class ImageLoader : SingletonBehavior<ImageLoader>
    {
#if JJLUTILITY_IMAGELOADER_CACHE
        private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
#endif
    }
}