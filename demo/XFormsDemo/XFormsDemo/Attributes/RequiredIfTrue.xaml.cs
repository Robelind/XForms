using Xamarin.Forms.Xaml;

namespace XFormsDemo.Attributes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RequiredIfTrue
	{
		public RequiredIfTrue()
		{
			InitializeComponent ();
		}
	}
}