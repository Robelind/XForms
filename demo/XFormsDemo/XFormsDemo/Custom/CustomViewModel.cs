using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Xamarin.Forms;
using XForms;

namespace XFormsDemo.Custom
{
    class CustomViewModel : ICustomValidation
    {
        public CustomViewModel()
        {
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }


        public ICommand CommitCmd { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ValueRequired")]
        public string Value1 { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Value2Required")]
        public string Value2 { get; set; }

        public string Validate()
        {
            return(Value1 == "xxx" ? "This won't fly" : null);
        }
    }
}
