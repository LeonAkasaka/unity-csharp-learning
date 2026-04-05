---
layout: page
title: break と continue（補足）
permalink: /csharp/break-and-continue/
---

# break と continue（補足）

`break` と `continue` は、ループの実行フローを変えるキーワードです。`break` は[反復処理のページ](/unity-csharp-learning/csharp/loops/)で無限ループ脱出の文脈で登場しましたが、通常の `for`・`while`・`foreach` でも使えます。

## 学習目標

- `break` でループを途中で終了できる
- `continue` で現在の反復だけをスキップして次の反復に進める
- ネストしたループでは最も内側のループにのみ作用することを理解できる

## 前提知識

- [反復処理](/unity-csharp-learning/csharp/loops/) を読んでいること

---

## 1. break — ループを途中で終了する

**書式：break 文**
```
break;
```

`break` を実行するとループがただちに終了し、ループの直後の処理に移ります。

<svg viewBox="0 0 400 195" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:500px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="bk-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="bk-arr-r" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#e53935"/></marker>
  </defs>
  <!-- 条件ボックス -->
  <rect x="100" y="12" width="160" height="36" rx="18" fill="#e8f5e9" stroke="#66bb6a" stroke-width="1.5"/>
  <text x="180" y="35" text-anchor="middle" font-size="13" fill="#2e7d32">条件チェック</text>
  <!-- ↓ true -->
  <line x1="180" y1="48" x2="180" y2="82" stroke="#555" stroke-width="1.5" marker-end="url(#bk-arr)"/>
  <text x="188" y="70" font-size="11" fill="#555">true</text>
  <!-- 本体ボックス -->
  <rect x="100" y="82" width="160" height="68" rx="6" fill="#fffde7" stroke="#ffd54f" stroke-width="1.5"/>
  <text x="180" y="104" text-anchor="middle" font-size="12" fill="#555">ループ本体</text>
  <!-- break; サブボックス -->
  <rect x="118" y="111" width="62" height="26" rx="4" fill="#ffebee" stroke="#ef9a9a" stroke-width="1.5"/>
  <text x="149" y="129" text-anchor="middle" font-size="12" fill="#c62828" font-family="monospace">break;</text>
  <!-- break → ループ後 -->
  <line x1="180" y1="124" x2="306" y2="124" stroke="#e53935" stroke-width="1.5" stroke-dasharray="5 3" marker-end="url(#bk-arr-r)"/>
  <!-- ループ後ボックス -->
  <rect x="306" y="108" width="82" height="32" rx="6" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/>
  <text x="347" y="129" text-anchor="middle" font-size="12" fill="#1565c0">ループ後</text>
  <!-- break なし → 次の反復（左回りアーク）-->
  <polyline points="180,150 180,175 50,175 50,30 100,30" fill="none" stroke="#555" stroke-width="1.5" marker-end="url(#bk-arr)"/>
  <text x="58" y="186" font-size="10" fill="#888">break なし → 次の反復</text>
</svg>

```csharp
for (int i = 0; i < 10; i++)
{
    if (i == 3)
    {
        break;
    }
    Console.WriteLine(i);
}
Console.WriteLine("完了");
```

```
0
1
2
完了
```

`i` が `3` になった時点でループを抜けるため、4 以降は出力されません。

### 条件を満たしたら終了する

`break` の代表的な使い方は「目的の条件が満たされたらループを打ち切る」です。

```csharp
bool hasLow = false;

for (int i = 0; i < 10; i++)
{
    int score = i * 10;
    if (score < 30)
    {
        hasLow = true;
        break;  // 1 つ見つかれば十分なので終了
    }
}

Console.WriteLine(hasLow ? "30 未満がある" : "すべて 30 以上");
```

---

## 2. continue — 現在の反復をスキップする

**書式：continue 文**
```
continue;
```

`continue` を実行すると、ループ本体の残りの処理を飛ばして次の反復（条件チェック）に移ります。

