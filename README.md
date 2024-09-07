# ToraCon-Helper

## なにこれ？
[HORI Force Feedback Truck Control System for Windows® PC（通称「トラコン」）](https://hori.jp/products/hpc/hpc-044/) を ETS2/ATS でちょっと便利にするためのヘルパーアプリです。

トラコン専用ってわけではないので、ほかのコントローラーやキーマウでも利用できます。

## 何するアプリ？
ETS2/ATS の [Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) の仕組みを利用して、プラグインをインストールする以外の設定変更無しで、ちょっと便利な操作を自動化します。

今までの便利操作系ツールは Key2Key などを利用して実現はできましたが、ETS/ATS のキーコンフィグを変更する必要がありました。そしてプログラミングがわからないとちょっとハードルが高い。。

このアプリは、[Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) の出力（外部アプリからテレメトリー情報を読み取る）と入力（外部アプリからコントローラーと同様に入力を送信）機能を両方を使って、ETS2/ATS のキーコンフィグ設定を一切変更せず、ちょっと便利な操作の自動化を実現しています。


## できること（アプリの設定で有効・無効の切り替え可能）
* ウィンカーが出ているときに逆側のウィンカーを出すと、消えるアクション
  * 特にトラコンのウィンカーレバーはスイッチ式のため、ウィンカーを「消す」操作が通常の車の操作と異なります。
  * このアクションで、例えば右ウィンカーが出ているときに左ウィンカー入力をすると、ウィンカーを「消します」
  * ただし、ただし、逆側のウィンカーが一瞬表示されてしまいます。これは残念ながら制限事項です。
    * テレメトリーから情報を取っているので、どうしてもウィンカーが表示されてからになってしまいます。
    * この制限事項が微妙・・・という方は、私の gist に [Key2Key を使って、この動作を実現する](https://gist.github.com/kuramochia/dd79fa29ecfd5af0a207b0b11116ff64) をご利用ください。

* 指定速度以下の場合にリターダーを全段戻す
  * 信号で止まった後、出発するときに「リターダーが！！」って怒られません？
  * 指定された速度以下になったときにリターダーを戻す入力をすると、全段戻ります。

* ウィンカー オートキャンセルのハンドル回転角度変更
  * ウィンカー オートキャンセル機能は ETS2/ATS に搭載されていますが、ハンドルをほぼ中央まで戻さないと動作しません。
  * ウィンカー オートキャンセルが動作するハンドル回転角度を指定できるようにしました。

* __リターダー1段入れたら全段入れる<sup>new! v1.2</sup>__
  * リターダーが0段の状態から1段入れると、一気にトラックの持つ最大段数まで上げます。

* __リターダーフルから1段戻したら全段戻す<sup>new! v1.2</sup>__
  * リターダーを最大段数から一段下げると、一気に全段戻します。

## 動作要件
* Microsoft Windows 10 / 11 x64 
* .NET Framework 4.8
    * 今サポートされている OS ならプリインストールされています。
* ETS2 / ATS
    * Telemetry DLL が x64 版のみ提供しているので、x86 でゲームされている方は動かないです。

## セットアップ方法
1. [リリースページ](https://github.com/kuramochia/ToraCon-Helper/releases) から最新の `ToraCon-Helper_vX.X.X.X.zip` という名前の zip ファイルをダウンロードし、解凍します。
2. 解凍したフォルダの中に `plugins` と `ToraCon-Helper` フォルダがあります。
3. `plugins\win_x64` フォルダにある `ToraCon-scs-telemetry.dll` を、ETS2/ATS の plugin フォルダにコピーします。（通常、プラグインフォルダは ETS2 = `C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2\bin\win_x64\plugins`、ATS = `C:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator\bin\win_x64\plugins` です。）
4. `ToraCon-Helper` フォルダを好きな場所に配置し、`ToraConHelper.exe` を起動します。
![](images/appimage.png)

## アプリの設定
設定を変更すると、`ToraConHelper.exe` と同じフォルダに `ToraCon-Helper_Settings.json` を出力します。

### 全体設定
#### 1. テレメトリー動作
[Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) を利用して、ETS2/ATS からテレメトリー情報を取得します。これを On にしないと、下記の各アクションが動作しません。

#### 2. アプリ終了時にタスクトレイに格納
このアプリケーションを「×」で終了したときに、アプリを終了せず、タスクトレイに収納します。

#### 3.起動時にタスクトレイに格納
このアプリケーションを再度起動したときに、画面を表示させずにタスクトレイに常駐します。

画面を表示したいときはタスクトレイのアイコンをダブルクリック、または右クリック → 表示 を選択します。


### アクション設定
#### ウィンカー戻す動作を実車に近づける

ウィンカーが出ているときに、逆側のウィンカー入力をすると、ウィンカーを消すことができます。（ただし一瞬だけ逆側のウィンカーが表示されます）

#### 指定速度以下でリターダーを全部戻す

指定された速度以下でリターダーを戻す入力を行うと、リターダーを一気に全段戻します。

その下のスライダーで、リターダーを全部戻す速度を指定します。0~100 km/h が指定できます。

#### ウィンカー オートキャンセルのハンドル回転角度変更

ウィンカー オートキャンセルが動作するハンドル回転角度を指定できます。

まず、ゲームで設定しているハンドルの回転角度を指定します。

そのあと、オートキャンセルが動作するまでのハンドル角度を指定します。

#### リターダー1段入れたら全段入れる<sup>new! v1.2</sup>

リターダーが0段の状態から1段入れると、一気にトラックの持つ最大段数まで上げます。

#### リターダーフルから1段戻したら全段戻す<sup>new! v1.2</sup>

リターダーを最大段数から一段下げると、一気に全段戻します。

## バグを発見した
[Issues](https://github.com/kuramochia/ToraCon-Helper/issues) をお願いします。

## 機能追加してほしい
[Discussions](https://github.com/kuramochia/ToraCon-Helper/discussions) や、[X(旧 Twitter)](https://x.com/kuramochia) にご相談ください。ただしやるとは言ってない。
 
## 参考にした Repository
先人の知恵、素晴らしい。感謝。~~ほぼパクり~~参考にさせていただきました。
* https://github.com/RenCloud/scs-sdk-plugin
* https://github.com/ETS2LA/scs-sdk-controller

## このアプリ作成の過程で手伝っていただいた方々
Key2Key のスクリプト作成含め、X でこのお三方にはいろいろご相談させていただきました。誠にありがとうございました！
* [ネック さん](https://x.com/NekMtNk)
    * ことの発端となった[ウインカー動作変更を Key2Key で実装](https://wiki3.jp/controller-mod/page/6)された方。
* [hideG さん](https://x.com/hideG_ran)
    * ETS2Talk などの ETS2 向けアプリ作成先駆者。
* [ダーさん](https://x.com/Darling04476831)
    * まなスカ、SCANIA 2016 S/R バニラ車両用 新型テールランプ with シーケンシャルウインカー 等の Modder。
