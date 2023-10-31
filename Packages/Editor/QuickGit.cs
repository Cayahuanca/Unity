using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Praecipua.EE
{
    public class QuickGit : EditorWindow
    {
        [MenuItem("Window/Praecipua/Git Quick Commit")]
        public static void QuickCommit()
        {
                RunGitCommandLog("add", "-A");
                RunGitCommandLog("commit", "-m \"" + "Commit from Unity " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\"");
        }

        [MenuItem("Window/Praecipua/Git Quick Push")]
        public static void QuickPush()
        {
            GetCurrentRemote(out string remoteName, out string remoteBranch);

            if (string.IsNullOrEmpty(remoteName))
            {
                UnityEngine.Debug.Log("No remote repository found.");
                return;
            }

            RunGitCommandLog("push", remoteName + " " + remoteBranch);
        }

        [MenuItem("Window/Praecipua/Git Quick Pull")]
        public static void QuickPull()
        {
            GetCurrentRemote(out string remoteName, out string remoteBranch);

            if (string.IsNullOrEmpty(remoteName))
            {
                UnityEngine.Debug.Log("No remote repository found.");
                return;
            }

            RunGitCommandLog("pull", remoteName + " " + remoteBranch);
        }

        private static void RunGitCommandLog(string command, string arguments)
        {
            if (!Directory.Exists(Application.dataPath + "/../.git"))
            {
                UnityEngine.Debug.Log("Git is not initialized.");
                return;
            }
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "git";
            startInfo.WorkingDirectory = Application.dataPath + "/..";
            startInfo.Arguments = command + " " + arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            StringBuilder outputBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                   outputBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            UnityEngine.Debug.Log(outputBuilder.ToString());
        }

        private static void GetCurrentRemote(out string remoteName, out string remoteBranch)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "git";
            startInfo.WorkingDirectory = Application.dataPath + "/..";
            startInfo.Arguments = "rev-parse --abbrev-ref --symbolic-full-name @{u}";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] parts = output.Trim().Split('/');
            if (parts.Length != 2)
            {
                remoteName = null;
                remoteBranch = null;
                UnityEngine.Debug.Log("Invalid remote branch format.");
                return;
            }

            remoteName = parts[0];
            remoteBranch = parts[1];
        }
    }

    public class GitSettingsProvider : SettingsProvider
    {
        private static bool ForceEnglish;
        private static bool GitInitialized;
        private static string projectPath = Application.dataPath + "/../";
        private static string commitMessage = "";
        public static string remoteRepositoryName = "";
        private static string remoteRepositoryNewName = "";
        private static string remoteRepositoryUrl = "";
        public static string remoteRepositoryBranch = "";
        private string gitLogtext = "";
        private const string RemoteURL = "Praecipua_Git_RemoteURL";
        private const string RemoteName = "Praecipua_Git_RemoteName";
        private const string RemoteBranch = "Praecipua_Git_RemoteBranch";

        private const string SettingPath = "Project/Praecipua/Quick Git";
        private const string ForceEnglishKey = "Praecipua_English";

        public GitSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes) {}
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new GitSettingsProvider(SettingPath, SettingsScope.Project);
            return provider;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            LoadSettings();
            if (Directory.Exists(Path.Combine(projectPath, ".git")))
            {
                GitInitialized = true;
                GetCurrentRemote(out string remoteName, out string remoteBranch);
                if (remoteName != null)
                {
                    remoteRepositoryName = remoteName;
                    remoteRepositoryBranch = remoteBranch;
                }
            }
            else
            {
                GitInitialized = false;
            }
        }

        public override void OnGUI(string searchContext)
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string lang = ci.Name;

            string SetupGitButtonText = "Setup Git to this project";
            string CommitButtonText = "Commit Changes";
            string CommitText = "Commit";
            string CurrentRemoteText = "Current Target Remote Repository: ";
            string RemotePushButtonText = "Push Changes to Remote Repository";
            string RemotePullButtonText = "Pull Changes from Remote Repository";
            string RemoteNotSetText = "Remote Repository is not set.";
            string RemoteSetText = "Set Remote Repository";
            string RemoteSetButtonText = "Add / Change Remote Repository";
            string RemoteURLNotSetText = "Remote Repository URL is not set.";
            string GitLogText = "Git Command Log";
            string GitLogButtonText = "Show Log to Console";

            if (ForceEnglish == false && lang == "ja-JP")
            {
                SetupGitButtonText = "Gitをプロジェクトにセットアップ";
                CommitButtonText = "変更をコミット";
                CommitText = "コミット";
                CurrentRemoteText = "現在のリモートリポジトリ: ";
                RemotePushButtonText = "リモートリポジトリに変更を送信";
                RemotePullButtonText = "リモートリポジトリの変更を取得";
                RemoteNotSetText = "リモートリポジトリが設定されていません。";
                RemoteSetText = "リモートリポジトリの設定";
                RemoteSetButtonText = "リモートリポジトリを追加(変更)";
                RemoteURLNotSetText = "リモートリポジトリのURLが設定されていません。";
                GitLogText = "Gitコマンドのログ";
                GitLogButtonText = "ログをコンソールに表示";
            }

            if (GitInitialized == false)
            {
                 if (GUILayout.Button(SetupGitButtonText))
                {
                    SetupGit();
                    GitInitialized = true;
                }
            }

            if (GitInitialized == true)
            {

                GUILayout.Label(CommitButtonText);
                commitMessage = GUILayout.TextField(commitMessage);
                if (GUILayout.Button(CommitText))
                {
                    RunGitCommand("add", "-A");

                    if (commitMessage == "")
                    {
                        commitMessage = "Commit from Unity " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    }
                    RunGitCommand("commit", "-m \"" + commitMessage + "\""); // コミットメッセージを設定
                }

                GUILayout.Space(10);

                if (!string.IsNullOrEmpty(remoteRepositoryName))
                {
                    GUILayout.Label(CurrentRemoteText + remoteRepositoryName + "/" + remoteRepositoryBranch);

                    if (GUILayout.Button(RemotePushButtonText))
                    {
                        RunGitCommand("push", remoteRepositoryName + " " + remoteRepositoryBranch);
                    }

                    if (GUILayout.Button(RemotePullButtonText))
                    {
                        RunGitCommand("pull", remoteRepositoryName + " " + remoteRepositoryBranch);
                    }

                }
                else
                {
                    GUILayout.Label(CurrentRemoteText + RemoteNotSetText);
                }

                GUILayout.Space(10);

                GUILayout.Label(RemoteSetText);
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(40));
                    remoteRepositoryNewName = GUILayout.TextField(remoteRepositoryNewName);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("URL", GUILayout.Width(40));
                    remoteRepositoryUrl = GUILayout.TextField(remoteRepositoryUrl);
                GUILayout.EndHorizontal();
                if (GUILayout.Button(RemoteSetButtonText))
                {
                    if (remoteRepositoryNewName == "")
                    {
                        remoteRepositoryNewName = "origin";
                        return;
                    }
                    if (remoteRepositoryUrl == "")
                    {
                        UnityEngine.Debug.Log(RemoteURLNotSetText);
                        return;
                    }
                    if (remoteRepositoryNewName == remoteRepositoryName)
                    {
                        RunGitCommand("remote", "set-url " + remoteRepositoryName + " " + remoteRepositoryUrl);
                    }
                    else
                    {
                        RunGitCommand("remote", "add " + remoteRepositoryNewName + " " + remoteRepositoryUrl);
                        remoteRepositoryName = remoteRepositoryNewName;
                    }
                }

                GUILayout.Space(10);

                GUILayout.Label(GitLogText);
                if (GUILayout.Button(GitLogButtonText))
                {
                    RunGitCommand("log", "-n 5");
                }
            }
        }

        static void SetupGit()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string lang = ci.Name;

            string GitInitializedText = "Git is initialized to this project.";
            string GitIgnoreDownloadedText = ".gitignore template file is downloaded.";
            string GitAlreadyInitializedText = "Git is already initialized to this project.";
            if (ForceEnglish == false && lang == "ja-JP")
            {
                GitInitializedText = "Gitがプロジェクトにセットアップされました。";
                GitIgnoreDownloadedText = ".gitignore のテンプレートファイルをダウンロードしました。";
                GitAlreadyInitializedText = "Git はすでにこのプロジェクトにセットアップされています。";
            }
            // Gitがすでにセットアップされているかどうかを確認
            if (!Directory.Exists(projectPath + ".git"))
            {
                // Gitがセットアップされていない場合、git initを実行
                RunGitCommand("init", "");

                // .gitignoreファイルをダウンロードして設置
                DownloadGitIgnore();

                UnityEngine.Debug.Log(GitInitializedText);
            }
            else if (!File.Exists(projectPath + ".gitignore"))
            {
                DownloadGitIgnore();
                UnityEngine.Debug.Log(GitIgnoreDownloadedText);
            }
            else
            {
                UnityEngine.Debug.Log(GitAlreadyInitializedText);
            }
        }

        // .gitignoreファイルをダウンロードして設置する
        static void DownloadGitIgnore()
        {
            string gitIgnoreUrl = "https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore";

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(gitIgnoreUrl,projectPath + ".gitignore");
            }
        }

        private static void RunGitCommand(string command, string arguments)
        {
            if (!Directory.Exists(Application.dataPath + "/../.git"))
            {
                UnityEngine.Debug.Log("Git is not initialized.");
                return;
            }
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "git";
            startInfo.WorkingDirectory = Application.dataPath + "/..";
            startInfo.Arguments = command + " " + arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            StringBuilder outputBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                   outputBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            UnityEngine.Debug.Log(outputBuilder.ToString());
        }

        private static void GetCurrentRemote(out string remoteName, out string remoteBranch)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "git";
            startInfo.WorkingDirectory = Application.dataPath + "/..";
            startInfo.Arguments = "rev-parse --abbrev-ref --symbolic-full-name @{u}";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] parts = output.Trim().Split('/');
            if (parts.Length != 2)
            {
                remoteName = null;
                remoteBranch = null;
                UnityEngine.Debug.Log("Invalid remote branch format.");
                return;
            }

            remoteName = parts[0];
            remoteBranch = parts[1];
        }

        // Load Settings
        private static void LoadSettings()
        {
            ForceEnglish = EditorPrefs.GetBool(ForceEnglishKey);
        }

        // Save Settings
        private static void SaveSettings()
        {

        }
    }
}
