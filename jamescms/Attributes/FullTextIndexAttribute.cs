using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jamescms
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FullTextIndexAttribute : Attribute
    {
        public FullTextIndexAttribute(string name, string column)
        {
            this.Name = name;
            this.Column = column;
        }

        public string Name { get; private set; }

        public string Column { get; private set; }

    }
}