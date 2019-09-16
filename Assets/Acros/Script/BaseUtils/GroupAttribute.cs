using UnityEngine;
using System;

namespace Acros
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        private string friendlyName;

        public GroupAttribute(string fName)
        {
            friendlyName = fName;
        }

        public string FriendlyName
        {
            get { return friendlyName; }
        }
    }


    [AttributeUsage(AttributeTargets.Field)]
    public class MemberAttribute : Attribute
    {
        private int categoryID;
        private string friendlyName;

        public MemberAttribute(int groupID,string fName)
        {
            categoryID = groupID;
            friendlyName = fName;
        }

        public int Category
        {
            get { return categoryID; }
        }

        public string FriendlyName
        {
            get { return friendlyName; }
        }
    }

}
