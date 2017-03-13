
using UnityEditor;
using UnityEngine;

namespace Common
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            System.Enum targetEnum = (System.Enum)property.GetTargetObject();
            
            string propName = flagSettings.EnumName;
            if (string.IsNullOrEmpty(propName))
                propName = property.name;

            var itemNames = System.Enum.GetNames(targetEnum.GetType());
            int exportedOriginal = flagSettings.Converter(property.intValue, true);

            EditorGUI.BeginProperty(position, label, property);

            int newValue = EditorGUI.MaskField(position, propName, exportedOriginal, itemNames);
            int removedFlags = (newValue & exportedOriginal) ^ exportedOriginal;
            int importedRemovedValues = flagSettings.Converter(removedFlags, false);
            int finalValue = flagSettings.Converter(newValue, false);
            
            //making sure values for removed flags get removed as well
            //to support enums with values reprisenting flag sets
            if (importedRemovedValues != 0)
            {
                finalValue &= ~importedRemovedValues;
            }
            property.intValue = finalValue;
            
            EditorGUI.EndProperty();
        }
        
    }
}
