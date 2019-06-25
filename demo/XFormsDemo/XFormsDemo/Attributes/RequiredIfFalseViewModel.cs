using System.Windows.Input;
using Xamarin.Forms;
using XForms.Attributes;

namespace XFormsDemo.Attributes
{
    class RequiredIfFalseViewModel
    {
        public RequiredIfFalseViewModel()
        {
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }

        public ICommand CommitCmd { get; set; }

        public bool Flag { get; set; }
        [RequiredIfFalse("Flag")]
        public string Value { get; set; }
    }
}
