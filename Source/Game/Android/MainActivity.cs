using Android.App;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Diagnostics;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Sdl.Android;

namespace AndroidGame
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : SilkActivity
    {
        GL Gl;
        IView? view;

        protected override void OnRun()
        {
            var options = ViewOptions.Default;
            options.API = new GraphicsAPI(ContextAPI.OpenGLES, ContextProfile.Compatability, ContextFlags.Default, new APIVersion(3, 0));
            view = Silk.NET.Windowing.Window.GetView(options);

            view.Load += OnLoad;
            view.Render += OnRender;
            view.Closing += OnClose;
            view.Run();
        }

        private unsafe void OnLoad()
        {
            Gl = GL.GetApi(view);
            Gl.ClearColor(1, 0.25f, 0, 1);
        }

        private unsafe void OnRender(double obj)
        {
            Gl.ClearColor(1, 0.25f, 0, 1);
            Console.WriteLine("Fuck you");
        }

        private void OnClose()
        {
            view?.Dispose();
        }
    }
}