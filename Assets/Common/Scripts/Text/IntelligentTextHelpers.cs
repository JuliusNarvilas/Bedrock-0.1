using System;
using UnityEngine;
using System.Collections.Generic;

namespace Common.Text
{
    [Serializable]
    public struct IntelligentTextLocalizationRecord
    {
        public string id;
        public string displayName;
        public string path;
    }

    [Serializable]
    public struct IntelligentTextLocalizationsContainer
    {
        public List<IntelligentTextLocalizationRecord> localizationList;
    }

    [Serializable]
    public class IntelligentTextLocalizationData
    {
        public List<IntelligentTextKeyValueRecord> textLocalizations;
        public List<IntelligentTextKeyValueRecord> inserts;
        public List<IntelligentTextStyleRecord> styles;
        public List<IntelligentTextImageRecord> images;
        public List<IntelligentTextTransform> transforms;
    }

    [Serializable]
    public struct IntelligentTextKeyValueRecord
    {
        public string id;
        public string data;
    }

    [Serializable]
    public class IntelligentTextStyleRecord
    {
        public string id;
        public string fontPath;
        public Color color;
        public float lineSpacing;
        public int fontSize;
    }

    public class IntelligentTextStyle
    {
        public Color Color;
        public Font Font;
        public int FontSize;
        public float LineSpacing;
    }

    [Serializable]
    public struct IntelligentTextImageRecord
    {
        public string id;
        public string texturePath;
        public string sprite;
        public string materialPath;
    }

    public struct IntelligentTextImage
    {
        public Sprite Sprite;
        public Material Material;
    }

    public class IntelligentTextSubMeshData
    {
        public List<int> Trinagles;
        public Material Material;
    }

    public class IntelligentTextLineInfo
    {
        public int StartCharIndex;
        public float PaddingBottom;
        public float Height;
        public float PaddingTop;
    }

    public enum IntelligentTextTransformAnchor
    {
        Top,
        Center,
        Bottom
    }

    [Serializable]
    public class IntelligentTextTransform
    {
        public string id;
        public float scale;
        public Vector2 offset;
        public float rotation;
        public IntelligentTextTransformAnchor pivot;
    }
}
