using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acros
{
    public class EventCategoryIDAttribute : GroupAttribute
    {
        public EventCategoryIDAttribute(string fName) : base(fName)
        { }
  
    }

    public class EventMemberIDAttribute : MemberAttribute
    {
        public EventMemberIDAttribute(int categoryID,string fName): base (categoryID,fName)
        {}

    }
}
