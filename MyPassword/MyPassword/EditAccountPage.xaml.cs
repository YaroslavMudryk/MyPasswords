namespace MyPassword;

public partial class EditAccountPage : ContentPage
{
	public EditAccountPage(string id)
	{
		InitializeComponent();
		BindingContext = new EditAccountPageViewModel(id);
	}
}