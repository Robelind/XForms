using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Switch = Xamarin.Forms.Switch;

namespace XForms
{
    public class Form : StackLayout
    {
        private readonly IDictionary<string, Feedback> _feedback = new Dictionary<string, Feedback>();

        /// <summary>
        /// Validation message color property. Bindable.
        /// </summary>
        public static readonly BindableProperty MessageColorProperty = BindableProperty.Create(nameof(MessageColor),
            typeof(Color), typeof(Form), Color.Red);

        /// <summary>
        /// Commit command property. Bindable.
        /// </summary>
        public static readonly BindableProperty CommitCommandProperty = BindableProperty.Create(nameof(CommitCommand),
            typeof(ICommand), typeof(Form));

        /// <summary>
        /// Commit button property. Bindable.
        /// </summary>
        public static readonly BindableProperty CommitButtonProperty = BindableProperty.CreateAttached(nameof(CommitButton), typeof(Button),
            typeof(Form), null);

        /// <summary>
        /// Commit button property. Bindable.
        /// </summary>
        public static readonly BindableProperty CommitMenuItemProperty = BindableProperty.CreateAttached(nameof(CommitMenuItem), typeof(MenuItem),
            typeof(Form), null);

        /// <summary>
        /// Validation message property. Bindable.
        /// </summary>
        public static readonly BindableProperty ValidationMessageProperty = BindableProperty.CreateAttached(nameof(ValidationMessage), typeof(bool),
            typeof(Form), false);

        /// <summary>
        /// Custom feedback property. Bindable.
        /// </summary>
        public static readonly BindableProperty CustomFeedbackProperty = BindableProperty.CreateAttached(nameof(CustomFeedback), typeof(Label),
            typeof(Form), null);

        private bool _customFeedbackVisible;
        private ICustomValidation _customValidation;

        /// <summary>
        /// Color of the failed validation messages.
        /// </summary>
        public Color MessageColor
        {
            get => (Color)this.GetValue(MessageColorProperty);
            set => this.SetValue(MessageColorProperty, value);
        }

        /// <summary>
        /// Command to execute when validation is successful.
        /// </summary>
        public ICommand CommitCommand
        {
            get => (ICommand) this.GetValue(CommitCommandProperty);
            set => this.SetValue(CommitCommandProperty, value);
        }

        /// <summary>
        /// Button within the form that triggers the validation.
        /// </summary>
        public Button CommitButton
        {
            get => (Button) this.GetValue(CommitButtonProperty);
            set => this.SetValue(CommitButtonProperty, value);
        }

        /// <summary>
        /// Menu item that triggers the validation.
        /// </summary>
        public MenuItem CommitMenuItem
        {
            get => (MenuItem)this.GetValue(CommitMenuItemProperty);
            set => this.SetValue(CommitMenuItemProperty, value);
        }

        public bool ValidationMessage
        {
            get => (bool) this.GetValue(ValidationMessageProperty);
            set => this.SetValue(ValidationMessageProperty, value);
        }

