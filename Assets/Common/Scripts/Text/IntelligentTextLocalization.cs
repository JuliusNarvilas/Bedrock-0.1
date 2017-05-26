using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Common.IO.Recources;

namespace Common.Text
{
    /// <summary>
    /// Intelligent text localization data for a culture or language.
    /// </summary>
    public class IntelligentTextLocalization
    {
        public const char PLACEHOLDER_OPENING = '{';
        public const char PLACEHOLDER_CLOSING = '}';

        /// <summary>
        /// Estimated required capacity for string building.
        /// </summary>
        private int m_TextMaxInitialCapacity;

        public Dictionary<string, string> TextLocalizations;
        public Dictionary<string, IntelligentTextStyle> Styles;
        public Dictionary<string, IntelligentTextImage> Images;
        public Dictionary<string, IntelligentTextTransform> Transforms;
        public Dictionary<string, string> Inserts;
        public List<ResourcesDBItem> TrackedResources;

        public IntelligentTextLocalization()
        {
            m_TextMaxInitialCapacity = 0;
            TextLocalizations = new Dictionary<string, string>();
            Styles = new Dictionary<string, IntelligentTextStyle>();
            Images = new Dictionary<string, IntelligentTextImage>();
            Transforms = new Dictionary<string, IntelligentTextTransform>();
            Inserts = new Dictionary<string, string>();
            TrackedResources = new List<ResourcesDBItem>();
        }

        /// <summary>
        /// Append more data for localization.
        /// Is used to combine global localizations with specialized ones.
        /// </summary>
        /// <param name="i_Data">Localization data.</param>
        public void Append(IntelligentTextLocalizationData i_Data)
        {
            int localizationCount = i_Data.text.Count;
            IntelligentTextKeyValueRecord localizationRecord;
            for (int i = 0; i < localizationCount; ++i)
            {
                localizationRecord = i_Data.text[i];
                TextLocalizations[localizationRecord.id] = localizationRecord.data;
                if(m_TextMaxInitialCapacity < localizationRecord.data.Length)
                {
                    m_TextMaxInitialCapacity = localizationRecord.data.Length;
                }
            }
            
            int insertsCount = i_Data.inserts.Count;
            IntelligentTextKeyValueRecord insertRecord;
            for (int i = 0; i < insertsCount; ++i)
            {
                insertRecord = i_Data.inserts[i];
                Inserts[insertRecord.id] = insertRecord.data;
            }
            
            int stylesCount = i_Data.styles.Count;
            IntelligentTextStyleRecord styleRecord;
            for (int i = 0; i < stylesCount; ++i)
            {
                styleRecord = i_Data.styles[i];
                var resource = ResourcesDB.GetByPath(styleRecord.fontPath);
                if (resource != null)
                {
                    var fontAsset = resource.Load<Font>();
                    if (fontAsset != null)
                    {
                        Styles[styleRecord.id] = new IntelligentTextStyle()
                        {
                            Color = styleRecord.color,
                            Font = fontAsset,
                            FontSize = styleRecord.fontSize,
                            LineSpacing = styleRecord.lineSpacing
                        };
                        TrackedResources.Add(resource);
                    }
                    else
                    {
                        resource.Unload();
                    }
                }
                Log.DebugLogErrorIf(
                    !Styles.ContainsKey(styleRecord.id),
                    "IntelligentText font not found ({0}) for style id: {1}",
                    styleRecord.fontPath, styleRecord.id
                    );
            }

            int imagesCount = i_Data.images.Count;
            IntelligentTextImageRecord imageRecord;
            for (int i = 0; i < imagesCount; ++i)
            {
                imageRecord = i_Data.images[i];
                var textureResource = ResourcesDB.GetByPath(imageRecord.texturePath);
                var materialResource = ResourcesDB.GetByPath(imageRecord.materialPath);
                if (textureResource != null && materialResource != null)
                {
                    var spriteAsset = textureResource.Load<Sprite>(imageRecord.sprite);
                    var materialAsset = materialResource.Load<Material>();
                    if (spriteAsset != null && materialAsset != null)
                    {
                        var materialClone = new Material(materialAsset);
                        materialClone.mainTexture = spriteAsset.texture;
                        Images[imageRecord.id] = new IntelligentTextImage() { Sprite = spriteAsset, Material = materialClone };
                        TrackedResources.Add(textureResource);
                        TrackedResources.Add(materialResource);
                    }
                    else
                    {
                        textureResource.Unload();
                        materialResource.Unload();
                    }
                }
                Log.DebugLogErrorIf(
                    !Images.ContainsKey(imageRecord.id),
                    "IntelligentText Localization image not found ({0}) for id: {1}",
                    imageRecord.texturePath, imageRecord.id
                    );
            }

            int transformsCount = i_Data.transforms.Count;
            IntelligentTextTransform transformRecord;
            for (int i = 0; i < transformsCount; ++i)
            {
                transformRecord = i_Data.transforms[i];
                Transforms[transformRecord.id] = transformRecord;
            }
        }

