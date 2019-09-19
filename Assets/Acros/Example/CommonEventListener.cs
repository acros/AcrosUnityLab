using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acros;
using System.Reflection;
using System;

public class CommonEventListener : MonoBehaviour {

    public EventListenerParam eventParam;

    public void Awake()
    {
        Dictionary<int, Type> allGroups = new Dictionary<int, Type>();
        Dictionary<int, Dictionary<int, MemberInfo>> groupMemberMaps = new Dictionary<int, Dictionary<int, MemberInfo>>();

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

        foreach (var groupClass in typeof(EventCategory).GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (groupClass.IsDefined(typeof(EventCategoryIDAttribute), true))
            {
                TypeInfo nestedGroupClass = (TypeInfo)groupClass;
                var uniqueID = nestedGroupClass.GetMember("INDEX");
                if (uniqueID != null && uniqueID.Length == 1)
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
                    allGroups.Add(indexValue, nestedGroupClass.AsType());

                    //Add Members
                    Dictionary<int, MemberInfo> memberList = new Dictionary<int, MemberInfo>();
                    foreach (var member in nestedGroupClass.GetMembers(BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (member.IsDefined(typeof(EventMemberIDAttribute), true))
                        {
                            int value = (int)GetValue(member);
#if UNITY_EDITOR
                            if (memberList.ContainsKey(value))
                            {
                                Debug.LogError("Duplicate member value in class " + groupClass.Name + "Conflict Members:" +
                                    memberList[value].Name + " and " + member.Name);
                                continue;
                            }
#endif
                            memberList.Add(value, typeof(int));
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

        //Print Log
        foreach (var item in allGroups)
        {
            Debug.Log("Event group:" + item.Value.Name + " index:" + item.Key);

            var members = groupMemberMaps[item.Key];
            foreach (var m in members)
            {
                Debug.Log("Relate member: " + m.Value.MemberType.ToString() + " =" + m.Key);
            }

        }


    }

}
