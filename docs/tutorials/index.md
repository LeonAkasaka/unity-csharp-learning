---
layout: page
title: Unity チュートリアル
permalink: /tutorials/
---

# Unity チュートリアル

Unity 基礎で学んだ知識を使って、テーマごとにゲームの機能を実装します。各セクションは独立しているので、興味のあるものから始めてかまいません。

## 3D キャラクター操作

| # | トピック | 概要 |
|---|---|---|
| 1 | [ユニティちゃん 3D チュートリアル](./character-control/unity-chan-3d/) | Animator Controller とパラメータによるアニメーション制御 |
| 2 | [SD ユニティちゃんチュートリアル](./character-control/unity-chan-sd/) | Animator.Play() によるアニメーションクリップ直接再生 |

## グリッドゲーム

| # | トピック | 概要 |
|---|---|---|
| 1 | [配列の基礎](./grid-games/array-basics/) | 一次元配列の宣言・初期化・ループ処理 |
| 2 | [二次元配列](./grid-games/array-2d/) | 二次元配列と盤面データの管理 |
| 3 | [三目並べ](./grid-games/tic-tac-toe/) | グリッドゲームの入門（手番管理・勝敗判定） |
| 4 | [ライツアウト](./grid-games/lights-out/) | 周囲探索・クリア判定の実装 |
| 5 | [マインスイーパー](./grid-games/minesweeper/) | セルの部品化・GridLayoutGroup の活用 |
| 6 | [ライフゲーム](./grid-games/life-game/) | 内部データと表示の分離・世代更新 |

## 会話シーン

| # | トピック | 概要 |
|---|---|---|
| 1 | [メッセージウィンドウ — ページ送り](./conversation-scenes/message-window-pagination/) | メッセージウィンドウの構築と、クリックでページを進める仕組みの実装 |
| 2 | [メッセージウィンドウ — 文字送り](./conversation-scenes/typewriter-animation/) | 文字を時間経過で 1 文字ずつ表示する演出の実装と、ページ送りとの組み合わせ |
| 3 | [キャラクター配置](./conversation-scenes/character-placement/) | Canvas 上に立ち絵を Image で配置し、フェードイン・フェードアウトを実装する |
| 4 | [キャラクターとメッセージウィンドウの連携](./conversation-scenes/scene-flow-control/) | キャラクター表示の完了を待ってからメッセージを表示する流れを実装する |

## 前提知識

- [Unity 基礎](/unity-csharp-learning/unity/) の内容（スクリプト・UI・Prefab など）を理解していること
- Unity 6 以降がインストールされていること

## 学習目標

- 外部アセットのキャラクターを、アニメーションを切り替えながら操作できるようになる
- 配列で盤面を管理するグリッドゲーム（オセロ・将棋・戦略シミュレーション、マス目の探索、タイルマップなど）を実装できるようになる
- RPG やノベルゲームで一般的な会話シーン（ページ送り、文字送り、立ち絵のフェード、進行管理）を実装できるようになる
