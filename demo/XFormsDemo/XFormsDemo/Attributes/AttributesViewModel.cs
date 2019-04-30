using System.Windows.Input;
using Xamarin.Forms;
using XForms.Attributes;

namespace XFormsDemo.Attributes
{
    class AttributesViewModel
    {
        public AttributesViewModel()
        {
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }

        public ICommand CommitCmd { get; set; }

        [RequiredTrue]
        public bool MustCheck { get; set; }
        public bool Value1Flag { get; set; }
        [RequiredIfTrue("Value1Flag")]
        public string Value1 { get; set; }
        public bool Value2Flag { get; set; }
        [RequiredIfFalse("Value2Flag")]
        public string Value2 { get; set; }
    }
}
