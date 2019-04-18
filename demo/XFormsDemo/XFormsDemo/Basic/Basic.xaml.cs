using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFormsDemo.Basic
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Basic : ContentPage
	{
		public Basic ()
		{
			InitializeComponent ();
		}
	}
}