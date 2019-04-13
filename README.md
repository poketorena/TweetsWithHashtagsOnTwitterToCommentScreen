# TweetsWithHashtagsOnTwitterToCommentScreen

Twitterから「#ctrl_sintyoku」というハッシュタグを拾ってComment Screenに投げるスクリプト

## 前提条件

Google Chromeがインストール済みであること。

## 使い方

### Windows or Mac

1. Visual Studio Installerを起動し、「.NET Core クロスプラットフォーム開発」にチェックを付けてインストールする。
1. ソリューションをクローンまたはダウンロードする。
1. ソリューションファイルをVisual Studioで開く。
1. Program.csを開き、ConsumerKey、ConsumerSecret、AccessToken、AccessTokenSecretを自分のものに書き換える。
1. F5等で実行する。



### Linux

以下を参考に気合でどうぞ。

https://dotnet.microsoft.com/download/linux-package-manager/ubuntu18-10/sdk-2.1.603

https://docs.microsoft.com/ja-jp/dotnet/core/tools/index?tabs=netcore2x

ただし

```
Connection refused Connection refused
Starting ChromeDriver 73.0.3683.68 (47787ec04b6e38e22703e856e101e840b65afe72) on port 38563
Only local connections are allowed.
Please protect ports used by ChromeDriver and related test frameworks to prevent access by malicious code.

```
と言われて動作しないのでこれをなんとかする必要があります。（未検証）

## 開発環境

* Windows 10 Home
* Visual Studio 2019 Community

## 使用したライブラリ

* .NET Core 2.1
* [CoreTweet](https://github.com/CoreTweet/CoreTweet)
* [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
* [Selenium.WebDriver](https://www.seleniumhq.org/)
* [Selenium.WebDriver.ChromeDriver](https://github.com/jsakamoto/nupkg-selenium-webdriver-chromedriver/)
* [Reactive Extensions](https://github.com/dotnet/reactive)

## 作者

[@science507](https://twitter.com/science507)
