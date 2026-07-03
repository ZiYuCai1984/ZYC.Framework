<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>


# トラブルシューティング

このページは、ZYC.Framework のホストまたはモジュール プロジェクトでよく発生する失敗点をまとめたものです。まず見えている症状から始め、その動作を担当する層を確認してください。

## クイック トリアージ

| 症状 | 最初に確認すること |
| --- | --- |
| `zyc` が見つからない | グローバル `ZYC.Framework.CLI` ツールをインストールまたは更新し、新しいシェルで `zyc new --help` を実行します。 |
| `zyc new` が失敗する | プロジェクト名、`--template`、`--output`、`--package-version`、対象フォルダーに既存ファイルがあるかを確認します。`--overwrite` は置き換えが意図されている場合だけ使います。 |
| モジュールが見つからない | ランタイム DLL が app ディレクトリにあり、`ZYC.Framework.Modules*.dll` に一致するか `ModuleConfig.AdditionalAssemblyNames` に列挙され、`ModuleBase` エントリポイントを含むか確認します。 |
| モジュールは発見されるがロードされない | `ModuleConfig.DisabledAssemblyNames` を確認します。無効化されたモジュールは情報として登録されますが、`LoadAsync` はスキップされます。 |
| モジュール ロード エラー ページが開く | モジュール名、例外、関数名を確認します。失敗は `LoadAsync` または `AfterLoadedAsync` で発生しています。 |
| メニュー項目が表示されない | 所有モジュールがロードされ、正しいメニュー プロバイダーに登録され、項目が hidden になっていないか確認します。 |
| ナビゲーションが Not Found を開く | URI に一致する `ITabItemFactory` がありません。ルート属性、ファクトリ登録、`ITabManager.NavigateAsync(...)` に渡す URI を確認します。 |
| ナビゲーションがエラー タブを開く | ファクトリ、タブ項目、ビュー、またはタブの `LoadAsync` が失敗しています。エラー タブとログの例外を確認します。 |
| NuGet インストール済みモジュールが有効にならない | ModuleManager から再インストールまたは更新し、`settings/nuget.module.assets.json` が書き込まれたことを確認してからホストを再起動します。 |
| Aspire リソースが表示されない | モジュールが `IExtensionResourcesProvider` または `ICommandlineResourcesProvider` を登録しているか、Aspire が有効または開始済みか確認します。 |
| 組み込みターミナルが失敗する | ターミナルの native DLL が出力ディレクトリの想定された `runtimes` フォルダーにコピーされているか確認します。 |
| ドキュメント変更が反映されない | `src/ZYC.Framework.Build.Doc/Templates` 配下を編集し、必要に応じて公開ドキュメントを再生成します。 |

## CLI とプロジェクト作成

