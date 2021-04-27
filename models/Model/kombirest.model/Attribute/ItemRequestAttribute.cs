using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Activity;

namespace Model.kombirest.model.Attribute
{
    public class ItemRequestAttribute : ValidationAttribute
    {
        private readonly int _max;

        public ItemRequestAttribute(int max)
        {
            _max = max;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList<ActivityModel.ItemInsertContext>;
            if (list != null)
            {
                return false;
            }
            if (list.Count > 20)
            {
                return false;
            }
            return true;
        }
    }
}
