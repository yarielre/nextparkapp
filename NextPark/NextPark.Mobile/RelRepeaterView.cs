using System.Collections;
using Xamarin.Forms;

namespace NextPark.Mobile.Repeater
{
    public class RelRepeaterView : RelativeLayout
    {
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate), 
            typeof(DataTemplate), 
            typeof(RelRepeaterView), 
            default(DataTemplate));

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource), 
            typeof(ICollection), 
            typeof(RelRepeaterView), 
            null, 
            BindingMode.OneWay, 
            propertyChanged: ItemsChanged);
        
        public RelRepeaterView()
        {
            return;
        }

        public ICollection ItemsSource
        {
            get => (ICollection) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        
        protected virtual View ViewFor(object item)
        {
            View view = null;

            if (ItemTemplate != null)
            {
                var content = ItemTemplate.CreateContent();

                view = content is View ? content as View : ((ViewCell)content).View;

                view.BindingContext = item;
            }

            return view;
        }
        
        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as RelRepeaterView;

            if (control == null) return;

            control.Children.Clear();

            var items = (ICollection) newValue;

            if (items == null) return;

            foreach (var item in items)
            {
                View view = control.ViewFor(item);
                control.Children.Add(view, (Constraint)view.GetValue(XConstraintProperty), (Constraint)view.GetValue(YConstraintProperty), (Constraint)view.GetValue(WidthConstraintProperty), (Constraint)view.GetValue(HeightConstraintProperty));
            }
        }
    }
}
