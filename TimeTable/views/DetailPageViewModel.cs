using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TimeTable.views
{
    [QueryProperty("currentClass", "currentClass")]
    public partial class DetailPageViewModel : ObservableObject
    {

        [ObservableProperty]
        ClassEntry? currentClass;

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}