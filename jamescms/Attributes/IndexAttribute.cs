using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jamescms
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute(string name, bool unique = false, bool descending = false)
        {
            this.Name = name;
            this.IsUnique = unique;
            this.Descending = descending;
        }

        public string Name { get; private set; }

        public bool IsUnique { get; private set; }

        public bool Descending { get; private set; }
    }
}