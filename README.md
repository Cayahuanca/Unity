
## Unity Files

Thank you for visiting.

There are some files that I made.

C# Code, Shader, VRChat Items, and more...

Some of them are available on BOOTH. We would appreciate it if you could purchase them as a donation.

Almost of them were made for personal use and has not been verified in other environments.

基本的には、個人用として作ったものであり、動作保証などは致しません。

  

## 1. Unity Editor Enhancer
 
![Editor Enhancer Poster](/Readme/EEXPoster.png)

- Supported Version: 
  - 2018 (5.x まで。それ以降は一部動かない可能性あり)
  - 2019 (2019.4.31f1, 2019.4.40f1 と、6.x で動作確認済)
  - 2021 (6.x のみ動作確認済)
  - 2022 (2022.3.6f1, 2022.3.22f1 と 6.x, 7.0 で動作確認済)
  - 6 (6000.0.0b11 と 6.x, 7.0 で動作確認済)
  - 团结引擎 (Tuanjie 1.5.1 と 7.0 で動作確認済)
  - ここに記載のないもの (他のマイナーバージョン違いや、中国の f1c1 と付くものなど) は、基本的に動くとは思いますが、実際に使用しての動作確認はしていません。

- within the hierarchy
  - Label with background color
    - Fine adjustments such as character style, position, color, theme function, etc.
  - Lines connecting object hierarchies (parent and child)
  - Alternating stripes in the background to prevent misselection of objects, etc.
  - Display custom icons for objects
  - Toggle the active state of an object
    - Support multiple batch switching
  - Missing Component warning display
  - Tile component icons
- ヒエラルキー内
  - 背景色付きのラベル
    - 文字のスタイルや位置、色、テーマ機能など細かい調整が可能
  - オブジェクトの階層(親子)をつなぐ線
  - オブジェクトの選択ミスなどを防ぐ、背景の交互の縞模様
  - オブジェクトに各自で設定したアイコンを表示
  - オブジェクトのActive 状態の切り替え
    - 複数の一括切り替えに対応
  - Missing Component の警告表示
  - コンポーネントのアイコンを並べて表示
 
![Features1](/Readme/EEXFeatures1.PNG)

- Custom icon settings for objects
  - Support for loading Unity built-in icons
  - Simplified version for instant setup
- Viewing imported file information
- Three-sided view of avatars, etc.

- オブジェクトのカスタムアイコン設定
  - 即座に設定できる簡易版
  - Unity ビルトインアイコンの読み込みに対応
- インポートしたファイル情報の表示
- アバターなどの三面図

![Features2](/Readme/EEXFeatures2.PNG)

- Batch deletion of components, including child objects
- Editing object coordinates and rotation relative to the world
- Show polygon count

- 子オブジェクト含めた、コンポーネントの一括削除
- ワールド基準でのオブジェクトの座標や回転の編集
- ポリゴン数の表示
![Features3](/Readme/EEXFeatures3.PNG)
  
- Context menu to open in Visual Studio Code 
- Visual Studio Code で開くコンテキストメニューの追加

[Check out this PDF for instructions on how to do it. (Japanese Only)](howtouse.pdf)

[VRChat Creator Companion Setup](vcc://vpm/addRepo?url=https://raw.githubusercontent.com/Cayahuanca/Unity/main/vpm-packages.json)

## 2. Face Camera

This is a shader for checking the expression of your avatar in VRChat.

It is not visible to others, nor does it appear in his VRCMirror or VRCCamera, which does not have reserved2 disabled.

これは、VRChat で、自身のアバターの表情を確認するためのシェーダーです。

他人からは見えず、reserved2 をオフにしていない VRCMirror や、VRCCamera にも映りません。

![FaceCamera](/Readme/FaceCamera.png)

## 3. Shader

This includes shaders for FaceCamera, as well as three improvements to UI/Default in Unity: one that does not show up in VRCCamera, one that only draws on one side, and one that has both, and an improvement to Unlit/Transparent so that you can just specify a color and Unlit/Transparent has been improved to make it semi-transparent by simply specifying a color.

これは、FaceCamera のシェーダーのほかに、Unity で、UI/Default を、VRCCamera に映らないもの、片面のみにしか描画されないもの、その両方を持つものの3種類に改良したもの、Unlit/Transparent を改良し、色を指定するだけで、半透明にできるものなどが含まれます。

## 4. Melon

This is an avatar gimmick that utilizes VRChat's PhysBones.

By placing them in the head, melon bread can be pulled out from the head instead of brains.

If an animation is created, it may be possible to link facial expressions.

これは、VRChat の PhysBones を利用した、アバターギミックです。

頭に仕込むことで、頭から脳みその代わりにメロンパンを引っ張り出すことができます。

アニメーションを作れば、表情を連動させることも可能でしょう。

![Melon](/Readme/Melon.png)
