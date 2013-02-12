using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace jamescms
{
    public class ValdiateBoolIsFalseAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var booleanvalue = (bool)value;

            if (booleanvalue == false)
                return true;
            else
                return false;
        }
    }
}