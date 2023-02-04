## Unity Files
Thank you for visiting.

There are some files that I made.
C# Code, Shader, VRChat Items, and more...

Some of them are available on BOOTH. We would appreciate it if you could purchase them as a donation.
一部のものは、BOOTH で販売しています。寄付として購入していただけるとありがたいです。
[Praecipua - BOOTH](https://praecipua.booth.pm/)

Almost of them were made for personal use and has not been verified in other environments.
基本的には、個人用として作ったものであり、動作保証などは致しません。

## 1. Transform Viewer / Editor

This adds an item for absolute position (world position) to the Transform in the Inspector.
There are three types, one that only displays absolute coordinates and one that can be edited (Japanese and English).
Include only one of Transform Viewer.cs, Transform Editor En.cs, Transform Editor Ja.cs in your project.

これは、Inspector の Transform に、絶対座標(ワールド座標)の項目を追加します。
絶対座標の表示のみ、編集も可能なもの(日本語、英語)の3種類があります。
Transform Viewer.cs, Transform Editor En.cs, Transform Editor Ja.cs のうち、一つだけをプロジェクトに入れてください。

![Transform Viewer / Editor](/Readme/1.Transform.png)

## 2. Hierarchy Color

This makes it easier to distinguish between Hierarchy and Project by adding a background color with space between each line.
From Edit/Preference/Hierarchy Customization you can change the color and enable/disable it.

これは、Hierarchy 及び、Project に、1行ずつ間を開けて背景色を付け、見分けやすくします。
Edit/Preference/Hierarchy Customization から、色の変更及び、有効無効の切り替えができます。

![Hierarchy Color](/Readme/2.Hierarchy.png)

## 3. File Info

This displays the information (file name, extension, file size, last modified, file path) of the file selected in Project.
It can be displayed from Window/File Info.

これは、Project で選択しているファイルの情報(ファイル名、拡張子、ファイルサイズ、最終更新、ファイルパス)を表示します。
Window/File Info から表示可能です。

![File Info](/Readme/3.FileInfo.png)

## 4. Polygon Counter

This adds a display of the number of polygons to the Mesh Filter column when the object selected in Hierarchy has a Mesh Filter.

これは、Hierarchy で選択したオブジェクトが、Mesh Filter を持つ場合に、Mesh Filter の欄に、ポリゴン数の表示を追加します。

![Polygon Counter](/Readme/4.Polygon.png)

## 5. Face Camera
This is a shader for checking the expression of your avatar in VRChat.
It is not visible to others, nor does it appear in his VRCMirror or VRCCamera, which does not have reserved2 disabled.

これは、VRChat で、自身のアバターの表情を確認するためのシェーダーです。
他人からは見えず、reserved2 をオフにしていない VRCMirror や、VRCCamera にも映りません。

![FaceCamera](/Readme/5.FaceCamera.png)

## 6. Shader
This includes shaders for FaceCamera, as well as three improvements to UI/Default in Unity: one that does not show up in VRCCamera, one that only draws on one side, and one that has both, and an improvement to Unlit/Transparent so that you can just specify a color and Unlit/Transparent has been improved to make it semi-transparent by simply specifying a color.

これは、FaceCamera のシェーダーのほかに、Unity で、UI/Default を、VRCCamera に映らないもの、片面のみにしか描画されないもの、その両方を持つものの3種類に改良したもの、Unlit/Transparent を改良し、色を指定するだけで、半透明にできるものなどが含まれます。

## 7. Melon

This is an avatar gimmick that utilizes VRChat's PhysBones.
By placing them in the head, melon bread can be pulled out from the head instead of brains.
If an animation is created, it may be possible to link facial expressions.

これは、VRChat の PhysBones を利用した、アバターギミックです。
頭に仕込むことで、頭から脳みその代わりにメロンパンを引っ張り出すことができます。
アニメーションを作れば、表情を連動させることも可能でしょう。
![Melon](/Readme/7.Melon.png)




