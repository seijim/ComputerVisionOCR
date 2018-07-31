using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Threading.Tasks;


namespace WpfAppCV
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingWindow settingWindow;
        string imageDocFilePath = string.Empty;
        string regionAllTextJson = string.Empty;
        string regionAllText = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            ComboBoxFormat.Text = "TEXT";

            if ((string)Properties.Settings.Default["VisionApiKey"] == string.Empty || (string)Properties.Settings.Default["TextAnalyticsApiKey"] == string.Empty)
            {
                settingWindow = new SettingWindow();
                settingWindow.ShowDialog();
            }
        }

        private void ButtonSelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = null;
            Nullable<bool> result = false;

            // ファイルを開くダイアログ
            dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".jpg";
            dlg.Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";
            dlg.Multiselect = true;
            result = dlg.ShowDialog();
            if (result == null || result == false)
            {
                return;
            }

            imageDocFilePath = Path.GetDirectoryName(dlg.FileNames[0]);
            ListBoxImages.Items.Clear();

            foreach (var fileName in dlg.FileNames)
            {
                ListBoxImages.Items.Add(Path.GetFileName(fileName));
            }
        }

        private async void ButtonAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxImages.Items.Count < 1 || ImageDoc.Source == null)
            {
                MessageBox.Show("画像ファイルを選択してください", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ButtonAnalysis.IsEnabled = false;

            try
            {
                // HTTP Client
                var client = new HttpClient();
                var uri = Utils.GetVisionApiUri();

                // HTTP header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Utils.GetVisionApiKey());

                // HTTP content
                var bitmapSource = (BitmapSource)ImageDoc.Source;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                MemoryStream memoryStream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);
                byte[] buffer = memoryStream.GetBuffer();
                memoryStream.Dispose();
                var content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Call REST API
                var response = await client.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    ButtonAnalysis.IsEnabled = true;
                    MessageBox.Show($"Vision API の呼び出し時にエラーが発生しました。詳細⇒ {response.ReasonPhrase}", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Receive result
                string responseResult = await response.Content.ReadAsStringAsync();
                var joRegionAllText = (JObject)JsonConvert.DeserializeObject(responseResult);
                regionAllTextJson = JsonConvert.SerializeObject(joRegionAllText, Formatting.Indented);

                regionAllText = string.Empty;

                var regions = (JArray)joRegionAllText["regions"];
                if (regions.Count >= 1)
                {
                    foreach (var region in regions)
                    {
                        string regionText = string.Empty;

                        var lines = (JArray)region["lines"];
                        if (lines.Count >= 1)
                        {
                            foreach (var line in lines)
                            {
                                var words = (JArray)line["words"];
                                if (words.Count >= 1)
                                {
                                    foreach (var word in words)
                                    {
                                        regionText += (string)word["text"];
                                    }
                                    regionText += "|";
                                }
                            }
                            regionText += "||";
                        }
                        regionAllText += regionText + "|||";
                    }
                }

                if (ComboBoxFormat.Text == "JSON")
                    TextBoxJson.Text = regionAllTextJson;
                else
                    TextBoxJson.Text = regionAllText.Replace("|||", Environment.NewLine).Replace("||", Environment.NewLine).Replace("|", "　");


                string resultKeyPhrases = string.Empty;

                var keyPhrases = await ExtractKeyPhrases(regionAllText);
                if (keyPhrases.Count >= 1)
                {
                    int cnt = 0;
                    foreach (string text in keyPhrases)
                    {
                        ++cnt;
                        resultKeyPhrases += text;
                        if (cnt < keyPhrases.Count)
                            resultKeyPhrases += ", ";
                    }
                    MessageBox.Show(resultKeyPhrases, "キーフレーズ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("キーフレーズは１つも抽出できませんでした", "キーフレーズ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                ButtonAnalysis.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ButtonAnalysis.IsEnabled = true;
                MessageBox.Show("エラーが発生しました。詳細⇒ " + ex.ToString(), "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private async Task<List<string>> ExtractKeyPhrases(string text)
        {
            List<string> listKeyPhrase = new List<string>();

            string subscriptionKey = Utils.GetTextAnalyticsApiKey();
            var uri = Utils.GetTextAnalyticsApiUri();

            HttpResponseMessage response;
            string lang = "ja";

            text = text.Replace("'", "");

            // Request body
            var joDoc = new JObject();
            joDoc["lang"] = lang;
            joDoc["id"] = "1";
            joDoc["text"] = text;
            var ja = new JArray();
            ja.Add(joDoc);
            var joDocs = new JObject();
            joDocs["documents"] = ja;
            var jsonstring = JsonConvert.SerializeObject(joDocs);
            byte[] byteData = Encoding.UTF8.GetBytes(jsonstring);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Console.WriteLine(response.ToString());
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                response = await client.PostAsync(uri, content);
                var httpContent = response.Content;
                var result = await httpContent.ReadAsStringAsync();
                var joResult = (JObject)JsonConvert.DeserializeObject(result);
                var jaDocuments = (JArray)joResult["documents"];
                // Check count of ducuments
                if (jaDocuments.Count == 0)
                    return listKeyPhrase;

                var jaResult = (JArray)joResult["documents"][0]["keyPhrases"];
                if (jaResult.Count >= 1)
                {
                    foreach (var jo in jaResult)
                    {
                        listKeyPhrase.Add(jo.ToString());
                    }
                }
            }

            return listKeyPhrase;
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }

        private void ListBoxImages_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxImages.SelectedItem == null)
            {
                return;
            }

            string filePath = imageDocFilePath + "\\" + ListBoxImages.SelectedItem.ToString();
            // Load targeted image
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            FileStream stream = File.OpenRead(filePath);
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            stream.Close();

            ImageDoc.Source = bitmapImage;
        }

        private void ComboBoxFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (regionAllTextJson == string.Empty || regionAllText == string.Empty)
                return;

            if ((string)ComboBoxFormat.SelectedValue == "1")
                TextBoxJson.Text = regionAllTextJson;
            else
                TextBoxJson.Text = regionAllText.Replace("|||", Environment.NewLine).Replace("||", Environment.NewLine).Replace("|", "　");

        }
    }
}
