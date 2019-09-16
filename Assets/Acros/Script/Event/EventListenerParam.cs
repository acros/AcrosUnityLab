using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Acros
{
    [Serializable]
    public struct EventListenerParam
    {
        [EventCategoryAdapter(Group = "eventGroup", Member = "eventMemberType")]
        public int eventGroup;

        [EventCategoryAdapter(Group = "eventGroup", Member = "eventMemberType")]
        public int eventMemberType;


        public string customEventName;
    }
}
