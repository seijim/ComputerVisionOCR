using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace WpfAppCV
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            if ((string)Properties.Settings.Default["VisionApiKey"] != string.Empty)
            {
                TextBoxVisionApiUrl.Text = (string)Properties.Settings.Default["VisionApiUri"];
                TextBoxVisionApiKey.Text = (string)Properties.Settings.Default["VisionApiKey"];
            }

            if ((string)Properties.Settings.Default["TextAnalyticsApiKey"] != string.Empty)
            {
                TextBoxVisionApiUrl.Text = (string)Properties.Settings.Default["TextAnalyticsApiUri"];
                TextBoxVisionApiKey.Text = (string)Properties.Settings.Default["TextAnalyticsApiKey"];
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["VisionApiUri"] = TextBoxVisionApiUrl.Text.Trim();
            Properties.Settings.Default["VisionApiKey"] = TextBoxVisionApiKey.Text.Trim();
            Properties.Settings.Default["TextAnalyticsApiUri"] = TextBoxTextAnalyticsApiUr.Text.Trim();
            Properties.Settings.Default["TextAnalyticsApiKey"] = TextBoxTextAnalyticsApiKey.Text.Trim();
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
