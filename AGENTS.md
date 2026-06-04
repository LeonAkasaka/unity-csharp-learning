# ドキュメント編集ガイド

## 公式ドキュメントリンクのルール

`docs/unity/` 以下で API を新規解説するとき、`**書式：...` ヘッダーの型・メンバ名部分を公式ドキュメントへのリンクにしてください。

```markdown
**書式：[Type.Member メソッド](URL)**
**書式：[Type.Property プロパティ](URL)**
```

括弧付きの修飾子はリンクの外に出す:

```markdown
**書式：[Random.Range メソッド](https://docs.unity3d.com/ScriptReference/Random.Range.html)（float）**
```

### URL パターン

| 対象 | URL |
|---|---|
| Unity クラス | `https://docs.unity3d.com/ScriptReference/ClassName.html` |
| Unity メソッド | `https://docs.unity3d.com/ScriptReference/ClassName.MethodName.html` |
| Unity プロパティ | `https://docs.unity3d.com/ScriptReference/ClassName-propertyName.html`（ハイフン区切り） |
| Unity 属性（Attribute） | `https://docs.unity3d.com/ScriptReference/AttributeName.html` |
| TextMeshPro パッケージ | `https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest/api/TMPro.ClassName.html` |
| Input System パッケージ | リンクしない（URL 構造が異なりリンク切れになりやすいため） |
| C# 標準ライブラリ | `https://learn.microsoft.com/dotnet/api/system.typename` |
