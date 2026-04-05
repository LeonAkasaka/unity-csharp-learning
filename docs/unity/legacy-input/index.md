---
layout: page
title: "補足: 旧来の Input クラスと InputManager"
permalink: /unity/legacy-input/
---

# 補足: 旧来の Input クラスと InputManager

Input System が登場する以前、Unity には **`Input` クラス** と **InputManager** という入力の仕組みが用意されていました。このページでは、それらが何であったかを簡単に紹介します。

> **このページは補足資料です。** 現在の学習には直接必要ありませんが、既存のプロジェクトや古いチュートリアルを読む際に役立ちます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 旧来の `Input` クラスの書き方を読み解ける
- InputManager が何であるかを説明できる
- Input System と旧来の仕組みの違いを大まかに把握できる

## 前提知識

- [Input System で入力操作](/unity-csharp-learning/unity/input-system/) を読んでいること

---

## 1. Input クラス

**`Input` クラス**は Unity の初期から存在する入力 API です。`UnityEngine` 名前空間に含まれているため、追加パッケージなしですぐに使えました。長年にわたって多くの Unity プロジェクトを支えてきた、実績ある仕組みです。

代表的なメソッドを以下に示します。

| メソッド | 説明 |
|---|---|
| `Input.GetKey(KeyCode.RightArrow)` | →キーを押している間ずっと `true` |
| `Input.GetKeyDown(KeyCode.Space)` | Spaceキーを押した瞬間の1フレームだけ `true` |
| `Input.GetKeyUp(KeyCode.Space)` | Spaceキーを離した瞬間の1フレームだけ `true` |
| `Input.GetAxis("Horizontal")` | 横方向の入力を −1.0 〜 1.0 の浮動小数点数で返す |

```csharp
using UnityEngine;

public class LegacyInputSample : MonoBehaviour
{
    private void Update()
    {
        // →キーを押している間、右へ移動
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * 5.0f * Time.deltaTime);
        }

        // Spaceキーを押した瞬間だけログ出力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("スペースキーが押されました");
        }
    }
}
```

`Input.GetAxis` は軸（縦・横）の入力をなめらかな数値で受け取る方法で、キーボードだけでなくゲームパッドのスティックにも対応していました。

```csharp
private void Update()
{
    var horizontal = Input.GetAxis("Horizontal"); // ←→ キーまたは左スティック
    transform.Translate(Vector3.right * horizontal * 5.0f * Time.deltaTime);
}
```

---

## 2. InputManager

**InputManager** は、`Input.GetAxis` などで使う軸名（`"Horizontal"` や `"Vertical"` など）を設定するための Unity エディター上のツールです。**Edit > Project Settings > Input Manager** から開くことができます。

あらかじめ定義された軸に対して、どのキーやボタンを割り当てるかを GUI で管理できる仕組みで、コードを書き換えずにキーの割り当てを変更できる点が便利でした。

---

## 3. 現在の立ち位置

Unity の公式ドキュメントは `Input` クラスを **"Legacy Input System"（旧来の入力システム）** と位置づけており、**新規プロジェクトでの使用は非推奨**と明記しています。

さらに重要なのは、Input System パッケージを導入したプロジェクトでは、**Project Settings > Player > Active Input Handling** の設定によって `Input` クラスの動作が変わる点です。

| Active Input Handling の設定 | 旧 `Input` クラスの扱い |
|---|---|
| Input Manager (Old) | 使用できる |
| Both | 両方使用できる |
| **Input System Package (New)** | **実行時に `InvalidOperationException` が発生する** |

このリポジトリのプロジェクトは "Input System Package (New)" に設定されているため、旧 `Input` クラスのコードはコンパイルは通るものの、実行するとエラーになります。

Input System との主な違いを整理します。

| | 旧来の `Input` クラス | Input System パッケージ |
|---|---|---|
| 導入 | 追加不要（組み込み） | パッケージマネージャーから追加 |
| キー指定 | `KeyCode` 列挙型 | デバイスのプロパティ（`rightArrowKey` など） |
| デバイス対応 | 基本的なデバイスのみ | 幅広いデバイスに対応・拡張しやすい |
| Unity の推奨 | **非推奨（Legacy）** | **現在推奨** |

> 💡 **ポイント**: 旧 `Input` クラスはシンプルで学習コストが低く、長年にわたって多くのプロジェクトを支えてきた実績があります。今も古いチュートリアルや既存プロジェクトのコードで目にすることがあるため、読み解けるようにしておくと役立ちます。

---

## まとめ

- 旧来の `Input` クラスは追加パッケージ不要で使える組み込みの入力 API
- `Input.GetKey` / `Input.GetKeyDown` / `Input.GetAxis` が主なメソッド
- InputManager はエディター上で軸の割り当てを管理するツール
- Unity 公式は新規プロジェクトでの使用を非推奨（Legacy）と位置づけている
- Input System Package (New) に設定されたプロジェクトでは、実行時に `InvalidOperationException` が発生する

---

## 次のステップ

[フィールドでデータを維持する](/unity-csharp-learning/unity/fields-basics/) では、速度などのパラメータをフィールドで管理し、Inspector から調整できるようにする方法を学びます。
