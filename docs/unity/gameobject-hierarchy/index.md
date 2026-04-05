---
layout: page
title: GameObject の親子関係
permalink: /unity/gameobject-hierarchy/
---

# GameObject の親子関係

Unity の Hierarchy ビューでは、ゲームオブジェクトを**木構造（ツリー構造）**で管理できます。ある GameObject を別の GameObject の「子」にすることで、位置・回転・スケールの継承や、グループとしての一括管理が可能になります。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- Hierarchy の木構造（親子関係）の概念を説明できる
- ローカル座標とワールド座標の違いを説明できる
- スクリプトから `transform.parent` と `transform.SetParent()` で親子関係を操作できる
- `transform.childCount`・`GetChild()`・`Find()` で子オブジェクトを扱える

## 前提知識

- [Instantiate() でオブジェクトを生成する](/unity-csharp-learning/unity/instantiate/) を読んでいること

---

## 1. Hierarchy の木構造

Unity の Hierarchy ビューでは、ゲームオブジェクトの左端にある ▶ をクリックすると「子オブジェクト」が展開されます。

```
Stage
Player
Items             ← 親オブジェクト
  ├─ Item(Clone)  ← 子オブジェクト
  ├─ Item(Clone)
  └─ Item(Clone)
```

**子は親の座標系を引き継ぎます**。親を移動・回転させると、子もそれに追従して動きます。これを使うと関連するオブジェクトをグループとして一括操作できます。

エディターで親子関係を設定するには、Hierarchy ビューで子にしたいオブジェクトを親オブジェクトの上にドラッグ & ドロップします。

---

## 2. スクリプトから親子関係を操作する

### transform.parent

**`Transform.parent`** — このオブジェクトの親 Transform を取得または設定します。親がない場合（ルートオブジェクト）は `null` を返します。<!-- [公式ドキュメント]() -->

**書式：parent プロパティ**
```csharp
public Transform parent { get; set; }
```

```csharp
if (transform.parent != null)
{
    Debug.Log("親の名前: " + transform.parent.name);
}
```

### transform.SetParent()

**`Transform.SetParent()`** — 親 Transform を設定します。<!-- [公式ドキュメント]() -->

**書式：SetParent メソッド**
```csharp
public void SetParent(Transform parent);
public void SetParent(Transform parent, bool worldPositionStays);
```

| パラメータ | 説明 |
|---|---|
| `parent` | 設定する親 Transform。`null` を指定するとルートに移動する |
| `worldPositionStays` | `true`（既定）でワールド座標を維持したまま親子を変更する。`false` でローカル座標が維持される |

```csharp
// itemContainer を親にする（ワールド座標位置は維持）
transform.SetParent(itemContainer);

// 親をなくしてルートオブジェクトに移動する
transform.SetParent(null);
```

---

## 3. ローカル座標とワールド座標

Unity の Transform には**2種類の座標系**があります。

| プロパティ | 説明 |
|---|---|
| `transform.position` | ワールド座標（シーン全体の原点を基準） |
| `transform.localPosition` | ローカル座標（親オブジェクトの位置を基準） |
| `transform.rotation` | ワールド回転 |
| `transform.localRotation` | ローカル回転（親からの相対） |
| `transform.localScale` | ローカルスケール（親スケールへの乗算） |

`position` / `rotation` / `localScale` は [Transform ページ](/unity-csharp-learning/unity/transform/) で解説済みです。このページでは親子関係に関わる `local` 系プロパティに注目します。

**`Transform.localPosition`** — 親オブジェクトを基準にしたローカル座標を取得または設定します。<!-- [公式ドキュメント]() -->

**書式：localPosition プロパティ**
```csharp
public Vector3 localPosition { get; set; }
```

**`Transform.localRotation`** — 親を基準にしたローカル回転を取得または設定します。<!-- [公式ドキュメント]() -->

**書式：localRotation プロパティ**
```csharp
public Quaternion localRotation { get; set; }
```

親オブジェクトの `position.x` が `3` で、子の `localPosition.x` が `1` の場合、子のワールド座標での X は `4` になります。

```csharp
// ワールド座標（シーン全体での絶対位置）
Debug.Log(transform.position);

// ローカル座標（親を基準にした相対位置）
Debug.Log(transform.localPosition);

// 親の真上に配置（ローカル座標で Y=1 に移動）
transform.localPosition = new Vector3(0, 1, 0);
```

> 💡 **ポイント**: 「親を基準にどこに置くか」を制御したい場合は `localPosition` を使います。「シーン全体でどこにあるか」を調べたい場合は `position` を使います。

### サンプル: 惑星の公転

親（Sun）が自転すると、子（Planet）がその周りを公転します。Planet の `localPosition` は常に `(3, 0, 0)`（Sun から見て右）で変わりませんが、`position`（ワールド座標）は Sun の回転に伴って毎フレーム変化します。

**シーンの準備**

1. **GameObject → 3D Object → Cube** を追加し、名前を `Sun`、Scale を `(2, 2, 2)` にする
2. `Sun` に `SunRotator` スクリプトを作成してアタッチする

Planet はスクリプト内で生成します。

