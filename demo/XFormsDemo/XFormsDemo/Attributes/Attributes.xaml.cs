using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFormsDemo.Attributes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Attributes : ContentPage
	{
		public Attributes ()
		{
			InitializeComponent ();
		}
	}
}