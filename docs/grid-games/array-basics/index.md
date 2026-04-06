---
layout: default
title: 配列の基礎
permalink: /grid-games/array-basics/
---

## 概要

このチュートリアルでは、C# における配列の基本を学びます。配列は複数のデータをまとめて管理するための重要な機能です。

### 学習目標

- 配列とは何かを理解する
- 配列の宣言と初期化方法を習得する
- 配列の要素へのアクセス方法を理解する
- Debug.Log を使って配列の内容を確認できるようになる
- ループ処理で配列の全要素を操作できるようになる

## 1. 配列とは

### 配列の概念

配列とは、同じ型の複数のデータを一つの変数名でまとめて管理できる仕組みです。

例えば、敵キャラクターの名前を管理する場合を考えてみましょう。

```csharp
// 配列を使わない場合
string enemy1 = "スライム";
string enemy2 = "ゴブリン";
string enemy3 = "オーク";
string enemy4 = "ドラゴン";
string enemy5 = "デーモン";
```

このように個別の変数で管理すると、データが増えるたびに変数を追加する必要があり、管理が大変です。

配列を使うと、次のようにまとめて管理できます。

```csharp
// 配列を使う場合
string[] enemies = new string[5];
enemies[0] = "スライム";
enemies[1] = "ゴブリン";
enemies[2] = "オーク";
enemies[3] = "ドラゴン";
enemies[4] = "デーモン";
```

他にも、プレイヤーのHPを管理する場合は以下のように書けます。型が文字列ではなく整数になっていますが、考え方は同じです。

```csharp
// 配列を使わない場合
int player1HP = 100;
int player2HP = 150;
int player3HP = 120;
int player4HP = 200;

// 配列を使う場合
int[] playerHP = new int[4];
playerHP[0] = 100;
playerHP[1] = 150;
playerHP[2] = 120;
playerHP[3] = 200;
```

文字列や整数といった基本型だけではなく、`GameObject` や `Vector3` など Unity の型でも同様に配列にできます。例えば `Sprite` の配列は以下のように書けます。

```csharp
// 配列を使わない場合
Sprite itemIcon1;
Sprite itemIcon2;
Sprite itemIcon3;

// 配列を使う場合
Sprite[] itemIcons = new Sprite[3];
```

### なぜ配列が必要なのか

配列を使うメリットは以下の通りです。

- データの一括管理: 関連するデータをグループ化できる
- ループ処理: for 文や foreach 文で効率的に処理できる
- コードの簡潔化: 変数の数を減らしてコードを読みやすくできる
- 柔軟性: データの数が変わっても対応しやすい

配列はプログラミングのデータ管理において中心的な役割を果たします。例えば、ゲーム開発において以下のような場面で活用されるでしょう。

- 敵キャラクターの管理
- アイテムリストの保持
- マップデータの格納

## 2. 配列の宣言と初期化

配列を使うには、まず宣言と初期化を行う必要があります。C# では様々な方法で配列を作成できます。

### 2-1. 基本的な宣言方法

配列の宣言は次の形式で行います。

```csharp
型[] 配列名;
```

例：

```csharp
int[] numbers;      // int型の配列
string[] names;     // string型の配列
float[] heights;    // float型の配列
```

ただし、この段階ではまだ配列は使えません。配列型の変数を用意しただけです。配列を利用するには、初期化が必要です。

### 2-2. サイズを指定して初期化

配列を使えるようにするには、**`new` 演算子**を使ってサイズを指定した**配列インスタンス**を生成します。

```csharp
配列名 = new 型[サイズ];
```

配列の宣言と同時に初期化を行うこともできます。

```csharp
型[] 配列名 = new 型[サイズ];
```

例：

```csharp
int[] numbers = new int[5];  // 5個のint型を格納できる配列
string[] names = new string[10];  // 10個のstring型を格納できる配列
float[] speeds = new float[3];  // 3個のfloat型を格納できる配列
```

この場合、配列の各要素は自動的に初期値で埋められます。

- 数値型（int, float など）: `0`
- bool型: `false`
- 参照型（string, オブジェクトなど）: `null`