```csharp
// SunRotator.cs — Sun にアタッチ
using UnityEngine;

public class SunRotator : MonoBehaviour
{
    private Transform _planet;

    private void Start()
    {
        // Planet を立方体で生成して Sun の子にする
        var planetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        planetObj.name = "Planet";
        _planet = planetObj.transform;
        _planet.SetParent(transform);
        _planet.localPosition = new Vector3(3, 0, 0);
    }

    private void Update()
    {
        // Sun を Y 軸周りに自転させる
        transform.Rotate(0, 30f * Time.deltaTime, 0, Space.World);

        // ローカル座標とワールド座標を並べて出力する
        Debug.Log($"localPosition: {_planet.localPosition}  |  world position: {_planet.position}");
    }
}
```

実行すると、Console ビューで `localPosition` は `(3, 0, 0)` のまま変化しないのに対し、`position` が毎フレーム異なる値になることを確認できます。立方体を使っているので、Sun の自転に対して Planet が追従して回っている様子も Scene ビューで見やすくなっています。

---

## 4. 子オブジェクトを走査・検索する

### childCount と GetChild()

**`Transform.childCount`** — 直下の子オブジェクトの数を返します。<!-- [公式ドキュメント]() -->

**書式：childCount プロパティ**
```csharp
public int childCount { get; }
```

**`Transform.GetChild()`** — インデックスで子 Transform を取得します。<!-- [公式ドキュメント]() -->

**書式：GetChild メソッド**
```csharp
public Transform GetChild(int index);
```

| パラメータ | 説明 |
|---|---|
| `index` | 取得する子のインデックス（0 から `childCount - 1` まで） |

```csharp
Debug.Log(transform.childCount);  // 直下の子の数を出力

for (var i = 0; i < transform.childCount; i++)
{
    var child = transform.GetChild(i);
    Debug.Log(child.name);
}
```

### transform.Find()

**`Transform.Find()`** — 名前で直下の子（または子孫）を検索します。見つからない場合は `null` を返します。<!-- [公式ドキュメント]() -->

**書式：Find メソッド**
```csharp
public Transform Find(string name);
```

| パラメータ | 説明 |
|---|---|
| `name` | 検索する名前。`/` 区切りで子孫パスを指定できる（例: `"Group/Item"`） |

```csharp
var target = transform.Find("Item(Clone)");
if (target != null)
{
    Destroy(target.gameObject);
}
```

> ⚠️ `Find()` は**直下の子のみ**検索します（孫は含まれません）。孫を検索するにはパス区切り `/` を使います。

---

## 5. 実践例: 生成オブジェクトを親でまとめて管理する

`Instantiate()` に親 Transform を渡すと、**生成と同時に子として配置**できます。

**書式：Instantiate メソッド（親指定）**
```csharp
public static Object Instantiate(Object original, Transform parent);
```

| パラメータ | 説明 |
|---|---|
| `original` | 複製元のオブジェクト |
| `parent` | 生成したオブジェクトの親になる Transform |

```csharp
// このオブジェクト（Spawner）の子としてアイテムを生成する
var item = Instantiate(_original, transform);
```

子として生成することで、`transform.childCount` を「現在シーンに存在する数」として使えます。これを使うと、出現上限のチェックが簡潔に書けます。

```csharp
private const int MaxItems = 5;

private void TrySpawn()
{
    if (transform.childCount >= MaxItems) return;  // 上限に達したらスキップ
    Instantiate(_original, transform);
    Debug.Log($"現在のアイテム数: {transform.childCount}");
}
```

アイテムが `Destroy()` で破棄されると、その分だけ `childCount` が減るため、「シーンに何個あるか」を別途カウントする必要がありません。

---

## まとめ

- Hierarchy の親子関係により、子は親の座標系（位置・回転・スケール）を引き継ぐ
- `transform.position` はワールド座標、`transform.localPosition` は親からの相対座標
- `transform.SetParent()` でスクリプトから親子関係を変更できる
- `transform.childCount` と `GetChild()` で子オブジェクトを数えたり取得できる
- `Instantiate(original, parent)` で生成と同時に親子関係を設定できる

---

## 理解度チェック

1. 親オブジェクトの `position` が `(5, 0, 0)` のとき、子の `localPosition` が `(1, 0, 0)` だとワールド座標はどこになりますか？
2. `transform.childCount` が減るのはどんな場合ですか？
3. `transform.Find("Enemy")` が `null` を返す場合、どのような原因が考えられますか？

<details markdown="1">
<summary>解答を見る</summary>

1. ワールド座標は `(6, 0, 0)`（親の位置 + 子のローカル位置）。
2. 子オブジェクトが `Destroy()` で破棄されたとき、または `SetParent()` で別の親に移動したとき。
3. その名前の子オブジェクトが直下に存在しない（孫以下にある・名前が違う・まだ生成されていないなど）。

</details>

---

## 次のステップ

[チュートリアル: スポナー](/unity-csharp-learning/unity/spawner/) では、`Instantiate()` と親子関係を組み合わせて、一定間隔でオブジェクトをスポーンするスポナーを実装します。
