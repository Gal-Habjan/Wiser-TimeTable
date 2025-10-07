using Firebase.Database;
using Microsoft.Extensions.Logging;

namespace TimeTable
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(new FirebaseClient("https://wiser-timetable-default-rtdb.europe-west1.firebasedatabase.app/"));
            builder.Services.AddSingleton<MainPage>();

            if (!Preferences.ContainsKey("NotificationTime"))
                Preferences.Set("NotificationTime", 15);
            return builder.Build();
        }
    }
}