<svg viewBox="0 0 420 195" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:530px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ct-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="ct-arr-b" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#1565c0"/></marker>
  </defs>
  <!-- 条件ボックス -->
  <rect x="110" y="12" width="160" height="36" rx="18" fill="#e8f5e9" stroke="#66bb6a" stroke-width="1.5"/>
  <text x="190" y="35" text-anchor="middle" font-size="13" fill="#2e7d32">条件チェック</text>
  <!-- ↓ true -->
  <line x1="190" y1="48" x2="190" y2="82" stroke="#555" stroke-width="1.5" marker-end="url(#ct-arr)"/>
  <text x="198" y="70" font-size="11" fill="#555">true</text>
  <!-- 本体ボックス -->
  <rect x="110" y="82" width="160" height="68" rx="6" fill="#fffde7" stroke="#ffd54f" stroke-width="1.5"/>
  <text x="190" y="104" text-anchor="middle" font-size="12" fill="#555">ループ本体</text>
  <!-- continue; サブボックス -->
  <rect x="122" y="111" width="82" height="26" rx="4" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/>
  <text x="163" y="129" text-anchor="middle" font-size="12" fill="#1565c0" font-family="monospace">continue;</text>
  <!-- continue → 次の反復（右回りアーク）-->
  <polyline points="204,124 360,124 360,30 270,30" fill="none" stroke="#1565c0" stroke-width="1.5" stroke-dasharray="5 3" marker-end="url(#ct-arr-b)"/>
  <text x="364" y="80" font-size="10" fill="#1565c0">次の反復</text>
  <!-- 通常の反復（左回りアーク）-->
  <polyline points="190,150 190,175 55,175 55,30 110,30" fill="none" stroke="#555" stroke-width="1.5" marker-end="url(#ct-arr)"/>
  <text x="62" y="186" font-size="10" fill="#888">continue なし → 通常の反復</text>
</svg>

```csharp
for (int i = 0; i < 6; i++)
{
    if (i % 2 == 0)
    {
        continue;  // 偶数はスキップ
    }
    Console.WriteLine(i);
}
```

```
1
3
5
```

`i` が偶数のとき `continue` が実行されると `Console.WriteLine` を飛ばして `i++` と条件チェックに進みます。

### while での注意点

`while` で `continue` を使うときは、ループ変数の更新が `continue` より**前**にあることを確認してください。

```csharp
int i = 0;
while (i < 6)
{
    i++;             // ← 更新を先に書く（continue より前）
    if (i % 2 == 0)
    {
        continue;    // i++ は済んでいるので無限ループにならない
    }
    Console.WriteLine(i);
}
```

> ⚠️ `for` 文なら更新式が自動的に実行されるため、この問題は起きません。`while` で `continue` を使う場合は要注意です。

---

## 3. ネストしたループでの動作

`break` と `continue` は**最も内側のループ**にのみ作用します。

```csharp
for (int i = 0; i < 3; i++)
{
    for (int j = 0; j < 3; j++)
    {
        if (j == 1) break;  // 内側の for だけを抜ける
        Console.WriteLine($"i={i}, j={j}");
    }
    // 内側が break してもここに来る
}
```

```
i=0, j=0
i=1, j=0
i=2, j=0
```

内側のループが `j == 1` で終了しても、外側の `i` ループは止まりません。

---

## よくあるミス

### while で continue の前にループ変数の更新を書く

```csharp
int i = 0;

// ❌ 無限ループ: i が偶数のとき i++ が永遠に実行されない
while (i < 6)
{
    if (i % 2 == 0)
    {
        continue;  // i++ より先に実行される
    }
    Console.WriteLine(i);
    i++;
}
```

`for` 文に書き換えれば安全です。

---

## まとめ

| キーワード | 動作 |
|---|---|
| `break` | ループをその場で終了し、ループの直後に移る |
| `continue` | 現在の反復の残りをスキップし、次の反復（条件チェック）に移る |

どちらも**最も内側のループ**にのみ作用します。

---

## 理解度チェック

1. 次のコードの出力結果を答えてください。

   ```csharp
   for (int i = 0; i < 5; i++)
   {
       if (i == 3) break;
       Console.WriteLine(i);
   }
   ```

2. 次のコードの出力結果を答えてください。

   ```csharp
   for (int i = 0; i < 5; i++)
   {
       if (i == 2) continue;
       Console.WriteLine(i);
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `0`・`1`・`2`。`i == 3` で `break` が実行されます。

2. `0`・`1`・`3`・`4`。`i == 2` のときだけ `continue` で `Console.WriteLine` がスキップされます。

</details>

---

## 次のステップ

`break` と `continue` は配列の走査でもよく使います。[配列と foreach（補足）](/unity-csharp-learning/csharp/arrays-and-foreach/) では `foreach` での活用例を学びます。