また、配列は作成後にサイズを変更できません（要素数は固定です）。

### 2-3. インスタンス生成後に値を入れる

ここまでで、配列インスタンスを生成できました。ただし `new` で作った直後の配列は、前述のように初期値で初期化されています。

次のセクションでは、添字（インデックス）を使って配列の要素にアクセスし、必要な値を入れたり取り出したりする方法を学びます。

## 3. 配列の要素へのアクセス

### 3-1. 要素と添字（インデックス）

配列に格納されている個々のデータを要素と呼びます。

各要素にアクセスするには**添字**を使います。添字は、一般に**インデックス**とも呼ばれます。

 添字は `0` から始まり（最初の要素は `0` 番目）、最後の要素の添字は `配列のサイズ - 1` になります。例えば、サイズが 5 の配列において有効な添字は `0, 1, 2, 3, 4` です。

```text
配列: [ "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" ]
添字:      0           1          2         3           4
```

### 3-2. 要素へのアクセス方法

配列の要素にアクセスするには、次の文法を使います。

```csharp
// 要素の取得
変数 = 配列名[添字];

// 要素の設定
配列名[添字] = 値;
```

例：

```csharp
string[] enemies = new string[5];
enemies[0] = "スライム";
enemies[1] = "ゴブリン";
enemies[2] = "オーク";
enemies[3] = "ドラゴン";
enemies[4] = "デーモン";

// 要素を取得
string first = enemies[0];   // "スライム" を取得
string third = enemies[2];   // "オーク" を取得

// 要素を変更
enemies[1] = "キングゴブリン";  // 2番目の要素を変更
enemies[4] = "魔王";            // 5番目の要素を変更
```

### 3-3. 実際に動作確認してみましょう

先ほどの `Sample` スクリプトに、要素へのアクセスを追加してみましょう。

```csharp
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        // 配列を作成
        string[] enemies = new string[5];

        // インスタンス生成後に、添字で各要素を初期化
        enemies[0] = "スライム";
        enemies[1] = "ゴブリン";
        enemies[2] = "オーク";
        enemies[3] = "ドラゴン";
        enemies[4] = "デーモン";

        // 各要素を取得して表示
        Debug.Log("enemies[0] = " + enemies[0]);  // スライム
        Debug.Log("enemies[1] = " + enemies[1]);  // ゴブリン
        Debug.Log("enemies[2] = " + enemies[2]);  // オーク
        Debug.Log("enemies[3] = " + enemies[3]);  // ドラゴン
        Debug.Log("enemies[4] = " + enemies[4]);  // デーモン

        Debug.Log("--- 要素を変更します ---");

        // 要素を変更
        enemies[1] = "キングゴブリン";
        enemies[4] = "魔王";

        // 変更後の内容を表示
        Debug.Log("enemies[1] = " + enemies[1]);  // キングゴブリン
        Debug.Log("enemies[4] = " + enemies[4]);  // 魔王

        // 数値の配列でも同様
        int[] scores = new int[3];
        scores[0] = 100;
        scores[1] = 200;
        scores[2] = 150;

        Debug.Log("scores[0] = " + scores[0]);  // 100
        Debug.Log("scores[1] = " + scores[1]);  // 200
        Debug.Log("scores[2] = " + scores[2]);  // 150
    }
}
```

Unity で再生して、Console ウィンドウで結果を確認してください。

### 3-4. 初期値が決まっている場合：配列初期化子

配列に格納する値が**初期化の時点で決まっている**なら、毎回 `配列名[添字] = 値;` を書く代わりに、**配列初期化子**でまとめて書けます。

```csharp
// 方法1: サイズを明示
型[] 配列名 = new 型[サイズ] { 値1, 値2, 値3, ... };

// 方法2: サイズを省略（要素数から自動的に決まる）
型[] 配列名 = new 型[] { 値1, 値2, 値3, ... };

// 方法3: new を省略（最も簡潔）
型[] 配列名 = { 値1, 値2, 値3, ... };
```

例：

