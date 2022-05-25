using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace Helper
{
    #region Optional
    [Serializable]
    public class Optional<T>
    {
        [SerializeField] private bool m_enabled;
        [SerializeField] private T m_value;

        public Optional(T _initialValue)
        {
            m_value = _initialValue;
            m_enabled = true;
        }

        public bool Enabled => m_enabled;
        public T Value => m_value;
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty valueProperty = _property.FindPropertyRelative("m_value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(
            Rect _position,
            SerializedProperty _property,
            GUIContent _label
            )
        {
            SerializedProperty valueProperty = _property.FindPropertyRelative("m_value");
            SerializedProperty enabledProperty = _property.FindPropertyRelative("m_enabled");

            _position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(_position, valueProperty, _label, true);
            EditorGUI.EndDisabledGroup();

            _position.x += _position.width + 24;
            _position.width = _position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            _position.x -= _position.width;
            EditorGUI.PropertyField(_position, enabledProperty, GUIContent.none);
        }
    }
#endif
    #endregion

    #region ReadOnly
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property,
            GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }

        public override void OnGUI(Rect _position,
            SerializedProperty _property,
            GUIContent _label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(_position, _property, _label, true);
            GUI.enabled = true;
        }
    }
#endif
    #endregion

    //public class ReadOnlyTickAttribute : PropertyAttribute
    //{
    //    private string m_text;
    //    public ReadOnlyTickAttribute(string _text)
    //    {
    //        m_text = _text;
    //    }

    //    public string Text => m_text;
    //}

    //[CustomPropertyDrawer(typeof(ReadOnlyTickAttribute))]
    //public class ReadOnlyTickDrawer : PropertyDrawer
    //{
    //    public override float GetPropertyHeight(SerializedProperty _property,
    //        GUIContent _label)
    //    {
    //        return EditorGUI.GetPropertyHeight(_property, _label, true);
    //    }

    //    public override void OnGUI(Rect _position,
    //        SerializedProperty _property,
    //        GUIContent _label)
    //    {
    //        GUI.enabled = false;
    //        Type ty = _property.GetType();
    //        //Attribute.GetCustomAttributes(typeof(ReadOnlyTickAttribute), true)
    //        List<Attribute> tempList = Attribute.GetCustomAttributes(typeof(ReadOnlyTickAttribute), true).ToList();
    //        //List<ReadOnlyTickAttribute> myROTA = new List<ReadOnlyTickAttribute>(Attribute.GetCustomAttributes(typeof(ReadOnlyTickAttribute), true).ToList().Cast<ReadOnlyTickAttribute>());
    //        ReadOnlyTickAttribute xd = (ReadOnlyTickAttribute)tempList[0];
    //        string tempText = xd.ToString()/*(this.GetType().GetCustomAttributes(false).FirstOrDefault() as ReadOnlyTickAttribute)?.Text*/;
    //        EditorGUI.PropertyField(_position, _property, new GUIContent(tempText), true);
    //        GUI.enabled = true;
    //    }
    //}

    //public class Helper : MonoBehaviour
    //{
    //    [SerializeField] private Optional<float> m_target;
    //    [SerializeField/*, ReadOnlyTick("Test")*/
    //     #if UNITY_EDITOR
    //     , ReadOnly
    //    #endif
    //    ] private float m_test = 1f;
    //}
}
