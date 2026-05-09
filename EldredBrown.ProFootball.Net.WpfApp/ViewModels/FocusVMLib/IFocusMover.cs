/*
 * Credit to Josh Smith: http: //joshsmithonwpf.wordpress.com/2010/03/16/control-input-focus-from-viewmodel-objects/
 */

using System;

namespace EldredBrown.ProFootball.NETCore.WpfApp.ViewModels.FocusVMLib
{
    /// <summary>
    /// Implemented by a ViewModel that needs to control
    /// where input focus is in a View.
    /// </summary>
    public interface IFocusMover
	{
		/// <summary>
		/// Raised when the input focus should move to 
		/// a control whose 'active' dependency property 
		/// is bound to the specified property.
		/// </summary>
		event EventHandler<MoveFocusEventArgs> MoveFocus;
	}

	public class MoveFocusEventArgs: EventArgs
	{
		public MoveFocusEventArgs(string focusedProperty)
		{
			this.FocusedProperty = focusedProperty;
		}

		public string FocusedProperty { get; private set; }
	}
}
