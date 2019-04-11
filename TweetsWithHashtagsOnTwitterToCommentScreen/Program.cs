using System;
using System.IO;
using System.Reflection;
using CoreTweet;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;

namespace TweetsWithHashtagsOnTwitterToCommentScreen
{
    class Program
    {
        static void Main(string[] args)
        {
            // トークンの作成
            var tokens = Tokens.Create(
                "ConsumerKey",
                "ConsumerSecret",
                "AccessToken",
                "AccessTokenSecret"
                );

            // 監視するハッシュタグ
            var hashtagText = "ctrl_sintyoku";

            // フィルターストリームへの接続とツイート時の対応
            var disposable = tokens.Streaming
                .FilterAsObservable(track => $"#{hashtagText}")
                .Subscribe(streamingMessage =>
                {
                    // JSONをパース
                    var parsedJson = JObject.Parse(streamingMessage.Json);

                    // LINQ to JSONでツイート本文を取り出す
                    var text = (string)parsedJson["text"];

                    // 成形なしでツイート本文を表示
                    Console.WriteLine(text);

                    // 改行
                    Console.WriteLine();

                    // ツイート本文からハッシュタグのテキストを除去するβ（デフォルトでは「#ctrl_sintyoku」を除去する）
                    var hashtagRemovedText = text.Replace($"#{hashtagText}", "");

                    // ハッシュタグ取り除いたテキストを表示
                    Console.WriteLine(hashtagRemovedText);

                    // ChromeOptionsオブジェクトを生成する
                    var options = new ChromeOptions();

                    // --headlessを追加（これを追加するとバックグラウンドでChromeを起動できる）
                    options.AddArgument("--headless");

                    // ChromeOptions付きでChromeDriverオブジェクトを生成する
                    var chrome = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), options);

                    // URLに移動する
                    chrome.Url = @"https://commentscreen.com/comments?room=" + hashtagText;

                    // テキストを自動入力する
                    chrome.FindElementByClassName("type_msg").SendKeys(hashtagRemovedText);

                    // 送信ボタンを押す
                    chrome.FindElementByClassName("send_btn").Click();

                    // ブラウザを閉じる
                    chrome.Quit();

                    // 送信確認用コメント
                    Console.WriteLine($"CommentScreenに「{hashtagRemovedText}」を送信しました");
                });

            // 接続確認用コメント
            Console.WriteLine("フィルターストリームに接続しました");

            // 以下終了のための処理
            while (true)
            {
                // キー入力をチェックし、Eが入力されたらプログラムを終了する
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().Key.ToString() == "E")
                    {
                        return;
                    }
                }
            }
        }
    }
}
