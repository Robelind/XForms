using System.Windows.Input;
using Xamarin.Forms;
using XForms.Attributes;

namespace XFormsDemo.Attributes
{
    class RequiredTrueViewModel
    {
        public RequiredTrueViewModel()
        {
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }

        public ICommand CommitCmd { get; set; }

        [RequiredTrue(ErrorMessage = "Switch must be flipped!")]
        public bool Value { get; set; }
    }
}
