using UnityEngine;
using UnityEditor;
using System;

namespace Common
{

    public enum EnumStringCasing
    {
        /// <summary>
        /// Matches string with casing and serializes to string with original enum casing.
        /// </summary>
        Strict,
        /// <summary>
        /// Matches string ignoring casing and serializes to string with original enum casing.
        /// </summary>
        LoosePreserve,
        /// <summary>
        /// Matches string ignoring casing and serializes to string with upper casing.
        /// </summary>
        LooseUpper,
        /// <summary>
        /// Matches string ignoring casing and serializes to string with lower casing.
        /// </summary>
        LooseLower
    }

    public static class EditorGUIExtensions
    {
        public const float DEFAULT_GUI_LINE_HEIGHT = 18;
        public const float DEFAULT_GUI_HEIGHT = 16;

        /// <summary>
        /// Default enum string converter for serializing enum strings as other string values and converting them back.
        /// This default varient just passes back the string.
        /// </summary>
        /// <param name="i_EnumName">Name of the enum.</param>
        /// <param name="i_Writing">
        /// If set to <c>true</c>, converts from enum string to some other string (for writing),
        /// otherwise conversion is made back to to the enum string (for reading).
        /// </param>
        /// <returns>Coonversion result.</returns>
        public static string DefaultEnumStringConverter(string i_EnumName, bool i_Writing)
        {
            return i_EnumName;
        }

        /// <summary>
        /// Helper function for displaying a child enum popup property for string data.
        /// </summary>
        /// <returns>Matched property or null if not found.</returns>
        /// <param name="i_Prop">Parent property.</param>
        /// <param name="i_Name">Name.</param>
        /// <param name="i_DisplayName">Display name.</param>
        /// <param name="i_EnumType">Enum type.</param>
        /// <param name="i_ContentArea">Content area reference of the container.</param>
        public static SerializedProperty CreateChildEnumGUIForName(
            this SerializedProperty i_Prop,
            string i_Name,
            string i_DisplayName,
            Type i_EnumType,
            ref Rect i_ContentArea,
            EnumStringCasing i_Casing = EnumStringCasing.LoosePreserve,
            Func<string, bool, string> i_Converter = null
        )
        {
            Func<string, bool, string> conversion = i_Converter ?? DefaultEnumStringConverter;
            SerializedProperty matchedProperty = i_Prop.FindPropertyRelative(i_Name);
            if (matchedProperty == null)
            {
                //no match
                return null;
            }
            string oldSelectedId = matchedProperty.stringValue;
            Enum selectedEnum = null;
            if (string.IsNullOrEmpty(oldSelectedId))
            {
                selectedEnum = (Enum)Enum.Parse(i_EnumType, Enum.GetNames(i_EnumType)[0]);
            }
            else
            {
                try
                {
                    switch (i_Casing)
                    {
                        case EnumStringCasing.Strict:
                            selectedEnum = (Enum)Enum.Parse(i_EnumType, conversion(oldSelectedId, false), false);
                            break;
                        case EnumStringCasing.LoosePreserve:
                        case EnumStringCasing.LooseUpper:
                        case EnumStringCasing.LooseLower:
                        default:
                            selectedEnum = (Enum)Enum.Parse(i_EnumType, conversion(oldSelectedId, false), true);
                            break;
                    }
                }
                catch
                {
                    selectedEnum = null;
                }
                if (selectedEnum == null)
                {
                    //default to first option if parsing failed
                    selectedEnum = (Enum)Enum.Parse(i_EnumType, Enum.GetNames(i_EnumType)[0]);
                }
            }
            Rect enumUIArea = new Rect(i_ContentArea.x, i_ContentArea.y, i_ContentArea.width, DEFAULT_GUI_HEIGHT);
            Rect labelUIArea = enumUIArea;
            labelUIArea.width *= 0.5f;
            enumUIArea.width *= 0.5f;
            enumUIArea.x += labelUIArea.width;
            i_ContentArea.y += DEFAULT_GUI_LINE_HEIGHT;

            EditorGUI.LabelField(labelUIArea, i_DisplayName);
            selectedEnum = EditorGUI.EnumPopup(enumUIArea, GUIContent.none, selectedEnum);
            string resultString = selectedEnum.ToString();
            switch (i_Casing)
            {
                case EnumStringCasing.Strict:
                case EnumStringCasing.LoosePreserve:
                    matchedProperty.stringValue = conversion(resultString, true);
                    break;
                case EnumStringCasing.LooseUpper:
                    matchedProperty.stringValue = conversion(resultString.ToUpper(), true);
                    break;
                case EnumStringCasing.LooseLower:
                    matchedProperty.stringValue = conversion(resultString.ToLower(), true);
                    break;
                default:
                    matchedProperty.stringValue = conversion(resultString, true);
                    break;
            }
            return matchedProperty;
        }

        /// <summary>
        /// Helper function for displaying an enum popup property for enum data.
        /// </summary>
        /// <returns>Matched serialized property using given name.</returns>
        /// <param name="prop">Container property to find a matching child property in.</param>
        /// <param name="name">Name to match the property by.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="enumType">Enum type.</param>
        /// <param name="contentArea">Content area reference of the container.</param>
        public static SerializedProperty CreateChildEnumGUIForInt(this SerializedProperty prop, string name, string displayName, Type enumType, ref Rect contentArea)
        {
            SerializedProperty matchedProperty = prop.FindPropertyRelative(name);
            if (matchedProperty == null)
            {
                //something wrong
                return null;
            }
            int oldSelectedId = matchedProperty.enumValueIndex;
            Enum selectedEnum = (Enum)Enum.ToObject(enumType, oldSelectedId);
            if (!Enum.IsDefined(enumType, selectedEnum))
            {
                selectedEnum = (Enum)Enum.Parse(enumType, Enum.GetNames(enumType)[0]);
            }

            Rect enumUIArea = new Rect(contentArea.x, contentArea.y, contentArea.width, DEFAULT_GUI_HEIGHT);
            Rect labelUIArea = enumUIArea;
            labelUIArea.width *= 0.5f;
            enumUIArea.width *= 0.5f;
            enumUIArea.x += labelUIArea.width;
            contentArea.y += DEFAULT_GUI_LINE_HEIGHT;

            EditorGUI.LabelField(labelUIArea, displayName);
            selectedEnum = EditorGUI.EnumPopup(enumUIArea, GUIContent.none, selectedEnum);
            matchedProperty.enumValueIndex = Convert.ToInt32(selectedEnum);
            return matchedProperty;
        }
    }
}