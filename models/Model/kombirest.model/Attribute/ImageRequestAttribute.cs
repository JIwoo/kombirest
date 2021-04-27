using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Model.kombirest.model.Attribute 
{
    public class ImageRequestAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        public ImageRequestAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public override bool IsValid(object value)
        {
            var list = value as List<IFormFile>;
            if (list != null)
            {
                return false;
            }
            if (list.Count > 3 || list.Count <= 0) 
            {
                return false;
            }
            return true;
        }
    }
}
