/*
 * Credit to Josh Smith: http: //joshsmithonwpf.wordpress.com/2010/03/16/control-input-focus-from-viewmodel-objects/
 */

using System;
using System.Windows;

namespace EldredBrown.ProFootball.NETCore.WpfApp.ViewModels.FocusVMLib
{
    public class FocusBinding : BindingDecoratorBase
    {
        public override object ProvideValue(IServiceProvider provider)
        {
            if (base.TryGetTargetItems(provider, out DependencyObject? elem, out DependencyProperty? prop))
            {
                FocusController.SetFocusableProperty(elem!, prop!);
            }

            return base.ProvideValue(provider);
        }
    }
}
