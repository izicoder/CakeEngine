using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CakeEngine;

record TextureInfo(string ImagePath, Vector2 Size) {
}

/// <summary>
/// Should only be created after raylib initialization
/// </summary>
class TextureRegistry {
    private Dictionary<string, TextureInfo> _availableTextures = [];
    private Dictionary<string, Texture2D> _loadedTextures = [];
    private string _missingTexturePath = "assets/missing.jpg";
    private Texture2D _missing;

    public TextureRegistry() {
        _missing = Raylib.LoadTexture(_missingTexturePath);
    }

    public void Add(string name, string imagepath, Vector2 size) {
        var texinfo = new TextureInfo(imagepath, size);
        _availableTextures[name] = texinfo;
    }

    public void Add(string name, TextureInfo texinfo) {
        _availableTextures[name] = texinfo;
    }

    public void Add((string, TextureInfo)[] entries) {
        foreach (var entry in entries) {
            _availableTextures[entry.Item1] = entry.Item2;
        }
    }

    public void Add(string name, string imagepath) {
        var imgsize = Raylib.LoadImage(imagepath);
        var texinfo = new TextureInfo(imagepath, new Vector2(imgsize.Width, imgsize.Height));
        _availableTextures[name] = texinfo;
    }

    public void Load(string name) {
        if (_availableTextures.ContainsKey(name)) {
            var entry = _availableTextures[name];
            var img = Raylib.LoadImage(entry.ImagePath);
            Raylib.ImageResizeNN(ref img, (int) entry.Size.X, (int) entry.Size.Y);
            var tex = Raylib.LoadTextureFromImage(img);
            _loadedTextures[name] = tex;
        }
    }

    public void Load(string[] names) {
        foreach (var name in names) {
            if (_availableTextures.ContainsKey(name)) {
                var entry = _availableTextures[name];
                var img = Raylib.LoadImage(entry.ImagePath);
                Raylib.ImageResizeNN(ref img, (int) entry.Size.X, (int) entry.Size.Y);
                var tex = Raylib.LoadTextureFromImage(img);
                _loadedTextures[name] = tex;
            }
        }
    }

    public Texture2D Get(string name) {
        if (_loadedTextures.ContainsKey(name) && Raylib.IsTextureReady(_loadedTextures[name]))
            return _loadedTextures[name];
        else
            return _missing;
    }
}
