using TimeTable.views;

namespace TimeTable;

public partial class ClassDetailPage : ContentPage
{

    ClassEntry classEntry;

    public ClassDetailPage(ClassEntry classEntry)
    {
        InitializeComponent();
        Title = "Class Details";
        this.classEntry = classEntry;
        var viewModel = new DetailPageViewModel
        {
            CurrentClass = classEntry
        };
        BindingContext = viewModel;
        var color = classEntry.Color;
        R.Value = color.Red * 255;
        G.Value = color.Green * 255;
        B.Value = color.Blue * 255;
        Preview.BackgroundColor = color;

    }
    public async void OnButtonClose(object sender, EventArgs e) {
        await Shell.Current.Navigation.PopModalAsync();
    }

    private void RGB_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var color = Color.FromRgb(R.Value / 255.0, G.Value / 255.0, B.Value / 255.0);
        Preview.BackgroundColor = color;
        classEntry.Color = color;
    }

    private void OnPonastaviClicked(object sender, EventArgs e)
    {
        classEntry.ResetColor();
        var color = classEntry.Color;
        R.Value = color.Red * 255;
        G.Value = color.Green * 255;
        B.Value = color.Blue * 255;
        Preview.BackgroundColor = color;
    }
}