        public Label CustomFeedback
        {
            get => (Label) this.GetValue(CustomFeedbackProperty);
            set => this.SetValue(CustomFeedbackProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            if(CommitButton == null && CommitMenuItem == null)
            {
                throw new ArgumentException("No commit element declared");
            }

            if(CommitButton != null)
            {
                CommitButton.Clicked += Commit;
            }

            if(CommitMenuItem != null)
            {
                CommitMenuItem.Clicked += Commit;
            }

            if(BindingContext is ICustomValidation validation)
            {
                if(CustomFeedback == null)
                {
                    throw new ArgumentException("No custom feedback element declared");
                }

                _customValidation = validation;
                _customFeedbackVisible = CustomFeedback.IsVisible;
            }

            base.OnBindingContextChanged();
        }
        
        private void Commit(object sender, EventArgs e)
        {
            if(BindingContext == null)
            {
                throw new ArgumentException("Binding context must be set");
            }

            ValidationContext validationContext = new ValidationContext(BindingContext, null, null);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(BindingContext, validationContext, validationResults, true);
            IEnumerable<string> active = _feedback.Keys.Intersect(validationResults.Select(r => r.MemberNames.First())).ToList();
            IEnumerable<string> inActive = _feedback.Keys.Except(active).ToList();

            this.HandleInactive(inActive);

            if(valid)
            {
                if(this.HandleCustomValidation())
                {
                    if(CommitCommand != null)
                    {
                        CommitCommand.Execute(null);
                    }
                    else
                    {
                        throw new ArgumentException("CommitCommand cannot be null");
                    }
                }
            }
            else
            {
                this.HandleValidationFailures(validationResults, active);
            }
        }

        private bool HandleCustomValidation()
        {
            string customMsg = null;

            if(_customValidation != null)
            {
                customMsg = _customValidation.Validate();
                if(customMsg != null)
                {
                    CustomFeedback.IsVisible = true;
                    CustomFeedback.Text = customMsg;
                }
                else
                {
                    CustomFeedback.IsVisible = _customFeedbackVisible;
                    CustomFeedback.Text = null;
                }
            }

            return(customMsg == null);
        }

        private void HandleValidationFailures(ICollection<ValidationResult> validationResults, IEnumerable<string> active)
        {
            foreach(var validationResult in validationResults)
            {
                string propName = validationResult.MemberNames.First();

                if(!active.Contains(propName))
                {
                    FindResult result = this.FindChild(this, propName);

                    if(result != null)
                    {
                        Feedback feedback = new Feedback();

                        _feedback.Add(validationResult.MemberNames.First(), feedback);
                        if(result.Index < result.Container.Children.Count - 1)
                        {
                            if((bool) result.Container.Children.ElementAt(result.Index + 1).GetValue(ValidationMessageProperty))
                            {
                                if(result.Container.Children.ElementAt(result.Index + 1) is Label label)
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
                                TextColor = MessageColor,
                                VerticalOptions = LayoutOptions.Center
                            };

                            if(result.Container is StackLayout stackLayout && stackLayout.Orientation == StackOrientation.Horizontal)
                            {
                                IViewContainer<View> parent = stackLayout.Parent as IViewContainer<View>;

                                // Element with failed validation is in a horizontal stack layout.
                                // Place the message after the stack layout.
                                Debug.Assert(parent != null);
                                for(int i = 0; i < parent.Children.Count; i++)
                                {
                                    if(parent.Children.ElementAt(i) == result.Container)
                                    {
                                        parent.Children.Insert(i + 1, feedback.Label);
                                    }
                                }
                                
                            }
                            else
                            {
                                result.Container.Children.Insert(result.Index + 1, feedback.Label);
                            }
                        }
                    }
                    else
                    {
                        // Property with validation isn't used in the form.
                    }
                }
                else
                {
                    // Message could have changed.
                    _feedback[propName].Label.Text = validationResult.ErrorMessage;
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
                    Debug.Assert(feedback.Label.Parent is IViewContainer<View>);
                    ((IViewContainer<View>)feedback.Label.Parent).Children.Remove(feedback.Label);
                }

                _feedback.Remove(propName);
            }
        }

        private FindResult FindChild(IViewContainer<View> container, string propName)
        {
            FindResult result = null;

            for(int index = 0; index < container.Children.Count; index++)
            {
                VisualElement child = container.Children.ElementAt(index);

                if(child is IViewContainer<View> viewContainer)
                {
                    result = this.FindChild(viewContainer, propName);
                }
                else
                {
                    Binding binding = child.GetBinding(Entry.TextProperty) ??
                                      child.GetBinding(Editor.TextProperty) ??
                                      child.GetBinding(Picker.SelectedItemProperty) ??
                                      child.GetBinding(Switch.IsToggledProperty) ??
                                      child.GetBinding(DatePicker.DateProperty);

                    if(binding?.Path == propName)
                    {
                        result = new FindResult {Container = container, Index = index};
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