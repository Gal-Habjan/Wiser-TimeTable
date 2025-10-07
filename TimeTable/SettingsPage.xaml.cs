using System.Diagnostics;
using System.Text.Json;
using Microsoft.Maui.Controls;
namespace TimeTable;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

public partial class SettingsPage : ContentPage
{
    List<(string, Color)> differentGroups = new List<(string, Color)>();
    public SettingsPage()
	{
		InitializeComponent();
        
        InitSettingsPageAsync();
    }

    // Asynchronous function to handle all async calls
    private async Task InitSettingsPageAsync()
    {
        await GetAllGroups();          // Fetch all groups asynchronously
        PopulateSettingsGrid();  // Populate the grid after groups are fetched
    }
    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
    public async Task GetAllGroups() {

        string jsonData;
        string appDataFilePath = Path.Combine(FileSystem.AppDataDirectory, "school.json");
        List<ClassEntry> classes = new List<ClassEntry>();
        // Read the local JSON file
        if (File.Exists(appDataFilePath))
        {
            jsonData = await File.ReadAllTextAsync(appDataFilePath);


            // Deserialize the JSON data to get the local hash

            var responses = JsonConvert.DeserializeObject<List<FirebaseResponse>>(jsonData);

            foreach (var response in responses)
            {
                if (response.Key == "Classes")
                {
                    var classesJson = JsonConvert.SerializeObject(response.Object);
                    classes = JsonConvert.DeserializeObject<List<ClassEntry>>(classesJson);
                }

            }

        }

        if (classes == null) return;

        classes.Sort((a, b) =>
        {
            var pDiff = string.Compare(a.Predmet, b.Predmet, StringComparison.Ordinal);
            if (pDiff != 0) return pDiff;
            return string.Compare(a.Skupina, b.Skupina, StringComparison.Ordinal);
        });

        differentGroups.Clear();
        foreach (var subject in classes) {
            string key = subject.Opis + " " + subject.Skupina;
            var color = subject.Color;
            if (!differentGroups.Contains((key, color)))
            {
                
                differentGroups.Add((key, color));
                
            }
        }
        
        
        
        Trace.WriteLine(differentGroups.Count);
    }
    private void PopulateSettingsGrid()
    {
        // Clear the grid first if you are repopulating
        SettingsGrid.Children.Clear();
        int row = 0; // Row index in the grid

        var colorLabel = new Label
        {
            Text = "Barvit urnik",
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };
        var colorSwitch = new Microsoft.Maui.Controls.Switch
        {
            IsToggled = GetSwitchState("ColorfulSchedule", true), // Get saved state
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        colorSwitch.Toggled += (sender, e) => {
            SaveSwitchState("ColorfulSchedule", e.Value);
        };

        SettingsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        SettingsGrid.Children.Add(colorLabel);
        Grid.SetRow(colorLabel, row);
        Grid.SetColumn(colorLabel, 0);

        SettingsGrid.Children.Add(colorSwitch);
        Grid.SetRow(colorSwitch, row);
        Grid.SetColumn(colorSwitch, 1);
        row++;

        var showHiddenLabel = new Label
        {
            Text = "Prikaži skrite predmete",
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };
        var showHiddenSwitch = new Microsoft.Maui.Controls.Switch
        {
            IsToggled = GetSwitchState("ShowHiddenClasses"), // Get saved state
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        showHiddenSwitch.Toggled += (sender, e) => {
            SaveSwitchState("ShowHiddenClasses", e.Value);
        };

        SettingsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        SettingsGrid.Children.Add(showHiddenLabel);
        Grid.SetRow(showHiddenLabel, row);
        Grid.SetColumn(showHiddenLabel, 0);

        SettingsGrid.Children.Add(showHiddenSwitch);
        Grid.SetRow(showHiddenSwitch, row);
        Grid.SetColumn(showHiddenSwitch, 1);
        row++;

        SettingsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) }); // Spacer row
        row++;

        foreach (var (groupName, color) in differentGroups)
        {
            SettingsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            // Create the label for the group
            var groupStack = new HorizontalStackLayout{Spacing = 3};
            groupStack.Add(new BoxView { WidthRequest = 10, BackgroundColor = color }); // Indentation
            groupStack.Add(new Label
            {
                Text = groupName,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
            });

            // Create the switch for the group
            var groupSwitch = new Microsoft.Maui.Controls.Switch
            {
                IsToggled = GetSwitchState(groupName), // Get saved state
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                
            };

            // Handle the switch toggle event
            groupSwitch.Toggled += (sender, e) => {
                SaveSwitchState(groupName, e.Value);
            };

            // Add label and switch to the grid
            SettingsGrid.Children.Add(groupStack); // First, add the label
            Grid.SetRow(groupStack, row); // Set the row position
            Grid.SetColumn(groupStack, 0); // Set the column position (0 for label)
            
            SettingsGrid.Children.Add(groupSwitch); // Then, add the switch
            Grid.SetRow(groupSwitch, row); // Set the row position
            Grid.SetColumn(groupSwitch, 1); // Set the column position (1 for switch)
            
            row++; // Move to the next row
        }
    }

   
    private bool GetSwitchState(string key, bool def = false)
    {
        
        return Preferences.Get(key, def);
    }


    private void SaveSwitchState(string key, bool state)
    {
        // Use Preferences to save the switch state
        Preferences.Set(key, state);
    }



}