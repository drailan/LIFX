
namespace LIFXSeeSharp.Logging
{
    public class Log
    {
        public static void Debug(string tag, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] [" + tag + "] " + message, args);
        }

        public static void Info(string tag, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[INFO] [" + tag + "] " + message, args);
        }

        public static void Error(string tag, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[ERROR] [" + tag + "] " + message, args);
        }

        public static void Wtf(string tag, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[WTF] [" + tag + "] " + message, args);
        }
    }
}
