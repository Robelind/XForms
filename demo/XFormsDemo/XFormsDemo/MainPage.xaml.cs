using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFormsDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}