# XForms brings attribute based validation to Xamarin Forms!

With XForms you are able to use the validation attributes you know and love from e.g. WPF or ASP.Net devlopment, to validate user input.
Just decorate your view model properties with validation attributes, wrap you input elements in an XForms form
and bind them to the view model properties.

For examples of everything below, build and run **XFormsDemo** in the repository.

XForms is available through [Nuget](https://www.nuget.org/packages/XForms/)

## Supported elements
XForms supports validation of the following elements:
* Entry
* Editor
* Picker

## Basic validation
Lets use the following example to break down the basic validation:
```
<ContentPage.Content>
	<xForms:Form CommitButton="{Binding Source={x:Reference SubmitBtn}}" CommitCommand="{Binding CommitCmd}">
        <Label Text="Required value:"/>
        <Entry Text="{Binding Value1}"/>
		<Button x:Name="SubmitBtn" Text="Ok" />
	</xForms:Form>
</ContentPage.Content>
```

The basic construct is the `xForms:Form`, within which all input elements must be placed.
The `CommitButton` is the button that triggers the validation.
The `CommitCommand` is the view model command to be executed when validation is successful.

This view model snippet shows the property to which the entry element is bound:
```
[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ValueRequired")]
public string Value1 { get; set; }
```
When the commit button is pressed, the validation of the input elements within the form will be triggered.
For each element which fails validation, an error message will be displayed below the element.
If validation succeeds, the commit command will be executed.

### Form layout
The `xForms:Form` is a vertically oriented stack layout, to enable layout of input elements within it.

### Validation feedback
The color of the validation feedback text can be configured by using the `MessageColor` property, e.g.:
```
<xForms:Form CommitButton="{Binding Source={x:Reference SubmitBtn}}" CommitCommand="{Binding CommitCmd}" MessageColor="Coral">
```

## Custom validation
XForms offers some customization of the validation and feedback.
### Validation
To perform custom validation when input elements validation has succeeded, have your view model implement `ICustomValidation`.
```
public string Validate()
{
	// Return an error message if validation fails, otherwise null.
}
```
The custom validation must be accompanied by a validation feedback element, declared using the form property `CustomFeedback`:
```
<ContentPage.Content>
	<xForms:Form CommitButton="{Binding Source={x:Reference SubmitBtn}}" CommitCommand="{Binding CommitCmd}"
		CustomFeedback="{Binding Source={x:Reference CustomFeedback}}">
		<Label x:Name="CustomFeedback" TextColor="Teal" IsVisible="false" />
		<Button x:Name="SubmitBtn" Text="Ok" />
	</xForms:Form>
</ContentPage.Content>
```
### Feedback
Customization of the appearance of element input validation messages can be done as seen below:

```
<xForms:Form CommitButton="{Binding Source={x:Reference SubmitBtn}}" CommitCommand="{Binding CommitCmd}">
	<Label Text="Value:"/>
	<Entry Text="{Binding Value1}"/>
	<Label IsVisible="False" xForms:Form.ValidationMessage="True" TextColor="DarkOrchid" />
	<Button x:Name="SubmitBtn" Text="Ok" />
</xForms:Form>
```
The attached property `ValidationMessage` is declared for the label to receive the validation message for the preceding input element.
Custom feedback elements can be declared with `IsVisible="True"`, to have them be always visible, i.e. occupy
space in the layout.
## Attributes
XForms comes with some additional validation attributes.
### Required if true
Using this attribute, a value is required if another boolean property is `true`.
```
public bool Flag { get; set; }
[RequiredIfTrue("Flag", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ValueRequired")]
public string Value { get; set; }
```
### Required if false
Using this attribute, a value is required if another boolean property is `false`.
```
public bool Flag { get; set; }
[RequiredIfFalse("Flag", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ValueRequired")]
public string Value { get; set; }
```
### Required true
Using this attribute, a boolen value is required to be `true`.
```
[RequiredTrue]
public bool Value { get; set; }
```

