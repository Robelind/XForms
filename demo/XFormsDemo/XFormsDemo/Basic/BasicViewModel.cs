using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Xamarin.Forms;
using XForms;

namespace XFormsDemo.Basic
{
    class BasicViewModel
    {
        public BasicViewModel()
        {
            CommitCmd = new Command(() => Application.Current.MainPage.DisplayAlert("XForms", "Form was committed!", "Ok"));
        }

        public ICommand CommitCmd { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ValueRequired")]
        public string Value1 { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Value2Required")]
        [Range(5, 10, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Range")]
        public string Value2 { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Value3Required")]
        public string Value3 { get; set; }
        //[Required(ErrorMessage = "Value 4 is crucial!")]
        //public string Value4 { get; set; }
        //[Required(ErrorMessage = "Value 5 must be")]
        //public string Value5 { get; set; }
        //[Required(ErrorMessage = "Date please")]
        //public DateTime? Value6 { get; set; }
        public IEnumerable<string> Values => new[] { "Value1", "Value1", "Value3" };
    }
}
