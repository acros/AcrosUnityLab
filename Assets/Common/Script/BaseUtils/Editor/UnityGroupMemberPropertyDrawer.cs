using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Acros
{
    [CustomPropertyDrawer(typeof(UnityGroupMemberPropertyAttribute),true)]
    public class UnityGroupMemberPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as UnityGroupMemberPropertyAttribute;

            SerializedProperty groupProp = null, memberProp = null;

            var groupInfoData = GetEventGroupMemberData(attr);

            if (attr.Group == property.name)
            {
                groupProp = property;
                if (!string.IsNullOrEmpty(attr.Member))
                    memberProp = FindPropertyRelativePath(property, attr.Member);

                if (groupProp.propertyType == SerializedPropertyType.Integer)
                {
                    groupProp.intValue = EditorGUI.IntPopup(position, groupProp.displayName, groupProp.intValue, groupInfoData.allNames,
                        groupInfoData.allValues);
                }

            }

        }

        class AcMemberInfo
        {
            public int value;
            public string friendlyName;
            public MemberInfo info;
        };

        class AcGroupInfo
        {
            public int groupIndex;
            public Type groupType;
            public string friendlyName;

            public Dictionary<int, AcMemberInfo> members = new Dictionary<int, AcMemberInfo>();
        };

        class AcPairData
        {
            public string[] allNames;
            public int[] allValues;

            public Dictionary<int, AcGroupInfo> groups = new Dictionary<int, AcGroupInfo>();
        }

        private static Dictionary<Type, AcPairData> InnerData = null;

        //Only for class EventCategory
        private static AcPairData GetEventGroupMemberData(UnityGroupMemberPropertyAttribute attr,bool forceRefresh = false)
        {
            if (InnerData != null && InnerData.ContainsKey(attr.GetType()))
                return InnerData[attr.GetType()];

            if(InnerData == null || forceRefresh)
                InnerData = new Dictionary<Type, AcPairData>();

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

            AcPairData pData = new AcPairData();

            var groupAttrType = attr.GetGroupAttrType();
            foreach (var groupClass in typeof(EventCategory).GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (groupClass.IsDefined(groupAttrType, true))
                {
                    AcGroupInfo tempGroup = new AcGroupInfo();

                    //Get Group Type
                    TypeInfo nestedGroupClass = (TypeInfo)groupClass;
                    var groupID = nestedGroupClass.GetMember("INDEX");
                    if (groupID != null && groupID.Length == 1)
                    {
                        //Add Group
                        int indexValue = (int)GetValue(groupID[0]);
                        tempGroup.groupIndex = indexValue;
                        tempGroup.groupType = groupAttrType;

                        var groupAttr = (EventCategoryIDAttribute)groupClass.GetCustomAttributes(attr.GetGroupAttrType(), true)[0];
                        tempGroup.friendlyName = groupAttr.FriendlyName;

                        //Add Members
                        Dictionary<int, AcMemberInfo> memberList = new Dictionary<int, AcMemberInfo>();
                        foreach (var member in nestedGroupClass.GetMembers(BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (member.IsDefined(attr.GetMemberAttrType(), true))
                            {
                                int memberIDValue = (int)GetValue(member);

                                var memberAttr = (EventMemberIDAttribute)member.GetCustomAttributes(attr.GetMemberAttrType(), true)[0];
                                AcMemberInfo tempMember = new AcMemberInfo();
                                tempMember.friendlyName = memberAttr.FriendlyName;
                                tempMember.value = memberIDValue;
                                tempMember.info = member;

                                memberList.Add(memberIDValue, tempMember);
                            }
                        }

                        tempGroup.members = memberList;

                        pData.groups.Add(tempGroup.groupIndex, tempGroup);
                    }
                    else
                    {
                        Debug.LogError("Member of [INDEX] in int type cannot be found in class :" + groupClass.Name);
                    }

                }
            }

            //Prepare data for GUI
            pData.allNames = new string[pData.groups.Count];
            pData.allValues = new int[pData.groups.Count];
            int index = 0;
            foreach (var p in pData.groups)
            {
                pData.allNames[index] = p.Value.friendlyName;
                pData.allValues[index] = p.Value.groupIndex;
                ++index;
            }

            InnerData.Add(attr.GetType(), pData);


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
