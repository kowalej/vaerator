using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.Enums
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescription : Attribute
    {
        private readonly string description;
        public string Description { get { return description; } }
        public EnumDescription(string description)
        {
            this.description = description;
        }
    }
}