        /// <summary>
        /// Localizes text by searching for text localization ids wrapped with <see cref="PLACEHOLDER_OPENING"/> and <see cref="PLACEHOLDER_CLOSING"/>.
        /// If i_TermOnly is true, the text input is interpreted as a single localization id straight away.
        /// </summary>
        /// <param name="i_Text">Text for localization.</param>
        /// <param name="i_TermOnly">Indicator for treating text input as localization term rather than text with placeholders.</param>
        /// <returns>Localized string.</returns>
        public string Localize(string i_Text, bool i_TermOnly = false)
        {
            if(i_TermOnly)
            {
                string replacement;
                if (TextLocalizations.TryGetValue(i_Text, out replacement))
                {
                    return replacement;
                }
                Log.DebugLogError("IntelligentTextLocalization: text localization id \"{0}\" not found", i_Text);
                return string.Empty;
            }

            StringBuilder result = new StringBuilder(m_TextMaxInitialCapacity);
            int textSize = i_Text.Length;
            int lastTextIndex = textSize - 1;
            for (int i = 0; i < textSize; ++i)
            {
                char letter = i_Text[i];
                if (letter == PLACEHOLDER_OPENING)
                {
                    //if placeholder is possible, test for it
                    if (i < lastTextIndex && i_Text[i + 1] != PLACEHOLDER_OPENING)
                    {
                        int endIndex = i_Text.IndexOf(PLACEHOLDER_CLOSING, i);
                        if (endIndex >= 0)
                        {
                            string key = i_Text.Substring(i + 1, endIndex - i - 2);
                            string replacement;
                            if (TextLocalizations.TryGetValue(key, out replacement))
                            {
                                result.Append(replacement);
                            }
                            else
                            {
                                Log.DebugLogError("IntelligentTextLocalization: text localization id \"{0}\" not found", key);
                            }
                            i = endIndex;
                        }
                        else
                        {
                            Log.DebugLogError("IntelligentTextLocalization bad text format: {0}", i_Text);
                            //end localization if no placeholders are possible
                            result.Append(i_Text, i, textSize - i);
                            break;
                        }
                    }
                    else
                    {
                        //2 placeholder openings are replaced with a single placeholder opening char
                        ++i;
                        result.Append(PLACEHOLDER_OPENING);
                    }
                }
                else
                {
                    result.Append(letter);
                }
            }
            return result.ToString();
        }

        public void Clear()
        {
            TextLocalizations.Clear();
            Images.Clear();
            Transforms.Clear();
            Inserts.Clear();
            Styles.Clear();

            int resourceCount = TrackedResources.Count;
            for (int i = 0; i < resourceCount; ++i)
            {
                TrackedResources[i].Unload();
            }
            TrackedResources.Clear();
        }
    }
}
