import os
import argparse
import shutil
import tarfile

def extract_tar_to_folder(unitypackage_file, copy_meta, force_overwrite):
    # Unitypackage ファイルの名前から拡張子を取り除いたフォルダを作成
    folder_name = os.path.splitext(unitypackage_file)[0]

    # 上述のフォルダがすでに存在するかチェック
    if os.path.exists(folder_name):
        if force_overwrite:
            # -f オプションが指定されている場合、上書きを自動的に承認
            print(f"フォルダ '{folder_name}' を上書きします。")
            shutil.rmtree(folder_name)
        else:
            # それ以外の場合は、上書きの確認を求める
            answer = input(f"フォルダ '{folder_name}' は既に存在します。上書きしますか？ (y/n): ").strip().lower()
            if answer != 'y':
                print("処理を中止しました.")
                return

    # Unitypackageファイルを解凍
    with tarfile.open(unitypackage_file, 'r') as tar:
        tar.extractall(path=folder_name)

    # Unitypackage の各フォルダ内にある pathname ファイルを処理
    for root, _, files in os.walk(folder_name):
        for file in files:
            if file == "pathname":
                pathname_path = os.path.join(root, "pathname")
                asset_path = os.path.join(root, "asset")

                # pathname ファイルから、作成するフォルダ名とファイル名を取得
                with open(pathname_path, 'r', encoding='utf-8', errors='replace') as pathname_file:
                    new_folder_path, new_file_name = os.path.split(pathname_file.readline().strip())

                # asset ファイルが、pathname ファイルと同じ階層に存在するかチェック
                if os.path.exists(asset_path):
                    # asset ファイルも存在する場合 (書き出しがファイルになる場合)
                    new_file_path = os.path.join(folder_name, new_folder_path, new_file_name)
                    os.makedirs(os.path.join(folder_name, new_folder_path), exist_ok=True)

                    # assetファイルをコピー
                    shutil.copy(asset_path, new_file_path)
                    print(f"Copied {asset_path} to {new_file_path}")

                    if copy_meta:
                        # asset.meta ファイルを pathname + .meta としてコピー
                        asset_meta_path = os.path.join(root, "asset.meta")
                        new_asset_meta_path = new_file_path + ".meta"
                        shutil.copy(asset_meta_path, new_asset_meta_path)
                        print(f"Copied {asset_meta_path} to {new_asset_meta_path}")
                else:
                    # asset ファイルが存在しない場合 (書き出しがフォルダになる場合)
                    new_file_path = os.path.join(folder_name, new_folder_path, new_file_name)
                    os.makedirs(os.path.join(folder_name, new_folder_path), exist_ok=True)

                    if copy_meta:
                        # asset.meta ファイルを pathname + .meta としてコピー
                        asset_meta_path = os.path.join(root, "asset.meta")
                        new_asset_meta_path = new_file_path + ".meta"
                        shutil.copy(asset_meta_path, new_asset_meta_path)
                        print(f"Copied {asset_meta_path} to {new_asset_meta_path}")

    # Assets と Packages 以外のフォルダ (つまり、GUID と同じ名前のフォルダ) を削除
    for item in os.listdir(folder_name):
        item_path = os.path.join(folder_name, item)
        if os.path.isdir(item_path) and item not in ["Assets", "Packages"]:
            shutil.rmtree(item_path)

def main():
    parser = argparse.ArgumentParser(description='Unitypackage ファイルを解凍する Python スクリプト')
    parser.add_argument('unitypackage_file', type=str, help='解凍対象の Unitypackage ファイルのパス')
    parser.add_argument('-m', '--meta', action='store_true', help='.meta ファイルも出力する')
    parser.add_argument('-f', '--force', action='store_true', help='フォルダを上書きする場合、確認せずに上書きする')
    args = parser.parse_args()

    extract_tar_to_folder(args.unitypackage_file, args.meta, args.force)
    print("展開が完了しました.")

if __name__ == "__main__":
    main()
