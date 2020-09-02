using Android.App;
using Android.Content.PM;
using Android.OS;
using static Android.Content.PM.ConfigChanges;

namespace MyTwitterForms.Droid
{
    [Activity(
        Label = "MyTwitterForms",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ScreenSize | Orientation | UiMode | ScreenLayout | SmallestScreenSize
    )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            this.LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[]? permissions,
            Permission[]? grantResults
        )
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
