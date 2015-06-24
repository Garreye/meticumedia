using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Meticumedia.WPF
{
    public class BindableSelectTreeView : TreeView
    {
        public static readonly DependencyProperty BindableSelectedItemProperty =
            DependencyProperty.Register("BindableSelectedItem", typeof(object), typeof(BindableSelectTreeView));

        public object BindableSelectedItem
        {
            get { return GetValue(BindableSelectedItemProperty); }
            set { throw new Exception("This property is read-only. To bind to it you must use 'Mode=OneWayToSource'."); }
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            SetValue(BindableSelectedItemProperty, base.SelectedItem);
        }
    }
}
