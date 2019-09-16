using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Acros
{
    [CustomPropertyDrawer(typeof(UnityGroupMemberPropertyAttribute))]
    public class UnityGroupMemberPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as UnityGroupMemberPropertyAttribute;


            SerializedProperty groupProp = null, memberProp = null;

            if (attr.Group == property.name)
            {
                groupProp = property;
                if (!string.IsNullOrEmpty(attr.Member))
                    memberProp = FindPropertyRelativePath(property, attr.Member);

                if (groupProp.propertyType == SerializedPropertyType.Integer)
                {
                    //TODO:
                    int value = EditorGUI.IntPopup(position, groupProp.displayName, groupProp.intValue,null,null);
                }

            }

        }

        class ValueInfo
        {
            public int[] intValues;
            public string[] strValues;
            public string[] names;
            public Type valueType;
        }


        class DataInfo
        {
            public ValueInfo parent;
            public Dictionary<object, ValueInfo> child;

        }

        private static Dictionary<Type, DataInfo> InnerData;

        //Only for class EventCategory
        private static DataInfo GetEventGroupMemberData(UnityGroupMemberPropertyAttribute attr)
        {
            if (InnerData != null && InnerData.ContainsKey(attr.GetType()))
                return InnerData[attr.GetType()];

            Dictionary<int, Type> allGroups = new Dictionary<int, Type>();
            Dictionary<int, Dictionary<int, Type>> groupMemberMaps = new Dictionary<int, Dictionary<int, Type>>();

            Func<MemberInfo, object> GetValue = (m) =>
             {
                 if (m.MemberType == MemberTypes.Field)
                 {
                     FieldInfo field = (FieldInfo)m;
                     return field.GetValue(null);
                 }
                 else
                 {
                     Debug.Assert(false, "Only support field type.");
                     return null;
                 }
             };

            var groupAttrType = attr.GetGroupAttrType();
            foreach (var groupClass in typeof(EventCategory).GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (groupClass.IsDefined(groupAttrType, true))
                {
                    //Get main index
                    var uniqueID =  groupClass.DeclaringType.GetMember("INDEX");
                    if (uniqueID != null && uniqueID.Length == 1 && uniqueID[0].DeclaringType == typeof(int))
                    {
                        //Add Group
                        int indexValue = (int)GetValue(uniqueID[0]);
#if UNITY_EDITOR
                        if (allGroups.ContainsKey(indexValue))
                        {
                            Debug.LogError("Duplicate [INDEX] value in class " + allGroups[indexValue].Name + " and " + groupClass.Name);
                            continue;
                        }
#endif
                        allGroups.Add(indexValue, groupClass.DeclaringType);

                        //Add Members
                        Dictionary<int, Type> memberList = new Dictionary<int, Type>();
                        foreach (var member in groupClass.DeclaringType.GetMembers(BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (member.IsDefined(attr.GetMemberAttrType(), true))
                            {
                                int value = (int)GetValue(member);
#if UNITY_EDITOR
                                if (memberList.ContainsKey(indexValue))
                                {
                                    Debug.LogError("Duplicate member value in class " + groupClass.Name + "Conflict Members:" + 
                                        memberList[value].Name + " and " + member.Name);
                                    continue;
                                }
#endif
                                memberList.Add(value, member.DeclaringType);
                            }
                        }

                        groupMemberMaps.Add(indexValue, memberList);
                    }
                    else
                    {
                        Debug.LogError("Member of [INDEX] in int type cannot be found in class :" + groupClass.Name);
                    }
                }


            }

            return InnerData[attr.GetType()];
        }


        public static SerializedProperty FindPropertyRelativePath(SerializedProperty property, string propertyName)
        {
            string path = property.propertyPath;

            if (path.LastIndexOf('.') >= 0)
            {
                path = path.Substring(0, path.LastIndexOf('.')) + "." + propertyName;
            }
            else
            {
                path = propertyName;
            }
            return property.serializedObject.FindProperty(path);
        }
    }

}
