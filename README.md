# ToraCon-Helper [![Current Release](https://img.shields.io/github/release/kuramochia/ToraCon-Helper)](https://github.com/kuramochia/ToraCon-Helper/releases) [![Licensed under the MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/kuramochia/ToraCon-Helper/blob/master/LICENSE)

## なにこれ？
[HORI Force Feedback Truck Control System for Windows® PC（通称「トラコン」）](https://hori.jp/products/hpc/hpc-044/) を ETS2/ATS でちょっと便利にするためのヘルパーアプリです。

トラコン専用ってわけではないので、ほかのコントローラーやキーマウでも利用できます。

## 何するアプリ？
ETS2/ATS の [Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) の仕組みを利用して、プラグインをインストールする以外の設定変更無しで、ちょっと便利な操作を自動化します。

今までの便利操作系ツールは Key2Key などを利用して実現はできましたが、ETS/ATS のキーコンフィグを変更する必要がありました。そしてプログラミングがわからないとちょっとハードルが高い。。

このアプリは、[Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) の出力（外部アプリからテレメトリー情報を読み取る）と入力（外部アプリからコントローラーと同様に入力を送信）機能を両方を使って、ETS2/ATS のキーコンフィグ設定をほとんど変更せず、ちょっと便利な操作の自動化を実現しています。


## 動作要件
* Microsoft Windows 10 / 11 x64 
* .NET Framework 4.8 または 4.8.1
    * 今サポートされている OS ならプリインストールされています。
* ETS2 / ATS
    * Telemetry DLL が x64 版のみ提供しているので、x86 でゲームされている方は利用できません。

## セットアップ方法
1. 下記のリンクをクリックし、Microsoft Store から ToraCon-Helper をダウンロード、インストールしてください。

<a href="https://apps.microsoft.com/detail/9n2vdmrmjw1s?referrer=appbadge&mode=direct">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="400"/>
</a>

2. `ToraCon-Helper` を起動します。
3. 初回起動時やアプリ更新時は、Telemetry DLL のインストールや更新が必要になるため、次のようなメッセージが表示されます。
  * 「はい」を選択すると、（Steam の既定のゲーム インストール先が Program Files なので）管理者権限でインストール プロセスが起動します。
  * 最新版の Telemetry DLL に更新後、次回以降の起動時はこちらは表示されません。

![](images/TelemetryDllInstallDialog.png)

4. Telemetry DLL のインストールプロセスは、次のようなウィンドウが表示されます。エラーが無ければ、DLL のコピーは終了です。右下の「閉じる」ボタンで閉じてください。
  * もし、__「エラーが発生しました」__ と表示された場合は、表示内容と一緒に [Issues](https://github.com/kuramochia/ToraCon-Helper/issues) へお願いします。
  * Telemetry DLL を手動でインストールする場合は、「プラグインを手動でコピー」ボタンを押し、表示された `ToraCon-scs-telemetry.dll` を ETS2/ATS のインストールフォルダ (例: Euro Truck Simulator 2\bin\win_x64\plugins) にコピーしてください。
  
![](images/TelemetryDllInstallWindow.png)

5. アプリ本体は次のような画面が表示されます。

![](images/appimage.png)

## アプリの設定
設定を変更すると、`MyDocuments¥ToraCon-Helper` フォルダに `ToraCon-Helper_Settings.json` を出力します。

### 全体設定
#### テレメトリー動作
* [Telemetry SDK](https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry) を利用して、ETS2/ATS からテレメトリー情報を取得します。これを On にしないと、下記の各アクションが動作しません。

#### アプリ終了時にタスクトレイに格納
* このアプリケーションを「×」で終了したときに、アプリを終了せず、タスクトレイに収納します。

#### 起動時にタスクトレイに格納
* このアプリケーションを再度起動したときに、画面を表示させずにタスクトレイに常駐します。
* 画面を表示したいときはタスクトレイのアイコンをダブルクリック、または右クリック → 表示 を選択します。

#### 起動時に最小化
* このアプリケーションを再度起動したときに、画面を最小化した状態にします。
* [起動時にタスクトレイに格納](#起動時にタスクトレイに格納) をオンにしている場合は、そちらを優先します。

#### スタートアップに登録
* Windows ユーザーのログイン時に自動的にこのアプリを起動するように設定できます。
  * 既定で On になっていますので、必要に応じて変更してください。
* 「設定」 ボタンを押すと、Windows のスタートアップ設定画面が表示されます。


### ウィンカー関連 アクション設定
#### ウィンカー戻す動作を実車に近づける
* ウィンカーが出ているときに、逆側のウィンカー入力をすると、ウィンカーを消すことができます。（ただし一瞬だけ逆側のウィンカーが表示されます）

#### (トラコン専用) DirectInput を使ってウィンカー戻す動作を実車に近づける
* ウィンカーが出ているときに、逆側のウィンカー入力をすると、ウィンカーを消すことができます。
  * 制限事項を撤廃するために DirectInput API を使って、トラコンのウィンカー入力を行います。
  * [HORI Force Feedback Truck Control System for Windows® PC（通称「トラコン」）](https://hori.jp/products/hpc/hpc-044/) 専用です。
  * ETS2/ATS のキー設定から、左右のウィンカーの割り当てを「解除」してください。
  * ウィンカーレバーは、`左レバー(Button40,41)` または `右レバー(Button46,47)` を選択できます。
  * 上記の DirectInput を利用しないアクションとは併用できませんので、どちらかを有効にしてお使いください。

#### ウィンカー オートキャンセルのハンドル回転角度変更

* ウィンカー オートキャンセルが動作するハンドル回転角度を指定できます。
* まず、ゲームで設定しているハンドルの回転角度を指定します。
* そのあと、オートキャンセルが動作するまでのハンドル角度を指定します。

#### 高速道路 車線変更用ショート ウィンカー
* 高速道路等で、車線変更用にウィンカーを出したら数秒で自動的に消える機能です。
* 一部の実車では、レバーを軽く押す、という操作なのですが、実現できないため、下記３つの条件指定で動作します。
  * 有効になる最低速度
  * 無効になるハンドル回転角度
  * ウィンカーが消えるまでの時間
* まずは初期値でお試しいただき、必要に応じて各条件を変更ください。

### リターダー関連 アクション設定
#### 指定速度以下でリターダーを全部戻す

* 指定された速度以下でリターダーを戻す入力を行うと、リターダーを一気に全段戻します。
* その下のスライダーで、リターダーを全部戻す速度を指定します。0~100 km/h が指定できます。

#### リターダー1段入れたら全段入れる

* リターダーが0段の状態から1段入れると、一気にトラックの持つ最大段数まで上げます。

#### リターダーフルから1段戻したら全段戻す

* リターダーを最大段数から一段下げると、一気に全段戻します。

#### 指定速度以下でリターダーを自動的にオフ
* 指定された速度以下になると、リターダーを操作無しに自動的にオフにします。
* 信号で停止したとき、リターダーをオフにし忘れてしまうことがなくなります。
* また、リターダーを普段利用しない場合でも、不意にリターダーをオンにしてしまった場合に、自動的にオフに戻ります。

#### リターダーの入力を N 段スキップ
* リターダーは通常4段や5段の操作が可能ですが、素早く操作したい方向けに、N 段スキップして、疑似的に操作段数を減らします。
* 1 段スキップの場合、リターダーの操作をする度に、0 -> 2 -> 4 段になります。戻す場合も同様です。


### エンジンブレーキ関連 アクション設定
#### 指定速度以下でエンジンブレーキを自動的にオフ

* 指定された速度以下になると、エンジンブレーキ トグルスイッチを押して自動的にオフにします。
* 「エンジンブレーキのトグル」キーを利用している場合に便利です。
* 信号で停止したとき、エンジンブレーキをオフにし忘れてしまうことがなくなります。
* また、エンジンブレーキを普段利用しない場合でも、不意にエンジンブレーキをオンにしてしまった場合に、自動的にオフに戻ります。

### その他 アクション設定
#### 給油を開始すると、ボタンを離しても満タンまで給油し続ける

* 給油時に Enter や OK ボタンを一度押して、給油を開始してしまえば、ボタンを離しても満タンまで給油が続きます（代わりにボタンを押し続けます）
* 給油するとき、満タンになるまでボタンを押し続けるのが面倒な方向けです。

## PowerToys
ETS2/ATS のいろんなフォルダ・ファイルを簡単に閲覧・起動できるページを用意しました。

![](images/appPowerToyPageImage.png)
![ゲーム内情報](images/game_info.png)

### PowerToys ページでできること
* ゲームの起動
* ゲームフォルダ の表示
* データフォルダ の表示と変更
  * ETS2/ATS の起動引数で `-homedir "D:\ETS2"` 等の設定をしている場合は、環境に合わせて修正してください。
* game.log.txt の（関連付けられたアプリでの）表示
* config.cfg の（関連付けられたアプリでの）表示
* mod フォルダ の表示
* プロファイルの一覧
  * ローカル プロファイルと Steam プロファイルが対象
  * 謎の(!?)プロファイル名をデコードして表示
  * 最終更新時刻が新しい順に表示
  * クリックするとそのプロファイルを開きます
* ゲーム内情報
  * 起動中のゲーム (ETS2 / ATS)
  * ナビ情報（移動距離、所要時間）
  * ゲーム内時間
  * リアル時間

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

## Telemetry 情報の一覧
[RenCloud](https://github.com/RenCloud/) さんの [README](https://github.com/RenCloud/scs-sdk-plugin/blob/master/README.md#telemetry-fields-and-the-c-object) を確認してください。
