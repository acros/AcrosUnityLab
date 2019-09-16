using UnityEngine;
using System;

namespace Acros
{
    //Custom draw - [This is unity editor]
    public abstract class UnityGroupMemberPropertyAttribute : PropertyAttribute
    {
        public string Group { get; set; }

        public string Member { get; set; }

        public abstract Type GetGroupAttrType();

        public abstract Type GetMemberAttrType();
    }


    public class EventCategoryAdapterAttribute : UnityGroupMemberPropertyAttribute
    {
        public override Type GetGroupAttrType()
        {
            return typeof(EventCategoryIDAttribute);
        }

        public override Type GetMemberAttrType()
        {
            return typeof(EventMemberIDAttribute);
        }
    }
}

