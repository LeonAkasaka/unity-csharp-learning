---
layout: page
title: 二次元配列
permalink: /grid-games/array-2d/
---

# 二次元配列

## 概要

二次元配列は行と列の 2 つのインデックスでデータを管理します。C# の文法（宣言・初期化・インデックスアクセス・GetLength・ネストループ）は以下のページで詳しく解説しています。

- [多次元配列（C# 基礎）](/unity-csharp-learning/csharp/multidimensional-arrays/) — `[,]` 構文・`GetLength`・ネストループ

ゲーム開発では盤面・タイルマップ・格子状 UI の管理に頻繁に登場します。このページでは Unity 上で二次元配列を活用する方法を学びます。

### 学習目標

- `GameObject[,]` でグリッド状にオブジェクトを生成・管理できる
- `int[,]` で盤面の状態データを管理し、`GameObject[,]` の見た目に反映できる
- 後続のグリッドゲーム（三目並べ・ライツアウト・マインスイーパー）の実装に応用できる

## 1. 二次元配列とグリッド

二次元配列は「行（row）」と「列（col）」の 2 つの番号で要素を指定します。

| 行/列 | 0  | 1  | 2  | 3  |
| --- | --- | --- | --- | --- |
| 0 | [0,0] | [0,1] | [0,2] | [0,3] |
| 1 | [1,0] | [1,1] | [1,2] | [1,3] |
| 2 | [2,0] | [2,1] | [2,2] | [2,3] |

`配列[row, col]` でアクセスします。左上が `[0, 0]`、右下（3行4列なら）が `[2, 3]` です。ゲームの盤面と一対一に対応させると「行 = Y 座標、列 = X 座標」として直感的に扱えます。

## 2. GameObject[,] でグリッドを生成する

`GameObject[,]` を使うと、グリッド状に並んだセルオブジェクトを配列で一括管理できます。

```csharp
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int _rows = 4;
    [SerializeField] private int _cols = 4;
    [SerializeField] private float _cellSize = 1.2f;

    private GameObject[,] _cells;

    private void Start()
    {
        _cells = new GameObject[_rows, _cols];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                _cells[row, col] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _cells[row, col].transform.position = new Vector3(col * _cellSize, row * _cellSize, 0f);
            }
        }
    }
}
```

`_cells[row, col]` で各セルの GameObject を行・列番号で直接取り出せます。`col * _cellSize` を X 軸、`row * _cellSize` を Y 軸に対応させることで均等なグリッドが完成します。

## 3. 複数の二次元配列を組み合わせる

`GameObject[,]` でオブジェクトを管理しながら、同じ行・列インデックスで別の二次元配列を参照するパターンです。ここでは `float[,]` で各セルの回転速度をランダムに決め、`Update()` で反映します。

```csharp
using UnityEngine;

public class GridRotator : MonoBehaviour
{
    [SerializeField] private int _rows = 4;
    [SerializeField] private int _cols = 4;
    [SerializeField] private float _cellSize = 1.2f;

    private GameObject[,] _cells;
    private float[,] _speeds;

    private void Start()
    {
        _cells = new GameObject[_rows, _cols];
        _speeds = new float[_rows, _cols];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                _cells[row, col] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _cells[row, col].transform.position = new Vector3(col * _cellSize, row * _cellSize, 0f);
                _speeds[row, col] = Random.Range(30f, 360f);
            }
        }
    }

    private void Update()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                _cells[row, col].transform.Rotate(0f, _speeds[row, col] * Time.deltaTime, 0f);
            }
        }
    }
}
```

`_cells[row, col]` と `_speeds[row, col]` は同じ `[row, col]` で対応します。`Update()` でも同じネストループを使うことで、すべてのセルを一括処理できます。セルの数（`_rows`・`_cols`）が変わってもコードを書き直す必要はありません。

## まとめ

- C# の二次元配列の文法は[多次元配列（C# 基礎）](/unity-csharp-learning/csharp/multidimensional-arrays/)で確認する
- `GameObject[,] _cells` でグリッド状のオブジェクトを行・列番号で管理する
- 同じ `[row, col]` で別の配列（`float[,]` など）と対応させることで、セルごとのデータを持たせられる
- `Update()` でも同じネストループを使い、グリッド全体を一括処理できる

---

## 課題: 項目選択

### 概要

複数のセルを格子状に並べ、そのうちの1つが常に選択状態になっているものとします。上下左右キーで選択状態を移動できるようにしましょう。

![](./image.png)

この課題は「小課題 項目選択1」の続きです。わからない場合は「[配列の基礎](/unity-csharp-learning/grid-games/array-basics/)」を先に攻略してください。

### Unity 側の準備

新規シーンの状態から UI の Canvas ゲームオブジェクトを作成します。

![](./image-1-q1.png)

作成した Canvas ゲームオブジェクトに Grid Layout Group コンポーネントを追加します。

![](./image-1.png)

追加した Grid Layout Group コンポーネントの Spacing の X と Y の値を 10 に、Child Alignment の設定を Middle Center に変更します。

グリッド上に並べるセルの行数または列数を Constraint の設定で固定化できます。例えば Constraint の設定を Fixed Column Count に変更して、下部の Constraint Count を 5 にすると列数が 5 列に固定されます。

この設定は課題に応じて、スクリプトから変更しても構いません。

![](./image-2.png)

新規に C# スクリプトを作成し、Canvas ゲームオブジェクトに設定します。

![](./image-3.png)

### スクリプト

Canvas ゲームオブジェクトに設定した C# スクリプトで、選択対象のセルを生成するところまでを記述します。

この場では、セルとして UI の Image コンポーネントを使って、色を設定します。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        for (var r = 0; r < 5; r++)
        {
            for (var c = 0; c < 5; c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;

                var image = obj.AddComponent<Image>();
                if (r == 0 && c == 0) { image.color = Color.red; }
                else { image.color = Color.white; }
            }
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
        if (keyboard.upArrowKey.wasPressedThisFrame) // 上キーを押した
        {

        }
        if (keyboard.downArrowKey.wasPressedThisFrame) // 下キーを押した
        {

        }
    }
}
```

![](./image-4.png)

実行結果

### 課題

#### 課題1

上下左右のキーを押したら選択状態のセルが指定の方向のセルに移動するように仕組みましょう。

右キーを押した場合、現在選択されているセル（赤いセル）が白になり、その１つ右にあるセルが選択状態になり赤くなるようにします。

左キーが押された場合、現在選択されているセル（赤いセル）が白になり、その１つ左にあるセルが選択状態になり赤くなるようにします。

上キーを押した場合、現在選択されているセル（赤いセル）が白になり、その１つ上にあるセルが選択状態になり赤くなるようにします。

下キーが押された場合、現在選択されているセル（赤いセル）が白になり、その１つ下にあるセルが選択状態になり赤くなるようにします。

キーの入力した方向にセルがない場合、無視するか、もしくは反対方向のセルが選択されるようにしてください。このとき、エラーが発生しないように注意してください。

#### 課題2

`SerializeField` を使って行数と列数を Inspector ビューから設定できるようにし、実行時に生成されるセルの数を変更できるようにしてください。

```csharp
[SerializeField]
private int _rows = 5;

[SerializeField]
private int _columns = 5;
```

![](./image-5.png)

セル数が変わってもレイアウトや動作に問題がないようにしましょう。

#### 課題3

Space キーを押すと、選択中のセルが消えるようにしてください。ユーザー視点でセルが消えたように見えれば実装方法は自由としますが、レイアウトが崩れないように注意してください。

選択中のセルが削除された時、削除したセルから最も近いセルに選択状態が切り替わるようにしてください。距離が同じセルが複数ある場合、どのセルが選択されてもよいものとします。

削除したセルが最後のセルの場合、選択状態を表すセル自体が存在しないので何もする必要はありません。エラーが出ないように注意してください。
