---
layout: page
title: ネットワーク通信
permalink: /networking/
---

# ネットワーク通信

ゲームとサーバーの間の通信を、低いレベルの仕組みから順に学びます。サーバーは C# のコンソールアプリとして作り、受け取った通信の内容をログに出して、目で見えるようにします。ゲーム側は Unity 標準の Web リクエストを使ってサーバーと対話します。

## 第 1 部 つながる

| # | トピック | 概要 |
|---|---|---|
| 1 | [TCP で受け取る](./tcp-receive/) | `TcpListener` で接続を待ち受け、届いたリクエストをそのまま表示して、HTTP がテキストであることを確かめる |
| 2 | [TCP で送る](./tcp-send/) | `TcpClient` でリクエストを手で書いて送り、応答の終わりがどう決まるかを確かめる |
| 3 | [ASP.NET Core でサーバーを作る](./aspnetcore-server/) | `MapGet` でパスとハンドラーを結び付け、ミドルウェアでリクエストと応答をログに出す |
| 4 | [HTTP のメソッドとステータスコード](./http-methods/) | `GET`・`POST`・`PUT`・`DELETE` でメッセージを操作し、ルートパラメーター・クエリ文字列・ステータスコードを扱う |
| 4.1 | [ブラウザに HTML を返す（補足）](./html-response/) | `Content-Type` と `charset`・`Results.Content` で HTML を返す・`GET` のフォーム・HTML インジェクションと XSS・`WebUtility.HtmlEncode` でエスケープする |
| 5 | [Unity から通信する](./unity-webrequest/) | `UnityWebRequest` で `GET` と `POST` を送り、コルーチンと `await` で応答を待ち、`result` で成否を判断する |
| 6 | [JSON でやり取りする](./json/) | サーバーと Unity の間で JSON を送受信し、`Content-Type`、`JsonUtility` の制約、URL のエンコードを扱う |
| 7 | [失敗に備える](./failure-handling/) | 時間の制限、重ねて送らない工夫、破棄されたときの中止、失敗したリクエストの送り直しを扱う |

## 第 2 部 そろえる

| # | トピック | 概要 |
|---|---|---|
| 8 | [複数のクライアントをつなぐ](./multiple-clients/) | 回数をサーバーで数える「いいね」のサーバーを作り、ヘッダーで名乗ったクライアントをログで区別する。Multiplayer Play Mode で Unity を 2 つ動かし、ほかのクライアントの変化はリクエストを送るまでわからないことを確かめる |

## 前提知識

- [.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) で、コンソールアプリを作って実行できること
- [async と await](/unity-csharp-learning/csharp/async-await/)、[キャンセル](/unity-csharp-learning/csharp/task-cancellation/)、[例外](/unity-csharp-learning/csharp/exceptions/) を理解していること
- Unity を使う回では、[Unity 基礎](/unity-csharp-learning/unity/) の内容を理解していること

## 学習目標

- HTTP のリクエストと応答が、TCP の上でやり取りされるテキストであることを説明できる
- C# でサーバーを作り、通信の内容をログで確かめながら開発できるようになる
- Unity からサーバーと通信し、ゲームの機能に組み込めるようになる
