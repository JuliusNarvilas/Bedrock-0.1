using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

/*
[CustomEditor(typeof(TestList))]
public class TestListEditor : Editor
{
}
*/
/*
[CustomPropertyDrawer(typeof(RewardItem))]
public class IngredientDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        if(GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f), "button"))
        {
            // do things
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) + 20f;
    }
}
*/

[CustomPropertyDrawer(typeof(TestListElement))]
public class TestListElementDrawer : PropertyDrawer
{
    // this is the height of each control line
    const int s_lineHeight = 18;
    const int s_uiHeight = 16;

    private Action<SerializedProperty> m_displayAction = null;
    private Rect contentPosition;
    private GUIContent cachedGUIContainer = new GUIContent();

    // Draw the property inside the given rect
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        pos.height *= 4;
        contentPosition = pos;
        contentPosition.y += s_lineHeight;

        cachedGUIContainer.text = "Test List Element";
        EditorGUI.PrefixLabel(pos, cachedGUIContainer);
        EditorGUI.BeginProperty(pos, label, prop);
        EditorGUI.indentLevel = 2;
        contentPosition = EditorGUI.IndentedRect(contentPosition);

        SerializedProperty typeProp = SelectEnumByInt(prop, "m_type", "RewardType", typeof(TestEnum));
        if(typeProp == null)
        {
            return;
        }

        TestEnum typePropEnum = (TestEnum)typeProp.enumValueIndex;
        switch (typePropEnum)
        {
            case TestEnum.TestEnum1:
                m_displayAction = DanceCardDisplay;
                break;
            case TestEnum.TestEnum2:
                m_displayAction = StringDataDisplay;
                break;
            case TestEnum.TestEnum3:
                m_displayAction = CurrencyDisplay;
                break;
            default:
                m_displayAction = null;
                break;
        }

        if (m_displayAction != null)
        {
            m_displayAction(prop);
        }
        else
        {
            SomethingWrongDisplay(prop);
        }

        EditorGUI.indentLevel = 0;
        EditorGUI.EndProperty();
    }



    private void DanceCardDisplay(SerializedProperty prop)
    {
    }

    private SerializedProperty SelectEnumByName(SerializedProperty prop, string name, string displayName, Type enumType)
    {
        SerializedProperty matchedProperty = prop.FindPropertyRelative(name);
        if (matchedProperty == null)
        {
            //something wrong
            return null;
        }
        string oldSelectedId = matchedProperty.stringValue;
        Enum selectedEnum = null;
        if (string.IsNullOrEmpty(oldSelectedId))
        {
            selectedEnum = (Enum) Enum.ToObject(enumType, 0);
        }
        else
        {
            selectedEnum = (Enum) Enum.Parse(enumType, oldSelectedId, true);
        }
        Rect uiArea = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, s_uiHeight);
        contentPosition.y += s_lineHeight;
        selectedEnum = EditorGUI.EnumPopup(uiArea, displayName, selectedEnum);
        matchedProperty.stringValue = selectedEnum.ToString();
        return matchedProperty;
    }

    private SerializedProperty SelectEnumByInt(SerializedProperty prop, string name, string displayName, Type enumType)
    {
        SerializedProperty matchedProperty = prop.FindPropertyRelative(name);
        if (matchedProperty == null)
        {
            //something wrong
            return null;
        }
        int oldSelectedId = matchedProperty.enumValueIndex;
        Enum selectedEnum = (Enum)Enum.ToObject(enumType, oldSelectedId);
        Rect uiArea = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, s_uiHeight);
        contentPosition.y += s_lineHeight;
        selectedEnum = EditorGUI.EnumPopup(uiArea, displayName, selectedEnum);
        matchedProperty.enumValueIndex = Convert.ToInt32(selectedEnum);
        return matchedProperty;
    }

    private void CurrencyDisplay(SerializedProperty prop)
    {
        SelectEnumByName(prop, "m_id", "Currency Type", typeof(TestEnum));
        
        SerializedProperty matchedProperty = prop.FindPropertyRelative("m_quantity");
        if (matchedProperty == null)
        {
            //something wrong
            return;
        }

        Rect newRect = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, s_uiHeight);
        contentPosition.y += s_lineHeight;

        Rect labelRect = newRect;
        labelRect.width *= 0.385f;
        Rect inputRect = newRect;
        inputRect.width *= 0.615f;
        inputRect.x += newRect.width * 0.385f;

        cachedGUIContainer.text = "Quantity";
        EditorGUI.PrefixLabel(labelRect, cachedGUIContainer);
        matchedProperty.intValue = EditorGUI.IntField(inputRect, matchedProperty.intValue);
        if (matchedProperty.intValue < 1)
        {
            matchedProperty.intValue = 1;
        }

        //EditorGUI.IntSlider(newRect, matchedProperty, 1, 1000000);
        //do quantity
    }

    private void GachaDisplay(SerializedProperty prop)
    {
    }

    private void StringDataDisplay(SerializedProperty prop)
    {
    }

    private void UnimplementedDisplay(SerializedProperty prop)
    {
        EditorGUI.LabelField(contentPosition, "", "Not implemented!");
    }

    private void SomethingWrongDisplay(SerializedProperty prop)
    {
        EditorGUI.LabelField(contentPosition, "", "Something went wrong!");
    }



    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        return s_lineHeight * 4.3f;
    }
}