```csharp
// 方法1: new を使う
int[] scores = new int[5] { 100, 200, 150, 300, 250 };

// 方法2: サイズを省略（自動的にサイズが決まる）
int[] scores2 = new int[] { 100, 200, 150, 300, 250 };

// 方法3: さらに簡潔な書き方
int[] scores3 = { 100, 200, 150, 300, 250 };
```

型を変えても考え方は同じです。例えば文字列の配列の場合は、以下のように初期化できます。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン" };
```

### 3-5. 添字の範囲に注意

重要: 配列の範囲外の添字を指定すると、エラーが発生します。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク" };  // サイズは3

Debug.Log(enemies[0]);  // OK: スライム
Debug.Log(enemies[2]);  // OK: オーク
Debug.Log(enemies[3]);  // エラー！ IndexOutOfRangeException

// サイズが3の配列の添字は 0, 1, 2 のみ有効
```

このエラーを防ぐには、配列のサイズを確認する必要があります。これについては後のセクションで学びます。

## 4. 配列のループ処理

配列の真価は、ループ処理と組み合わせることで発揮されます。配列の全要素を順番に処理する方法を学びましょう。

### 4-1. for 文で配列を走査

`for` 文を使うと、配列の各要素に順番にアクセスできます。

基本パターンは以下のようになるでしょう。

```csharp
for (int i = 0; i < 繰り返し回数; i++)
{
    // 配列名[i] の処理
}
```

例：

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

// 全要素を順番に表示
for (int i = 0; i < 5; i++)
{
    Debug.Log(enemies[i]);
}
```

このコードは以下と同じ結果になります。

```csharp
Debug.Log(enemies[0]);
Debug.Log(enemies[1]);
Debug.Log(enemies[2]);
Debug.Log(enemies[3]);
Debug.Log(enemies[4]);
```

### 4-2. 実際に動作確認してみましょう

`Sample` スクリプトで `for` 文を使った配列の走査を試してみましょう。

```csharp
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        // 配列を作成
        string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

        Debug.Log("=== 敵キャラクター一覧 ===");

        // for 文で全要素を表示
        for (int i = 0; i < 5; i++)
        {
            Debug.Log((i + 1) + "番目: " + enemies[i]);
        }

        Debug.Log("=== スコア一覧 ===");

        // 数値の配列でも同様
        int[] scores = { 100, 200, 150, 300, 250 };

        for (int i = 0; i < 5; i++)
        {
            Debug.Log("scores[" + i + "] = " + scores[i]);
        }

        Debug.Log("=== 合計を計算 ===");

        // 配列の要素を合計する
        int total = 0;
        for (int i = 0; i < 5; i++)
        {
            total += scores[i];  // total = total + scores[i]
        }
        Debug.Log("合計: " + total);  // 1000
    }
}
```

Unity で再生して、Console ウィンドウで結果を確認してください。

### 確認ポイント

- `for` 文のカウンター変数 `i` は添字として使います
- `i` は `0` から始まり、`4` まで繰り返されます（`i < 5` のため）
- ループ内で `enemies[i]` のように配列の各要素にアクセスできます
- 配列の要素を使った計算（合計など）も簡単に行えます

### 4-3. 問題点：マジックナンバー

上記のコードには問題があります。繰り返し回数を `5` というリテラル（マジックナンバー）で指定しています。

```csharp
for (int i = 0; i < 5; i++)  // この 5 はどこから来たのか？
```

これには以下の問題があります：

1. 可読性: なぜ `5` なのか、コードを読むだけではわかりにくい
2. 保守性: 配列のサイズが変わったら、すべての `5` を変更する必要がある
3. エラーリスク: 配列のサイズと繰り返し回数が一致しないと、範囲外アクセスが発生する

例えば、配列に要素を追加した場合を考えてみましょう。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン", "キングスライム" };

// ループの回数を修正し忘れると...
for (int i = 0; i < 5; i++)  // 6個目の要素が表示されない！
{
    Debug.Log(enemies[i]);
}
```

あるいは、配列から要素を削除した場合は以下のような問題が発生します。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク" };

