using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace dg.Sql.Bindable
{
    public abstract class BindableCollection : IBindableCollection
    {
        // Support for Data Binding
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if ((listAccessors != null) && (listAccessors.Length != 0))
            {
                return null;
            }
            return BindableCollection.GetPropertyDescriptors(this.GetType());
        }
        // Support for Data Binding
        internal static PropertyDescriptorCollection GetPropertyDescriptors(Type typeOfObject)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeOfObject);
            ArrayList list = new ArrayList();
            foreach (PropertyDescriptor descriptor in properties)
            {
                HiddenForDataBindingAttribute attribute = (HiddenForDataBindingAttribute)descriptor.Attributes[typeof(HiddenForDataBindingAttribute)];
                if ((attribute == null) || !attribute.IsHidden)
                {
                    list.Add(descriptor);
                }
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[])list.ToArray(typeof(PropertyDescriptor)));
        }
    }
}
