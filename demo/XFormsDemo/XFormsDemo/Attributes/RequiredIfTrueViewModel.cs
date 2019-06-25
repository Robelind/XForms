using System.Windows.Input;
using Xamarin.Forms;
using XForms.Attributes;

namespace XFormsDemo.Attributes
{
    class RequiredIfTrueViewModel
    {
        public RequiredIfTrueViewModel()
        {
            Flag = true;
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }

        public ICommand CommitCmd { get; set; }

        public bool Flag { get; set; }
        [RequiredIfTrue("Flag")]
        public string Value { get; set; }
    }
}
