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

## 前提知識

- [.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) で、コンソールアプリを作って実行できること
- [async と await](/unity-csharp-learning/csharp/async-await/)、[キャンセル](/unity-csharp-learning/csharp/task-cancellation/)、[例外](/unity-csharp-learning/csharp/exceptions/) を理解していること
- Unity を使う回では、[Unity 基礎](/unity-csharp-learning/unity/) の内容を理解していること

## 学習目標

- HTTP のリクエストと応答が、TCP の上でやり取りされるテキストであることを説明できる
- C# でサーバーを作り、通信の内容をログで確かめながら開発できるようになる
- Unity からサーバーと通信し、ゲームの機能に組み込めるようになる