推奨される作成フローは、グローバル dotnet ツールを使います。

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.3.8
dotnet tool update --global ZYC.Framework.CLI --version 1.3.8
zyc new MyCompany.Tools --template minimal
```

インストール後も `zyc` を使えない場合:

- 新しいターミナルを開き、更新された tool path を反映します。
- `dotnet tool list --global` を実行し、`ZYC.Framework.CLI` がインストール済みであることを確認します。
- `zyc new --help` を実行し、CLI コマンドが解決されることを確認します。

プロジェクト作成が失敗する場合:

- `MyCompany.Tools` のような有効なドット区切りの C# プロジェクト名を使います。
- ホストだけなら `--template minimal`、ホストとモジュール分割なら `--template modular` を使います。
- 対象フォルダーをプロジェクト名から導出したくない場合は `--output` を使います。
- 生成プロジェクトで特定のパッケージ バージョンを参照する場合は `--package-version` を使います。
- 既存の生成ファイルを置き換える意図がある場合だけ `--overwrite` を使います。

## モジュール発見

起動時、ホストはアプリケーション ディレクトリからモジュール アセンブリを発見します。標準の組み込みモジュール DLL はファイル名で照合され、追加モジュールは `ModuleConfig.AdditionalAssemblyNames` に列挙できます。

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

モジュールが発見されない場合:

- DLL がアプリケーション ディレクトリにあることを確認します。
- DLL 名が標準の `ZYC.Framework.Modules*.dll` パターンに一致するか、`AdditionalAssemblyNames` に列挙されていることを確認します。
- アセンブリに `ModuleBase` から派生した具象型が含まれることを確認します。
- `*.Abstractions` アセンブリだけを列挙しないでください。Abstractions プロジェクトは契約を定義するだけで、ランタイム モジュールのエントリポイントではありません。

モジュール情報には表示されるがロードされない場合:

- DLL ファイル名が `DisabledAssemblyNames` に含まれていないか確認します。
- 無効化されたモジュールは発見されますが、`LoadAsync` は呼ばれません。
- `DisabledAssemblyNames` からファイル名を削除するか、ModuleManager で再有効化し、実行中ホストが動的リロードしない場合は再起動します。

## モジュール ロード エラー

ホストは 2 つのフェーズのロード失敗を記録します。

- `LoadAsync`: 通常、メニュー、タブ、ステータス項目、ランタイム サービスを登録します。
- `AfterLoadedAsync`: 他のモジュールのロード後に依存する処理を実行できます。

モジュール ロード エラー ページが開いたら、表示されるモジュール名、関数名、例外から調査します。`AppConfig.SuppressModuleLoadError` はページを抑制できますが、根本原因は修正しません。

典型的な原因:

- 必要なサービスが解決される前に登録されていない。
- ビューまたはタブ項目のコンストラクターが、登録時または起動ナビゲーション時に例外を投げる。
- モジュールが別のモジュールの有効化を前提にしているが、その依存モジュールが無効または欠落している。
- モジュールが期待するローカル ファイル、native DLL、外部プロセスが欠落している。

## メニュー、タブ、ルーティング

メニュー項目は通常、モジュールの `LoadAsync` から登録されます。メニュー項目が表示されない場合:

- モジュール自体がエラーなくロードされたことを確認します。
- File、Tools、Extensions、About、Settings など正しいプロバイダーに登録します。
- 状態または構成で項目が hidden になっていないか確認します。
- priority と anchor は項目を作成しないため、順序確認は項目が見えるようになってから行います。

タブ ナビゲーションは `ITabItemFactory` に依存します。Not Found が開く場合:

- ファクトリがロード済みアセンブリに登録されていることを確認します。
- `TabItemRoute` の scheme、host、path がナビゲーション URI と一致することを確認します。
- ファイル プレビューのような汎用ルートが、より具体的なルートより先に一致し得る場合は factory priority を確認します。
- single-instance タブが新規作成ではなく既存タブを再利用していないか確認します。

エラー タブが開く場合、ルートには一致していますが、作成またはロードが失敗しています。エラー タブの例外を確認し、ファクトリ、タブ項目コンストラクター、ビュー コンストラクター、タブ `LoadAsync` を調べます。

## ワークスペースと復元タイミング

起動ナビゲーション、プロトコル転送ナビゲーション、復元時のモジュール処理は、ワークスペースとタブ復元パイプラインの準備完了を待つ必要があります。タブが別ワークスペースで開く、または復元後に消える場合:

- `TabManagerRestoreCompleted` の後で起動ナビゲーションを実行します。
- ユーザー操作の場合はフォーカス中のワークスペースを使います。
- 復元または既知のワークスペースへの転送では明示的な workspace id を使います。
- タブの移動、作成、クローズは UI コレクションを直接変更せず、`ITabManager` 経由で行います。

## Config と State

具象 `IConfig` と `IState` 型は、モジュール アセンブリ登録時に settings ディレクトリからロードされます。設定が読み書きされない場合:

- config または state 型が具象型で、ロード済みランタイム アセンブリに存在することを確認します。
- config または state 型を期待する前に、モジュール アセンブリが発見されていることを確認します。
- settings ファイルがソース ツリーではなく、ホストの settings ディレクトリにあることを確認します。
- contract 型だけを abstractions アセンブリに置いて、そこからランタイム state が作成されると期待しないでください。

## シングル インスタンスと Mutex override

`settings/mutex-id.override` が存在しない限り、ホストは製品情報から single-instance mutex id を作ります。Tools > Override Mutex Id でこのファイルを作成、更新、削除できます。

Override を変更した後はホストを再起動してください。Mutex と startup URI pipe 名は起動時に作られるため、実行中のプロセスはすぐには identity を切り替えません。Side-by-side instance、startup URI forwarding、foreground-window activation が想定外に動く場合は、まず現在の `mutex-id.override` を確認します。

## NuGet モジュール

ModuleManager は一時プロジェクトを restore し、解決された runtime asset graph を `settings/nuget.module.assets.json` に書き込んで NuGet モジュールをインストールします。ホストは次回起動時にそのファイルを読み取ります。

NuGet モジュールをインストールしたのに有効にならない場合:

- restore が成功し、assets ファイルが `settings` 配下に存在することを確認します。
- 起動時発見で runtime assembly をロードできるよう、ホストを再起動します。
- パッケージが現在のホスト ターゲットである `net10.0-windows` と互換の runtime assembly を含むことを確認します。
- インストール済みモジュール アセンブリが `ModuleConfig.DisabledAssemblyNames` で無効化されていないか確認します。
- assets ファイルが古いパッケージ内容を指す場合は、再インストールまたは削除後に再インストールします。

既知のパッケージが検索結果に出ない場合、NuGet search は `IncludeRegex` より先に実行される点に注意してください。返された NuGet page に含まれないパッケージは regex filter に届きません。`NuGetModuleConfig.SearchTerm`、`SearchSkip`、`SearchTake` を確認します。`SearchTake` は NuGet.org の 1 request 上限である 1000 に clamp されるため、後続ページには `SearchSkip` を使います。

Install、uninstall、refresh は同じ module-assets pipeline を共有し、ModuleManager operation coordinator により直列化されます。これらの command が無効に見える場合は、現在の restore/search operation が完了するまで待ってから次の操作を開始してください。

## Aspire とサイドカー リソース

Aspire リソースは、モジュールの extension provider から提供されます。モジュールは直接 Aspire builder をカスタマイズする `IExtensionResourcesProvider`、またはコマンドライン サイドカー用の `ICommandlineResourcesProvider` を登録できます。

リソースが表示されない場合:

- Aspire が resource graph を構築する前に、提供元モジュールがロードされたことを確認します。
- provider 型がモジュール アセンブリに登録されていることを確認します。
- コマンドライン リソースでは、リソース名、作業ディレクトリ、コマンドが有効であることを確認します。
- `AspireConfig.AutoStart` が無効な場合は、必要に応じて Aspire を手動で開始します。

Aspire dashboard が開けない場合は、Aspire プロセスが `ASPNETCORE_URLS` と `AppHost:BrowserToken` を生成したか確認します。dashboard URI はこれらの値から構築されます。

## CLI ターミナルの native 依存

CLI モジュールは、アプリケーション出力からターミナル native 依存をロードします。組み込みターミナルが早期に失敗する場合、次のファイルが存在することを確認してください。

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

欠落している場合は、CLI モジュールとターミナル依存の package output および copy-local 動作を確認します。

## ドキュメント テンプレート

`ZYC.Framework.Build.Doc` が使うドキュメント ソースは次の場所です。

```text
src\ZYC.Framework.Build.Doc\Templates
```

生成済みの root `docs` ファイルを編集すると一時的には効いたように見えても、後で消える可能性があります。対応する template file に変更を移し、ドキュメントを再生成してください。
