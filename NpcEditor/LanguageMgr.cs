using System.Reflection;
using System.Resources;

namespace NpcEditor
{
    public static class LanguageMgr
    {
        private static ResourceManager _rm;

        static LanguageMgr()
        {
            _rm = new ResourceManager("NpcEditor.Language", Assembly.GetExecutingAssembly());
        }

        public static string GetTranslation(string name, params object[] args)
        {
            string text = _rm.GetString(name);
            try
            {
                text = string.Format(text, args);
            }
            catch //(Exception exception)
            {
                //log.Error("Parameters number error, ID: " + name + " (Arg count=" + args.Length + ")", exception);
            }

            if (string.IsNullOrEmpty(text))
                text = name;

            return text;
        }
    }
}
