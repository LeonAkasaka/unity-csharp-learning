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
| 4 | [値を受け取って結果を返す](./parameters/) | クエリ文字列とルートパラメーターの値をハンドラーの引数で受け取り、`Results` で `200`・`400`・`404` を返し分ける |
| 4.1 | [ブラウザに HTML を返す（補足）](./html-response/) | `Content-Type` と `charset`・`Results.Content` で HTML を返す・`GET` のフォーム・HTML インジェクションと XSS・`WebUtility.HtmlEncode` でエスケープする |
| 5 | [本文でデータを送る](./request-body/) | リクエストの本文と `Content-Type`・`Content-Length` を知り、`MapPost` と `Stream` の引数で本文を読む。`405` を確かめ、実用例としてメッセージを保存して `201` を返し、共有データを `lock` で守る |
| 5.1 | [HttpContext で仕組みを見る（補足）](./http-context/) | 同じ処理を `HttpContext` だけで書き直し、ハンドラーの引数と `IResult` が `Request` と `Response` をどう読み書きしているかを確かめる |
| 6 | [Unity から通信する](./unity-webrequest/) | `UnityWebRequest` で `GET` と `POST` を送り、コルーチンと `await` で応答を待ち、`result` で成否を判断する |
| 7 | [JSON でやり取りする](./json/) | サーバーと Unity の間で JSON を送受信し、`Content-Type`、`JsonUtility` の制約、URL のエンコードを扱う |
| 8 | [失敗に備える](./failure-handling/) | 時間の制限、重ねて送らない工夫、破棄されたときの中止、失敗したリクエストの送り直しを扱う |

## 第 2 部 そろえる

| # | トピック | 概要 |
|---|---|---|
| 9 | [複数のクライアントをつなぐ](./multiple-clients/) | 回数をサーバーで数える「いいね」のサーバーを作り、ヘッダーで名乗ったクライアントをログで区別する。Multiplayer Play Mode で Unity を 2 つ動かし、ほかのクライアントの変化はリクエストを送るまでわからないことを確かめる |
| 10 | [ポーリングで追いつく](./polling/) | 一定の間隔で取得して表示を追いつかせ、間隔と遅れ・リクエスト数の関係を見る。応答が送った順に届かないことを確かめ、小さい値を無視して表示が戻るのを防ぐ |
| 10.1 | [変化がなければ本文を返さない（補足）](./conditional-get/) | `ETag` と `If-None-Match` で条件付きリクエストを送り、変化がなければ `304 Not Modified` を返す。減らせるものと減らせないものを確かめる |

## 前提知識

- [.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) で、コンソールアプリを作って実行できること
- [async と await](/unity-csharp-learning/csharp/async-await/)、[キャンセル](/unity-csharp-learning/csharp/task-cancellation/)、[例外](/unity-csharp-learning/csharp/exceptions/) を理解していること
- Unity を使う回では、[Unity 基礎](/unity-csharp-learning/unity/) の内容を理解していること

## 学習目標

- HTTP のリクエストと応答が、TCP の上でやり取りされるテキストであることを説明できる
- C# でサーバーを作り、通信の内容をログで確かめながら開発できるようになる
- Unity からサーバーと通信し、ゲームの機能に組み込めるようになる
