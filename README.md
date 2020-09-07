# MyTwitterForms

Xamarin.Forms練習用のかんたんなTwitterクライアントアプリ

![CI](https://github.com/aridai/MyTwitterForms/workflows/CI/badge.svg)
![C#](https://img.shields.io/static/v1?label=language&message=C%23&color=brightgreen)
![Xamarin.Forms](https://img.shields.io/static/v1?label=framework&message=Xamarin.Forms&color=purple)

## 開発

### CI

GitHub Actionsを利用  
-> 詳細: `.github/workflows/CI.yml`

### コマンド

依存関係の復元:

```
msbuild ./MyTwitterForms.sln /t:restore
dotnet tool restore
```

ビルド & テスト:

```
msbuild ./MyTwitterForms.sln /t:build
testDllPath=$(find **/bin/ -name '*Tests.dll' -type f | head -1)
dotnet xunit $testDllPath
```

フォーマッタ:

```
dotnet dotnet-format
```
