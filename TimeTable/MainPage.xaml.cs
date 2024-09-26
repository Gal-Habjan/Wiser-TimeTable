using System.Text.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Storage;
namespace TimeTable
{

    /*  HOW TO USE
     *  zacne se v mainPage()
     * in detailPage for when clicking on labels
     * setting for the settings button
     * 
     * 

     */
    public partial class MainPage : ContentPage
    {
        public List<ClassEntry> availableClasses = new List<ClassEntry>();
        public MainPage()
        {
            InitializeComponent();
            InitializeTimetable(); // creates the table (defines rows and collums) each line is a row too thats why there is more than there are hours
            PopulateTimeTable(); // put classes there and seperators and stuff
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Call your function here every time the page appears
            RefreshTimetableAsync();
        }

        private void InitializeTimetable()
        {
            // Define columns (Time + Days of the Week)
            TimetableGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Time column
            for (int i = 0; i < 5; i++)
            {
                TimetableGrid.ColumnDefinitions.Add(new ColumnDefinition());
                TimetableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Define rows (7:00 to 20:00)
            TimetableGrid.RowDefinitions.Add(new RowDefinition()); // First row for time/days header
            for (int i = 7; i <= 20; i++)
            {
                TimetableGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) }); // Separator row
                TimetableGrid.RowDefinitions.Add(new RowDefinition()); // Time slots from 7:00 to 20:00
            }

