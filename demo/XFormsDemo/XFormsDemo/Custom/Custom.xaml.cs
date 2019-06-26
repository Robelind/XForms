using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFormsDemo.Custom
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Custom : ContentPage
	{
		public Custom ()
		{
			InitializeComponent ();
		}
	}
}