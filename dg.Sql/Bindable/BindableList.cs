using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace dg.Sql.Bindable
{
    public abstract class BindableList<Type> : List<Type>, IBindableCollection
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
    }
}
