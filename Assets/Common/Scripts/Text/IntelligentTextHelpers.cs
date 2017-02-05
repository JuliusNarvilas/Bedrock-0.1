using System;
using UnityEngine;
using System.Collections.Generic;

namespace Common.Text
{
    /// <summary>
    /// Record of existing localization.
    /// </summary>
    [Serializable]
    public struct IntelligentTextLocalizationRecord
    {
        public string id;
        public string displayName;
        public string path;
    }

    /// <summary>
    /// The structure of existing localizations are expected to be in.
    /// </summary>
    [Serializable]
    public struct IntelligentTextLocalizationsContainer
    {
        public List<IntelligentTextLocalizationRecord> localizationList;
    }

    /// <summary>
    /// Localization data.
    /// </summary>
    [Serializable]
    public class IntelligentTextLocalizationData
    {
        public List<IntelligentTextKeyValueRecord> text;
        public List<IntelligentTextKeyValueRecord> inserts;
        public List<IntelligentTextStyleRecord> styles;
        public List<IntelligentTextImageRecord> images;
        public List<IntelligentTextTransform> transforms;
    }

    /// <summary>
    /// Simple grouping of data that is in the Localizations' structure.
    /// </summary>
    [Serializable]
    public struct IntelligentTextKeyValueRecord
    {
        public string id;
        public string data;
    }

    /// <summary>
    /// Text style information structure in serialised data.
    /// </summary>
    [Serializable]
    public class IntelligentTextStyleRecord
    {
        public string id;
        public string fontPath;
        public Color color;
        public float lineSpacing;
        public int fontSize;
    }

    /// <summary>
    /// Text style data used in the intelligent text system.
    /// </summary>
    public class IntelligentTextStyle
    {
        public Color Color;
        public Font Font;
        public int FontSize;
        public float LineSpacing;
    }

    /// <summary>
    /// Text image information structure in serialised data.
    /// </summary>
    [Serializable]
    public struct IntelligentTextImageRecord
    {
        public string id;
        public string texturePath;
        public string sprite;
        public string materialPath;
    }

    /// <summary>
    /// Text image data used in intelligent text system.
    /// </summary>
    public struct IntelligentTextImage
    {
        public Sprite Sprite;
        public Material Material;
    }

    /// <summary>
    /// Sub mesh data used in final intelligent text mesh building.
    /// </summary>
    public class IntelligentTextSubMeshData
    {
        public List<int> Trinagles;
        public Material Material;
    }

    /// <summary>
    /// information on line text layout for adjustments and inserting new submesh objects.
    /// </summary>
    public class IntelligentTextLineInfo
    {
        public int StartCharIndex;
        public float PaddingBottom;
        public float Height;
        public float PaddingTop;
    }

    /// <summary>
    /// Vertical alignment setting for positioning items inserted into intelligent text.
    /// </summary>
    public enum IntelligentTextTransformAnchor
    {
        /// <summary>
        /// Align vertical positioning on the line top point and overflow downwards.
        /// </summary>
        Top,
        /// <summary>
        /// Align vertical positioning on the line centre point and overflow in both vertical directions..
        /// </summary>
        Center,
        /// <summary>
        /// Align vertical positioning on the line bottom point and overflow upwards.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Transform information for intelligent text inserted items.
    /// </summary>
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
