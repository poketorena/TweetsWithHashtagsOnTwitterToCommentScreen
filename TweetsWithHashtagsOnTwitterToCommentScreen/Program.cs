using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

                    // ツイート本文からハッシュタグの文字列を除去するβ（デフォルトでは「#ctrl_sintyoku」を除去する）
                    var hashtagRemovedText = RemoveHashtag(text);

                    // ハッシュタグ取り除いた文字列を表示
                    Console.WriteLine(hashtagRemovedText);

                    // 省略用URLの開始インデックスを取得
                    var clipStartIndex = hashtagRemovedText.IndexOf(@"https://t.co/");

                    var clipedText = hashtagRemovedText;

                    if (clipStartIndex != -1)
                    {
                        // 長いツイートの場合末尾に省略用URLが付くのでそれを取り除く
                        clipedText = hashtagRemovedText.Substring(0, clipStartIndex - 1);
                    }

                    // 省略URLを取り除いた文字列を表示
                    Console.WriteLine(clipedText);

                    // ChromeOptionsオブジェクトを生成する
                    var options = new ChromeOptions();

                    // --headlessを追加（これを追加するとバックグラウンドでChromeを起動できる）
                    options.AddArgument("--headless");

                    // ChromeOptions付きでChromeDriverオブジェクトを生成する
                    var chrome = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), options);

                    // URLに移動する
                    chrome.Url = @"https://commentscreen.com/comments?room=" + hashtagText;

                    try
                    {
                        // 文字列を自動入力する
                        chrome.FindElementByClassName("type_msg").SendKeys(clipedText);

                        // 送信ボタンを押す
                        chrome.FindElementByClassName("send_btn").Click();

                        // 送信確認用コメント
                        Console.WriteLine($"CommentScreenに「{clipedText}」を送信しました");
                    }
                    catch (Exception e)
                    {
                        // 例外を握りつぶすてコンソールに例外内容を出力する
                        Console.WriteLine("Chromeの自動操作時に例外が発生しました。おそらく絵文字が原因です。");
                        Console.WriteLine($"送信しようとした文字列は「{clipedText}」です。");
                        Console.WriteLine("---以下エラーメッセージ---");
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        // ブラウザを閉じる
                        chrome.Quit();
                    }

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
                        // リソースの開放
                        disposable.Dispose();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ハッシュタグを除去した文字列を返します。
        /// </summary>
        /// <param name="sourceText">ハッシュタグを含む文字列</param>
        /// <returns>ハッシュタグを除去した文字列</returns>
        static string RemoveHashtag(string sourceText)
        {
            var splitedTexts = sourceText.Split("\r\n").ToList();

            var resultTexts = new List<string>();

            var skip = false;

            foreach (var line in splitedTexts)
            {
                if (line.Length > 3 && line.Substring(0, 2) == "RT")
                {
                    continue;
                }

                var stringBuilder = new StringBuilder();

                foreach (var c in line)
                {
                    if (skip)
                    {
                        if (c == ' ')
                        {
                            skip = false;
                        }
                        continue;
                    }

                    // ハッシュタグの読み飛ばし
                    if (c == '#')
                    {
                        skip = true;
                        continue;
                    }

                    stringBuilder.Append(c);
                }
                skip = false;

                resultTexts.Add(stringBuilder.ToString());
            }

            if (resultTexts.Count == 1)
            {
                return resultTexts[0];
            }
            else
            {
                var stringBuilder = new StringBuilder();

                foreach (var (line, index) in resultTexts.Indexed())
                {
                    stringBuilder.Append(line);

                    if (index != resultTexts.Count - 1)
                    {
                        stringBuilder.Append("\r\n");
                    }
                }

                return stringBuilder.ToString();
            }
        }
    }
}
