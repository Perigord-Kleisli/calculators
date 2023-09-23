using System;
using Gtk;

namespace csharp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.csharp.csharp", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            Gtk.CssProvider provider = new CssProvider();

            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("style.css")) {
              using (var reader = new System.IO.StreamReader(stream)) {
                provider.LoadFromData(reader.ReadToEnd());
              }
            }

            var win = new MainWindow();
            app.AddWindow(win);
            Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, provider, 800);

            win.Show();
            Application.Run();
        }
    }
}
