/*
 * Credit to Josh Smith: http: //joshsmithonwpf.wordpress.com/2010/03/16/control-input-focus-from-viewmodel-objects/
 */

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace EldredBrown.ProFootball.NETCore.WpfApp.ViewModels.FocusVMLib
{
    public static class FocusController
    {
        #region FocusableProperty

        public static DependencyProperty GetFocusableProperty(DependencyObject obj)
        {
            return (DependencyProperty)obj.GetValue(FocusablePropertyProperty);
        }

        public static void SetFocusableProperty(DependencyObject obj, DependencyProperty value)
        {
            obj.SetValue(FocusablePropertyProperty, value);
        }

        public static readonly DependencyProperty FocusablePropertyProperty = 
            DependencyProperty.RegisterAttached(
            "FocusableProperty",
            typeof(DependencyProperty),
            typeof(FocusController),
            new UIPropertyMetadata(null, OnFocusablePropertyChanged));

        static void OnFocusablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement element) ||
                !(e.NewValue is DependencyProperty property))
            {
                return;
            }

            element.DataContextChanged += delegate
            {
                CreateHandler(element, property);
            };

            if (element.DataContext != null)
            {
                CreateHandler(element, property);
            }
        }

        static void CreateHandler(DependencyObject element, DependencyProperty property)
        {
            if (!(element.GetValue(FrameworkElement.DataContextProperty) is IFocusMover focusMover))
            {
                if (element.GetValue(MoveFocusSinkProperty) is MoveFocusSink handler)
                {
                    handler.ReleaseReferences();
                    element.ClearValue(MoveFocusSinkProperty);
                }
            }
            else
            {
                var handler = new MoveFocusSink(element as UIElement, property);
                focusMover.MoveFocus += handler.HandleMoveFocus;
                element.SetValue(MoveFocusSinkProperty, handler);
            }
        }

        #endregion // FocusableProperty

        #region MoveFocusSink

        static readonly DependencyProperty MoveFocusSinkProperty = DependencyProperty.RegisterAttached("MoveFocusSink",
            typeof(MoveFocusSink), typeof(FocusController), new UIPropertyMetadata(null));

        private class MoveFocusSink
        {
            private UIElement? _element;
            private DependencyProperty? _property;

            public MoveFocusSink(UIElement? element, DependencyProperty? property)
            {
                _element = element;
                _property = property;
            }

            public void HandleMoveFocus(object? sender, MoveFocusEventArgs e)
            {
                if (_element is null || _property is null)
                {
                    return;
                }

                var binding = BindingOperations.GetBinding(_element, _property);
                if (binding is null || e.FocusedProperty != binding.Path.Path)
                {
                    return;
                }

                // Delay the call to allow the current batch
                // of processing to finish before we shift focus.
                _element.Dispatcher.BeginInvoke((Action)delegate
                {
                    _element.Focus();
                },
                DispatcherPriority.Background);
            }

            public void ReleaseReferences()
            {
                _element = null;
                _property = null;
            }
        }

        #endregion // MoveFocusSink
    }
}