            DrawTimeTable(); // mostly just lines and text on top


        }
        public async void PopulateTimeTable() // class labels
        { // Use System.Text.Json or Newtonsoft.Json

            string[] daysOfWeek = ["Ponedeljek", "Torek", "Sreda", "Četrtek", "Petek"];
            await GetAvailableSubjects();
            foreach (var classEntry in availableClasses)
            {
                
                string[] timeRange = classEntry.Ura.Split('-');
                var startTime = int.Parse(timeRange[0].Split(':')[0]);
                var endTime = int.Parse(timeRange[1].Split(':')[0]);

                int dayColumn = Array.IndexOf(daysOfWeek, classEntry.Dan) * 2 + 1; 

                int rowSpan = (endTime - startTime) * 2 - 1;
                int startRow = (startTime - 6) * 2;

                hasOverlap(classEntry, availableClasses);
                int fontSize = classEntry.hasOverlap ? 8 : 10;

                //ime
                var firstPart = new Span
                {
                    Text = classEntry.Opis + "\n",
                    FontSize = fontSize, 

                };

                //prostor
                var secondPart = new Span
                {
                    Text = classEntry.Prostor,
                    FontSize = fontSize-2,

                };

                // Create a formatted string with both spans
                var formattedString = new FormattedString();
                formattedString.Spans.Add(firstPart);
                formattedString.Spans.Add(secondPart);
                //putting them together
                // Create the label
                var classLabel = new Label
                {
                    FormattedText = formattedString,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    BackgroundColor = Color.FromRgb(80, 80, 80), // Background color
                    Margin = new Thickness(1)
                };
                classLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnClassLabelTapped(classEntry)) // Pass the classEntry to the tap handler
                });




                TimetableGrid.Children.Add(classLabel);
                Grid.SetRow(classLabel, startRow);

                Grid.SetRowSpan(classLabel, rowSpan);
                
                Grid.SetColumn(classLabel, dayColumn);

                if (classEntry.hasOverlap) {
                    if (!classEntry.isFirst)
                    {

                        Grid.SetColumn(classLabel, dayColumn + 1);
                    }

                    Grid.SetColumnSpan(classLabel, 1);

                }
                else
                {
                    Grid.SetColumnSpan(classLabel, 2);

                }
            }
        }
        public void DrawTimeTable()
        {
            var timeHeader = new Label { Text = "Time", FontSize = 7 };
            TimetableGrid.Children.Add(timeHeader);
            Grid.SetRow(timeHeader, 0);
            Grid.SetColumn(timeHeader, 0);

            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri" };
            for (int i = 0; i < daysOfWeek.Length; i++)
            {
                //CHANGE DAY LABELS HERE
                var dayLabel = new Label { Text = daysOfWeek[i] };
                TimetableGrid.Children.Add(dayLabel);
                Grid.SetRow(dayLabel, 0);
                Grid.SetColumn(dayLabel, i * 2 + 1);
                Grid.SetColumnSpan(dayLabel, 2);
            }

            // Add time slots and separators
            for (int i = 7; i <= 20; i++)
            {
                // Add time label
                var timeLabel = new Label { Text = $"{i}:00", HeightRequest = 40, FontSize = 5 };
                TimetableGrid.Children.Add(timeLabel);
                Grid.SetRow(timeLabel, (i - 6) * 2);
                Grid.SetColumn(timeLabel, 0);

                // Add separator (BoxView as a white line)
                var separator = new BoxView { BackgroundColor = Color.FromRgb(255, 255, 255), HeightRequest = 1 };
                TimetableGrid.Children.Add(separator);
                Grid.SetRow(separator, (i - 6) * 2 - 1);
                Grid.SetColumnSpan(separator, 11); // Span the separator across all columns
            }
        }
        public void hasOverlap(ClassEntry subject, List<ClassEntry> classes)
        {


            
            DateTime.TryParse(subject.Datum, out DateTime thisSubjectDate);
            foreach (ClassEntry classEntry in classes)
            {
                if (classEntry == subject)
                {
                    classEntry.isFirst = true;
                    
                    continue;
                }
                DateTime.TryParse(classEntry.Datum, out DateTime classDate);
                if (thisSubjectDate == classDate)
                {
                    string[] timeRange1 = classEntry.Ura.Split('-');
                    int startTime1 = int.Parse(timeRange1[0].Split(':')[0]);
                    int endTime1 = int.Parse(timeRange1[1].Split(':')[0]);
                    string[] timeRange2 = subject.Ura.Split('-');
                    int startTime2 = int.Parse(timeRange2[0].Split(':')[0]);
                    int endTime2 = int.Parse(timeRange2[1].Split(':')[0]);

                    if (startTime2 < endTime1 && startTime1 < endTime2)
                    {
                        if (!subject.isFirst) {
                            
                            subject.isFirst = !classEntry.isFirst;
                        }

                        subject.hasOverlap = true;
                        return;
                    }
                }
                if (thisSubjectDate < classDate)
                {
                    subject.hasOverlap = false;
                    return;
                }

            }
            subject.hasOverlap = false;
           
        }
        private async void OnClassLabelTapped(ClassEntry classEntry)
        {
            // Navigate to the ClassDetailPage, passing the ClassEntry object
            await Shell.Current.Navigation.PushModalAsync(new ClassDetailPage(classEntry));
        }
        private void OnRefreshButtonClicked(object sender, EventArgs e)
        {
            RefreshTimetableAsync();
        }
        public void RefreshTimetableAsync()
        {
            // Clear the existing children in the grid
            TimetableGrid.Children.Clear();

            // Optionally, you can reset any other properties or settings here

            // Call the method to repopulate the timetable
            DrawTimeTable();
            PopulateTimeTable();
        }
        public async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new SettingsPage());
        }
        public async Task GetAvailableSubjects()
        {

            
            string jsonData;
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("school.json");
                using var reader = new StreamReader(stream);
                jsonData = await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found)
                Trace.WriteLine($"Error loading JSON file: {ex.Message}");
                return;
            }
            availableClasses.Clear();
            // Deserialize the JSON data (make sure to create your classes accordingly)
            List<ClassEntry> classes = JsonSerializer.Deserialize<RootObject>(jsonData)?.Classes;
            DateTime currentDate = DateTime.Now.Date.AddDays(7);
            int currentDayOfWeek = (int)currentDate.DayOfWeek;
            DateTime minDate = currentDate.AddDays(-currentDayOfWeek);
            DateTime maxDate = currentDayOfWeek == 6 ? currentDate.AddDays(7) : currentDate.AddDays(7 - currentDayOfWeek);

            foreach (var classEntry in classes)
            {
                //DateTime.TryParseExact(classEntry.Datum, "dd.MM.yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out DateTime d1);
                //Trace.WriteLine(d1 +" " + minDate +" "+maxDate+" "+ (d1 >= minDate)+" "+(d1 < maxDate));
                if (DateTime.TryParseExact(classEntry.Datum, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime classDate) && classDate >= minDate && classDate < maxDate)
                {
                    if (!Preferences.ContainsKey(classEntry.Skupina))
                    {
                        availableClasses.Add(classEntry);
                        Preferences.Set(classEntry.Skupina, true);
                        continue;
                    }
                    if (Preferences.Get(classEntry.Skupina, false))
                    {
                        availableClasses.Add(classEntry);
                    }
                }

            }
        }
    }

}