// ループの回数を修正し忘れると...
for (int i = 0; i < 5; i++)  // IndexOutOfRangeException エラー！
{
    Debug.Log(enemies[i]);
}
```

この問題を解決するために、配列の **`Length` プロパティ**を使います。

## 5. 配列の長さ

### 5-1. Length プロパティ

C# の配列は全て `Array` 型の共通機能を持っています。`Length` プロパティは `Array` 型が提供するプロパティの一つで、配列のサイズ（要素の数）を取得できます。


```csharp
public int Length { get; }
```

`Length` プロパティは配列の要素数を `int` 型で返します。

例：

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };
int count = enemies.Length;  // 5 が代入される

int[] numbers = new int[10];
int size = numbers.Length;   // 10 が代入される
```

### 5-2. Length を使った安全なループ

`Length` プロパティを使うことで、マジックナンバーを避け、配列のサイズが変わっても自動的に対応できます。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

// Length を使った安全なループ
for (int i = 0; i < enemies.Length; i++)
{
    Debug.Log(enemies[i]);
}
```

この書き方には以下のメリットがあります。

1. 自動対応: 配列のサイズが変わっても、コードを変更する必要がない
2. 可読性: 「配列の要素数だけ繰り返す」という意図が明確
3. 安全性: 範囲外アクセスが発生しない

例えば、配列に要素を追加しても問題なく動作します。

```csharp
// 6個に増やしても...
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン", "キングスライム" };

// コードを変更しなくても全要素が処理される
for (int i = 0; i < enemies.Length; i++)  // enemies.Length は 6
{
    Debug.Log(enemies[i]);
}
```

### 5-3. 実際に動作確認してみましょう

先ほどの `Sample` スクリプトを `Length` を使って書き直してみましょう。

```csharp
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        // 配列を作成
        string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

        Debug.Log("配列の要素数: " + enemies.Length);
        Debug.Log("=== 敵キャラクター一覧 ===");

        // Length を使った安全なループ
        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log((i + 1) + "番目: " + enemies[i]);
        }

        Debug.Log("=== スコア一覧 ===");

        // 数値の配列でも同様
        int[] scores = { 100, 200, 150, 300, 250 };

        for (int i = 0; i < scores.Length; i++)
        {
            Debug.Log("scores[" + i + "] = " + scores[i]);
        }

        Debug.Log("=== 合計を計算 ===");

        // 配列の要素を合計する
        int total = 0;
        for (int i = 0; i < scores.Length; i++)
        {
            total += scores[i];
        }
        Debug.Log("合計: " + total);

        // 要素を追加しても問題なく動作することを確認
        Debug.Log("=== 要素を追加したテスト ===");

        string[] moreEnemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン", "キングスライム", "ダークナイト" };
        Debug.Log("要素数: " + moreEnemies.Length);

        for (int i = 0; i < moreEnemies.Length; i++)
        {
            Debug.Log(moreEnemies[i]);
        }
    }
}
```

Unity で再生して、Console ウィンドウで結果を確認してください。

### 5-4. その他の Length の活用

`Length` プロパティは最後の要素へのアクセスや、条件分岐でも活用できます。

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク" };

// 最後の要素にアクセス
int lastIndex = enemies.Length - 1;  // 3 - 1 = 2
Debug.Log("最後の要素: " + enemies[lastIndex]);  // オーク

// 配列が空かどうかのチェック
if (enemies.Length > 0)
{
    Debug.Log("配列には要素があります");
}

// 条件分岐で安全にアクセス
int index = 5;
if (index >= 0 && index < enemies.Length)
{
    Debug.Log(enemies[index]);  // 範囲内ならアクセス
}
else
{
    Debug.Log("範囲外です");  // 範囲外なら警告
}
```

### 確認ポイント

- `Length` プロパティは要素の個数を返します
- 添字は `0` から `Length - 1` までが有効範囲です
- 例: 要素数が 5 の場合、有効な添字は `0, 1, 2, 3, 4` です
- ループの条件式には必ず `配列名.Length` を使いましょう

## 6. foreach 文による配列の走査

### 6-1. foreach 文とは

配列の全要素を順番に処理するだけなら、`foreach` 文を使うとより簡潔に書けます。

文法：

```csharp
foreach (型 変数名 in 配列名)
{
    // 変数名 で各要素にアクセス
}
```

