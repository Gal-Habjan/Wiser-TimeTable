using TimeTable.views;

namespace TimeTable;

public partial class ClassDetailPage : ContentPage
{

    public ClassDetailPage(ClassEntry classEntry)
    {
        InitializeComponent();
        Title = "Class Details";
        var viewModel = new DetailPageViewModel
        {
            CurrentClass = classEntry
        };
        BindingContext = viewModel;

    }
    public async void OnButtonClose(object sender, EventArgs e) {
        await Shell.Current.Navigation.PopModalAsync();
    }
}