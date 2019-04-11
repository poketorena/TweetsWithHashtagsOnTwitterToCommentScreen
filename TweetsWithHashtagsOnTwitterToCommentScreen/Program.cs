using CoreTweet;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

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

            // フィルターストリームへの接続とツイート時の対応
            var disposable = tokens.Streaming
                .FilterAsObservable(track => "#ctrl_sintyoku")
                .Subscribe(streamingMessage =>
                {
                    // JSONをパース
                    var parsedJson = JObject.Parse(streamingMessage.Json);

                    // LINQ to JSONでツイート本文を取り出す
                    var text = (string)parsedJson["text"];

                    // 成形なしでツイート本文を表h時
                    Console.WriteLine(text);

                    // 改行
                    Console.WriteLine();

                    // 「#ctrl_sintyoku」を除去したツイート本文
                    Console.WriteLine(text.Replace("#ctrl_sintyoku", ""));
                });

            // 疑似的な無限待機
            Thread.Sleep(int.MaxValue);
        }
    }
}