例：

```csharp
string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

// foreach 文で全要素を表示
foreach (string enemy in enemies)
{
    Debug.Log(enemy);
}
```

このコードは、以下の `for` 文と同じ結果になります：

```csharp
for (int i = 0; i < enemies.Length; i++)
{
    Debug.Log(enemies[i]);
}
```

### 6-2. foreach 文のメリット

`foreach` 文には以下のメリットがあります。

- 簡潔性: 添字を管理する必要がない
- 可読性: 「配列の各要素に対して処理する」という意図が明確
  - コード量が少なく、書き間違いが減る
- 安全性: 範囲外アクセスが起こりえない

### 6-3. for 文と foreach 文の違い

以下のような場合、foreach 文が適しています。

- 配列の全要素を順番に読み取る
- 添字（インデックス）が不要
- 要素の値を変更しない

一方で、次の条件なら for 文が必要になります。

- 添字（インデックス）が必要（例: 「〇番目」を表示）
- 配列の要素を変更する
- 特定の範囲だけを処理する（例: 最初の5個だけ）
- 逆順に処理する

例：

```csharp
int[] scores = { 100, 200, 150, 300, 250 };

// foreach: 全要素を読み取るだけ
foreach (int score in scores)
{
    Debug.Log("スコア: " + score);
}

// for: 添字が必要な場合
for (int i = 0; i < scores.Length; i++)
{
    Debug.Log((i + 1) + "番目のスコア: " + scores[i]);
}

// for: 要素を変更する場合
for (int i = 0; i < scores.Length; i++)
{
    scores[i] = scores[i] * 2;  // 全要素を2倍にする
}
```

### 6-4. 実際に動作確認してみましょう

`Sample` スクリプトに `foreach` 文を追加してみましょう。

```csharp
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        // 配列を作成
        string[] enemies = { "スライム", "ゴブリン", "オーク", "ドラゴン", "デーモン" };

        Debug.Log("=== foreach 文で表示 ===");

        // foreach 文で全要素を表示
        foreach (string enemy in enemies)
        {
            Debug.Log(enemy);
        }

        Debug.Log("=== for 文で番号付き表示 ===");

        // 番号が必要な場合は for 文
        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log((i + 1) + "番目: " + enemies[i]);
        }

        Debug.Log("=== 合計を計算（foreach） ===");

        int[] scores = { 100, 200, 150, 300, 250 };
        int total = 0;

        // foreach で合計を計算
        foreach (int score in scores)
        {
            total += score;
        }
        Debug.Log("合計: " + total);

        Debug.Log("=== 平均を計算（foreach） ===");

        float average = (float)total / scores.Length;
        Debug.Log("平均: " + average);

        Debug.Log("=== 要素を2倍にする（for） ===");

        // 要素を変更するには for 文が必要
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = scores[i] * 2;
        }

        // 変更後を foreach で表示
        foreach (int score in scores)
        {
            Debug.Log("2倍後: " + score);
        }
    }
}
```

Unity で再生して、Console ウィンドウで結果を確認してください。

### 6-5. foreach 文の注意点

`foreach` 文で取得する変数は読み取り専用です。要素を変更しようとするとエラーになります。

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

// これはエラー！
foreach (int num in numbers)
{
    num = num * 2;  // エラー: foreach の反復変数に代入できません
}

