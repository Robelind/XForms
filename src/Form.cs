using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace XForms
{
    public class Form : StackLayout
    {
        private readonly IDictionary<string, Feedback> _feedback = new Dictionary<string, Feedback>();

        public static readonly BindableProperty CommitCommandProperty = BindableProperty.Create(nameof(CommitCommand),
            typeof(ICommand),
            typeof(Form));

        public static readonly BindableProperty CommitButtonProperty = BindableProperty.CreateAttached("CommitButton",
            typeof(bool),
            typeof(Form), false);

        public static readonly BindableProperty ValidationMessageProperty = BindableProperty.CreateAttached(
            "ValidationMessage",
            typeof(bool), typeof(Form), false);

        public ICommand CommitCommand
        {
            get => (ICommand) this.GetValue(CommitCommandProperty);
            set => this.SetValue(CommitCommandProperty, value);
        }

        public bool CommitButton
        {
            get => (bool) this.GetValue(CommitButtonProperty);
            set => this.SetValue(CommitButtonProperty, value);
        }

        public bool ValidationMessage
        {
            get => (bool) this.GetValue(ValidationMessageProperty);
            set => this.SetValue(ValidationMessageProperty, value);
        }

        protected override void OnChildAdded(Element child)
        {
            if(child is Button button)
            {
                if((bool) button.GetValue(CommitButtonProperty))
                {
                    button.Clicked += Commit;
                }
            }
            //if(child is Entry)
            //{
            //    Binding binding = child.GetBinding(Entry.TextProperty);
            //    PropertyInfo prop = child.BindingContext.GetType().GetProperties().SingleOrDefault(p => p.Name == binding.Path);
            //}

            base.OnChildAdded(child);
        }

        protected override void OnBindingContextChanged()
        {
            //foreach(var child in Children)
            //{
            //    if(child is Entry)
            //    {
            //        Binding binding = child.GetBinding(Entry.TextProperty);
            //        PropertyInfo prop = (child.BindingContext ?? BindingContext).GetType().GetProperties()
            //            .SingleOrDefault(p => p.Name == binding.Path);
            //    }
            //    else if(child is Button button)
            //    {
            //        button.Clicked += Commit;
            //    }
            //}

            base.OnBindingContextChanged();
        }

        private void Commit(object sender, EventArgs e)
        {
            ValidationContext validationContext = new ValidationContext(BindingContext, null, null);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if(Validator.TryValidateObject(BindingContext, validationContext, validationResults, true))
            {
                CommitCommand.Execute(null);
            }
            else
            {
                IEnumerable<string> active = _feedback.Keys.Intersect(validationResults.Select(r => r.MemberNames.First())).ToList();
                IEnumerable<string> notActive = _feedback.Keys.Except(active).ToList();

                this.HandleInactive(notActive);
                this.HandleValidationFailures(validationResults, active);
            }
        }

        private void HandleValidationFailures(ICollection<ValidationResult> validationResults, IEnumerable<string> active)
        {
            foreach(var validationResult in validationResults)
            {
                if(!active.Contains(validationResult.MemberNames.First()))
                {
                    FindResult result = this.FindChild(Children, validationResult.MemberNames.First());
                    Feedback feedback = new Feedback();

                    _feedback.Add(validationResult.MemberNames.First(), feedback);
                    if(result.Index < result.Children.Count - 1)
                    {
                        if((bool) result.Children.ElementAt(result.Index + 1).GetValue(ValidationMessageProperty))
                        {
                            if(result.Children.ElementAt(result.Index + 1) is Label label)
                            {
                                label.Text = validationResult.ErrorMessage;
                                feedback.Label = label;
                                feedback.Visible = label.IsVisible;
                                label.IsVisible = true;
                            }
                        }
                    }

                    if(feedback.Label == null)
                    {
                        feedback.Label = new Label
                        {
                            Text = validationResult.ErrorMessage,
                            TextColor = Color.Red,
                            VerticalOptions = LayoutOptions.Center
                        };
                        result.Children.Insert(result.Index + 1, feedback.Label);
                    }
                }
            }
        }

        private void HandleInactive(IEnumerable<string> propNames)
        {
            foreach(string propName in propNames)
            {
                Feedback feedback = _feedback[propName];

                if(feedback.Visible.HasValue)
                {
                    if(!feedback.Visible.Value)
                    {
                        feedback.Label.IsVisible = false;
                    }
                    else
                    {
                        feedback.Label.Text = null;
                    }
                }
                else
                {
                    FindResult result = this.FindChild(Children, propName);

                    result.Children.Remove(feedback.Label);
                }

                _feedback.Remove(propName);
            }
        }

        private FindResult FindChild(IList<View> children, string propName)
        {
            FindResult result = null;

            for(int index = 0; index < children.Count; index++)
            {
                VisualElement child = children.ElementAt(index);

                if(child is IViewContainer<View> viewContainer)
                {
                    result = this.FindChild(viewContainer.Children, propName);
                }
                else
                {
                    Binding binding = child.GetBinding(Entry.TextProperty) ??
                        child.GetBinding(Editor.TextProperty) ??
                        child.GetBinding(Picker.SelectedItemProperty) ??
                        child.GetBinding(DatePicker.DateProperty);

                    if(binding?.Path == propName)
                    {
                        result = new FindResult {Children = children, Index = index};
                    }
                }

                if(result != null)
                {
                    break;
                }
            }

            return(result);
        }
    }
}