# Azure Cognitive Services - Computer Vision API (OCR) v2 を利用したサンプル WPF アプリケーション 
<a href="https://westus.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fc">Azure Cognitive Services - Computer Vision API v2 の OCR 機能</a>と <a href="https://westus.dev.cognitive.microsoft.com/docs/services/TextAnalytics.V2.0/operations/56f30ceeeda5650db055a3c6">Azure Cognitive Services - Text Analytics API v2 キーフレーズ自動抽出機能</a>を利用した以下のような画像解析 WPF アプリケーションのサンプルコードとなります。インストーラー (Installer.zip) も用意しておりますので、お試しください。<br/>
<img src="./images/computerVisonWpfApp08.png" /><br/><br/>
## アプリケーションの利用手順<br/><br/>
アプリケーションを起動すると、設定画面が表示されますので、Azure Portal で作成した Cognitive Services の Computer Vision API と Text Analytics API のエンドポイントとキーを設定し、「設定の保存」をクリックしてください。再度、設定が必要な場合は、メイン画面の「ファイル」メニューの「設定画面の表示」をクリックしてください。  
<img src="./images/computerVisonWpfApp01.png" /><br/><br/>
設定を終えると、メイン画面が表示されますので、解析したい画像ファイルを選択します。複数の画像ファイルを同時に選択することが可能です。
<img src="./images/computerVisonWpfApp03.png" /><br/>
<img src="./images/computerVisonWpfApp04.png" width="70%"/><br/><br/>
画像ファイル名をクリックすると、プレビュー表示されます。
<img src="./images/computerVisonWpfApp05.png" /><br/><br/>
分析したい画像ファイル名をクリックし、「画像分析の実行」ボタンをクリックすると、Computer Vision API と Text Analytics API が呼び出されます。
<img src="./images/computerVisonWpfApp06.png" /><br/><br/>
実行結果は、以下のように表示されます。メッセージ ボックスには、キーフレーズが表示され、メイン画面左下に OCR 機能で読み取ったテキストが表示されます。
<img src="./images/computerVisonWpfApp07.png" /><br/><br/>
コンボボックスで、「JSON」を指定すると、Computer Vision API から返却されたそのままの JSON が表示されます。
<img src="./images/computerVisonWpfApp08.png" /><br/><br/>