// 要素を変更するには for 文を使う
for (int i = 0; i < numbers.Length; i++)
{
    numbers[i] = numbers[i] * 2;  // OK
}
```

### 確認ポイント

- `foreach` 文は配列の全要素を順番に処理する
- 添字が不要な場合は `foreach` の方が簡潔
- 要素を変更する場合や添字が必要な場合は `for` 文を使う
- 用途に応じて `for` と `foreach` を使い分けましょう

## まとめ

このチュートリアルでは、C# における配列の基本を学びました。

### 学んだ内容

- 配列とは: 同じ型の複数のデータを一つの変数名でまとめて管理できる仕組み
- 配列の宣言: `型[] 配列名;` の形式で宣言する
- 配列の初期化:
  - サイズを指定: `new 型[サイズ]` 
  - 初期値を指定: `{ 値1, 値2, ... }`
- 要素へのアクセス: `配列名[添字]` で取得・設定（添字は 0 から始まる）
- Length プロパティ: `配列名.Length` で要素数を取得
- ループ処理:
  - `for` 文: 添字が必要な場合、要素を変更する場合
  - `foreach` 文: 全要素を読み取るだけの場合

### 重要なポイント

- 添字は `0` から始まり、`配列のサイズ - 1` まで有効です
- ループの条件式には `配列名.Length` を使い、マジックナンバーを避けましょう
- 範囲外アクセスは `IndexOutOfRangeException` エラーを引き起こします
- `for` と `foreach` は用途に応じて使い分けましょう

### 次のステップ

配列の基礎を習得したら、以下の内容に進むことをお勧めします：

- 多次元配列（2次元配列、ジャグ配列）
- 配列を使ったゲームの実装（マインスイーパー、三目並べなど）
- より柔軟なコレクション（`List<T>`、`Dictionary` など）

配列はプログラミングの基本であり、今後のゲーム開発で頻繁に使用します。しっかりと理解を深めてください。

---

## 課題: 項目選択

### 概要

複数のセルを直列に並べ、そのうちの1つが常に選択状態になっているものとします。左右キーで選択状態を移動できるようにしましょう。

![](./image.png)

### Unity側の準備

新規シーンの状態から UI の Canvas ゲームオブジェクトを作成します。

![](./image-1.png)

作成した Canvas ゲームオブジェクトに Horizontal Layout Group コンポーネントを追加します。

![](./image-2.png)

追加した Horizontal Layout Group コンポーネントの Child Alignment の設定を Middle Center に変更します。

![](./image-3.png)

新規に C# スクリプトを作成し、Canvas ゲームオブジェクトに設定します。

![](./image-4.png)

### スクリプト

Canvas ゲームオブジェクトに設定した C# スクリプトで、選択対象のセルを生成するところまでを記述します。

この場では、セルとして UI の `Image` コンポーネントを使って、色を設定します。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        for (var i = 0; i < 5; i++)
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.SetParent(transform, false);

            var image = obj.AddComponent<Image>();
            if (i == 0) { image.color = Color.red; }
            else { image.color = Color.white; }
        }
    }

    private void Update()
    {
	    
		var keyboard = Keyboard.current;
		if (keyboard == null) { return; } // 入力デバイスがない場合は処理しない
		
		if (keyboard.leftArrowKey.wasPressedThisFrame) // 左キーを押した
		{

		}
		if (keyboard.rightArrowKey.wasPressedThisFrame) // 右キーを押した
		{

		}
    }
}
```

![](./image-5.png)

実行結果

### 課題

#### 課題1

左右のキーを押したら選択状態のセルが指定の方向のセルに移動するように仕組みましょう。

右キーを押した場合、現在選択されているセル（赤いセル）が白になり、その１つ右にあるセルが選択状態になり赤くなるようにします。

左キーが押された場合、現在選択されているセル（赤いセル）が白になり、その１つ左にあるセルが選択状態になり赤くなるようにします。

キーの入力した方向にセルがない場合、無視するか、もしくは反対方向のセルが選択されるようにしてください。このとき、エラーが発生しないように注意してください。

#### 課題2

`SerializeField` を使ってセル数を Inspector ビューから設定できるようにし、実行時に生成されるセルの数を変更できるようにしてください。

```csharp
[SerializeField]
private int _count = 5;
```

![](./image-6.png)

セル数が変わっても動作に問題がないようにしましょう。

#### 課題3

Space キーを押すと、選択中のセルが消えるようにしてください。ユーザー視点でセルが消えたように見えれば実装方法は自由としますが、レイアウトが崩れないように注意してください。

削除されたセルを選択することはできません。

選択中のセルが削除された時、削除されたセルから最も近い右方向にある有効なセルを選択してください。右方向にセルが存在しない場合、削除されたセルから最も近い左方向にある有効なセルを選択してください。

削除したセルが最後のセルの場合、選択状態を表すセル自体が存在しないので何もする必要はありません。エラーが出ないように注意してください。
