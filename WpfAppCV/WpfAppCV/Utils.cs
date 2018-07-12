using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WpfAppCV
{
    public class Utils
    {
        public static string GetVisionApiUri()
        {
            return (string)Properties.Settings.Default["VisionApiUri"];
        }

        public static string GetVisionApiKey()
        {
            return (string)Properties.Settings.Default["VisionApiKey"];

        }

        public static string GetTextAnalyticsApiUri()
        {
            return (string)Properties.Settings.Default["TextAnalyticsApiUri"];
        }

        public static string GetTextAnalyticsApiKey()
        {
            return (string)Properties.Settings.Default["TextAnalyticsApiKey"];

        }
    }
}
