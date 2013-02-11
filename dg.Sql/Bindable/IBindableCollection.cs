using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace dg.Sql.Bindable
{
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class HiddenForDataBindingAttribute : System.Attribute
    {
        // Fields
        private readonly bool _isHidden;

        // Methods
        public HiddenForDataBindingAttribute()
        {
        }

        public HiddenForDataBindingAttribute(bool isHidden)
        {
            this._isHidden = isHidden;
        }

        // Properties
        public bool IsHidden
        {
            get
            {
                return this._isHidden;
            }
        }
    }
    public interface IBindableCollection
    {
        // Support for Data Binding
        PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors);
    }
}
