using Microsoft.WindowsAPICodePack.Shell;
using OW_Scoreboard_Tool.Entities;
using OW_Scoreboard_Tool.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Net;

namespace OW_Scoreboard_Tool
{
    public partial class Form1 : Form
    {
        #region Inital Properties
        public string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string xmlVersion = "0.4";
        const int Bytes_TO_READ = sizeof(Int64);
        Series Match1 = new Series();
        private bool isInitializing = true;
        private bool isListeningForKey = false;


        List<Replay> Replays = new List<Replay>();
        BindingList<Team> BracketTeams = new BindingList<Team>();
        FileSystemWatcher replayWatcher = new FileSystemWatcher();

        public BindingList<Gametype> GametypeList = new BindingList<Gametype>
        {
            new Gametype()
        };
        public BindingList<Hero> HeroList = new BindingList<Hero>
        {
            new Hero()
        };
        public BindingList<Map> MapList = new BindingList<Map>
        {
            new Map()
        };
        public BindingList<Role> RoleList = new BindingList<Role>
        {
            new Role()
        };
        private List<Gametype> DefaultGametypeList = new List<Gametype>
        {
            new Gametype()
        };
        private List<Hero> DefaultHeroList = new List<Hero>
        {
            new Hero()
        };

        private List<Map> DefaultMapList = new List<Map>
        {
            new Map()
        };
        private List<Role> DefaultRoleList = new List<Role>
        {
            new Role()
        };

        List<string> FolderList;
        List<string> Match1Files;
        List<string> GeneralFiles;
        List<string> PlayerFiles;
        List<string> SettingFiles;
        List<string> ReplayFiles;
        List<string> BracketFiles;

        int[] customColors = new int[]
            {
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#8cba11")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#ff9c00")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#202224")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#C80013")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#27AAE1")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#D45800")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#D47900")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#FFD700")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#FFFF00")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#CCFF00")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#00AB84")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#00FFFF")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#800080")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#523FFF")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#FF00FF")),
                ColorTranslator.ToOle(ColorTranslator.FromHtml("#FF6EC7"))
            };

        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            GenerateRoles();
            GenerateHeroes();
            GenerateGametypes();
            GenerateMaps();
            GenerateFolderList();
            GenerateFileList();
            colorThemeDialog.CustomColors = customColors;

            CheckFolders();
            CheckFiles();

            CreateFileWatcher(path + FolderList[4]);

            LoadCustomGametypes();
            LoadCustomHeroes();
            LoadCustomMaps();
            LoadCustomRoles();

            if (GametypeList.Count == 1)
            {
                DefaultGametypes();
            }
            if (HeroList.Count == 1)
            {
                DefaultHeroes();
            }
            if (MapList.Count == 1)
            {
                DefaultMaps();
            }
            if (RoleList.Count == 1)
            {
                DefaultRoles();
            }

            LinkHeroList();
            LinkMapList();
            LinkRoleList();
        }

        /// <summary>
        /// Default Initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            StartHttpServer();
            isInitializing = false;
            RegisterHotkeys();


            loadText(m1MutualInfo, "Match1", "DivisionNumber");

            loadScore(m1t1Score, "Match1", "t1Score");
            loadText(m1t1Name, "Match1", "t1Name");
            loadText(m1t1SR, "Match1", "t1SR");
            loadText(m1t1p1Name, "Match1", "t1p1Name");
            loadText(m1t1p2Name, "Match1", "t1p2Name");
            loadText(m1t1p3Name, "Match1", "t1p3Name");
            loadText(m1t1p4Name, "Match1", "t1p4Name");
            loadText(m1t1p5Name, "Match1", "t1p5Name");
            loadText(m1t1p6Name, "Match1", "t1p6Name");
            loadColor(m1t1ColorButton, "Match1", "t1Color");
            loadButton(m1t1LogoButton, "Match1", "t1Logo");

            loadScore(m1t2Score, "Match1", "t2Score");
            loadText(m1t2Name, "Match1", "t2Name");
            loadText(m1t2SR, "Match1", "t2SR");
            loadText(m1t2p1Name, "Match1", "t2p1Name");
            loadText(m1t2p2Name, "Match1", "t2p2Name");
            loadText(m1t2p3Name, "Match1", "t2p3Name");
            loadText(m1t2p4Name, "Match1", "t2p4Name");
            loadText(m1t2p5Name, "Match1", "t2p5Name");
            loadText(m1t2p6Name, "Match1", "t2p6Name");
            loadColor(m1t2ColorButton, "Match1", "t2Color");
            loadButton(m1t2LogoButton, "Match1", "t2Logo");

            loadScore(m1m1t1Score, "Match1", "m1t1Score");
            loadScore(m1m2t1Score, "Match1", "m2t1Score");
            loadScore(m1m3t1Score, "Match1", "m3t1Score");
            loadScore(m1m4t1Score, "Match1", "m4t1Score");
            loadScore(m1m5t1Score, "Match1", "m5t1Score");
            loadScore(m1m6t1Score, "Match1", "m6t1Score");
            loadScore(m1m7t1Score, "Match1", "m7t1Score");

            loadScore(m1m1t2Score, "Match1", "m1t2Score");
            loadScore(m1m2t2Score, "Match1", "m2t2Score");
            loadScore(m1m3t2Score, "Match1", "m3t2Score");
            loadScore(m1m4t2Score, "Match1", "m4t2Score");
            loadScore(m1m5t2Score, "Match1", "m5t2Score");
            loadScore(m1m6t2Score, "Match1", "m6t2Score");
            loadScore(m1m7t2Score, "Match1", "m7t2Score");

            LoadPlayerHeroes();

            LoadPlayerRoles();

            loadText(m1t1p1Info, "Match1", "t1p1Info");
            loadText(m1t1p2Info, "Match1", "t1p2Info");
            loadText(m1t1p3Info, "Match1", "t1p3Info");
            loadText(m1t1p4Info, "Match1", "t1p4Info");
            loadText(m1t1p5Info, "Match1", "t1p5Info");
            loadText(m1t1p6Info, "Match1", "t1p6Info");
            
            loadText(m1t2p1Info, "Match1", "t2p1Info");
            loadText(m1t2p2Info, "Match1", "t2p2Info");
            loadText(m1t2p3Info, "Match1", "t2p3Info");
            loadText(m1t2p4Info, "Match1", "t2p4Info");
            loadText(m1t2p5Info, "Match1", "t2p5Info");
            loadText(m1t2p6Info, "Match1", "t2p6Info");

            loadButton(m1t1p1ImageButton, "Match1", "t1p1Image");
            loadButton(m1t1p2ImageButton, "Match1", "t1p2Image");
            loadButton(m1t1p3ImageButton, "Match1", "t1p3Image");
            loadButton(m1t1p4ImageButton, "Match1", "t1p4Image");
            loadButton(m1t1p5ImageButton, "Match1", "t1p5Image");
            loadButton(m1t1p6ImageButton, "Match1", "t1p6Image");

            loadButton(m1t2p1ImageButton, "Match1", "t2p1Image");
            loadButton(m1t2p2ImageButton, "Match1", "t2p2Image");
            loadButton(m1t2p3ImageButton, "Match1", "t2p3Image");
            loadButton(m1t2p4ImageButton, "Match1", "t2p4Image");
            loadButton(m1t2p5ImageButton, "Match1", "t2p5Image");
            loadButton(m1t2p6ImageButton, "Match1", "t2p6Image");

            LoadMapsPlayed();

            loadText(message, "General", "message");
            loadText(host, "General", "host");
            loadText(analyst1, "General", "analyst1");
            loadText(analyst2, "General", "analyst2");
            loadText(caster1, "General", "caster1");
            loadText(caster2, "General", "caster2");
            loadText(utility1, "General", "utility1");
            loadText(utility2, "General", "utility2");
            loadText(utility3, "General", "utility3");
            loadText(utility4, "General", "utility4");
            loadText(utility5, "General", "utility5");
            loadText(utility6, "General", "utility6");
            loadText(utility7, "General", "utility7");
            loadText(utility8, "General", "utility8");
            loadText(utility9, "General", "utility9");
            loadText(utility10, "General", "utility10");
            loadText(utility11, "General", "utility11");
            loadText(utility12, "General", "utility12");
            loadText(utility13, "General", "utility13");
            loadText(utility14, "General", "utility14");
            loadText(utility15, "General", "utility15");
            loadText(utility16, "General", "utility16");
            loadText(utility17, "General", "utility17");
            loadText(utility18, "General", "utility18");
            loadText(utility19, "General", "utility19");
            loadText(utility20, "General", "utility20");
            loadText(utility21, "General", "utility21");
            loadText(utility22, "General", "utility22");

            loadButton(imageButton1, "General", GeneralFiles[59].Replace(".png", ""));
            loadButton(imageButton2, "General", GeneralFiles[60].Replace(".png", ""));
            loadButton(imageButton3, "General", GeneralFiles[61].Replace(".png", ""));
            loadButton(imageButton4, "General", GeneralFiles[62].Replace(".png", ""));
            loadButton(imageButton5, "General", GeneralFiles[63].Replace(".png", ""));
            loadButton(imageButton6, "General", GeneralFiles[64].Replace(".png", ""));
            loadButton(imageButton7, "General", GeneralFiles[65].Replace(".png", ""));
            loadButton(imageButton8, "General", GeneralFiles[66].Replace(".png", ""));
            loadButton(imageButton9, "General", GeneralFiles[67].Replace(".png", ""));
            loadButton(imageButton10, "General", GeneralFiles[68].Replace(".png", ""));
            loadButton(imageButton11, "General", GeneralFiles[69].Replace(".png", ""));
            loadButton(imageButton12, "General", GeneralFiles[70].Replace(".png", ""));
            loadButton(imageButton13, "General", GeneralFiles[71].Replace(".png", ""));
            loadButton(imageButton14, "General", GeneralFiles[72].Replace(".png", ""));
            loadButton(imageButton15, "General", GeneralFiles[73].Replace(".png", ""));
            loadButton(imageButton16, "General", GeneralFiles[74].Replace(".png", ""));
            loadButton(imageButton17, "General", GeneralFiles[75].Replace(".png", ""));
            loadButton(imageButton18, "General", GeneralFiles[76].Replace(".png", ""));
            loadButton(imageButton19, "General", GeneralFiles[77].Replace(".png", ""));
            loadButton(imageButton20, "General", GeneralFiles[78].Replace(".png", ""));
            loadButton(imageButton21, "General", GeneralFiles[79].Replace(".png", ""));
            loadButton(imageButton22, "General", GeneralFiles[80].Replace(".png", ""));
            loadButton(imageButton23, "General", GeneralFiles[81].Replace(".png", ""));
            loadButton(imageButton24, "General", GeneralFiles[82].Replace(".png", ""));
            loadButton(imageButton25, "General", GeneralFiles[83].Replace(".png", ""));
            loadButton(imageButton26, "General", GeneralFiles[84].Replace(".png", ""));
            loadButton(imageButton27, "General", GeneralFiles[85].Replace(".png", ""));
            loadButton(imageButton28, "General", GeneralFiles[86].Replace(".png", ""));

            loadColor(colorThemeButton1, "General", "colorTheme1");
            loadColor(colorThemeButton2, "General", "colorTheme2");
            loadColor(colorThemeButton3, "General", "colorTheme3");
            loadColor(colorThemeButton4, "General", "colorTheme4");
            loadColor(colorThemeButton5, "General", "colorTheme5");
            loadColor(colorThemeButton6, "General", "colorTheme6");
            loadColor(colorThemeButton7, "General", "colorTheme7");
            loadColor(colorThemeButton8, "General", "colorTheme8");

            loadText(playerBox, "Player", "player");
            loadText(playerSRBox, "Player", "sr");
            loadText(playerTeamBox, "Player", "team");
            loadText(playerTeamInfoBox, "Player", "info");

            loadCombo(playerHeroBox1, "Player", "hero1");
            loadCombo(playerHeroBox2, "Player", "hero2");
            loadCombo(playerHeroBox3, "Player", "hero3");
            loadCombo(playerRoleBox, "Player", "role");

            loadButton(playerImageButton, "Player", GeneralFiles[8].Replace(".png", ""));
            loadButton(playerTeamLogoButton, "Player", GeneralFiles[6].Replace(".png", ""));

            loadBracket();

            updateSeries();

            updateReplayPage();
            updateList();
         
        }

        #endregion

        #region HTTPServer
        private HttpListener httpListener;

        public void StartHttpServer()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/"); // Set your desired port and URL
            httpListener.Start();
            Console.WriteLine("HTTP Server started. Listening for requests...");

            httpListener.BeginGetContext(OnRequestReceived, httpListener);
        }

        private void OnRequestReceived(IAsyncResult result)
        {
            if (httpListener == null || !httpListener.IsListening) return;

            var context = httpListener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;

            string responseString = "OK";

            // Check for /trigger endpoint
            if (request.Url.AbsolutePath == "/trigger")
            {
                Console.WriteLine("Triggered.. request...", request);
                string action = request.QueryString["action"];
                Console.WriteLine("Triggered.. but...", action);

                if (!string.IsNullOrEmpty(action))
                {
                    HandleAction(action);
                    responseString = $"Action '{action}' triggered!";
                }
                else
                {
                    responseString = "No action specified.";
                }
            }
            // Check for /set endpoint to set the hero image
            else if (request.Url.AbsolutePath == "/set")
            {
                Console.WriteLine("Set hero image request...", request);
                string teamSide = request.QueryString["teamSide"];
                string imageUrl = request.QueryString["imageUrl"];

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    Console.WriteLine("Side picked: " + teamSide);
                    string tempFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUrl);
                    string tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

                    try
                    {
                        if (File.Exists(imageUrl))
                        {
                            // Local file case
                            SetHeroImage(teamSide == "team1" ? "m1t1Logo" : "m1t2Logo", imageUrl);
                        }
                        else
                        {
                            // Download the image to a local temp folder and set it
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(imageUrl, tempPath);
                            }

                            // Attempt to set the downloaded image
                            SetHeroImage(teamSide == "team1" ? "m1t1Logo" : "m1t2Logo", tempPath);
                        }
                    }
                    catch (IOException ioEx)
                    {
                        Console.WriteLine("Could not set hero image: " + ioEx.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An unexpected error occurred: " + ex.Message);
                    }
                }
                else
                {
                    responseString = "No image URL specified.";
                }
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            // Listen for the next request
            httpListener.BeginGetContext(OnRequestReceived, httpListener);
        }



        private void HandleAction(string action)
        {
            // Nested function to handle UI thread-safe invocation
            void InvokeOnUI(Action uiAction)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate { uiAction(); });
                }
                else
                {
                    uiAction();
                }
            }

            // Switch on the action and call the appropriate method
            switch (action)
            {
                case "swapteams":
                    InvokeOnUI(() => m1SwapButton_Click(this, null));
                    break;

                case "increment_t1":
                    InvokeOnUI(() => IncrementTeamScore("Team1"));
                    break;

                case "increment_t2":
                    InvokeOnUI(() => IncrementTeamScore("Team2"));
                    break;

                case "decrement_t1":
                    InvokeOnUI(() => DecrementTeamScore("Team1"));
                    break;

                case "decrement_t2":
                    InvokeOnUI(() => DecrementTeamScore("Team2"));
                    break;

                case "reset":
                    InvokeOnUI(() => m1ResetButton_Click(this, null));
                    break;

                case "update":
                    InvokeOnUI(() => updateSeries());
                    break;

                default:
                    Console.WriteLine($"Unknown action: {action}");
                    break;
            }
        }


        #endregion


        #region HotKeys
        /// <summary>
        /// Handling Hotkey Details
        /// </summary>
        /// 
        private Control inputControl = null;
        private Dictionary<string, int> hotkeyIds = new Dictionary<string, int>();

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifiers
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        // Hotkey state
        private bool isShiftPressed = false;
        private bool isCtrlPressed = false;
        private bool isAltPressed = false;
        private Keys selectedKey;
        private string currentHotkeyName = string.Empty;

        private void SetHotkey(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if (!isListeningForKey)
                {
                    StartListeningForHotkey(clickedButton.Name);
                    Console.WriteLine($"Listening for hotkey {clickedButton.Name}");
                }
                else
                {
                    StopListening();
                }
            }
        }

        private void StartListeningForHotkey(string btnName)
        {
            if (!isListeningForKey)
            {
                currentHotkeyName = btnName;
                string inputName = btnName.Replace("BTN_", "INPUT_");
                inputControl = this.Controls.Find(inputName, true).FirstOrDefault();

                if (inputControl != null)
                {
                    inputControl.Text = "Press any key combination...";
                }

                isListeningForKey = true;
                this.KeyPreview = true;
                this.KeyDown += Form1_KeyDown;
                this.KeyUp += Form1_KeyUp;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isListeningForKey)
            {
                // Track modifier keys
                isShiftPressed = e.Shift;
                isCtrlPressed = e.Control;
                isAltPressed = e.Alt;

                // Capture the non-modifier key
                if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu && e.KeyCode != Keys.LWin && e.KeyCode != Keys.RWin)
                {
                    selectedKey = e.KeyCode;
                    UpdateHotkeyLabel();
                    StopListening(); // Stop listening after capturing the hotkey
                    RegisterHotkeys(); // Register the hotkey after capturing
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isListeningForKey)
            {
                // Reset modifier keys when released
                isShiftPressed = e.Shift;
                isCtrlPressed = e.Control;
                isAltPressed = e.Alt;

                // Stop listening only when all keys are released
                if (!isShiftPressed && !isCtrlPressed && !isAltPressed)
                {
                    StopListening();
                }
            }
        }

        private void UpdateHotkeyLabel()
        {
            string hotkey = "";
            if (isCtrlPressed) hotkey += "Ctrl + ";
            if (isShiftPressed) hotkey += "Shift + ";
            if (isAltPressed) hotkey += "Alt + ";
            hotkey += selectedKey.ToString();

            inputControl.Text = hotkey;
        }

        private void StopListening()
        {
            isListeningForKey = false;
            this.KeyPreview = false;
            this.KeyDown -= Form1_KeyDown;
            this.KeyUp -= Form1_KeyUp;
        }

        // Hotkey registration and unregistration
        private void RegisterHotkeys()
        {
            uint modifier = 0;
            if (isCtrlPressed) modifier |= MOD_CONTROL;
            if (isShiftPressed) modifier |= MOD_SHIFT;
            if (isAltPressed) modifier |= MOD_ALT;

            // Unregister only the hotkey associated with the current button
            UnregisterHotkeys(currentHotkeyName);

            int hotkeyId = currentHotkeyName.GetHashCode();
            hotkeyIds[currentHotkeyName] = hotkeyId;

            RegisterHotKey(this.Handle, hotkeyId, modifier, (uint)selectedKey);
            Console.WriteLine($"Registered hotkey for {currentHotkeyName} with ID {hotkeyId}");
        }


        private void UnregisterHotkeys(string hotkeyName)
        {
            if (hotkeyIds.ContainsKey(hotkeyName))
            {
                int hotkeyId = hotkeyIds[hotkeyName];
                UnregisterHotKey(this.Handle, hotkeyId);
                hotkeyIds.Remove(hotkeyName);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int pressedHotkeyId = m.WParam.ToInt32();
                string hotkeyName = hotkeyIds.FirstOrDefault(x => x.Value == pressedHotkeyId).Key;
                Console.WriteLine($"Hotkey pressed: {hotkeyName}");

                if (!string.IsNullOrEmpty(hotkeyName))
                {
                    Console.WriteLine($"Hotkey pressed: {hotkeyName}");
                    PerformHotkeyAction(hotkeyName);
                }
            }
            base.WndProc(ref m);
        }

        private void PerformHotkeyAction(string hotkeyName)
        {
            switch (hotkeyName)
            {
                case "BTN_SET_HK_SWAPTEAMS":
                    m1SwapButton_Click(hotkeyName, null);
                    break;

                case "BTN_SET_HK_RESET":
                    m1ResetButton_Click(hotkeyName, null);
                    break;

                case "BTN_SET_HK_INCREMENT_T1":
                    IncrementTeamScore("Team1");
                    break;

                case "BTN_SET_HK_INCREMENT_T2":
                    IncrementTeamScore("Team2");
                    break;

                case "BTN_SET_HK_DECREMENT_T1":
                    DecrementTeamScore("Team1");
                    break;

                case "BTN_SET_HK_DECREMENT_T2":
                    DecrementTeamScore("Team2");
                    break;



                // Add more cases for other hotkeys
                default:
                    Console.WriteLine($"No action defined for hotkey: {hotkeyName}");
                    break;
            }
        }

        private void IncrementTeamScore(string teamName)
        {
            if (teamName == "team1")
            {
                NumericUpDown scoreControl = this.Controls.Find("m1t1Score", true).FirstOrDefault() as NumericUpDown;
                if (scoreControl != null)
                {
                    scoreControl.Value += 1;
                }
            }
            else if (teamName == "team2")
            {
                NumericUpDown scoreControl = this.Controls.Find("m1t2Score", true).FirstOrDefault() as NumericUpDown;
                if (scoreControl != null)
                {
                    scoreControl.Value += 1;
                }
            }

        }

        private void DecrementTeamScore(string teamName)
        {
           if (teamName == "team1")
            {
                NumericUpDown scoreControl = this.Controls.Find("m1t1Score", true).FirstOrDefault() as NumericUpDown;
                if (scoreControl != null)
                {
                    if (scoreControl.Value > 0)
                    {
                        scoreControl.Value -= 1;
                    }
                }
            }
            else if (teamName == "team2")
            {
                NumericUpDown scoreControl = this.Controls.Find("m1t2Score", true).FirstOrDefault() as NumericUpDown;
                if (scoreControl != null)
                {
                    if (scoreControl.Value > 0)
                    {
                        scoreControl.Value -= 1;
                    }
                }
            }
        }

        private void SwapTeams()
        {
            MessageBox.Show("Teams swapped!");
        }

        #endregion






        #region Button Actions
        /// <summary>
        /// Button to swap team information
        /// </summary>
        private void m1SwapButton_Click(object sender, EventArgs e)
        {

            String temp;
            temp = m1t1Name.Text;
            m1t1Name.Text = m1t2Name.Text;
            m1t2Name.Text = temp;
            temp = "";
            m1t1Name.Modified = true;
            m1t2Name.Modified = true;

            decimal temporary;
            temporary = m1t1Score.Value;
            m1t1Score.Value = m1t2Score.Value;
            m1t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m1t1Score.Value;
            m1m1t1Score.Value = m1m1t2Score.Value;
            m1m1t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m2t1Score.Value;
            m1m2t1Score.Value = m1m2t2Score.Value;
            m1m2t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m3t1Score.Value;
            m1m3t1Score.Value = m1m3t2Score.Value;
            m1m3t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m4t1Score.Value;
            m1m4t1Score.Value = m1m4t2Score.Value;
            m1m4t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m5t1Score.Value;
            m1m5t1Score.Value = m1m5t2Score.Value;
            m1m5t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m6t1Score.Value;
            m1m6t1Score.Value = m1m6t2Score.Value;
            m1m6t2Score.Value = temporary;
            temporary = 0;

            temporary = m1m7t1Score.Value;
            m1m7t1Score.Value = m1m7t2Score.Value;
            m1m7t2Score.Value = temporary;
            temporary = 0;

            temp = m1t1SR.Text;
            m1t1SR.Text = m1t2SR.Text;
            m1t2SR.Text = temp;
            temp = "";
            m1t1SR.Modified = true;
            m1t2SR.Modified = true;

            temp = m1t1LogoButton.AccessibleDescription;
            m1t1LogoButton.AccessibleDescription = m1t2LogoButton.AccessibleDescription;
            m1t2LogoButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p1Name.Text;
            m1t1p1Name.Text = m1t2p1Name.Text;
            m1t2p1Name.Text = temp;
            temp = "";
            m1t1p1Name.Modified = true;
            m1t2p1Name.Modified = true;

            temp = m1t1p2Name.Text;
            m1t1p2Name.Text = m1t2p2Name.Text;
            m1t2p2Name.Text = temp;
            temp = "";
            m1t1p2Name.Modified = true;
            m1t2p2Name.Modified = true;

            temp = m1t1p3Name.Text;
            m1t1p3Name.Text = m1t2p3Name.Text;
            m1t2p3Name.Text = temp;
            temp = "";
            m1t1p3Name.Modified = true;
            m1t2p3Name.Modified = true;

            temp = m1t1p4Name.Text;
            m1t1p4Name.Text = m1t2p4Name.Text;
            m1t2p4Name.Text = temp;
            temp = "";
            m1t1p4Name.Modified = true;
            m1t2p4Name.Modified = true;

            temp = m1t1p5Name.Text;
            m1t1p5Name.Text = m1t2p5Name.Text;
            m1t2p5Name.Text = temp;
            temp = "";
            m1t1p5Name.Modified = true;
            m1t2p5Name.Modified = true;

            temp = m1t1p6Name.Text;
            m1t1p6Name.Text = m1t2p6Name.Text;
            m1t2p6Name.Text = temp;
            temp = "";
            m1t2p6Name.Modified = true;
            m1t1p6Name.Modified = true;

            int temps = 0;
            temps = m1t1p1Hero.SelectedIndex;
            m1t1p1Hero.SelectedIndex = m1t2p1Hero.SelectedIndex;
            m1t2p1Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p2Hero.SelectedIndex;
            m1t1p2Hero.SelectedIndex = m1t2p2Hero.SelectedIndex;
            m1t2p2Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p3Hero.SelectedIndex;
            m1t1p3Hero.SelectedIndex = m1t2p3Hero.SelectedIndex;
            m1t2p3Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p4Hero.SelectedIndex;
            m1t1p4Hero.SelectedIndex = m1t2p4Hero.SelectedIndex;
            m1t2p4Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p5Hero.SelectedIndex;
            m1t1p5Hero.SelectedIndex = m1t2p5Hero.SelectedIndex;
            m1t2p5Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p6Hero.SelectedIndex;
            m1t1p6Hero.SelectedIndex = m1t2p6Hero.SelectedIndex;
            m1t2p6Hero.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p1Role.SelectedIndex;
            m1t1p1Role.SelectedIndex = m1t2p1Role.SelectedIndex;
            m1t2p1Role.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p2Role.SelectedIndex;
            m1t1p2Role.SelectedIndex = m1t2p2Role.SelectedIndex;
            m1t2p2Role.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p3Role.SelectedIndex;
            m1t1p3Role.SelectedIndex = m1t2p3Role.SelectedIndex;
            m1t2p3Role.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p4Role.SelectedIndex;
            m1t1p4Role.SelectedIndex = m1t2p4Role.SelectedIndex;
            m1t2p4Role.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p5Role.SelectedIndex;
            m1t1p5Role.SelectedIndex = m1t2p5Role.SelectedIndex;
            m1t2p5Role.SelectedIndex = temps;
            temps = 0;

            temps = m1t1p6Role.SelectedIndex;
            m1t1p6Role.SelectedIndex = m1t2p6Role.SelectedIndex;
            m1t2p6Role.SelectedIndex = temps;
            temps = 0;

            temp = m1t1p1Info.Text;
            m1t1p1Info.Text = m1t2p1Info.Text;
            m1t2p1Info.Text = temp;
            temp = "";
            m1t1p1Info.Modified = true;
            m1t2p1Info.Modified = true;

            temp = m1t1p2Info.Text;
            m1t1p2Info.Text = m1t2p2Info.Text;
            m1t2p2Info.Text = temp;
            temp = "";
            m1t1p2Info.Modified = true;
            m1t2p2Info.Modified = true;

            temp = m1t1p3Info.Text;
            m1t1p3Info.Text = m1t2p3Info.Text;
            m1t2p3Info.Text = temp;
            temp = "";
            m1t1p3Info.Modified = true;
            m1t2p3Info.Modified = true;

            temp = m1t1p4Info.Text;
            m1t1p4Info.Text = m1t2p4Info.Text;
            m1t2p4Info.Text = temp;
            temp = "";
            m1t1p4Info.Modified = true;
            m1t2p4Info.Modified = true;

            temp = m1t1p5Info.Text;
            m1t1p5Info.Text = m1t2p5Info.Text;
            m1t2p5Info.Text = temp;
            temp = "";
            m1t1p5Info.Modified = true;
            m1t2p5Info.Modified = true;
            
            temp = m1t1p6Info.Text;
            m1t1p6Info.Text = m1t2p6Info.Text;
            m1t2p6Info.Text = temp;
            temp = "";
            m1t1p6Info.Modified = true;
            m1t2p6Info.Modified = true;

            temp = m1t1p1ImageButton.AccessibleDescription;
            m1t1p1ImageButton.AccessibleDescription = m1t2p1ImageButton.AccessibleDescription;
            m1t2p1ImageButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p2ImageButton.AccessibleDescription;
            m1t1p2ImageButton.AccessibleDescription = m1t2p2ImageButton.AccessibleDescription;
            m1t2p2ImageButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p3ImageButton.AccessibleDescription;
            m1t1p3ImageButton.AccessibleDescription = m1t2p3ImageButton.AccessibleDescription;
            m1t2p3ImageButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p4ImageButton.AccessibleDescription;
            m1t1p4ImageButton.AccessibleDescription = m1t2p4ImageButton.AccessibleDescription;
            m1t2p4ImageButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p5ImageButton.AccessibleDescription;
            m1t1p5ImageButton.AccessibleDescription = m1t2p5ImageButton.AccessibleDescription;
            m1t2p5ImageButton.AccessibleDescription = temp;
            temp = "";

            temp = m1t1p6ImageButton.AccessibleDescription;
            m1t1p6ImageButton.AccessibleDescription = m1t2p6ImageButton.AccessibleDescription;
            m1t2p6ImageButton.AccessibleDescription = temp;
            temp = "";

            Color tempColor = new Color();
            tempColor = m1t1ColorButton.BackColor;
            m1t1ColorButton.BackColor = m1t2ColorButton.BackColor;
            m1t2ColorButton.BackColor = tempColor;
            tempColor = Color.Transparent;

            updateReplayPage();

        }

        /// <summary>
        /// Button to reset series information
        /// </summary>
        private void m1ResetButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all match data?", "Reset Match Data?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                resetText(m1MutualInfo);

                resetScore(m1t1Score);
                resetText(m1t1Name);
                resetText(m1t1SR);
                resetText(m1t1p1Name);
                resetText(m1t1p2Name);
                resetText(m1t1p3Name);
                resetText(m1t1p4Name);
                resetText(m1t1p5Name);
                resetText(m1t1p6Name);

                resetScore(m1t2Score);
                resetText(m1t2Name);
                resetText(m1t2SR);
                resetText(m1t2p1Name);
                resetText(m1t2p2Name);
                resetText(m1t2p3Name);
                resetText(m1t2p4Name);
                resetText(m1t2p5Name);
                resetText(m1t2p6Name);

                resetCombo(m1t1p1Hero);
                resetCombo(m1t1p2Hero);
                resetCombo(m1t1p3Hero);
                resetCombo(m1t1p4Hero);
                resetCombo(m1t1p5Hero);
                resetCombo(m1t1p6Hero);

                resetCombo(m1t2p1Hero);
                resetCombo(m1t2p2Hero);
                resetCombo(m1t2p3Hero);
                resetCombo(m1t2p4Hero);
                resetCombo(m1t2p5Hero);
                resetCombo(m1t2p6Hero);

                resetCombo(m1t1p1Role);
                resetCombo(m1t1p2Role);
                resetCombo(m1t1p3Role);
                resetCombo(m1t1p4Role);
                resetCombo(m1t1p5Role);
                resetCombo(m1t1p6Role);

                resetCombo(m1t2p1Role);
                resetCombo(m1t2p2Role);
                resetCombo(m1t2p3Role);
                resetCombo(m1t2p4Role);
                resetCombo(m1t2p5Role);
                resetCombo(m1t2p6Role);

                resetText(m1t1p1Info);
                resetText(m1t1p2Info);
                resetText(m1t1p3Info);
                resetText(m1t1p4Info);
                resetText(m1t1p5Info);
                resetText(m1t1p6Info);

                resetText(m1t2p1Info);
                resetText(m1t2p2Info);
                resetText(m1t2p3Info);
                resetText(m1t2p4Info);
                resetText(m1t2p5Info);
                resetText(m1t2p6Info);

                resetButton(m1t1p1ImageButton);
                resetButton(m1t1p2ImageButton);
                resetButton(m1t1p3ImageButton);
                resetButton(m1t1p4ImageButton);
                resetButton(m1t1p5ImageButton);
                resetButton(m1t1p6ImageButton);

                resetButton(m1t2p1ImageButton);
                resetButton(m1t2p2ImageButton);
                resetButton(m1t2p3ImageButton);
                resetButton(m1t2p4ImageButton);
                resetButton(m1t2p5ImageButton);
                resetButton(m1t2p6ImageButton);

                resetSide(m1Neutral);

                resetCombo(m1m1Map);
                resetCombo(m1m2Map);
                resetCombo(m1m3Map);
                resetCombo(m1m4Map);
                resetCombo(m1m5Map);
                resetCombo(m1m6Map);
                resetCombo(m1m7Map);

                resetScore(m1m1t1Score);
                resetScore(m1m1t2Score);
                resetScore(m1m2t1Score);
                resetScore(m1m2t2Score);
                resetScore(m1m3t1Score);
                resetScore(m1m3t2Score);
                resetScore(m1m4t1Score);
                resetScore(m1m4t2Score);
                resetScore(m1m5t1Score);
                resetScore(m1m5t2Score);
                resetScore(m1m6t1Score);
                resetScore(m1m6t2Score);
                resetScore(m1m7t1Score);
                resetScore(m1m7t2Score);

                resetButton(m1t1LogoButton);
                resetButton(m1t2LogoButton);

                resetColor(colorThemeDialog, m1t1ColorButton);
                resetColor(colorThemeDialog, m1t2ColorButton);
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        /// <summary>
        /// Button to update series information
        /// </summary>
        private void m1UpdateButton_Click(object sender, EventArgs e)
        {
            updateText(m1MutualInfo, "Match1", "DivisionNumber");

            updateScore(m1t1Score, "Match1", "t1Score");
            updateText(m1t1Name, "Match1", "t1Name");
            updateText(m1t1SR, "Match1", "t1SR");
            updateText(m1t1p1Name, "Match1", "t1p1Name");
            updateText(m1t1p2Name, "Match1", "t1p2Name");
            updateText(m1t1p3Name, "Match1", "t1p3Name");
            updateText(m1t1p4Name, "Match1", "t1p4Name");
            updateText(m1t1p5Name, "Match1", "t1p5Name");
            updateText(m1t1p6Name, "Match1", "t1p6Name");
            updateColor(m1t1ColorButton, "Match1", "t1Color");

            updateScore(m1t2Score, "Match1", "t2Score");
            updateText(m1t2Name, "Match1", "t2Name");
            updateText(m1t2SR, "Match1", "t2SR");
            updateText(m1t2p1Name, "Match1", "t2p1Name");
            updateText(m1t2p2Name, "Match1", "t2p2Name");
            updateText(m1t2p3Name, "Match1", "t2p3Name");
            updateText(m1t2p4Name, "Match1", "t2p4Name");
            updateText(m1t2p5Name, "Match1", "t2p5Name");
            updateText(m1t2p6Name, "Match1", "t2p6Name");
            updateColor(m1t2ColorButton, "Match1", "t2Color");

            updateHero(m1t1p1Hero, "Match1", "t1p1Hero");
            updateHero(m1t1p2Hero, "Match1", "t1p2Hero");
            updateHero(m1t1p3Hero, "Match1", "t1p3Hero");
            updateHero(m1t1p4Hero, "Match1", "t1p4Hero");
            updateHero(m1t1p5Hero, "Match1", "t1p5Hero");
            updateHero(m1t1p6Hero, "Match1", "t1p6Hero");

            updateHero(m1t2p1Hero, "Match1", "t2p1Hero");
            updateHero(m1t2p2Hero, "Match1", "t2p2Hero");
            updateHero(m1t2p3Hero, "Match1", "t2p3Hero");
            updateHero(m1t2p4Hero, "Match1", "t2p4Hero");
            updateHero(m1t2p5Hero, "Match1", "t2p5Hero");
            updateHero(m1t2p6Hero, "Match1", "t2p6Hero");

            updateRole(m1t1p1Role, "Match1", "t1p1Role");
            updateRole(m1t1p2Role, "Match1", "t1p2Role");
            updateRole(m1t1p3Role, "Match1", "t1p3Role");
            updateRole(m1t1p4Role, "Match1", "t1p4Role");
            updateRole(m1t1p5Role, "Match1", "t1p5Role");
            updateRole(m1t1p6Role, "Match1", "t1p6Role");

            updateRole(m1t2p1Role, "Match1", "t2p1Role");
            updateRole(m1t2p2Role, "Match1", "t2p2Role");
            updateRole(m1t2p3Role, "Match1", "t2p3Role");
            updateRole(m1t2p4Role, "Match1", "t2p4Role");
            updateRole(m1t2p5Role, "Match1", "t2p5Role");
            updateRole(m1t2p6Role, "Match1", "t2p6Role");

            updateText(m1t1p1Info, "Match1", "t1p1Info");
            updateText(m1t1p2Info, "Match1", "t1p2Info");
            updateText(m1t1p3Info, "Match1", "t1p3Info");
            updateText(m1t1p4Info, "Match1", "t1p4Info");
            updateText(m1t1p5Info, "Match1", "t1p5Info");
            updateText(m1t1p6Info, "Match1", "t1p6Info");

            updateText(m1t2p1Info, "Match1", "t2p1Info");
            updateText(m1t2p2Info, "Match1", "t2p2Info");
            updateText(m1t2p3Info, "Match1", "t2p3Info");
            updateText(m1t2p4Info, "Match1", "t2p4Info");
            updateText(m1t2p5Info, "Match1", "t2p5Info");
            updateText(m1t2p6Info, "Match1", "t2p6Info");

            updateButton(m1t1p1ImageButton, "Match1", "t1p1Image");
            updateButton(m1t1p2ImageButton, "Match1", "t1p2Image");
            updateButton(m1t1p3ImageButton, "Match1", "t1p3Image");
            updateButton(m1t1p4ImageButton, "Match1", "t1p4Image");
            updateButton(m1t1p5ImageButton, "Match1", "t1p5Image");
            updateButton(m1t1p6ImageButton, "Match1", "t1p6Image");

            updateButton(m1t2p1ImageButton, "Match1", "t2p1Image");
            updateButton(m1t2p2ImageButton, "Match1", "t2p2Image");
            updateButton(m1t2p3ImageButton, "Match1", "t2p3Image");
            updateButton(m1t2p4ImageButton, "Match1", "t2p4Image");
            updateButton(m1t2p5ImageButton, "Match1", "t2p5Image");
            updateButton(m1t2p6ImageButton, "Match1", "t2p6Image");

            updateSide(m1Attack, "Match1", "t1Side", "t2Side");
            updateSide(m1Defend, "Match1", "t1Side", "t2Side");
            updateSide(m1Neutral, "Match1", "t1Side", "t2Side");

            updateMap(m1m1Map, m1m1Selected, "Match1", "m1Map");
            updateMap(m1m2Map, m1m2Selected, "Match1", "m2Map");
            updateMap(m1m3Map, m1m3Selected, "Match1", "m3Map");
            updateMap(m1m4Map, m1m4Selected, "Match1", "m4Map");
            updateMap(m1m5Map, m1m5Selected, "Match1", "m5Map");
            updateMap(m1m6Map, m1m6Selected, "Match1", "m6Map");
            updateMap(m1m7Map, m1m7Selected, "Match1", "m7Map");

            updateScore(m1m1t1Score, "Match1", "m1t1Score");
            updateScore(m1m1t2Score, "Match1", "m1t2Score");
            updateScore(m1m2t1Score, "Match1", "m2t1Score");
            updateScore(m1m2t2Score, "Match1", "m2t2Score");
            updateScore(m1m3t1Score, "Match1", "m3t1Score");
            updateScore(m1m3t2Score, "Match1", "m3t2Score");
            updateScore(m1m4t1Score, "Match1", "m4t1Score");
            updateScore(m1m4t2Score, "Match1", "m4t2Score");
            updateScore(m1m5t1Score, "Match1", "m5t1Score");
            updateScore(m1m5t2Score, "Match1", "m5t2Score");
            updateScore(m1m6t1Score, "Match1", "m6t1Score");
            updateScore(m1m6t2Score, "Match1", "m6t2Score");
            updateScore(m1m7t1Score, "Match1", "m7t1Score");
            updateScore(m1m7t2Score, "Match1", "m7t2Score");

            updateCompleted(m1m1Completed, m1t1Name, m1t2Name, m1m1t1Score, m1m1t2Score, "Match1", "m1MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m1Map);
            updateCompleted(m1m2Completed, m1t1Name, m1t2Name, m1m2t1Score, m1m2t2Score, "Match1", "m2MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m2Map);
            updateCompleted(m1m3Completed, m1t1Name, m1t2Name, m1m3t1Score, m1m3t2Score, "Match1", "m3MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m3Map);
            updateCompleted(m1m4Completed, m1t1Name, m1t2Name, m1m4t1Score, m1m4t2Score, "Match1", "m4MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m4Map);
            updateCompleted(m1m5Completed, m1t1Name, m1t2Name, m1m5t1Score, m1m5t2Score, "Match1", "m5MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m5Map);
            updateCompleted(m1m6Completed, m1t1Name, m1t2Name, m1m6t1Score, m1m6t2Score, "Match1", "m6MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m6Map);
            updateCompleted(m1m7Completed, m1t1Name, m1t2Name, m1m7t1Score, m1m7t2Score, "Match1", "m7MapWin", m1t1LogoButton.AccessibleDescription, m1t2LogoButton.AccessibleDescription, m1m7Map);

            updateButton(m1t1LogoButton, "Match1", "t1Logo");
            updateButton(m1t2LogoButton, "Match1", "t2Logo");

            updateSeries();
            updateReplayPage();

            //Match1.printAll();
        }

        /// <summary>
        /// Button to reset general information
        /// </summary>
        private void generalResetButton_Click(object sender, EventArgs e)
        { DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all general data?", "Reset General Data?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                resetText(message);
                resetText(host);
                resetText(analyst1);
                resetText(analyst2);
                resetText(caster1);
                resetText(caster2);
                resetText(utility1);
                resetText(utility2);
                resetText(utility3);
                resetText(utility4);
                resetText(utility5);
                resetText(utility6);
                resetText(utility7);
                resetText(utility8);
                resetText(utility9);
                resetText(utility10);
                resetText(utility11);
                resetText(utility12);
                resetText(utility13);
                resetText(utility14);
                resetText(utility15);
                resetText(utility16);
                resetText(utility17);
                resetText(utility18);
                resetText(utility19);
                resetText(utility20);
                resetText(utility21);
                resetText(utility22);
                resetColor(colorThemeDialog, colorThemeButton1);
                resetColor(colorThemeDialog, colorThemeButton2);
                resetColor(colorThemeDialog, colorThemeButton3);
                resetColor(colorThemeDialog, colorThemeButton4);
                resetColor(colorThemeDialog, colorThemeButton5);
                resetColor(colorThemeDialog, colorThemeButton6);
                resetColor(colorThemeDialog, colorThemeButton7);
                resetColor(colorThemeDialog, colorThemeButton8);
                resetButton(imageButton1);
                resetButton(imageButton2);
                resetButton(imageButton3);
                resetButton(imageButton4);
                resetButton(imageButton5);
                resetButton(imageButton6);
                resetButton(imageButton7);
                resetButton(imageButton8);
                resetButton(imageButton9);
                resetButton(imageButton10);
                resetButton(imageButton11);
                resetButton(imageButton12);
                resetButton(imageButton13);
                resetButton(imageButton14);
                resetButton(imageButton15);
                resetButton(imageButton16);
                resetButton(imageButton17);
                resetButton(imageButton18);
                resetButton(imageButton19);
                resetButton(imageButton20);
                resetButton(imageButton21);
                resetButton(imageButton22);
                resetButton(imageButton23);
                resetButton(imageButton24);
                resetButton(imageButton25);
                resetButton(imageButton26);
                resetButton(imageButton27);
                resetButton(imageButton28);
            }
            else if(dialogResult == DialogResult.No)
            {
                //do something else
            }

        }

        /// <summary>
        /// Button to update general information
        /// </summary>
        private void generalUpdateButton_Click(object sender, EventArgs e)
        {
            updateText(message, "General", "message");
            updateText(host, "General", "host");
            updateText(analyst1, "General", "analyst1");
            updateText(analyst2, "General", "analyst2");
            updateText(caster1, "General", "caster1");
            updateText(caster2, "General", "caster2");
            updateText(utility1, "General", "utility1");
            updateText(utility2, "General", "utility2");
            updateText(utility3, "General", "utility3");
            updateText(utility4, "General", "utility4");
            updateText(utility5, "General", "utility5");
            updateText(utility6, "General", "utility6");
            updateText(utility7, "General", "utility7");
            updateText(utility8, "General", "utility8");
            updateText(utility9, "General", "utility9");
            updateText(utility10, "General", "utility10");
            updateText(utility11, "General", "utility11");
            updateText(utility12, "General", "utility12");
            updateText(utility13, "General", "utility13");
            updateText(utility14, "General", "utility14");
            updateText(utility15, "General", "utility15");
            updateText(utility16, "General", "utility16");
            updateText(utility17, "General", "utility17");
            updateText(utility18, "General", "utility18");
            updateText(utility19, "General", "utility19");
            updateText(utility20, "General", "utility20");
            updateText(utility21, "General", "utility21");
            updateText(utility22, "General", "utility22");

            updateButton(imageButton1, "General", GeneralFiles[59].Replace(".png", ""));
            updateButton(imageButton2, "General", GeneralFiles[60].Replace(".png", ""));
            updateButton(imageButton3, "General", GeneralFiles[61].Replace(".png", ""));
            updateButton(imageButton4, "General", GeneralFiles[62].Replace(".png", ""));
            updateButton(imageButton5, "General", GeneralFiles[63].Replace(".png", ""));
            updateButton(imageButton6, "General", GeneralFiles[64].Replace(".png", ""));
            updateButton(imageButton7, "General", GeneralFiles[65].Replace(".png", ""));
            updateButton(imageButton8, "General", GeneralFiles[66].Replace(".png", ""));
            updateButton(imageButton9, "General", GeneralFiles[67].Replace(".png", ""));
            updateButton(imageButton10, "General", GeneralFiles[68].Replace(".png", ""));
            updateButton(imageButton11, "General", GeneralFiles[69].Replace(".png", ""));
            updateButton(imageButton12, "General", GeneralFiles[70].Replace(".png", ""));
            updateButton(imageButton13, "General", GeneralFiles[71].Replace(".png", ""));
            updateButton(imageButton14, "General", GeneralFiles[72].Replace(".png", ""));
            updateButton(imageButton15, "General", GeneralFiles[73].Replace(".png", ""));
            updateButton(imageButton16, "General", GeneralFiles[74].Replace(".png", ""));
            updateButton(imageButton17, "General", GeneralFiles[75].Replace(".png", ""));
            updateButton(imageButton18, "General", GeneralFiles[76].Replace(".png", ""));
            updateButton(imageButton19, "General", GeneralFiles[77].Replace(".png", ""));
            updateButton(imageButton20, "General", GeneralFiles[78].Replace(".png", ""));
            updateButton(imageButton21, "General", GeneralFiles[79].Replace(".png", ""));
            updateButton(imageButton22, "General", GeneralFiles[80].Replace(".png", ""));
            updateButton(imageButton23, "General", GeneralFiles[81].Replace(".png", ""));
            updateButton(imageButton24, "General", GeneralFiles[82].Replace(".png", ""));
            updateButton(imageButton25, "General", GeneralFiles[83].Replace(".png", ""));
            updateButton(imageButton26, "General", GeneralFiles[84].Replace(".png", ""));
            updateButton(imageButton27, "General", GeneralFiles[85].Replace(".png", ""));
            updateButton(imageButton28, "General", GeneralFiles[86].Replace(".png", ""));


            updateColor(colorThemeButton1, "General", "colorTheme1");
            updateColor(colorThemeButton2, "General", "colorTheme2");
            updateColor(colorThemeButton3, "General", "colorTheme3");
            updateColor(colorThemeButton4, "General", "colorTheme4");
            updateColor(colorThemeButton5, "General", "colorTheme5");
            updateColor(colorThemeButton6, "General", "colorTheme6");
            updateColor(colorThemeButton7, "General", "colorTheme7");
            updateColor(colorThemeButton8, "General", "colorTheme8");
        }

        /// <summary>
        /// Button to reset the player of the match information
        /// </summary>
        private void playerReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all player data?", "Reset Player Data?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                resetText(playerBox);
                resetText(playerSRBox);
                resetText(playerTeamBox);
                resetText(playerTeamInfoBox);

                resetCombo(playerHeroBox1);
                resetCombo(playerHeroBox2);
                resetCombo(playerHeroBox3);
                resetCombo(playerRoleBox);

                resetButton(playerImageButton);
                resetButton(playerTeamLogoButton);
            }
            else if(dialogResult == DialogResult.No)
            {
                //do nothing
            }
        }

        /// <summary>
        /// Button to update the player of the match information
        /// </summary>
        private void playerUpdate_Click(object sender, EventArgs e)
        {
            updateText(playerBox, "Player", "player");
            updateText(playerSRBox, "Player", "sr");
            updateText(playerTeamBox, "Player", "team");
            updateText(playerTeamInfoBox, "Player", "info");

            updateHero(playerHeroBox1, "Player", "hero1");
            updateHero(playerHeroBox2, "Player", "hero2");
            updateHero(playerHeroBox3, "Player", "hero3");

            updateRole(playerRoleBox, "Player", "role");

            updateButton(playerImageButton, "Player", GeneralFiles[8].Replace(".png", ""));
            updateButton(playerTeamLogoButton, "Player", GeneralFiles[6].Replace(".png", ""));
        }

        /// <summary>
        /// Button to reset the replay playlist folder
        /// </summary>
        private void replayReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all replays?", "Reset Replay Data?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path + FolderList[5]);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                updateList();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do nothing
            }
        }

        /// <summary>
        /// Button to clean dirty files from the replay playlist folder
        /// </summary>
        private void replayClean_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path + FolderList[5]);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Length < 1000000 || file.Length == 0)
                {
                    file.Delete();
                }
            }
        }

        /// <summary>
        /// Copies replay details to clipboard for the selected replay
        /// </summary>
        private void replayCopyDetailsButton_Click(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                Replay selctedReplay = Replays[replayListView.SelectedItems[0].Index];
                string team = "";
                string map = "";
                string description = "";
                if (selctedReplay.Description != null)
                {
                    description = selctedReplay.Description;
                }
                if (selctedReplay.Team != null)
                {
                    team = selctedReplay.Team.Name;
                }
                if (selctedReplay.Map != null)
                {
                    map = selctedReplay.Map.Name;
                }
                string players = "";
                foreach (Player player in selctedReplay.Players)
                {
                    players += player.Name + " ";
                }

                MessageBox.Show("Team: " + team +
                "\nPlayers: " + players +
                "\nMap: " + map +
                "\nDescription: " + description
                    );

                Clipboard.SetText("Team: " + team +
                    "\nPlayers: " + players +
                    "\nMap: " + map +
                    "\nDescription: " + description
                    );
            }
        }

        /// <summary>
        /// Saves the file path to a text file for the selected replay.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaySetButton_Click(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                using (StreamWriter sw = File.CreateText(path + FolderList[4] + "\\" + ReplayFiles[2]))
                {
                    sw.WriteLine(Replays[replayListView.SelectedItems[0].Index].Name);
                }
            }
        }

        /// <summary>
        /// Save's the selected replay to the Desk Replay file
        /// </summary>
        private void replaySaveButton_Click(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                File.Copy(path + FolderList[5] + "\\" + Replays[replayListView.SelectedItems[0].Index].Name, path + FolderList[4] + "\\" + ReplayFiles[1], true);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replayClearButton_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens color dialog to assign a color that that button
        /// </summary>
        private void colorThemeButton_Click(object sender, EventArgs e)
        {
            if (colorThemeDialog.ShowDialog(this) == DialogResult.OK)
            {
                ((Button)sender).BackColor = colorThemeDialog.Color;
            }
        }

        /// <summary>
        /// Resets the files related to the teams in the bracket
        /// </summary>
        private void bracketTeamsReset_Click(object sender, EventArgs e)
        {
            resetText(bracketTeam1);
            resetText(bracketTeam2);
            resetText(bracketTeam3);
            resetText(bracketTeam4);
            resetText(bracketTeam5);
            resetText(bracketTeam6);
            resetText(bracketTeam7);
            resetText(bracketTeam8);
            resetText(bracketTeam9);
            resetText(bracketTeam10);
            resetText(bracketTeam11);
            resetText(bracketTeam12);
            resetText(bracketTeam13);
            resetText(bracketTeam14);
            resetText(bracketTeam15);
            resetText(bracketTeam16);

            resetButton(bracketImage1);
            resetButton(bracketImage2);
            resetButton(bracketImage3);
            resetButton(bracketImage4);
            resetButton(bracketImage5);
            resetButton(bracketImage6);
            resetButton(bracketImage7);
            resetButton(bracketImage8);
            resetButton(bracketImage9);
            resetButton(bracketImage10);
            resetButton(bracketImage11);
            resetButton(bracketImage12);
            resetButton(bracketImage13);
            resetButton(bracketImage14);
            resetButton(bracketImage15);
            resetButton(bracketImage16);

            resetScore(bracketTeam1W);
            resetScore(bracketTeam1L);
            resetScore(bracketTeam1MW);
            resetScore(bracketTeam1ML);
            resetScore(bracketTeam2W);
            resetScore(bracketTeam2L);
            resetScore(bracketTeam2MW);
            resetScore(bracketTeam2ML);
            resetScore(bracketTeam3W);
            resetScore(bracketTeam3L);
            resetScore(bracketTeam3MW);
            resetScore(bracketTeam3ML);
            resetScore(bracketTeam4W);
            resetScore(bracketTeam4L);
            resetScore(bracketTeam4MW);
            resetScore(bracketTeam4ML);
            resetScore(bracketTeam5W);
            resetScore(bracketTeam5L);
            resetScore(bracketTeam5MW);
            resetScore(bracketTeam5ML);
            resetScore(bracketTeam6W);
            resetScore(bracketTeam6L);
            resetScore(bracketTeam6MW);
            resetScore(bracketTeam6ML);
            resetScore(bracketTeam7W);
            resetScore(bracketTeam7L);
            resetScore(bracketTeam7MW);
            resetScore(bracketTeam7ML);
            resetScore(bracketTeam8W);
            resetScore(bracketTeam8L);
            resetScore(bracketTeam8MW);
            resetScore(bracketTeam8ML);
            resetScore(bracketTeam9W);
            resetScore(bracketTeam9L);
            resetScore(bracketTeam9MW);
            resetScore(bracketTeam9ML);
            resetScore(bracketTeam10W);
            resetScore(bracketTeam10L);
            resetScore(bracketTeam10MW);
            resetScore(bracketTeam10ML);
            resetScore(bracketTeam11W);
            resetScore(bracketTeam11L);
            resetScore(bracketTeam11MW);
            resetScore(bracketTeam11ML);
            resetScore(bracketTeam12W);
            resetScore(bracketTeam12L);
            resetScore(bracketTeam12MW);
            resetScore(bracketTeam12ML);
            resetScore(bracketTeam13W);
            resetScore(bracketTeam13L);
            resetScore(bracketTeam13MW);
            resetScore(bracketTeam13ML);
            resetScore(bracketTeam14W);
            resetScore(bracketTeam14L);
            resetScore(bracketTeam14MW);
            resetScore(bracketTeam14ML);
            resetScore(bracketTeam15W);
            resetScore(bracketTeam15L);
            resetScore(bracketTeam15MW);
            resetScore(bracketTeam15ML);
            resetScore(bracketTeam16W);
            resetScore(bracketTeam16L);
            resetScore(bracketTeam16MW);
            resetScore(bracketTeam16ML);

            BracketTeams.Clear();
            BracketTeams.Add(new Team());

        }

        /// <summary>
        /// Updates the file system with the list of teams in the bracket: name, logo, Win/Loss, and Map Win/Loss
        /// </summary>
        private void bracketTeamsUpdate_Click(object sender, EventArgs e)
        {

            updateText(bracketTeam1, FolderList[9].Replace("\\", ""), BracketFiles[0].Replace(".txt", ""));
            updateText(bracketTeam2, FolderList[9].Replace("\\", ""), BracketFiles[1].Replace(".txt", ""));
            updateText(bracketTeam3, FolderList[9].Replace("\\", ""), BracketFiles[2].Replace(".txt", ""));
            updateText(bracketTeam4, FolderList[9].Replace("\\", ""), BracketFiles[3].Replace(".txt", ""));
            updateText(bracketTeam5, FolderList[9].Replace("\\", ""), BracketFiles[4].Replace(".txt", ""));
            updateText(bracketTeam6, FolderList[9].Replace("\\", ""), BracketFiles[5].Replace(".txt", ""));
            updateText(bracketTeam7, FolderList[9].Replace("\\", ""), BracketFiles[6].Replace(".txt", ""));
            updateText(bracketTeam8, FolderList[9].Replace("\\", ""), BracketFiles[7].Replace(".txt", ""));
            updateText(bracketTeam9, FolderList[9].Replace("\\", ""), BracketFiles[8].Replace(".txt", ""));
            updateText(bracketTeam10, FolderList[9].Replace("\\", ""), BracketFiles[9].Replace(".txt", ""));
            updateText(bracketTeam11, FolderList[9].Replace("\\", ""), BracketFiles[10].Replace(".txt", ""));
            updateText(bracketTeam12, FolderList[9].Replace("\\", ""), BracketFiles[11].Replace(".txt", ""));
            updateText(bracketTeam13, FolderList[9].Replace("\\", ""), BracketFiles[12].Replace(".txt", ""));
            updateText(bracketTeam14, FolderList[9].Replace("\\", ""), BracketFiles[13].Replace(".txt", ""));
            updateText(bracketTeam15, FolderList[9].Replace("\\", ""), BracketFiles[14].Replace(".txt", ""));
            updateText(bracketTeam16, FolderList[9].Replace("\\", ""), BracketFiles[15].Replace(".txt", ""));

            updateButton(bracketImage1, FolderList[9].Replace("\\", ""), BracketFiles[16].Replace(".png", ""));
            updateButton(bracketImage2, FolderList[9].Replace("\\", ""), BracketFiles[17].Replace(".png", ""));
            updateButton(bracketImage3, FolderList[9].Replace("\\", ""), BracketFiles[18].Replace(".png", ""));
            updateButton(bracketImage4, FolderList[9].Replace("\\", ""), BracketFiles[19].Replace(".png", ""));
            updateButton(bracketImage5, FolderList[9].Replace("\\", ""), BracketFiles[20].Replace(".png", ""));
            updateButton(bracketImage6, FolderList[9].Replace("\\", ""), BracketFiles[21].Replace(".png", ""));
            updateButton(bracketImage7, FolderList[9].Replace("\\", ""), BracketFiles[22].Replace(".png", ""));
            updateButton(bracketImage8, FolderList[9].Replace("\\", ""), BracketFiles[23].Replace(".png", ""));
            updateButton(bracketImage9, FolderList[9].Replace("\\", ""), BracketFiles[24].Replace(".png", ""));
            updateButton(bracketImage10, FolderList[9].Replace("\\", ""), BracketFiles[25].Replace(".png", ""));
            updateButton(bracketImage11, FolderList[9].Replace("\\", ""), BracketFiles[26].Replace(".png", ""));
            updateButton(bracketImage12, FolderList[9].Replace("\\", ""), BracketFiles[27].Replace(".png", ""));
            updateButton(bracketImage13, FolderList[9].Replace("\\", ""), BracketFiles[28].Replace(".png", ""));
            updateButton(bracketImage14, FolderList[9].Replace("\\", ""), BracketFiles[29].Replace(".png", ""));
            updateButton(bracketImage15, FolderList[9].Replace("\\", ""), BracketFiles[30].Replace(".png", ""));
            updateButton(bracketImage16, FolderList[9].Replace("\\", ""), BracketFiles[31].Replace(".png", ""));

            updateScore(bracketTeam1W, FolderList[9].Replace("\\", ""), BracketFiles[176].Replace(".txt", ""));
            updateScore(bracketTeam1L, FolderList[9].Replace("\\", ""), BracketFiles[177].Replace(".txt", ""));
            updateScore(bracketTeam1MW, FolderList[9].Replace("\\", ""), BracketFiles[178].Replace(".txt", ""));
            updateScore(bracketTeam1ML, FolderList[9].Replace("\\", ""), BracketFiles[179].Replace(".txt", ""));
            updateScore(bracketTeam2W, FolderList[9].Replace("\\", ""), BracketFiles[180].Replace(".txt", ""));
            updateScore(bracketTeam2L, FolderList[9].Replace("\\", ""), BracketFiles[181].Replace(".txt", ""));
            updateScore(bracketTeam2MW, FolderList[9].Replace("\\", ""), BracketFiles[182].Replace(".txt", ""));
            updateScore(bracketTeam2ML, FolderList[9].Replace("\\", ""), BracketFiles[183].Replace(".txt", ""));
            updateScore(bracketTeam3W, FolderList[9].Replace("\\", ""), BracketFiles[184].Replace(".txt", ""));
            updateScore(bracketTeam3L, FolderList[9].Replace("\\", ""), BracketFiles[185].Replace(".txt", ""));
            updateScore(bracketTeam3MW, FolderList[9].Replace("\\", ""), BracketFiles[186].Replace(".txt", ""));
            updateScore(bracketTeam3ML, FolderList[9].Replace("\\", ""), BracketFiles[187].Replace(".txt", ""));
            updateScore(bracketTeam4W, FolderList[9].Replace("\\", ""), BracketFiles[188].Replace(".txt", ""));
            updateScore(bracketTeam4L, FolderList[9].Replace("\\", ""), BracketFiles[189].Replace(".txt", ""));
            updateScore(bracketTeam4MW, FolderList[9].Replace("\\", ""), BracketFiles[190].Replace(".txt", ""));
            updateScore(bracketTeam4ML, FolderList[9].Replace("\\", ""), BracketFiles[191].Replace(".txt", ""));
            updateScore(bracketTeam5W, FolderList[9].Replace("\\", ""), BracketFiles[192].Replace(".txt", ""));
            updateScore(bracketTeam5L, FolderList[9].Replace("\\", ""), BracketFiles[193].Replace(".txt", ""));
            updateScore(bracketTeam5MW, FolderList[9].Replace("\\", ""), BracketFiles[194].Replace(".txt", ""));
            updateScore(bracketTeam5ML, FolderList[9].Replace("\\", ""), BracketFiles[195].Replace(".txt", ""));
            updateScore(bracketTeam6W, FolderList[9].Replace("\\", ""), BracketFiles[196].Replace(".txt", ""));
            updateScore(bracketTeam6L, FolderList[9].Replace("\\", ""), BracketFiles[197].Replace(".txt", ""));
            updateScore(bracketTeam6MW, FolderList[9].Replace("\\", ""), BracketFiles[198].Replace(".txt", ""));
            updateScore(bracketTeam6ML, FolderList[9].Replace("\\", ""), BracketFiles[199].Replace(".txt", ""));
            updateScore(bracketTeam7W, FolderList[9].Replace("\\", ""), BracketFiles[200].Replace(".txt", ""));
            updateScore(bracketTeam7L, FolderList[9].Replace("\\", ""), BracketFiles[201].Replace(".txt", ""));
            updateScore(bracketTeam7MW, FolderList[9].Replace("\\", ""), BracketFiles[202].Replace(".txt", ""));
            updateScore(bracketTeam7ML, FolderList[9].Replace("\\", ""), BracketFiles[203].Replace(".txt", ""));
            updateScore(bracketTeam8W, FolderList[9].Replace("\\", ""), BracketFiles[204].Replace(".txt", ""));
            updateScore(bracketTeam8L, FolderList[9].Replace("\\", ""), BracketFiles[205].Replace(".txt", ""));
            updateScore(bracketTeam8MW, FolderList[9].Replace("\\", ""), BracketFiles[206].Replace(".txt", ""));
            updateScore(bracketTeam8ML, FolderList[9].Replace("\\", ""), BracketFiles[207].Replace(".txt", ""));
            updateScore(bracketTeam9W, FolderList[9].Replace("\\", ""), BracketFiles[208].Replace(".txt", ""));
            updateScore(bracketTeam9L, FolderList[9].Replace("\\", ""), BracketFiles[209].Replace(".txt", ""));
            updateScore(bracketTeam9MW, FolderList[9].Replace("\\", ""), BracketFiles[210].Replace(".txt", ""));
            updateScore(bracketTeam9ML, FolderList[9].Replace("\\", ""), BracketFiles[211].Replace(".txt", ""));
            updateScore(bracketTeam10W, FolderList[9].Replace("\\", ""), BracketFiles[212].Replace(".txt", ""));
            updateScore(bracketTeam10L, FolderList[9].Replace("\\", ""), BracketFiles[213].Replace(".txt", ""));
            updateScore(bracketTeam10MW, FolderList[9].Replace("\\", ""), BracketFiles[214].Replace(".txt", ""));
            updateScore(bracketTeam10ML, FolderList[9].Replace("\\", ""), BracketFiles[215].Replace(".txt", ""));
            updateScore(bracketTeam11W, FolderList[9].Replace("\\", ""), BracketFiles[216].Replace(".txt", ""));
            updateScore(bracketTeam11L, FolderList[9].Replace("\\", ""), BracketFiles[217].Replace(".txt", ""));
            updateScore(bracketTeam11MW, FolderList[9].Replace("\\", ""), BracketFiles[218].Replace(".txt", ""));
            updateScore(bracketTeam11ML, FolderList[9].Replace("\\", ""), BracketFiles[219].Replace(".txt", ""));
            updateScore(bracketTeam12W, FolderList[9].Replace("\\", ""), BracketFiles[220].Replace(".txt", ""));
            updateScore(bracketTeam12L, FolderList[9].Replace("\\", ""), BracketFiles[221].Replace(".txt", ""));
            updateScore(bracketTeam12MW, FolderList[9].Replace("\\", ""), BracketFiles[222].Replace(".txt", ""));
            updateScore(bracketTeam12ML, FolderList[9].Replace("\\", ""), BracketFiles[223].Replace(".txt", ""));
            updateScore(bracketTeam13W, FolderList[9].Replace("\\", ""), BracketFiles[224].Replace(".txt", ""));
            updateScore(bracketTeam13L, FolderList[9].Replace("\\", ""), BracketFiles[225].Replace(".txt", ""));
            updateScore(bracketTeam13MW, FolderList[9].Replace("\\", ""), BracketFiles[226].Replace(".txt", ""));
            updateScore(bracketTeam13ML, FolderList[9].Replace("\\", ""), BracketFiles[227].Replace(".txt", ""));
            updateScore(bracketTeam14W, FolderList[9].Replace("\\", ""), BracketFiles[228].Replace(".txt", ""));
            updateScore(bracketTeam14L, FolderList[9].Replace("\\", ""), BracketFiles[229].Replace(".txt", ""));
            updateScore(bracketTeam14MW, FolderList[9].Replace("\\", ""), BracketFiles[230].Replace(".txt", ""));
            updateScore(bracketTeam14ML, FolderList[9].Replace("\\", ""), BracketFiles[231].Replace(".txt", ""));
            updateScore(bracketTeam15W, FolderList[9].Replace("\\", ""), BracketFiles[232].Replace(".txt", ""));
            updateScore(bracketTeam15L, FolderList[9].Replace("\\", ""), BracketFiles[233].Replace(".txt", ""));
            updateScore(bracketTeam15MW, FolderList[9].Replace("\\", ""), BracketFiles[234].Replace(".txt", ""));
            updateScore(bracketTeam15ML, FolderList[9].Replace("\\", ""), BracketFiles[235].Replace(".txt", ""));
            updateScore(bracketTeam16W, FolderList[9].Replace("\\", ""), BracketFiles[236].Replace(".txt", ""));
            updateScore(bracketTeam16L, FolderList[9].Replace("\\", ""), BracketFiles[237].Replace(".txt", ""));
            updateScore(bracketTeam16MW, FolderList[9].Replace("\\", ""), BracketFiles[238].Replace(".txt", ""));
            updateScore(bracketTeam16ML, FolderList[9].Replace("\\", ""), BracketFiles[239].Replace(".txt", ""));

            setBracketTeams();

            LoadTeamsInBracket();
        }

        /// <summary>
        /// Resets the files related to the bracket
        /// </summary>
        private void bracketReset_Click(object sender, EventArgs e)
        {
            resetCombo(bracketR1M1Team1);
            resetCombo(bracketR1M1Team2);
            resetCombo(bracketR1M2Team1);
            resetCombo(bracketR1M2Team2);
            resetCombo(bracketR1M3Team1);
            resetCombo(bracketR1M3Team2);
            resetCombo(bracketR1M4Team1);
            resetCombo(bracketR1M4Team2);
            resetCombo(bracketR1M5Team1);
            resetCombo(bracketR1M5Team2);
            resetCombo(bracketR1M6Team1);
            resetCombo(bracketR1M6Team2);
            resetCombo(bracketR1M7Team1);
            resetCombo(bracketR1M7Team2);
            resetCombo(bracketR1M8Team1);
            resetCombo(bracketR1M8Team2);
            resetCombo(bracketQFM1Team1);
            resetCombo(bracketQFM1Team2);
            resetCombo(bracketQFM2Team1);
            resetCombo(bracketQFM2Team2);
            resetCombo(bracketQFM3Team1);
            resetCombo(bracketQFM3Team2);
            resetCombo(bracketQFM4Team1);
            resetCombo(bracketQFM4Team2);
            resetCombo(bracketSFM1Team1);
            resetCombo(bracketSFM1Team2);
            resetCombo(bracketSFM2Team1);
            resetCombo(bracketSFM2Team2);
            resetCombo(bracketWFM1Team1);
            resetCombo(bracketWFM1Team2);
            resetCombo(bracketLR1M1Team1);
            resetCombo(bracketLR1M1Team2);
            resetCombo(bracketLR1M2Team1);
            resetCombo(bracketLR1M2Team2);
            resetCombo(bracketLR1M3Team1);
            resetCombo(bracketLR1M3Team2);
            resetCombo(bracketLR1M4Team1);
            resetCombo(bracketLR1M4Team2);
            resetCombo(bracketLR2M1Team1);
            resetCombo(bracketLR2M1Team2);
            resetCombo(bracketLR2M2Team1);
            resetCombo(bracketLR2M2Team2);
            resetCombo(bracketLR2M3Team1);
            resetCombo(bracketLR2M3Team2);
            resetCombo(bracketLR2M4Team1);
            resetCombo(bracketLR2M4Team2);
            resetCombo(bracketLR3M1Team1);
            resetCombo(bracketLR3M1Team2);
            resetCombo(bracketLR3M2Team1);
            resetCombo(bracketLR3M2Team2);
            resetCombo(bracketLR4M1Team1);
            resetCombo(bracketLR4M1Team2);
            resetCombo(bracketLR4M2Team1);
            resetCombo(bracketLR4M2Team2);
            resetCombo(bracketLSFM1Team1);
            resetCombo(bracketLSFM1Team2);
            resetCombo(bracketLFM1Team1);
            resetCombo(bracketLFM1Team2);
            resetCombo(bracketGF1M1Team1);
            resetCombo(bracketGF1M1Team2);
            resetCombo(bracketGF2M1Team1);
            resetCombo(bracketGF2M1Team2);
            resetCombo(bracketTPM1Team1);
            resetCombo(bracketTPM1Team2);

            resetScore(bracketR1M1Score1);
            resetScore(bracketR1M1Score2);
            resetScore(bracketR1M2Score1);
            resetScore(bracketR1M2Score2);
            resetScore(bracketR1M3Score1);
            resetScore(bracketR1M3Score2);
            resetScore(bracketR1M4Score1);
            resetScore(bracketR1M4Score2);
            resetScore(bracketR1M5Score1);
            resetScore(bracketR1M5Score2);
            resetScore(bracketR1M6Score1);
            resetScore(bracketR1M6Score2);
            resetScore(bracketR1M7Score1);
            resetScore(bracketR1M7Score2);
            resetScore(bracketR1M8Score1);
            resetScore(bracketR1M8Score2);
            resetScore(bracketQFM1Score1);
            resetScore(bracketQFM1Score2);
            resetScore(bracketQFM2Score1);
            resetScore(bracketQFM2Score2);
            resetScore(bracketQFM3Score1);
            resetScore(bracketQFM3Score2);
            resetScore(bracketQFM4Score1);
            resetScore(bracketQFM4Score2);
            resetScore(bracketSFM1Score1);
            resetScore(bracketSFM1Score2);
            resetScore(bracketSFM2Score1);
            resetScore(bracketSFM2Score2);
            resetScore(bracketWFM1Score1);
            resetScore(bracketWFM1Score2);
            resetScore(bracketLR1M1Score1);
            resetScore(bracketLR1M1Score2);
            resetScore(bracketLR1M2Score1);
            resetScore(bracketLR1M2Score2);
            resetScore(bracketLR1M3Score1);
            resetScore(bracketLR1M3Score2);
            resetScore(bracketLR1M4Score1);
            resetScore(bracketLR1M4Score2);
            resetScore(bracketLR2M1Score1);
            resetScore(bracketLR2M1Score2);
            resetScore(bracketLR2M2Score1);
            resetScore(bracketLR2M2Score2);
            resetScore(bracketLR2M3Score1);
            resetScore(bracketLR2M3Score2);
            resetScore(bracketLR2M4Score1);
            resetScore(bracketLR2M4Score2);
            resetScore(bracketLR3M1Score1);
            resetScore(bracketLR3M1Score2);
            resetScore(bracketLR3M2Score1);
            resetScore(bracketLR3M2Score2);
            resetScore(bracketLR4M1Score1);
            resetScore(bracketLR4M1Score2);
            resetScore(bracketLR4M2Score1);
            resetScore(bracketLR4M2Score2);
            resetScore(bracketLSFM1Score1);
            resetScore(bracketLSFM1Score2);
            resetScore(bracketLFM1Score1);
            resetScore(bracketLFM1Score2);
            resetScore(bracketGF1M1Score1);
            resetScore(bracketGF1M1Score2);
            resetScore(bracketGF2M1Score1);
            resetScore(bracketGF2M1Score2);
            resetScore(bracketTPM1Score1);
            resetScore(bracketTPM1Score2);

        }

        /// <summary>
        ///  Updates the file system with the bracket's updates
        /// </summary>
        private void bracketUpdate_Click(object sender, EventArgs e)
        {
            updateCombo(bracketR1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[32].Replace(".txt", ""));
            updateCombo(bracketR1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[33].Replace(".txt", ""));
            updateCombo(bracketR1M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[34].Replace(".txt", ""));
            updateCombo(bracketR1M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[35].Replace(".txt", ""));
            updateCombo(bracketR1M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[36].Replace(".txt", ""));
            updateCombo(bracketR1M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[37].Replace(".txt", ""));
            updateCombo(bracketR1M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[38].Replace(".txt", ""));
            updateCombo(bracketR1M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[39].Replace(".txt", ""));
            updateCombo(bracketR1M5Team1, FolderList[9].Replace("\\", ""), BracketFiles[40].Replace(".txt", ""));
            updateCombo(bracketR1M5Team2, FolderList[9].Replace("\\", ""), BracketFiles[41].Replace(".txt", ""));
            updateCombo(bracketR1M6Team1, FolderList[9].Replace("\\", ""), BracketFiles[42].Replace(".txt", ""));
            updateCombo(bracketR1M6Team2, FolderList[9].Replace("\\", ""), BracketFiles[43].Replace(".txt", ""));
            updateCombo(bracketR1M7Team1, FolderList[9].Replace("\\", ""), BracketFiles[44].Replace(".txt", ""));
            updateCombo(bracketR1M7Team2, FolderList[9].Replace("\\", ""), BracketFiles[45].Replace(".txt", ""));
            updateCombo(bracketR1M8Team1, FolderList[9].Replace("\\", ""), BracketFiles[46].Replace(".txt", ""));
            updateCombo(bracketR1M8Team2, FolderList[9].Replace("\\", ""), BracketFiles[47].Replace(".txt", ""));
            updateCombo(bracketQFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[48].Replace(".txt", ""));
            updateCombo(bracketQFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[49].Replace(".txt", ""));
            updateCombo(bracketQFM2Team1, FolderList[9].Replace("\\", ""), BracketFiles[50].Replace(".txt", ""));
            updateCombo(bracketQFM2Team2, FolderList[9].Replace("\\", ""), BracketFiles[51].Replace(".txt", ""));
            updateCombo(bracketQFM3Team1, FolderList[9].Replace("\\", ""), BracketFiles[52].Replace(".txt", ""));
            updateCombo(bracketQFM3Team2, FolderList[9].Replace("\\", ""), BracketFiles[53].Replace(".txt", ""));
            updateCombo(bracketQFM4Team1, FolderList[9].Replace("\\", ""), BracketFiles[54].Replace(".txt", ""));
            updateCombo(bracketQFM4Team2, FolderList[9].Replace("\\", ""), BracketFiles[55].Replace(".txt", ""));
            updateCombo(bracketSFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[56].Replace(".txt", ""));
            updateCombo(bracketSFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[57].Replace(".txt", ""));
            updateCombo(bracketSFM2Team1, FolderList[9].Replace("\\", ""), BracketFiles[58].Replace(".txt", ""));
            updateCombo(bracketSFM2Team2, FolderList[9].Replace("\\", ""), BracketFiles[59].Replace(".txt", ""));
            updateCombo(bracketWFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[60].Replace(".txt", ""));
            updateCombo(bracketWFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[61].Replace(".txt", ""));
            updateCombo(bracketLR1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[62].Replace(".txt", ""));
            updateCombo(bracketLR1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[63].Replace(".txt", ""));
            updateCombo(bracketLR1M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[64].Replace(".txt", ""));
            updateCombo(bracketLR1M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[65].Replace(".txt", ""));
            updateCombo(bracketLR1M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[66].Replace(".txt", ""));
            updateCombo(bracketLR1M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[67].Replace(".txt", ""));
            updateCombo(bracketLR1M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[68].Replace(".txt", ""));
            updateCombo(bracketLR1M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[69].Replace(".txt", ""));
            updateCombo(bracketLR2M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[70].Replace(".txt", ""));
            updateCombo(bracketLR2M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[71].Replace(".txt", ""));
            updateCombo(bracketLR2M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[72].Replace(".txt", ""));
            updateCombo(bracketLR2M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[73].Replace(".txt", ""));
            updateCombo(bracketLR2M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[74].Replace(".txt", ""));
            updateCombo(bracketLR2M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[75].Replace(".txt", ""));
            updateCombo(bracketLR2M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[76].Replace(".txt", ""));
            updateCombo(bracketLR2M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[77].Replace(".txt", ""));
            updateCombo(bracketLR3M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[78].Replace(".txt", ""));
            updateCombo(bracketLR3M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[79].Replace(".txt", ""));
            updateCombo(bracketLR3M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[80].Replace(".txt", ""));
            updateCombo(bracketLR3M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[81].Replace(".txt", ""));
            updateCombo(bracketLR4M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[82].Replace(".txt", ""));
            updateCombo(bracketLR4M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[83].Replace(".txt", ""));
            updateCombo(bracketLR4M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[84].Replace(".txt", ""));
            updateCombo(bracketLR4M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[85].Replace(".txt", ""));
            updateCombo(bracketLSFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[86].Replace(".txt", ""));
            updateCombo(bracketLSFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[87].Replace(".txt", ""));
            updateCombo(bracketLFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[88].Replace(".txt", ""));
            updateCombo(bracketLFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[89].Replace(".txt", ""));
            updateCombo(bracketGF1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[90].Replace(".txt", ""));
            updateCombo(bracketGF1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[91].Replace(".txt", ""));
            updateCombo(bracketGF2M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[92].Replace(".txt", ""));
            updateCombo(bracketGF2M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[93].Replace(".txt", ""));
            updateCombo(bracketTPM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[94].Replace(".txt", ""));
            updateCombo(bracketTPM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[95].Replace(".txt", ""));

            updateScore(bracketR1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[96].Replace(".txt", ""));
            updateScore(bracketR1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[97].Replace(".txt", ""));
            updateScore(bracketR1M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[98].Replace(".txt", ""));
            updateScore(bracketR1M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[99].Replace(".txt", ""));
            updateScore(bracketR1M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[100].Replace(".txt", ""));
            updateScore(bracketR1M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[101].Replace(".txt", ""));
            updateScore(bracketR1M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[102].Replace(".txt", ""));
            updateScore(bracketR1M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[103].Replace(".txt", ""));
            updateScore(bracketR1M5Score1, FolderList[9].Replace("\\", ""), BracketFiles[104].Replace(".txt", ""));
            updateScore(bracketR1M5Score2, FolderList[9].Replace("\\", ""), BracketFiles[105].Replace(".txt", ""));
            updateScore(bracketR1M6Score1, FolderList[9].Replace("\\", ""), BracketFiles[106].Replace(".txt", ""));
            updateScore(bracketR1M6Score2, FolderList[9].Replace("\\", ""), BracketFiles[107].Replace(".txt", ""));
            updateScore(bracketR1M7Score1, FolderList[9].Replace("\\", ""), BracketFiles[108].Replace(".txt", ""));
            updateScore(bracketR1M7Score2, FolderList[9].Replace("\\", ""), BracketFiles[109].Replace(".txt", ""));
            updateScore(bracketR1M8Score1, FolderList[9].Replace("\\", ""), BracketFiles[110].Replace(".txt", ""));
            updateScore(bracketR1M8Score2, FolderList[9].Replace("\\", ""), BracketFiles[111].Replace(".txt", ""));
            updateScore(bracketQFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[112].Replace(".txt", ""));
            updateScore(bracketQFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[113].Replace(".txt", ""));
            updateScore(bracketQFM2Score1, FolderList[9].Replace("\\", ""), BracketFiles[114].Replace(".txt", ""));
            updateScore(bracketQFM2Score2, FolderList[9].Replace("\\", ""), BracketFiles[115].Replace(".txt", ""));
            updateScore(bracketQFM3Score1, FolderList[9].Replace("\\", ""), BracketFiles[116].Replace(".txt", ""));
            updateScore(bracketQFM3Score2, FolderList[9].Replace("\\", ""), BracketFiles[117].Replace(".txt", ""));
            updateScore(bracketQFM4Score1, FolderList[9].Replace("\\", ""), BracketFiles[118].Replace(".txt", ""));
            updateScore(bracketQFM4Score2, FolderList[9].Replace("\\", ""), BracketFiles[119].Replace(".txt", ""));
            updateScore(bracketSFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[120].Replace(".txt", ""));
            updateScore(bracketSFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[121].Replace(".txt", ""));
            updateScore(bracketSFM2Score1, FolderList[9].Replace("\\", ""), BracketFiles[122].Replace(".txt", ""));
            updateScore(bracketSFM2Score2, FolderList[9].Replace("\\", ""), BracketFiles[123].Replace(".txt", ""));
            updateScore(bracketWFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[124].Replace(".txt", ""));
            updateScore(bracketWFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[125].Replace(".txt", ""));
            updateScore(bracketLR1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[126].Replace(".txt", ""));
            updateScore(bracketLR1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[127].Replace(".txt", ""));
            updateScore(bracketLR1M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[128].Replace(".txt", ""));
            updateScore(bracketLR1M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[129].Replace(".txt", ""));
            updateScore(bracketLR1M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[130].Replace(".txt", ""));
            updateScore(bracketLR1M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[131].Replace(".txt", ""));
            updateScore(bracketLR1M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[132].Replace(".txt", ""));
            updateScore(bracketLR1M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[133].Replace(".txt", ""));
            updateScore(bracketLR2M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[134].Replace(".txt", ""));
            updateScore(bracketLR2M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[135].Replace(".txt", ""));
            updateScore(bracketLR2M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[136].Replace(".txt", ""));
            updateScore(bracketLR2M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[137].Replace(".txt", ""));
            updateScore(bracketLR2M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[138].Replace(".txt", ""));
            updateScore(bracketLR2M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[139].Replace(".txt", ""));
            updateScore(bracketLR2M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[140].Replace(".txt", ""));
            updateScore(bracketLR2M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[141].Replace(".txt", ""));
            updateScore(bracketLR3M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[142].Replace(".txt", ""));
            updateScore(bracketLR3M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[143].Replace(".txt", ""));
            updateScore(bracketLR3M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[144].Replace(".txt", ""));
            updateScore(bracketLR3M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[145].Replace(".txt", ""));
            updateScore(bracketLR4M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[146].Replace(".txt", ""));
            updateScore(bracketLR4M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[147].Replace(".txt", ""));
            updateScore(bracketLR4M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[148].Replace(".txt", ""));
            updateScore(bracketLR4M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[149].Replace(".txt", ""));
            updateScore(bracketLSFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[150].Replace(".txt", ""));
            updateScore(bracketLSFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[151].Replace(".txt", ""));
            updateScore(bracketLFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[152].Replace(".txt", ""));
            updateScore(bracketLFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[153].Replace(".txt", ""));
            updateScore(bracketGF1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[154].Replace(".txt", ""));
            updateScore(bracketGF1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[155].Replace(".txt", ""));
            updateScore(bracketGF2M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[156].Replace(".txt", ""));
            updateScore(bracketGF2M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[157].Replace(".txt", ""));
            updateScore(bracketTPM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[158].Replace(".txt", ""));
            updateScore(bracketTPM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[159].Replace(".txt", ""));

        }

        /// <summary>
        /// Opens file dialog to assign an image path to a button's description
        /// </summary>
        private void GetImageForButton(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.png; )|*.png";
            open.InitialDirectory = path + FolderList[8];
            DialogResult res = open.ShowDialog();

            if (res == DialogResult.OK)
            {
                string selectedFilePath = open.FileName;
                ((Button)sender).AccessibleDescription = selectedFilePath;

                string baseName = ((Button)sender).Name;
                string pictureBoxName = "";
                // Extract the base name from the sender's Name property
                if (baseName.Contains("ImageButton"))
                {
                    baseName = baseName.Replace("ImageButton", "");
                    pictureBoxName = baseName + "HeroIcon";
                }
                else if (baseName.Contains("LogoButton"))
                {
                    baseName = baseName.Replace("LogoButton", "");
                    pictureBoxName = baseName + "Logo";
                }

                // Construct the PictureBox name

                // Set the image using the shared method
                SetHeroImage(pictureBoxName, selectedFilePath);
            }
        }



        /// <summary>
        /// Resets the image path attached to the button
        /// </summary>
        /// <param name="sender">Button who's image path is being cleared</param>
        /// <param name="e">Event</param>
        private void resetButtonImagePath_Click(object sender, EventArgs e)
        {
            ((Button)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl).AccessibleDescription = "";
        }

        #endregion

        #region Toolbar

        /// <summary>
        /// Toolbar option to export series information
        /// </summary>
        private void exportSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSeries();
        }

        /// <summary>
        /// Toolbar option to import series information
        /// </summary>
        private void importSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importSeries();
        }

        /// <summary>
        /// Toolbar option to import home team information
        /// </summary>
        private void importHomeTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTeam("h");
        }

        /// <summary>
        /// Toolbar option to import away team information
        /// </summary>
        private void importAwayTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTeam("a");
        }

        /// <summary>
        /// Toolbar option to export home team information
        /// </summary>
        private void exportHomeTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTeam("h");
        }

        /// <summary>
        /// Toolbar option to export away team information
        /// </summary>
        private void exportAwayTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTeam("a");
        }

        /// <summary>
        /// Toolbar option to open the About window
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        /// <summary>
        /// Opens dialog to create a Gametrype
        /// </summary>
        private void createGametypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateGametypeDialog gametypeDialog = new CreateGametypeDialog();
            if (gametypeDialog.ShowDialog() == DialogResult.OK)
            {
                CreateCustomGametype(gametypeDialog.CreatedGametype);
            }

        }

        /// <summary>
        /// Opens dialog to create a Hero
        /// </summary>
        private void createHeroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateHeroDialog heroDialog = new CreateHeroDialog();
            if (heroDialog.ShowDialog() == DialogResult.OK)
            {
                CreateCustomHero(heroDialog.CreatedHero);
            }

        }

        /// <summary>
        /// Opens dialog to create a Map
        /// </summary>
        /// <param name="sender"></param>
        private void createMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateMapDialog mapDialog = new CreateMapDialog();
            mapDialog.SetGametypeList(GametypeList);
            if (mapDialog.ShowDialog() == DialogResult.OK)
            {
                CreateCustomMap(mapDialog.CreatedMap);
            }
        }

        /// <summary>
        /// Opens dialog to create a Role 
        /// </summary>
        private void createRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRoleDialog roleDialog = new CreateRoleDialog();
            if (roleDialog.ShowDialog() == DialogResult.OK)
            {
                CreateCustomRole(roleDialog.CreatedRole);
            }
            else
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void editGametypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void editHeroToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void editMapToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void editRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void deleteGametypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void deleteHeroToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void deleteMapToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void deleteRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens dialog to set the Gametype list to default Overwatch Gametypes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultGametypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to return all Gametypes to the default list?", "Reset Gametypes?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DefaultGametypes();
            }
            else
            {

            }
        }

        /// <summary>
        /// Opens dialog to set the Hero list to default Overwatch Heroes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultHeroesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to return all Heroes to the default list?", "Reset Heroes?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DefaultHeroes();
            }
            else
            {

            }
        }

        /// <summary>
        /// Opens dialog to set the Map list to default Overwatch Maps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to return all Maps to the default list?", "Reset Maps?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DefaultMaps();
            }
            else
            {

            }
        }

        /// <summary>
        /// Opens dialog to set the Role list to default Overwatch Roles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to return all Roles to the default list?", "Reset Roles?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DefaultRoles();
            }
            else
            {

            }
        }

        /// <summary>
        /// Opens dialog to set the list of all custom objects to default Overwatch Objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to return all custom objects to the default list?", "Reset Custom Objects?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DefaultGametypes();
                DefaultHeroes();
                DefaultMaps();
                DefaultRoles();
            }
            else
            {

            }
        }

        #endregion

        #region Updaters
        /// <summary>
        /// Method for writing to the text file for the text box
        /// </summary>
        /// <param name="field">The text box that is being read from</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateText(TextBox field, String folder, String file)
        {
            if (field.Modified)
            {
                using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
                {
                    sw.WriteLine(field.Text.Trim());
                }

                field.Modified = false;
            }
        }

        /// <summary>
        /// Method for writing to the text file for the NumericUpDown used for score recording
        /// </summary>
        /// <param name="field">The NumericUpDown that is being read from</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateScore(NumericUpDown field, String folder, String file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                sw.WriteLine(field.Value.ToString().Trim());
            }
        }

        /// <summary>
        /// Method for writing to the text file for the combobox text and overwriting the image of the hero in that text
        /// </summary>
        /// <param name="field">The Combobox that is being read from</param>
        /// <param name="radio1">Image style, portrait</param>
        /// <param name="radio2">Image style, icon</param>
        /// <param name="radio3">Image style, 3D</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateHero(ComboBox field, String folder, String file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                    sw.WriteLine(((Hero)field.SelectedValue).Name);
            }
            string imagepath = path + "\\" + folder + "\\" + file + ".png";

            ((Hero)field.SelectedValue).Icon.Save(imagepath);
        }

        /// <summary>
        /// Method for writing to the text file for the combobox text and overwriting the image of the role in that text
        /// </summary>
        /// <param name="field">The Combobox that is being read from</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateRole(ComboBox field, String folder, String file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                sw.WriteLine(((Role)field.SelectedValue).Name);
            }
            ((Role)field.SelectedValue).Icon.Save(path + "\\" + folder + "\\" + file + ".png");

        }

        /// <summary>
        /// Method for updating the image used for attack/defend
        /// </summary>
        /// <param name="field">RadioButton stating which side the home team is on</param>
        /// <param name="folder">The target folde</param>
        /// <param name="file1">The file name before the extension for the home team</param>
        /// <param name="file2">The file name before the extension for the away team</param>
        private void updateSide(RadioButton field, String folder, String file1, String file2)
        {
            if (field.Checked == true)
            {
                if (field.Text == "A")
                {
                    Properties.Resources.Icon_offense.Save(path + "\\" + folder + "\\" + file1 + ".png");
                    Properties.Resources.Icon_defense.Save(path + "\\" + folder + "\\" + file2 + ".png");
                }
                else if(field.Text == "D")
                {
                    Properties.Resources.Icon_defense.Save(path + "\\" + folder + "\\" + file1 + ".png");
                    Properties.Resources.Icon_offense.Save(path + "\\" + folder + "\\" + file2 + ".png");
                }
                else
                {
                    Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file1 + ".png");
                    Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file2 + ".png");
                }

            }
        }

        /// <summary>
        /// Method for writing to the text file for the combobox text and overwriting the image of the map in that text
        /// </summary>
        /// <param name="field">The Combobox that is being read from</param>
        /// <param name="check">Radio button checked if the map is the current selection</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateMap(ComboBox field, RadioButton check, String folder, String file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                sw.WriteLine(((Map)field.SelectedValue).Name.Replace("Assault", "").Replace("Escort", "").Replace("Hybrid", "").Replace("Control", "").Replace("Push", "").Replace("Flashpoint", "").Replace("Clash", ""));
            }
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + "Gametype" + ".txt"))
            {
                sw.WriteLine(((Map)field.SelectedValue).Gametype.Name);
            }
            ((Map)field.SelectedValue).Icon.Save(path + "\\" + folder + "\\" + file + ".png");
            ((Map)field.SelectedValue).Gametype.Icon.Save(path + "\\" + folder + "\\" + file + "Gametype" + ".png");
        }

        /// <summary>
        /// Method for writing to the text file and overwriting the logo for the map winner
        /// </summary>
        /// <param name="field">Checkbox for if a map has been completed</param>
        /// <param name="team1">Home Team name</param>
        /// <param name="team2">Away Team name</param>
        /// <param name="score1">Home Team map score</param>
        /// <param name="score2">Away Team map score</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        /// <param name="team1Logo">Team 1 logo</param>
        /// <param name="team2Logo">Team 2 logo</param>
        private void updateCompleted(CheckBox field, TextBox team1, TextBox team2, NumericUpDown score1, NumericUpDown score2, String folder, String file, string team1Logo, string team2Logo, ComboBox map)
        {
            string winner = "";
            string winnerLogo = "";

            if (field.Checked == true)
            {
                if (score1.Value > score2.Value)
                {
                    winner = team1.Text;
                    winnerLogo = team1Logo;

                    updateLogos(winnerLogo, folder, file);

                }
                else if (score2.Value > score1.Value)
                {
                    winner = team2.Text;
                    winnerLogo = team2Logo;
                    updateLogos(winnerLogo, folder, file);
                }
                else
                {
                    winner = "DRAW";
                    Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");

                }

                using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
                {
                    sw.WriteLine(winner.Trim());
                }

                GrayScale(((Map)map.SelectedValue).Icon).Save(path + "\\" + folder + "\\" + file.Replace("Win","") + ".png");
            }
            else
            {
                using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
                {
                    sw.WriteLine(winner.Trim());
                }
                Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");
            }
        }

        /// <summary>
        /// Method for updating the logo of a team that won a map
        /// </summary>
        /// <param name="field">The text box that is being read from</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateLogos(string field, String folder, String file)
        {
            if (field != "")
            {
                try
                {
                    Bitmap logo = new Bitmap(field);
                    logo.Save(path + "\\" + folder + "\\" + file + ".png");
                }
                catch(Exception e)
                {
                    MessageBox.Show("The path \"" + path + "\\" + folder + "\\" + file + ".png" + "\" does not exist!");
                    field = "";
                    Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");
                }
            }
            else
            {
                Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");
            }
        }

        /// <summary>
        /// Method for writing to the text file for the color dialog
        /// </summary>
        /// <param name="field">The color dialog that is being read from</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void updateColor(Button field, String folder, String file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                sw.WriteLine(ColorTranslator.ToHtml(Color.FromArgb(field.BackColor.ToArgb())));
            }
        }

        /// <summary>
        /// Method for writing the image path for the button to the text file and saving the image to a file
        /// </summary>
        private void updateButton(Button field, String folder, String file)
        {
            if (field.AccessibleDescription != null)
            {
                using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + "Path" + ".txt"))
                {
                    sw.WriteLine(field.AccessibleDescription.Trim());
                }

                if (field.AccessibleDescription != "")
                {
                    try
                    {
                        Bitmap logo = new Bitmap(field.AccessibleDescription);
                        logo.Save(path + "\\" + folder + "\\" + file + ".png");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("The path \"" + path + "\\" + folder + "\\" + file + ".png" + "\" does not exist!");
                        field.AccessibleDescription = "";
                        Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");
                    }
                }
                else
                {
                    Properties.Resources.Icon_none.Save(path + "\\" + folder + "\\" + file + ".png");
                }
            }
        }

        /// <summary>
        /// Method for updating the Series of the form
        /// </summary>
        private void updateSeries()
        {
            Match1 = new Series("Match1", updateTeams("h"), updateTeams("a"), updateMapPool(), m1t1Score.Value.ToString(), m1t2Score.Value.ToString());
            Match1.Games[0].HomeScore = m1m1t1Score.Value.ToString();
            Match1.Games[0].AwayScore = m1m1t2Score.Value.ToString();
            Match1.Games[1].HomeScore = m1m2t1Score.Value.ToString();
            Match1.Games[1].AwayScore = m1m2t2Score.Value.ToString();
            Match1.Games[2].HomeScore = m1m3t1Score.Value.ToString();
            Match1.Games[2].AwayScore = m1m3t2Score.Value.ToString();
            Match1.Games[3].HomeScore = m1m4t1Score.Value.ToString();
            Match1.Games[3].AwayScore = m1m4t2Score.Value.ToString();
            Match1.Games[4].HomeScore = m1m5t1Score.Value.ToString();
            Match1.Games[4].AwayScore = m1m5t2Score.Value.ToString();
            Match1.Games[5].HomeScore = m1m6t1Score.Value.ToString();
            Match1.Games[5].AwayScore = m1m6t2Score.Value.ToString();
            Match1.Games[6].HomeScore = m1m7t1Score.Value.ToString();
            Match1.Games[6].AwayScore = m1m7t2Score.Value.ToString();
            Match1.Version = xmlVersion;
        }

        /// <summary>
        /// Method used for updating the players based on team side provided
        /// </summary>
        /// <param name="side">The side of the team</param>
        /// <returns>Lists of players</returns>
        private List<Player> updatePlayers(string side)
        {
            List<Player> players = new List<Player>();
            if (side.Equals("h"))
            {
                players.Add(new Player(m1t1p1Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p1Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p1Role.Text), m1t1p1Info.Text, m1t1p1ImageButton.AccessibleDescription));
                players.Add(new Player(m1t1p2Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p2Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p2Role.Text), m1t1p2Info.Text, m1t1p2ImageButton.AccessibleDescription));
                players.Add(new Player(m1t1p3Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p3Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p3Role.Text), m1t1p3Info.Text, m1t1p3ImageButton.AccessibleDescription));
                players.Add(new Player(m1t1p4Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p4Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p4Role.Text), m1t1p4Info.Text, m1t1p4ImageButton.AccessibleDescription));
                players.Add(new Player(m1t1p5Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p5Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p5Role.Text), m1t1p5Info.Text, m1t1p5ImageButton.AccessibleDescription));
                players.Add(new Player(m1t1p6Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t1p6Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t1p6Role.Text), m1t1p6Info.Text, m1t1p6ImageButton.AccessibleDescription));
            }
            else if(side.Equals("a"))
            {
                players.Add(new Player(m1t2p1Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p1Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p1Role.Text), m1t2p1Info.Text, m1t2p1ImageButton.AccessibleDescription));
                players.Add(new Player(m1t2p2Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p2Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p2Role.Text), m1t2p2Info.Text, m1t2p2ImageButton.AccessibleDescription));
                players.Add(new Player(m1t2p3Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p3Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p3Role.Text), m1t2p3Info.Text, m1t2p3ImageButton.AccessibleDescription));
                players.Add(new Player(m1t2p4Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p4Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p4Role.Text), m1t2p4Info.Text, m1t2p4ImageButton.AccessibleDescription));
                players.Add(new Player(m1t2p5Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p5Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p5Role.Text), m1t2p5Info.Text, m1t2p5ImageButton.AccessibleDescription));
                players.Add(new Player(m1t2p6Name.Text, new List<Hero> { HeroList.FirstOrDefault(s => s.Name == m1t2p6Hero.Text) }, RoleList.FirstOrDefault(s => s.Name == m1t2p6Role.Text), m1t2p6Info.Text, m1t2p6ImageButton.AccessibleDescription));
            }
            return players;

        }

        /// <summary>
        /// Method used for updating the team based on the team side provided
        /// </summary>
        /// <param name="side">The side of the team</param>
        /// <returns></returns>
        private Team updateTeams(string side)
        {
            if (side.Equals("h"))
            {
                Match1.Home = new Team(m1t1Name.Text, m1t1SR.Text, m1t1LogoButton.AccessibleDescription, updatePlayers(side), ColorTranslator.ToHtml(Color.FromArgb(m1t1ColorButton.BackColor.ToArgb())));
                Match1.Home.Version = xmlVersion;
                return Match1.Home;
            }
            else
            {
                Match1.Away = new Team(m1t2Name.Text, m1t2SR.Text, m1t2LogoButton.AccessibleDescription, updatePlayers(side), ColorTranslator.ToHtml(Color.FromArgb(m1t2ColorButton.BackColor.ToArgb())));
                Match1.Away.Version = xmlVersion;
                return Match1.Away;
            }
        }

        /// <summary>
        /// Method for updating the Map Pool of a series
        /// </summary>
        /// <returns>The list of all different games in the series</returns>
        private List<Game> updateMapPool()
        {
            List<Map> mapPerGame = new List<Map>
            {
                MapList.SingleOrDefault(s => s.Name == m1m1Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m2Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m3Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m4Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m5Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m6Map.Text),
                MapList.SingleOrDefault(s => s.Name == m1m7Map.Text)
            };

            List<Game> mapPool = new List<Game>();
            foreach(Map m in mapPerGame){
                mapPool.Add(new Game(m, false, false, "0", "0", new Team()));
            }

            return mapPool;
        }

        /// <summary>
        /// Updates the replay list on the replay page
        /// </summary>
        private void updateList()
        {
            replayListView.Items.Clear();
            replayImageList.Images.Clear();
            Replays.Clear();
            var di = new DirectoryInfo(path + FolderList[5]);
            int imageIndex = 0;
            foreach (FileInfo fi in di.GetFiles())
            {
                ShellFile shellFile = ShellFile.FromFilePath(fi.FullName);
                Bitmap shellThumb = shellFile.Thumbnail.Bitmap;
                replayImageList.Images.Add(shellThumb);
                ListViewItem item = new ListViewItem();
                item.Text = fi.Name;
                item.ImageIndex = imageIndex;
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add(fi.CreationTime.ToShortTimeString());
                replayListView.Items.Add(item);
                Replays.Add(new Replay());
                Replays[imageIndex].Players = new List<Player>();
                Replays[imageIndex].ReplayPath = fi.FullName;
                Replays[imageIndex].Name = fi.Name;
                Replays[imageIndex].Created = fi.CreationTime;
                Replays[imageIndex].Thumbnail = shellThumb;
                imageIndex++;
            }
        }

        /// <summary>
        /// Updates the team and map lists on the replay page
        /// </summary>
        private void updateReplayPage()
        {
            replayTeamComboBox.Items.Clear();
            replayMapComboBox.Items.Clear();

            replayTeamComboBox.Items.Add("");
            replayTeamComboBox.Items.Add(Match1.Home.Name);
            replayTeamComboBox.Items.Add(Match1.Away.Name);

            replayMapComboBox.Items.Add("");
            replayMapComboBox.Items.Add(Match1.Games[0].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[1].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[2].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[3].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[4].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[5].Map.Name);
            replayMapComboBox.Items.Add(Match1.Games[6].Map.Name);
        }

        /// <summary>
        /// Updates the text file path for the selected ComboBox
        /// </summary>
        private void updateCombo(ComboBox field, string folder, string file)
        {
            using (StreamWriter sw = File.CreateText(path + "\\" + folder + "\\" + file + ".txt"))
            {
                sw.WriteLine(((Team)field.SelectedItem).Name);
            }
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Method for loading the text box based on the file being read
        /// </summary>
        /// <param name="field">The text box that is being written to</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void loadText(TextBox field, String folder, String file)
        {
            if (File.Exists(path + "\\" + folder + "\\" + file + ".txt"))
            {
                string loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + ".txt");
                field.Text = loadingText.Trim();
            }
            else
            {
                File.Create(path + "\\" + folder + "\\" + file + ".txt");
            }

        }

        /// <summary>
        /// Method for loading the NumericUpDown text based on the file being reaad
        /// </summary>
        /// <param name="field">The NumericUpDown that is being written to</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void loadScore(NumericUpDown field, String folder, String file)
        {
            if (File.Exists(path + "\\" + folder + "\\" + file + ".txt"))
            {
                string loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + ".txt");
                decimal number;
                Decimal.TryParse(loadingText, out number);
                field.Value = number;
            }
            else
            {
                File.Create(path + "\\" + folder + "\\" + file + ".txt");
            }

        }

        /// <summary>
        /// Method for loading the Combobox text based on the file being reaad
        /// </summary>
        /// <param name="field">The Combobox having its item set</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void loadCombo(ComboBox field, String folder, String file)
        {
            if (File.Exists(path + "\\" + folder + "\\" + file + ".txt"))
            {
                string loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + ".txt");
                if (loadingText.Trim() == "" && file.Contains("Map"))
                {
                    loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + "Gametype" + ".txt");
                }
                field.SelectedIndex = field.FindString(loadingText.Trim());
                if(field.SelectedIndex == -1)
                {
                    field.SelectedIndex = 0;
                }
            }
            else
            {
                File.Create(path + "\\" + folder + "\\" + file + ".txt");
            }
        }

        /// <summary>
        /// Method for loading the text box based on the file being read
        /// </summary>
        /// <param name="field">The text box that is being written to</param>
        /// <param name="folder">The target folder</param>
        /// <param name="file">The file name before the extension</param>
        private void loadColor(Button button, String folder, String file)
        {
            if (File.Exists(path + "\\" + folder + "\\" + file + ".txt"))
            {
                string loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + ".txt");
                Color loadedColor = System.Drawing.ColorTranslator.FromHtml(loadingText.Trim());
                button.BackColor = loadedColor;
            }
            else
            {
                File.Create(path + "\\" + folder + "\\" + file + ".txt");
            }

        }

        private void loadButton (Button field, String folder, String file)
        {
            if (File.Exists(path + "\\" + folder + "\\" + file + "Path" + ".txt"))
            {
                string loadingText = File.ReadAllText(path + "\\" + folder + "\\" + file + "Path" + ".txt");
                field.AccessibleDescription = loadingText.Trim();
            }
            else
            {
                File.Create(path + "\\" + folder + "\\" + file + "Path" + ".txt");
            }

        }

        /// <summary>
        /// Methodd for loading the series of the form
        /// </summary>
        private void loadSeries()
        {
            loadTeams("h");
            loadTeams("a");
            loadMaps();
        }

        /// <summary>
        /// Method for loading the team based on side
        /// </summary>
        /// <param name="side">Side of the team</param>
        private void loadTeams(string side)
        {
            if (side == "h")
            {
                m1t1Name.Text = Match1.Home.Name;
                m1t1Score.Text = Match1.HomeScore;
                m1t1SR.Text = Match1.Home.Sr;
                m1t1LogoButton.AccessibleDescription = Match1.Home.LogoPath;
                m1t1ColorButton.BackColor = System.Drawing.ColorTranslator.FromHtml(Match1.Home.Color);

                m1t1p1Name.Text = Match1.Home.Players[0].Name;
                m1t1p1Info.Text = Match1.Home.Players[0].Info;
                m1t1p1ImageButton.AccessibleDescription = Match1.Home.Players[0].ImagePath;
                try
                {
                    m1t1p1Hero.SelectedIndex = m1t1p1Hero.FindStringExact(Match1.Home.Players[0].Heroes[0].Name);
                }
                catch
                {
                    m1t1p1Hero.SelectedIndex = m1t1p1Hero.FindStringExact("");
                }
                try
                {
                    m1t1p1Role.SelectedIndex = m1t1p1Role.FindStringExact(Match1.Home.Players[0].Role.Name);
                }
                catch
                {
                    m1t1p1Role.SelectedIndex = m1t1p1Role.FindStringExact("");
                }
                
                m1t1p2Name.Text = Match1.Home.Players[1].Name;
                m1t1p2Info.Text = Match1.Home.Players[1].Info;
                m1t1p2ImageButton.AccessibleDescription = Match1.Home.Players[1].ImagePath;
                try
                {
                    m1t1p2Hero.SelectedIndex = m1t1p2Hero.FindStringExact(Match1.Home.Players[1].Heroes[0].Name);
                }
                catch
                {
                    m1t1p2Hero.SelectedIndex = m1t1p2Hero.FindStringExact("");
                }
                try
                {
                    m1t1p2Role.SelectedIndex = m1t1p2Role.FindStringExact(Match1.Home.Players[1].Role.Name);
                }
                catch
                {
                    m1t1p2Role.SelectedIndex = m1t1p2Role.FindStringExact("");
                }

                m1t1p3Name.Text = Match1.Home.Players[2].Name;
                m1t1p3Info.Text = Match1.Home.Players[2].Info;
                m1t1p3ImageButton.AccessibleDescription = Match1.Home.Players[2].ImagePath;
                try
                {
                    m1t1p3Hero.SelectedIndex = m1t1p3Hero.FindStringExact(Match1.Home.Players[2].Heroes[0].Name);
                }
                catch
                {
                    m1t1p3Hero.SelectedIndex = m1t1p3Hero.FindStringExact("");
                }
                try
                {
                    m1t1p3Role.SelectedIndex = m1t1p3Role.FindStringExact(Match1.Home.Players[2].Role.Name);
                }
                catch
                {
                    m1t1p3Role.SelectedIndex = m1t1p3Role.FindStringExact("");
                }

                m1t1p4Name.Text = Match1.Home.Players[3].Name;
                m1t1p4Info.Text = Match1.Home.Players[3].Info;
                m1t1p4ImageButton.AccessibleDescription = Match1.Home.Players[3].ImagePath;
                try
                {
                    m1t1p4Hero.SelectedIndex = m1t1p4Hero.FindStringExact(Match1.Home.Players[3].Heroes[0].Name);
                }
                catch
                {
                    m1t1p4Hero.SelectedIndex = m1t1p4Hero.FindStringExact("");
                }
                try
                {
                    m1t1p4Role.SelectedIndex = m1t1p4Role.FindStringExact(Match1.Home.Players[3].Role.Name);
                }
                catch
                {
                    m1t1p4Role.SelectedIndex = m1t1p4Role.FindStringExact("");
                }

                m1t1p5Name.Text = Match1.Home.Players[4].Name;
                m1t1p5Info.Text = Match1.Home.Players[4].Info;
                m1t1p5ImageButton.AccessibleDescription = Match1.Home.Players[4].ImagePath;
                try
                {
                    m1t1p5Hero.SelectedIndex = m1t1p5Hero.FindStringExact(Match1.Home.Players[4].Heroes[0].Name);
                }
                catch
                {
                    m1t1p5Hero.SelectedIndex = m1t1p5Hero.FindStringExact("");
                }
                try
                {
                    m1t1p5Role.SelectedIndex = m1t1p5Role.FindStringExact(Match1.Home.Players[4].Role.Name);
                }
                catch
                {
                    m1t1p5Role.SelectedIndex = m1t1p5Role.FindStringExact("");
                }

                m1t1p6Name.Text = Match1.Home.Players[5].Name;
                m1t1p6Info.Text = Match1.Home.Players[5].Info;
                m1t1p6ImageButton.AccessibleDescription = Match1.Home.Players[5].ImagePath;
                try
                {
                    m1t1p6Hero.SelectedIndex = m1t1p6Hero.FindStringExact(Match1.Home.Players[5].Heroes[0].Name);
                }
                catch
                {
                    m1t1p6Hero.SelectedIndex = m1t1p6Hero.FindStringExact("");
                }
                try
                {
                    m1t1p6Role.SelectedIndex = m1t1p6Role.FindStringExact(Match1.Home.Players[5].Role.Name);
                }
                catch
                {
                    m1t1p6Role.SelectedIndex = m1t1p6Role.FindStringExact("");
                }
            }
            else
            {
                m1t2Name.Text = Match1.Away.Name;
                m1t2Score.Text = Match1.AwayScore;
                m1t2SR.Text = Match1.Away.Sr;
                m1t2LogoButton.AccessibleDescription = Match1.Away.LogoPath;
                m1t2ColorButton.BackColor = System.Drawing.ColorTranslator.FromHtml(Match1.Away.Color); 

                m1t2p1Name.Text = Match1.Away.Players[0].Name;
                m1t2p1Info.Text = Match1.Away.Players[0].Info;
                m1t2p1ImageButton.AccessibleDescription = Match1.Away.Players[0].ImagePath;
                try
                {
                    m1t2p1Hero.SelectedIndex = m1t2p1Hero.FindStringExact(Match1.Away.Players[0].Heroes[0].Name);
                }
                catch
                {
                    m1t2p1Hero.SelectedIndex = m1t2p1Hero.FindStringExact("");
                }
                try
                {
                    m1t2p1Role.SelectedIndex = m1t2p1Role.FindStringExact(Match1.Away.Players[0].Role.Name);
                }
                catch
                {
                    m1t2p1Role.SelectedIndex = m1t2p1Role.FindStringExact("");
                }

                m1t2p2Name.Text = Match1.Away.Players[1].Name;
                m1t2p2Info.Text = Match1.Away.Players[1].Info;
                m1t2p2ImageButton.AccessibleDescription = Match1.Away.Players[1].ImagePath;
                try
                {
                    m1t2p2Hero.SelectedIndex = m1t2p2Hero.FindStringExact(Match1.Away.Players[1].Heroes[0].Name);
                }
                catch
                {
                    m1t2p2Hero.SelectedIndex = m1t2p2Hero.FindStringExact("");
                }
                try
                {
                    m1t2p2Role.SelectedIndex = m1t2p2Role.FindStringExact(Match1.Away.Players[1].Role.Name);
                }
                catch
                {
                    m1t2p2Role.SelectedIndex = m1t2p2Role.FindStringExact("");
                }

                m1t2p3Name.Text = Match1.Away.Players[2].Name;
                m1t2p3Info.Text = Match1.Away.Players[2].Info;
                m1t2p3ImageButton.AccessibleDescription = Match1.Away.Players[2].ImagePath;
                try
                {
                    m1t2p3Hero.SelectedIndex = m1t2p3Hero.FindStringExact(Match1.Away.Players[2].Heroes[0].Name);
                }
                catch
                {
                    m1t2p3Hero.SelectedIndex = m1t2p3Hero.FindStringExact("");
                }
                try
                {
                    m1t2p3Role.SelectedIndex = m1t2p3Role.FindStringExact(Match1.Away.Players[2].Role.Name);
                }
                catch
                {
                    m1t2p3Role.SelectedIndex = m1t2p3Role.FindStringExact("");
                }

                m1t2p4Name.Text = Match1.Away.Players[3].Name;
                m1t2p4Info.Text = Match1.Away.Players[3].Info;
                m1t2p4ImageButton.AccessibleDescription = Match1.Away.Players[3].ImagePath;
                try
                {
                    m1t2p4Hero.SelectedIndex = m1t2p4Hero.FindStringExact(Match1.Away.Players[3].Heroes[0].Name);
                }
                catch
                {
                    m1t2p4Hero.SelectedIndex = m1t2p4Hero.FindStringExact("");
                }
                try
                {
                    m1t2p4Role.SelectedIndex = m1t2p4Role.FindStringExact(Match1.Away.Players[3].Role.Name);
                }
                catch
                {
                    m1t2p4Role.SelectedIndex = m1t2p4Role.FindStringExact("");
                }

                m1t2p5Name.Text = Match1.Away.Players[4].Name;
                m1t2p5Info.Text = Match1.Away.Players[4].Info;
                m1t2p5ImageButton.AccessibleDescription = Match1.Away.Players[4].ImagePath;
                try
                {
                    m1t2p5Hero.SelectedIndex = m1t2p5Hero.FindStringExact(Match1.Away.Players[4].Heroes[0].Name);
                }
                catch
                {
                    m1t2p5Hero.SelectedIndex = m1t2p5Hero.FindStringExact("");
                }
                try
                {
                    m1t2p5Role.SelectedIndex = m1t2p5Role.FindStringExact(Match1.Away.Players[4].Role.Name);
                }
                catch
                {
                    m1t2p5Role.SelectedIndex = m1t2p5Role.FindStringExact("");
                }

                m1t2p6Name.Text = Match1.Away.Players[5].Name;
                m1t2p6Info.Text = Match1.Away.Players[5].Info;
                m1t2p6ImageButton.AccessibleDescription = Match1.Away.Players[5].ImagePath;
                try
                {
                    m1t2p6Hero.SelectedIndex = m1t2p6Hero.FindStringExact(Match1.Away.Players[5].Heroes[0].Name);
                }
                catch
                {
                    m1t2p6Hero.SelectedIndex = m1t2p6Hero.FindStringExact("");
                }
                try
                {
                    m1t2p6Role.SelectedIndex = m1t2p6Role.FindStringExact(Match1.Away.Players[5].Role.Name);
                }
                catch
                {
                    m1t2p6Role.SelectedIndex = m1t2p6Role.FindStringExact("");
                }
            }
        }

        /// <summary>
        /// Method for loading the games information from the series
        /// </summary>
        private void loadMaps()
        {
            decimal number;

            try
            {
                m1m1Map.SelectedIndex = m1m1Map.FindStringExact(Match1.Games[0].Map.Name);
                Decimal.TryParse(Match1.Games[0].HomeScore, out number);
                m1m1t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[0].AwayScore, out number);
                m1m1t2Score.Value = number;
                number = 0;
                m1m1Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m1Map.SelectedIndex = m1m1Map.FindStringExact("");
                m1m1t1Score.Value = 0;
                m1m1t2Score.Value = 0;
                m1m1Completed.Checked = false;
            }

            try
            {
                m1m2Map.SelectedIndex = m1m2Map.FindStringExact(Match1.Games[1].Map.Name);
                Decimal.TryParse(Match1.Games[1].HomeScore, out number);
                m1m2t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[1].AwayScore, out number);
                m1m2t2Score.Value = number;
                number = 0;
                m1m2Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m2Map.SelectedIndex = m1m2Map.FindStringExact("");
                m1m2t1Score.Value = 0;
                m1m2t2Score.Value = 0;
                m1m2Completed.Checked = false;
            }

            try
            {
                m1m3Map.SelectedIndex = m1m3Map.FindStringExact(Match1.Games[2].Map.Name);
                Decimal.TryParse(Match1.Games[2].HomeScore, out number);
                m1m3t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[2].AwayScore, out number);
                m1m3t2Score.Value = number;
                number = 0;
                m1m3Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m3Map.SelectedIndex = m1m3Map.FindStringExact("");
                m1m3t1Score.Value = 0;
                m1m3t2Score.Value = 0;
                m1m3Completed.Checked = false;
            }

            try
            {
                m1m4Map.SelectedIndex = m1m4Map.FindStringExact(Match1.Games[3].Map.Name);
                Decimal.TryParse(Match1.Games[3].HomeScore, out number);
                m1m4t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[3].AwayScore, out number);
                m1m4t2Score.Value = number;
                number = 0;
                m1m4Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m4Map.SelectedIndex = m1m4Map.FindStringExact("");
                m1m4t1Score.Value = 0;
                m1m4t2Score.Value = 0;
                m1m4Completed.Checked = false;
            }

            try
            {
                m1m5Map.SelectedIndex = m1m5Map.FindStringExact(Match1.Games[4].Map.Name);
                Decimal.TryParse(Match1.Games[4].HomeScore, out number);
                m1m5t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[4].AwayScore, out number);
                m1m5t2Score.Value = number;
                number = 0;
                m1m5Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m5Map.SelectedIndex = m1m5Map.FindStringExact("");
                m1m5t1Score.Value = 0;
                m1m5t2Score.Value = 0;
                m1m5Completed.Checked = false;
            }

            try
            {
                m1m6Map.SelectedIndex = m1m6Map.FindStringExact(Match1.Games[5].Map.Name);
                Decimal.TryParse(Match1.Games[5].HomeScore, out number);
                m1m6t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[5].AwayScore, out number);
                m1m6t2Score.Value = number;
                number = 0;
                m1m6Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m6Map.SelectedIndex = m1m6Map.FindStringExact("");
                m1m6t1Score.Value = 0;
                m1m6t2Score.Value = 0;
                m1m6Completed.Checked = false;
            }

            try
            {
                m1m7Map.SelectedIndex = m1m7Map.FindStringExact(Match1.Games[6].Map.Name);
                Decimal.TryParse(Match1.Games[6].HomeScore, out number);
                m1m7t1Score.Value = number;
                number = 0;
                Decimal.TryParse(Match1.Games[6].AwayScore, out number);
                m1m7t2Score.Value = number;
                number = 0;
                m1m7Completed.Checked = Match1.Games[0].Completed;
            }
            catch
            {
                m1m7Map.SelectedIndex = m1m7Map.FindStringExact("");
                m1m7t1Score.Value = 0;
                m1m7t2Score.Value = 0;
                m1m7Completed.Checked = false;
            }
        }

        /// <summary>
        /// Method to load the player's heroes for the series.
        /// </summary>
        private void LoadPlayerHeroes()
        {
            loadCombo(m1t1p1Hero, "Match1", "t1p1Hero");
            loadCombo(m1t1p2Hero, "Match1", "t1p2Hero");
            loadCombo(m1t1p3Hero, "Match1", "t1p3Hero");
            loadCombo(m1t1p4Hero, "Match1", "t1p4Hero");
            loadCombo(m1t1p5Hero, "Match1", "t1p5Hero");
            loadCombo(m1t1p6Hero, "Match1", "t1p6Hero");

            loadCombo(m1t2p1Hero, "Match1", "t2p1Hero");
            loadCombo(m1t2p2Hero, "Match1", "t2p2Hero");
            loadCombo(m1t2p3Hero, "Match1", "t2p3Hero");
            loadCombo(m1t2p4Hero, "Match1", "t2p4Hero");
            loadCombo(m1t2p5Hero, "Match1", "t2p5Hero");
            loadCombo(m1t2p6Hero, "Match1", "t2p6Hero");
        }

        /// <summary>
        /// Method to load the player's roles for the series.
        /// </summary>
        private void LoadPlayerRoles()
        {
            loadCombo(m1t1p1Role, "Match1", "t1p1Role");
            loadCombo(m1t1p2Role, "Match1", "t1p2Role");
            loadCombo(m1t1p3Role, "Match1", "t1p3Role");
            loadCombo(m1t1p4Role, "Match1", "t1p4Role");
            loadCombo(m1t1p5Role, "Match1", "t1p5Role");
            loadCombo(m1t1p6Role, "Match1", "t1p6Role");

            loadCombo(m1t2p1Role, "Match1", "t2p1Role");
            loadCombo(m1t2p2Role, "Match1", "t2p2Role");
            loadCombo(m1t2p3Role, "Match1", "t2p3Role");
            loadCombo(m1t2p4Role, "Match1", "t2p4Role");
            loadCombo(m1t2p5Role, "Match1", "t2p5Role");
            loadCombo(m1t2p6Role, "Match1", "t2p6Role");
        }

        /// <summary>
        /// Method to load the maps played for the series.
        /// </summary>
        private void LoadMapsPlayed()
        {
            loadCombo(m1m1Map, "Match1", "m1Map");
            loadCombo(m1m2Map, "Match1", "m2Map");
            loadCombo(m1m3Map, "Match1", "m3Map");
            loadCombo(m1m4Map, "Match1", "m4Map");
            loadCombo(m1m5Map, "Match1", "m5Map");
            loadCombo(m1m6Map, "Match1", "m6Map");
            loadCombo(m1m7Map, "Match1", "m7Map");
        }

        /// <summary>
        /// Method to load the comboboxes in the bracket with the team names from the bracket team file
        /// </summary>
        private void LoadTeamsInBracket()
        {
            loadCombo(bracketR1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[32].Replace(".txt", ""));
            loadCombo(bracketR1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[33].Replace(".txt", ""));
            loadCombo(bracketR1M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[34].Replace(".txt", ""));
            loadCombo(bracketR1M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[35].Replace(".txt", ""));
            loadCombo(bracketR1M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[36].Replace(".txt", ""));
            loadCombo(bracketR1M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[37].Replace(".txt", ""));
            loadCombo(bracketR1M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[38].Replace(".txt", ""));
            loadCombo(bracketR1M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[39].Replace(".txt", ""));
            loadCombo(bracketR1M5Team1, FolderList[9].Replace("\\", ""), BracketFiles[40].Replace(".txt", ""));
            loadCombo(bracketR1M5Team2, FolderList[9].Replace("\\", ""), BracketFiles[41].Replace(".txt", ""));
            loadCombo(bracketR1M6Team1, FolderList[9].Replace("\\", ""), BracketFiles[42].Replace(".txt", ""));
            loadCombo(bracketR1M6Team2, FolderList[9].Replace("\\", ""), BracketFiles[43].Replace(".txt", ""));
            loadCombo(bracketR1M7Team1, FolderList[9].Replace("\\", ""), BracketFiles[44].Replace(".txt", ""));
            loadCombo(bracketR1M7Team2, FolderList[9].Replace("\\", ""), BracketFiles[45].Replace(".txt", ""));
            loadCombo(bracketR1M8Team1, FolderList[9].Replace("\\", ""), BracketFiles[46].Replace(".txt", ""));
            loadCombo(bracketR1M8Team2, FolderList[9].Replace("\\", ""), BracketFiles[47].Replace(".txt", ""));
            loadCombo(bracketQFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[48].Replace(".txt", ""));
            loadCombo(bracketQFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[49].Replace(".txt", ""));
            loadCombo(bracketQFM2Team1, FolderList[9].Replace("\\", ""), BracketFiles[50].Replace(".txt", ""));
            loadCombo(bracketQFM2Team2, FolderList[9].Replace("\\", ""), BracketFiles[51].Replace(".txt", ""));
            loadCombo(bracketQFM3Team1, FolderList[9].Replace("\\", ""), BracketFiles[52].Replace(".txt", ""));
            loadCombo(bracketQFM3Team2, FolderList[9].Replace("\\", ""), BracketFiles[53].Replace(".txt", ""));
            loadCombo(bracketQFM4Team1, FolderList[9].Replace("\\", ""), BracketFiles[54].Replace(".txt", ""));
            loadCombo(bracketQFM4Team2, FolderList[9].Replace("\\", ""), BracketFiles[55].Replace(".txt", ""));
            loadCombo(bracketSFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[56].Replace(".txt", ""));
            loadCombo(bracketSFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[57].Replace(".txt", ""));
            loadCombo(bracketSFM2Team1, FolderList[9].Replace("\\", ""), BracketFiles[58].Replace(".txt", ""));
            loadCombo(bracketSFM2Team2, FolderList[9].Replace("\\", ""), BracketFiles[59].Replace(".txt", ""));
            loadCombo(bracketWFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[60].Replace(".txt", ""));
            loadCombo(bracketWFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[61].Replace(".txt", ""));
            loadCombo(bracketLR1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[62].Replace(".txt", ""));
            loadCombo(bracketLR1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[63].Replace(".txt", ""));
            loadCombo(bracketLR1M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[64].Replace(".txt", ""));
            loadCombo(bracketLR1M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[65].Replace(".txt", ""));
            loadCombo(bracketLR1M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[66].Replace(".txt", ""));
            loadCombo(bracketLR1M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[67].Replace(".txt", ""));
            loadCombo(bracketLR1M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[68].Replace(".txt", ""));
            loadCombo(bracketLR1M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[69].Replace(".txt", ""));
            loadCombo(bracketLR2M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[70].Replace(".txt", ""));
            loadCombo(bracketLR2M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[71].Replace(".txt", ""));
            loadCombo(bracketLR2M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[72].Replace(".txt", ""));
            loadCombo(bracketLR2M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[73].Replace(".txt", ""));
            loadCombo(bracketLR2M3Team1, FolderList[9].Replace("\\", ""), BracketFiles[74].Replace(".txt", ""));
            loadCombo(bracketLR2M3Team2, FolderList[9].Replace("\\", ""), BracketFiles[75].Replace(".txt", ""));
            loadCombo(bracketLR2M4Team1, FolderList[9].Replace("\\", ""), BracketFiles[76].Replace(".txt", ""));
            loadCombo(bracketLR2M4Team2, FolderList[9].Replace("\\", ""), BracketFiles[77].Replace(".txt", ""));
            loadCombo(bracketLR3M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[78].Replace(".txt", ""));
            loadCombo(bracketLR3M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[79].Replace(".txt", ""));
            loadCombo(bracketLR3M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[80].Replace(".txt", ""));
            loadCombo(bracketLR3M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[81].Replace(".txt", ""));
            loadCombo(bracketLR4M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[82].Replace(".txt", ""));
            loadCombo(bracketLR4M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[83].Replace(".txt", ""));
            loadCombo(bracketLR4M2Team1, FolderList[9].Replace("\\", ""), BracketFiles[84].Replace(".txt", ""));
            loadCombo(bracketLR4M2Team2, FolderList[9].Replace("\\", ""), BracketFiles[85].Replace(".txt", ""));
            loadCombo(bracketLSFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[86].Replace(".txt", ""));
            loadCombo(bracketLSFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[87].Replace(".txt", ""));
            loadCombo(bracketLFM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[88].Replace(".txt", ""));
            loadCombo(bracketLFM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[89].Replace(".txt", ""));
            loadCombo(bracketGF1M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[90].Replace(".txt", ""));
            loadCombo(bracketGF1M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[91].Replace(".txt", ""));
            loadCombo(bracketGF2M1Team1, FolderList[9].Replace("\\", ""), BracketFiles[92].Replace(".txt", ""));
            loadCombo(bracketGF2M1Team2, FolderList[9].Replace("\\", ""), BracketFiles[93].Replace(".txt", ""));
            loadCombo(bracketTPM1Team1, FolderList[9].Replace("\\", ""), BracketFiles[94].Replace(".txt", ""));
            loadCombo(bracketTPM1Team2, FolderList[9].Replace("\\", ""), BracketFiles[95].Replace(".txt", ""));

        }

        /// <summary>
        /// Loads Custom Gametypes from Gametype Directory
        /// </summary>
        private void LoadCustomGametypes()
        {
            var serializer = new XmlSerializer(typeof(Gametype));
            string dir = path + FolderList[10];
            Gametype gametype = new Gametype();

            foreach (String file in Directory.GetFiles(dir, "*.gametype", SearchOption.AllDirectories))
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    try
                    {
                        gametype = (Gametype)serializer.Deserialize(reader);
                        Image image;
                        using (var temp = new Bitmap(path + gametype.IconPath))
                        {
                            image = new Bitmap(temp);
                        }
                        gametype.Icon = new Bitmap(image);
                        GametypeList.Add(gametype);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the Gametype, some data could be missing!");
                    }
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Loads Custom Heroes from Hero Directory
        /// </summary>
        private void LoadCustomHeroes()
        {
            var serializer = new XmlSerializer(typeof(Hero));
            string dir = path + FolderList[11];
            Hero hero = new Hero();

            foreach (String file in Directory.GetFiles(dir, "*.hero", SearchOption.AllDirectories))
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    try
                    {
                        hero = (Hero)serializer.Deserialize(reader);
                        Image image;
                        using (var temp = new Bitmap(path + hero.IconPath))
                        {
                            image = new Bitmap(temp);
                        }
                        hero.Icon = new Bitmap(image);
                        HeroList.Add(hero);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the Hero, some data could be missing!");
                    }
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Loads Custom Maps from Map Directory
        /// </summary>
        private void LoadCustomMaps()
        {
            var serializer = new XmlSerializer(typeof(Map));
            string dir = path + FolderList[12];
            Map map = new Map();

            foreach (String file in Directory.GetFiles(dir, "*.map", SearchOption.AllDirectories))
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    try
                    {
                        map = (Map)serializer.Deserialize(reader);
                        Image image;
                        using (var temp = new Bitmap(path + map.IconPath))
                        {
                            image = new Bitmap(temp);
                        }
                        map.Icon = new Bitmap(image);
                        if (!GametypeList.Any(n => n.Name == map.Gametype.Name))
                        {
                            MessageBox.Show("The Gametype associated with Map(" + map.Name + ") currently does not exist.");
                        }
                        MapList.Add(map);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the Map, some data could be missing!");
                    }
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Loads Custom Roles from Role Directory
        /// </summary>
        private void LoadCustomRoles()
        {
            var serializer = new XmlSerializer(typeof(Role));
            string dir = path + FolderList[13];
            Role role = new Role();

            foreach (String file in Directory.GetFiles(dir, "*.role", SearchOption.AllDirectories))
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    try
                    {
                        role = (Role)serializer.Deserialize(reader);
                        Image image;
                        using (var temp = new Bitmap(path + role.IconPath))
                        {
                            image = new Bitmap(temp);
                        }
                        role.Icon = new Bitmap(image);
                        RoleList.Add(role);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the Role, some data could be missing!");
                    }
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Loads the Bracket with the teams in the file system
        /// </summary>
        private void loadBracket()
        {
            loadText(bracketTeam1, FolderList[9].Replace("\\", ""), BracketFiles[0].Replace(".txt", ""));
            loadText(bracketTeam2, FolderList[9].Replace("\\", ""), BracketFiles[1].Replace(".txt", ""));
            loadText(bracketTeam3, FolderList[9].Replace("\\", ""), BracketFiles[2].Replace(".txt", ""));
            loadText(bracketTeam4, FolderList[9].Replace("\\", ""), BracketFiles[3].Replace(".txt", ""));
            loadText(bracketTeam5, FolderList[9].Replace("\\", ""), BracketFiles[4].Replace(".txt", ""));
            loadText(bracketTeam6, FolderList[9].Replace("\\", ""), BracketFiles[5].Replace(".txt", ""));
            loadText(bracketTeam7, FolderList[9].Replace("\\", ""), BracketFiles[6].Replace(".txt", ""));
            loadText(bracketTeam8, FolderList[9].Replace("\\", ""), BracketFiles[7].Replace(".txt", ""));
            loadText(bracketTeam9, FolderList[9].Replace("\\", ""), BracketFiles[8].Replace(".txt", ""));
            loadText(bracketTeam10, FolderList[9].Replace("\\", ""), BracketFiles[9].Replace(".txt", ""));
            loadText(bracketTeam11, FolderList[9].Replace("\\", ""), BracketFiles[10].Replace(".txt", ""));
            loadText(bracketTeam12, FolderList[9].Replace("\\", ""), BracketFiles[11].Replace(".txt", ""));
            loadText(bracketTeam13, FolderList[9].Replace("\\", ""), BracketFiles[12].Replace(".txt", ""));
            loadText(bracketTeam14, FolderList[9].Replace("\\", ""), BracketFiles[13].Replace(".txt", ""));
            loadText(bracketTeam15, FolderList[9].Replace("\\", ""), BracketFiles[14].Replace(".txt", ""));
            loadText(bracketTeam16, FolderList[9].Replace("\\", ""), BracketFiles[15].Replace(".txt", ""));

            loadButton(bracketImage1, FolderList[9].Replace("\\", ""), BracketFiles[16].Replace(".png", ""));
            loadButton(bracketImage2, FolderList[9].Replace("\\", ""), BracketFiles[17].Replace(".png", ""));
            loadButton(bracketImage3, FolderList[9].Replace("\\", ""), BracketFiles[18].Replace(".png", ""));
            loadButton(bracketImage4, FolderList[9].Replace("\\", ""), BracketFiles[19].Replace(".png", ""));
            loadButton(bracketImage5, FolderList[9].Replace("\\", ""), BracketFiles[20].Replace(".png", ""));
            loadButton(bracketImage6, FolderList[9].Replace("\\", ""), BracketFiles[21].Replace(".png", ""));
            loadButton(bracketImage7, FolderList[9].Replace("\\", ""), BracketFiles[22].Replace(".png", ""));
            loadButton(bracketImage8, FolderList[9].Replace("\\", ""), BracketFiles[23].Replace(".png", ""));
            loadButton(bracketImage9, FolderList[9].Replace("\\", ""), BracketFiles[24].Replace(".png", ""));
            loadButton(bracketImage10, FolderList[9].Replace("\\", ""), BracketFiles[25].Replace(".png", ""));
            loadButton(bracketImage11, FolderList[9].Replace("\\", ""), BracketFiles[26].Replace(".png", ""));
            loadButton(bracketImage12, FolderList[9].Replace("\\", ""), BracketFiles[27].Replace(".png", ""));
            loadButton(bracketImage13, FolderList[9].Replace("\\", ""), BracketFiles[28].Replace(".png", ""));
            loadButton(bracketImage14, FolderList[9].Replace("\\", ""), BracketFiles[29].Replace(".png", ""));
            loadButton(bracketImage15, FolderList[9].Replace("\\", ""), BracketFiles[30].Replace(".png", ""));
            loadButton(bracketImage16, FolderList[9].Replace("\\", ""), BracketFiles[31].Replace(".png", ""));

            linkBracketToTeams();

            setBracketTeams();

            LoadTeamsInBracket();
            
            loadScore(bracketR1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[96].Replace(".txt", ""));
            loadScore(bracketR1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[97].Replace(".txt", ""));
            loadScore(bracketR1M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[98].Replace(".txt", ""));
            loadScore(bracketR1M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[99].Replace(".txt", ""));
            loadScore(bracketR1M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[100].Replace(".txt", ""));
            loadScore(bracketR1M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[101].Replace(".txt", ""));
            loadScore(bracketR1M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[102].Replace(".txt", ""));
            loadScore(bracketR1M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[103].Replace(".txt", ""));
            loadScore(bracketR1M5Score1, FolderList[9].Replace("\\", ""), BracketFiles[104].Replace(".txt", ""));
            loadScore(bracketR1M5Score2, FolderList[9].Replace("\\", ""), BracketFiles[105].Replace(".txt", ""));
            loadScore(bracketR1M6Score1, FolderList[9].Replace("\\", ""), BracketFiles[106].Replace(".txt", ""));
            loadScore(bracketR1M6Score2, FolderList[9].Replace("\\", ""), BracketFiles[107].Replace(".txt", ""));
            loadScore(bracketR1M7Score1, FolderList[9].Replace("\\", ""), BracketFiles[108].Replace(".txt", ""));
            loadScore(bracketR1M7Score2, FolderList[9].Replace("\\", ""), BracketFiles[109].Replace(".txt", ""));
            loadScore(bracketR1M8Score1, FolderList[9].Replace("\\", ""), BracketFiles[110].Replace(".txt", ""));
            loadScore(bracketR1M8Score2, FolderList[9].Replace("\\", ""), BracketFiles[111].Replace(".txt", ""));
            loadScore(bracketQFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[112].Replace(".txt", ""));
            loadScore(bracketQFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[113].Replace(".txt", ""));
            loadScore(bracketQFM2Score1, FolderList[9].Replace("\\", ""), BracketFiles[114].Replace(".txt", ""));
            loadScore(bracketQFM2Score2, FolderList[9].Replace("\\", ""), BracketFiles[115].Replace(".txt", ""));
            loadScore(bracketQFM3Score1, FolderList[9].Replace("\\", ""), BracketFiles[116].Replace(".txt", ""));
            loadScore(bracketQFM3Score2, FolderList[9].Replace("\\", ""), BracketFiles[117].Replace(".txt", ""));
            loadScore(bracketQFM4Score1, FolderList[9].Replace("\\", ""), BracketFiles[118].Replace(".txt", ""));
            loadScore(bracketQFM4Score2, FolderList[9].Replace("\\", ""), BracketFiles[119].Replace(".txt", ""));
            loadScore(bracketSFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[120].Replace(".txt", ""));
            loadScore(bracketSFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[121].Replace(".txt", ""));
            loadScore(bracketSFM2Score1, FolderList[9].Replace("\\", ""), BracketFiles[122].Replace(".txt", ""));
            loadScore(bracketSFM2Score2, FolderList[9].Replace("\\", ""), BracketFiles[123].Replace(".txt", ""));
            loadScore(bracketWFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[124].Replace(".txt", ""));
            loadScore(bracketWFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[125].Replace(".txt", ""));
            loadScore(bracketLR1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[126].Replace(".txt", ""));
            loadScore(bracketLR1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[127].Replace(".txt", ""));
            loadScore(bracketLR1M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[128].Replace(".txt", ""));
            loadScore(bracketLR1M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[129].Replace(".txt", ""));
            loadScore(bracketLR1M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[130].Replace(".txt", ""));
            loadScore(bracketLR1M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[131].Replace(".txt", ""));
            loadScore(bracketLR1M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[132].Replace(".txt", ""));
            loadScore(bracketLR1M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[133].Replace(".txt", ""));
            loadScore(bracketLR2M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[134].Replace(".txt", ""));
            loadScore(bracketLR2M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[135].Replace(".txt", ""));
            loadScore(bracketLR2M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[136].Replace(".txt", ""));
            loadScore(bracketLR2M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[137].Replace(".txt", ""));
            loadScore(bracketLR2M3Score1, FolderList[9].Replace("\\", ""), BracketFiles[138].Replace(".txt", ""));
            loadScore(bracketLR2M3Score2, FolderList[9].Replace("\\", ""), BracketFiles[139].Replace(".txt", ""));
            loadScore(bracketLR2M4Score1, FolderList[9].Replace("\\", ""), BracketFiles[140].Replace(".txt", ""));
            loadScore(bracketLR2M4Score2, FolderList[9].Replace("\\", ""), BracketFiles[141].Replace(".txt", ""));
            loadScore(bracketLR3M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[142].Replace(".txt", ""));
            loadScore(bracketLR3M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[143].Replace(".txt", ""));
            loadScore(bracketLR3M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[144].Replace(".txt", ""));
            loadScore(bracketLR3M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[145].Replace(".txt", ""));
            loadScore(bracketLR4M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[146].Replace(".txt", ""));
            loadScore(bracketLR4M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[147].Replace(".txt", ""));
            loadScore(bracketLR4M2Score1, FolderList[9].Replace("\\", ""), BracketFiles[148].Replace(".txt", ""));
            loadScore(bracketLR4M2Score2, FolderList[9].Replace("\\", ""), BracketFiles[149].Replace(".txt", ""));
            loadScore(bracketLSFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[150].Replace(".txt", ""));
            loadScore(bracketLSFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[151].Replace(".txt", ""));
            loadScore(bracketLFM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[152].Replace(".txt", ""));
            loadScore(bracketLFM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[153].Replace(".txt", ""));
            loadScore(bracketGF1M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[154].Replace(".txt", ""));
            loadScore(bracketGF1M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[155].Replace(".txt", ""));
            loadScore(bracketGF2M1Score1, FolderList[9].Replace("\\", ""), BracketFiles[156].Replace(".txt", ""));
            loadScore(bracketGF2M1Score2, FolderList[9].Replace("\\", ""), BracketFiles[157].Replace(".txt", ""));
            loadScore(bracketTPM1Score1, FolderList[9].Replace("\\", ""), BracketFiles[158].Replace(".txt", ""));
            loadScore(bracketTPM1Score2, FolderList[9].Replace("\\", ""), BracketFiles[159].Replace(".txt", ""));

            loadScore(bracketTeam1W, FolderList[9].Replace("\\", ""), BracketFiles[176].Replace(".txt", ""));
            loadScore(bracketTeam1L, FolderList[9].Replace("\\", ""), BracketFiles[177].Replace(".txt", ""));
            loadScore(bracketTeam1MW, FolderList[9].Replace("\\", ""), BracketFiles[178].Replace(".txt", ""));
            loadScore(bracketTeam1ML, FolderList[9].Replace("\\", ""), BracketFiles[179].Replace(".txt", ""));
            loadScore(bracketTeam2W, FolderList[9].Replace("\\", ""), BracketFiles[180].Replace(".txt", ""));
            loadScore(bracketTeam2L, FolderList[9].Replace("\\", ""), BracketFiles[181].Replace(".txt", ""));
            loadScore(bracketTeam2MW, FolderList[9].Replace("\\", ""), BracketFiles[182].Replace(".txt", ""));
            loadScore(bracketTeam2ML, FolderList[9].Replace("\\", ""), BracketFiles[183].Replace(".txt", ""));
            loadScore(bracketTeam3W, FolderList[9].Replace("\\", ""), BracketFiles[184].Replace(".txt", ""));
            loadScore(bracketTeam3L, FolderList[9].Replace("\\", ""), BracketFiles[185].Replace(".txt", ""));
            loadScore(bracketTeam3MW, FolderList[9].Replace("\\", ""), BracketFiles[186].Replace(".txt", ""));
            loadScore(bracketTeam3ML, FolderList[9].Replace("\\", ""), BracketFiles[187].Replace(".txt", ""));
            loadScore(bracketTeam4W, FolderList[9].Replace("\\", ""), BracketFiles[188].Replace(".txt", ""));
            loadScore(bracketTeam4L, FolderList[9].Replace("\\", ""), BracketFiles[189].Replace(".txt", ""));
            loadScore(bracketTeam4MW, FolderList[9].Replace("\\", ""), BracketFiles[190].Replace(".txt", ""));
            loadScore(bracketTeam4ML, FolderList[9].Replace("\\", ""), BracketFiles[191].Replace(".txt", ""));
            loadScore(bracketTeam5W, FolderList[9].Replace("\\", ""), BracketFiles[192].Replace(".txt", ""));
            loadScore(bracketTeam5L, FolderList[9].Replace("\\", ""), BracketFiles[193].Replace(".txt", ""));
            loadScore(bracketTeam5MW, FolderList[9].Replace("\\", ""), BracketFiles[194].Replace(".txt", ""));
            loadScore(bracketTeam5ML, FolderList[9].Replace("\\", ""), BracketFiles[195].Replace(".txt", ""));
            loadScore(bracketTeam6W, FolderList[9].Replace("\\", ""), BracketFiles[196].Replace(".txt", ""));
            loadScore(bracketTeam6L, FolderList[9].Replace("\\", ""), BracketFiles[197].Replace(".txt", ""));
            loadScore(bracketTeam6MW, FolderList[9].Replace("\\", ""), BracketFiles[198].Replace(".txt", ""));
            loadScore(bracketTeam6ML, FolderList[9].Replace("\\", ""), BracketFiles[199].Replace(".txt", ""));
            loadScore(bracketTeam7W, FolderList[9].Replace("\\", ""), BracketFiles[200].Replace(".txt", ""));
            loadScore(bracketTeam7L, FolderList[9].Replace("\\", ""), BracketFiles[201].Replace(".txt", ""));
            loadScore(bracketTeam7MW, FolderList[9].Replace("\\", ""), BracketFiles[202].Replace(".txt", ""));
            loadScore(bracketTeam7ML, FolderList[9].Replace("\\", ""), BracketFiles[203].Replace(".txt", ""));
            loadScore(bracketTeam8W, FolderList[9].Replace("\\", ""), BracketFiles[204].Replace(".txt", ""));
            loadScore(bracketTeam8L, FolderList[9].Replace("\\", ""), BracketFiles[205].Replace(".txt", ""));
            loadScore(bracketTeam8MW, FolderList[9].Replace("\\", ""), BracketFiles[206].Replace(".txt", ""));
            loadScore(bracketTeam8ML, FolderList[9].Replace("\\", ""), BracketFiles[207].Replace(".txt", ""));
            loadScore(bracketTeam9W, FolderList[9].Replace("\\", ""), BracketFiles[208].Replace(".txt", ""));
            loadScore(bracketTeam9L, FolderList[9].Replace("\\", ""), BracketFiles[209].Replace(".txt", ""));
            loadScore(bracketTeam9MW, FolderList[9].Replace("\\", ""), BracketFiles[210].Replace(".txt", ""));
            loadScore(bracketTeam9ML, FolderList[9].Replace("\\", ""), BracketFiles[211].Replace(".txt", ""));
            loadScore(bracketTeam10W, FolderList[9].Replace("\\", ""), BracketFiles[212].Replace(".txt", ""));
            loadScore(bracketTeam10L, FolderList[9].Replace("\\", ""), BracketFiles[213].Replace(".txt", ""));
            loadScore(bracketTeam10MW, FolderList[9].Replace("\\", ""), BracketFiles[214].Replace(".txt", ""));
            loadScore(bracketTeam10ML, FolderList[9].Replace("\\", ""), BracketFiles[215].Replace(".txt", ""));
            loadScore(bracketTeam11W, FolderList[9].Replace("\\", ""), BracketFiles[216].Replace(".txt", ""));
            loadScore(bracketTeam11L, FolderList[9].Replace("\\", ""), BracketFiles[217].Replace(".txt", ""));
            loadScore(bracketTeam11MW, FolderList[9].Replace("\\", ""), BracketFiles[218].Replace(".txt", ""));
            loadScore(bracketTeam11ML, FolderList[9].Replace("\\", ""), BracketFiles[219].Replace(".txt", ""));
            loadScore(bracketTeam12W, FolderList[9].Replace("\\", ""), BracketFiles[220].Replace(".txt", ""));
            loadScore(bracketTeam12L, FolderList[9].Replace("\\", ""), BracketFiles[221].Replace(".txt", ""));
            loadScore(bracketTeam12MW, FolderList[9].Replace("\\", ""), BracketFiles[222].Replace(".txt", ""));
            loadScore(bracketTeam12ML, FolderList[9].Replace("\\", ""), BracketFiles[223].Replace(".txt", ""));
            loadScore(bracketTeam13W, FolderList[9].Replace("\\", ""), BracketFiles[224].Replace(".txt", ""));
            loadScore(bracketTeam13L, FolderList[9].Replace("\\", ""), BracketFiles[225].Replace(".txt", ""));
            loadScore(bracketTeam13MW, FolderList[9].Replace("\\", ""), BracketFiles[226].Replace(".txt", ""));
            loadScore(bracketTeam13ML, FolderList[9].Replace("\\", ""), BracketFiles[227].Replace(".txt", ""));
            loadScore(bracketTeam14W, FolderList[9].Replace("\\", ""), BracketFiles[228].Replace(".txt", ""));
            loadScore(bracketTeam14L, FolderList[9].Replace("\\", ""), BracketFiles[229].Replace(".txt", ""));
            loadScore(bracketTeam14MW, FolderList[9].Replace("\\", ""), BracketFiles[230].Replace(".txt", ""));
            loadScore(bracketTeam14ML, FolderList[9].Replace("\\", ""), BracketFiles[231].Replace(".txt", ""));
            loadScore(bracketTeam15W, FolderList[9].Replace("\\", ""), BracketFiles[232].Replace(".txt", ""));
            loadScore(bracketTeam15L, FolderList[9].Replace("\\", ""), BracketFiles[233].Replace(".txt", ""));
            loadScore(bracketTeam15MW, FolderList[9].Replace("\\", ""), BracketFiles[234].Replace(".txt", ""));
            loadScore(bracketTeam15ML, FolderList[9].Replace("\\", ""), BracketFiles[235].Replace(".txt", ""));
            loadScore(bracketTeam16W, FolderList[9].Replace("\\", ""), BracketFiles[236].Replace(".txt", ""));
            loadScore(bracketTeam16L, FolderList[9].Replace("\\", ""), BracketFiles[237].Replace(".txt", ""));
            loadScore(bracketTeam16MW, FolderList[9].Replace("\\", ""), BracketFiles[238].Replace(".txt", ""));
            loadScore(bracketTeam16ML, FolderList[9].Replace("\\", ""), BracketFiles[239].Replace(".txt", ""));

        }

        #endregion

        #region Resetters
        /// <summary>
        /// Resets the text in a text box
        /// </summary>
        private void resetText(TextBox field)
        {
            field.Text = "";
        }

        /// <summary>
        /// Resets the value in a Numeric Up Down
        /// </summary>
        private void resetScore(NumericUpDown field)
        {
            field.Value = 0;
        }

        /// <summary>
        ///  Resets the Radio Button list to have the default selected
        /// </summary>
        private void resetSide(RadioButton field)
        {
            field.Checked = true;
        }

        /// <summary>
        /// Resets the color dialog and button to transparent color
        /// </summary>
        private void resetColor(ColorDialog field, Button button)
        {
            field.Color = Color.Transparent;
            button.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Resets the description of a button, typically a stored path
        /// </summary>
        private void resetButton(Button field)
        {
            field.AccessibleDescription = "";
        }

        /// <summary>
        /// Resets the selected index of a ComboBox
        /// </summary>
        private void resetCombo(ComboBox field)
        {
            field.SelectedIndex = 0;
        }
        #endregion

        #region Importers

        /// <summary>
        /// Method for importing a series from an XML
        /// </summary>
        private void importSeries()
        {
            var serializer = new XmlSerializer(typeof(Series));
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path + FolderList[6];
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML-File | *.xml";
            DialogResult res = dialog.ShowDialog();
            Series series = new Series();
            if (res == DialogResult.OK)
            {
                using (XmlReader reader = XmlReader.Create(dialog.FileName))
                {
                    try
                    {
                        series = (Series)serializer.Deserialize(reader);
                        if (Convert.ToDecimal(series.Version) < Convert.ToDecimal(xmlVersion))
                        {
                            MessageBox.Show("The series you are trying to load may be an older version, some data could be missing!");
                        }
                        else if (Convert.ToDecimal(series.Version) > Convert.ToDecimal(xmlVersion))
                        {
                            MessageBox.Show("The series you are trying to load may be a newer version, some data could be missing!");
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the series, some data could be missing!");
                    }
                    reader.Close();
                }

                Match1 = series;
                Match1.Name = Path.GetFileNameWithoutExtension(dialog.FileName);
                loadSeries();

            }
        }

        /// <summary>
        /// Method for importing a teaam from an XML
        /// </summary>
        /// <param name="side">Side of the team</param>
        private void importTeam(string side)
        {
            var serializer = new XmlSerializer(typeof(Team));
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path + FolderList[7];
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML-File | *.xml";
            DialogResult res = dialog.ShowDialog();
            Team team = new Team();
            if (res == DialogResult.OK)
            {
                using (XmlReader reader = XmlReader.Create(dialog.FileName))
                {
                    try
                    {
                        team = (Team)serializer.Deserialize(reader);
                        if (Convert.ToDecimal(team.Version) < Convert.ToDecimal(xmlVersion))
                        {
                            MessageBox.Show("The team you are trying to load may be an older version, some data could be missing!");
                        }
                        else if (Convert.ToDecimal(team.Version) > Convert.ToDecimal(xmlVersion))
                        {
                            MessageBox.Show("The team you are trying to load may be a newer version, some data could be missing!");
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was an issue loading the team, some data could be missing!");
                    }
                    reader.Close();
                }
                if (side == "h")
                {
                    Match1.Home = team;
                    if (team.LogoPath != string.Empty)
                    {
                        Match1.Home.LogoPath = path + team.LogoPath;
                    }
                }
                else
                {
                    Match1.Away = team;
                    if (team.LogoPath != string.Empty)
                    {
                        Match1.Away.LogoPath = path + team.LogoPath;
                    }
                }
                loadTeams(side);
            }
        }

        #endregion

        #region Exporters

        /// <summary>
        /// Method for exporting series information to an XML
        /// </summary>
        private void exportSeries()
        {
            updateSeries();
            var serializer = new XmlSerializer(Match1.GetType());
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = path+FolderList[6];
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML-File | *.xml";
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                using (var writer = XmlWriter.Create(dialog.FileName))
                {
                    Match1.Name = Path.GetFileNameWithoutExtension(dialog.FileName);
                    serializer.Serialize(writer, Match1);
                }
            }
        }

        /// <summary>
        /// Method for exporting a teams information to an XML
        /// </summary>
        /// <param name="side">Side of the team</param>
        private void ExportTeam(string side)
        {
            updateTeams(side);
            var serializer = new XmlSerializer(new Team().GetType());
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = path + FolderList[7];
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML-File | *.xml";
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                using (var writer = XmlWriter.Create(dialog.FileName))
                {
                    if (side == "h")
                    {
                        ExportTeamLogo(Match1.Home);
                        serializer.Serialize(writer, Match1.Home);
                    }
                    else
                    {
                        ExportTeamLogo(Match1.Away);
                        serializer.Serialize(writer, Match1.Away);
                    }
                }
            }
        }
        
        private void ExportTeamLogo(Team team)
        {
            if(team.LogoPath != string.Empty)
            { 
                Bitmap teamLogo = new Bitmap(team.LogoPath);
                try
                {
                    team.LogoPath = FolderList[7] + "\\" + team.Name.Replace(":", "-").Replace(" ", "_") + ".png";
                    teamLogo.Save(path + team.LogoPath);
                }
                catch
                {
                    MessageBox.Show("There was an issue exporting the team, the team name might have unsupported characters!");
                }
            }
        }
        #endregion

        #region Boolean Methods

        /// <summary>
        /// Method that creates folders if they are missing
        /// </summary>
        private void CheckFolders()
        {
            foreach (var folder in FolderList)
            {
                if (!Directory.Exists(path + folder))
                {
                    Directory.CreateDirectory(path + folder);
                }

            }
        }

        /// <summary>
        /// Method that creates files if they are missing
        /// </summary>
        private void CheckFiles()
        {

            foreach (var file in Match1Files)
            {
                if(!File.Exists(path + FolderList[0] + "\\" + file))
                {
                    File.Create(path + FolderList[0] + "\\" + file).Close();
                }
            }

            foreach (var file in GeneralFiles)
            {
                if (!File.Exists(path + FolderList[1] + "\\" + file))
                {
                    File.Create(path + FolderList[1] + "\\" + file).Close();
                }
            }
            foreach (var file in PlayerFiles)
            {
                if (!File.Exists(path + FolderList[2] + "\\" + file))
                {
                    File.Create(path + FolderList[2] + "\\" + file).Close();
                }
            }

            foreach (var file in SettingFiles)
            {
                if (!File.Exists(path + FolderList[3] + "\\" + file))
                {
                    File.Create(path + FolderList[3] + "\\" + file).Close();
                }
            }

            foreach (var file in ReplayFiles)
            {
                if (!File.Exists(path + FolderList[4] + "\\" + file))
                {
                    File.Create(path + FolderList[4] + "\\" + file).Close();
                }
            }

            foreach (var file in BracketFiles)
            {
                if (!File.Exists(path + FolderList[9] + "\\" + file))
                {
                    File.Create(path + FolderList[9] + "\\" + file).Close();
                }
            }

        }

        /// <summary>
        /// Method that checks if too files are equal
        /// </summary>
        /// <param name="first">File 1's info</param>
        /// <param name="second">File 2's info</param>
        /// <returns>Boolean</returns>
        private bool IsFileEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            int iterations = (int)Math.Ceiling((double)first.Length / Bytes_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[Bytes_TO_READ];
                byte[] two = new byte[Bytes_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, Bytes_TO_READ);
                    fs2.Read(two, 0, Bytes_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region Generators

        /// <summary>
        /// Generates the Default Roles
        /// </summary>
        private void GenerateRoles()
        {
            DefaultRoleList.Add(new Role("DPS", Properties.Resources.Icon_dps, ""));
            DefaultRoleList.Add(new Role("Flex", Properties.Resources.Icon_flex, ""));
            DefaultRoleList.Add(new Role("Support", Properties.Resources.Icon_support, ""));
            DefaultRoleList.Add(new Role("Tank", Properties.Resources.Icon_tank, ""));
        }

        /// <summary>
        /// Generates the Default Heroes
        /// </summary>
        private void GenerateHeroes()
        {
            DefaultHeroList.Add(new Hero("Ana", Properties.Resources.Ana, ""));
            DefaultHeroList.Add(new Hero("Ashe", Properties.Resources.Ashe, ""));
            DefaultHeroList.Add(new Hero("Baptiste", Properties.Resources.Baptiste, ""));
            DefaultHeroList.Add(new Hero("Bastion", Properties.Resources.Bastion, ""));
            DefaultHeroList.Add(new Hero("Brigitte", Properties.Resources.Brigitte, ""));
            DefaultHeroList.Add(new Hero("Cassidy", Properties.Resources.Cassidy, ""));
            DefaultHeroList.Add(new Hero("D.Va", Properties.Resources.Dva, ""));
            DefaultHeroList.Add(new Hero("Doomfist", Properties.Resources.Doomfist, ""));
            DefaultHeroList.Add(new Hero("Echo", Properties.Resources.Echo, ""));
            DefaultHeroList.Add(new Hero("Genji", Properties.Resources.Genji, ""));
            DefaultHeroList.Add(new Hero("Hanzo", Properties.Resources.Hanzo, ""));
            DefaultHeroList.Add(new Hero("Illari", Properties.Resources.Illari, ""));
            DefaultHeroList.Add(new Hero("JunkerQueen", Properties.Resources.JunkerQueen, ""));
            DefaultHeroList.Add(new Hero("Junkrat", Properties.Resources.Junkrat, ""));
            DefaultHeroList.Add(new Hero("Juno", Properties.Resources.Juno, ""));
            DefaultHeroList.Add(new Hero("Kiriko", Properties.Resources.Kiriko, ""));
            DefaultHeroList.Add(new Hero("LifeWeaver", Properties.Resources.LifeWeaver, ""));
            DefaultHeroList.Add(new Hero("Lúcio", Properties.Resources.Lucio, ""));
            DefaultHeroList.Add(new Hero("Mauga", Properties.Resources.Mauga, ""));
            DefaultHeroList.Add(new Hero("Mei", Properties.Resources.Mei, ""));
            DefaultHeroList.Add(new Hero("Mercy", Properties.Resources.Mercy, ""));
            DefaultHeroList.Add(new Hero("Moira", Properties.Resources.Moira, ""));
            DefaultHeroList.Add(new Hero("Orisa", Properties.Resources.Orisa, ""));
            DefaultHeroList.Add(new Hero("Pharah", Properties.Resources.Pharah, ""));
            DefaultHeroList.Add(new Hero("Ramattra", Properties.Resources.Ramattra, ""));
            DefaultHeroList.Add(new Hero("Reaper", Properties.Resources.Reaper, ""));
            DefaultHeroList.Add(new Hero("Reinhardt", Properties.Resources.Reinhardt, ""));
            DefaultHeroList.Add(new Hero("Roadhog", Properties.Resources.Roadhog, ""));
            DefaultHeroList.Add(new Hero("Sigma", Properties.Resources.Sigma, ""));
            DefaultHeroList.Add(new Hero("Sojourn", Properties.Resources.Sojourn, ""));
            DefaultHeroList.Add(new Hero("Soldier: 76", Properties.Resources.Soldier76, ""));
            DefaultHeroList.Add(new Hero("Sombra", Properties.Resources.Sombra, ""));
            DefaultHeroList.Add(new Hero("Symmetra", Properties.Resources.Symmetra, ""));
            DefaultHeroList.Add(new Hero("Torbjörn", Properties.Resources.Torbjorn, ""));
            DefaultHeroList.Add(new Hero("Tracer", Properties.Resources.Tracer, ""));
            DefaultHeroList.Add(new Hero("Venture", Properties.Resources.Venture, ""));
            DefaultHeroList.Add(new Hero("Widowmaker", Properties.Resources.Widowmaker, ""));
            DefaultHeroList.Add(new Hero("Winston", Properties.Resources.Winston, ""));
            DefaultHeroList.Add(new Hero("Wrecking Ball", Properties.Resources.WreckingBall, ""));
            DefaultHeroList.Add(new Hero("Zarya", Properties.Resources.Zarya, ""));
            DefaultHeroList.Add(new Hero("Zenyatta", Properties.Resources.Zenyatta, ""));
        }

        /// <summary>
        /// Generates the Default Gametypes
        /// </summary>
        private void GenerateGametypes()
        {
            DefaultGametypeList.Add(new Gametype("Assault", Properties.Resources.Icon_assault, ""));
            DefaultGametypeList.Add(new Gametype("Control", Properties.Resources.Icon_control, ""));
            DefaultGametypeList.Add(new Gametype("Escort", Properties.Resources.Icon_escort, ""));
            DefaultGametypeList.Add(new Gametype("Hybrid", Properties.Resources.Icon_hybrid, ""));
            DefaultGametypeList.Add(new Gametype("Push", Properties.Resources.Icon_push, ""));
            DefaultGametypeList.Add(new Gametype("Flashpoint", Properties.Resources.Icon_flashpoint, ""));
            DefaultGametypeList.Add(new Gametype("Clash", Properties.Resources.Icon_clash, ""));


        }

        /// <summary>
        /// Generates the Default Maps
        /// </summary>
        private void GenerateMaps()
        {
            DefaultMapList.Add(new Map("Assault", DefaultGametypeList[1], Properties.Resources.Assault, ""));
            DefaultMapList.Add(new Map("Clash", DefaultGametypeList[7], Properties.Resources.Clash, ""));
            DefaultMapList.Add(new Map("Control", DefaultGametypeList[2], Properties.Resources.Control, ""));
            DefaultMapList.Add(new Map("Escort", DefaultGametypeList[3], Properties.Resources.Escort, ""));
            DefaultMapList.Add(new Map("Flashpoint", DefaultGametypeList[6], Properties.Resources.Flashpoint, ""));
            DefaultMapList.Add(new Map("Hybrid", DefaultGametypeList[4], Properties.Resources.Hybrid, ""));
            DefaultMapList.Add(new Map("Push", DefaultGametypeList[5], Properties.Resources.Push, ""));
            DefaultMapList.Add(new Map("Antartic Peninsula", DefaultGametypeList[2], Properties.Resources.Antartic_Peninsula, ""));
            DefaultMapList.Add(new Map("Blizzard World", DefaultGametypeList[4], Properties.Resources.Blizzard_World, ""));
            DefaultMapList.Add(new Map("Busan", DefaultGametypeList[2], Properties.Resources.Busan, ""));
            DefaultMapList.Add(new Map("Circuit Royale", DefaultGametypeList[3], Properties.Resources.Circuit_Royale, ""));
            DefaultMapList.Add(new Map("Colosseo", DefaultGametypeList[5], Properties.Resources.Colosseo, ""));
            DefaultMapList.Add(new Map("Dorado", DefaultGametypeList[3], Properties.Resources.Dorado, ""));
            DefaultMapList.Add(new Map("Eichenwalde", DefaultGametypeList[4], Properties.Resources.Eichenwalde, ""));
            DefaultMapList.Add(new Map("Esperança", DefaultGametypeList[5], Properties.Resources.Esperanca, ""));
            DefaultMapList.Add(new Map("Hanamura", DefaultGametypeList[1], Properties.Resources.Hanamura, ""));
            DefaultMapList.Add(new Map("Hanaoka", DefaultGametypeList[7], Properties.Resources.Hanaoka, ""));
            DefaultMapList.Add(new Map("Havana", DefaultGametypeList[3], Properties.Resources.Havana, ""));
            DefaultMapList.Add(new Map("Hollywood", DefaultGametypeList[4], Properties.Resources.Hollywood, ""));
            DefaultMapList.Add(new Map("Horizon Lunar Colony", DefaultGametypeList[1], Properties.Resources.Horizon_Lundar_Colony, ""));
            DefaultMapList.Add(new Map("Ilios", DefaultGametypeList[2], Properties.Resources.Ilios, ""));
            DefaultMapList.Add(new Map("Junkertown", DefaultGametypeList[3], Properties.Resources.Junkertown, ""));
            DefaultMapList.Add(new Map("King\'s Row", DefaultGametypeList[4], Properties.Resources.King_s_Row, ""));
            DefaultMapList.Add(new Map("Lijiang Tower", DefaultGametypeList[2], Properties.Resources.Lijiang_Tower, ""));
            DefaultMapList.Add(new Map("Midtown", DefaultGametypeList[4], Properties.Resources.Midtown, ""));
            DefaultMapList.Add(new Map("Paris", DefaultGametypeList[1], Properties.Resources.Paris, ""));
            DefaultMapList.Add(new Map("Nepal", DefaultGametypeList[2], Properties.Resources.Nepal, ""));
            DefaultMapList.Add(new Map("New Junk City", DefaultGametypeList[6], Properties.Resources.New_Junk_City, ""));
            DefaultMapList.Add(new Map("Numbani", DefaultGametypeList[4], Properties.Resources.Numbani, ""));
            DefaultMapList.Add(new Map("Oasis", DefaultGametypeList[2], Properties.Resources.Oasis, ""));
            DefaultMapList.Add(new Map("Paraíso", DefaultGametypeList[4], Properties.Resources.Paraiso, ""));
            DefaultMapList.Add(new Map("Paris", DefaultGametypeList[2], Properties.Resources.Paris, ""));
            DefaultMapList.Add(new Map("Rialto", DefaultGametypeList[3], Properties.Resources.Rialto, ""));
            DefaultMapList.Add(new Map("Route 66", DefaultGametypeList[3], Properties.Resources.Route_66, ""));
            DefaultMapList.Add(new Map("Runasapi", DefaultGametypeList[5], Properties.Resources.Runasapi, ""));
            DefaultMapList.Add(new Map("Samoa", DefaultGametypeList[3], Properties.Resources.Samoa, ""));
            DefaultMapList.Add(new Map("Shambali", DefaultGametypeList[3], Properties.Resources.Shambali, ""));
            DefaultMapList.Add(new Map("Suravasa", DefaultGametypeList[6], Properties.Resources.Suravasa, ""));
            DefaultMapList.Add(new Map("Temple of Anubis", DefaultGametypeList[1], Properties.Resources.Temple_of_Anubis, ""));
            DefaultMapList.Add(new Map("Throne of Anubis", DefaultGametypeList[7], Properties.Resources.Throne_of_Anubis, ""));
            DefaultMapList.Add(new Map("Toronto", DefaultGametypeList[5], Properties.Resources.Toronto, ""));
            DefaultMapList.Add(new Map("Volskaya Industries", DefaultGametypeList[1], Properties.Resources.Volskaya_Industries, ""));
            DefaultMapList.Add(new Map("Watchpoint: Gibraltar", DefaultGametypeList[3], Properties.Resources.Watchpoint_Gibraltar, ""));

        }

        /// <summary>
        /// Generates a list of all files that exist
        /// </summary>
        private void GenerateFileList()
        {
            Match1Files = new List<string>
            {
                "DivisionNumber.txt",
                "m1Map.png",
                "m1Map.txt",
                "m1MapGametype.png",
                "m1MapGametype.txt",
                "m1MapWin.png",
                "m1MapWin.txt",
                "m1t1Score.txt",
                "m1t2Score.txt",
                "m2Map.png",
                "m2Map.txt",
                "m2MapGametype.png",
                "m2MapGametype.txt",
                "m2MapWin.png",
                "m2MapWin.txt",
                "m2t1Score.txt",
                "m2t2Score.txt",
                "m3Map.png",
                "m3Map.txt",
                "m3MapGametype.png",
                "m3MapGametype.txt",
                "m3MapWin.png",
                "m3MapWin.txt",
                "m3t1Score.txt",
                "m3t2Score.txt",
                "m4Map.png",
                "m4Map.txt",
                "m4MapGametype.png",
                "m4MapGametype.txt",
                "m4MapWin.png",
                "m4MapWin.txt",
                "m4t1Score.txt",
                "m4t2Score.txt",
                "m5Map.png",
                "m5Map.txt",
                "m5MapGametype.png",
                "m5MapGametype.txt",
                "m5MapWin.png",
                "m5MapWin.txt",
                "m5t1Score.txt",
                "m5t2Score.txt",
                "m6Map.png",
                "m6Map.txt",
                "m6MapGametype.png",
                "m6MapGametype.txt",
                "m6MapWin.png",
                "m6MapWin.txt",
                "m6t1Score.txt",
                "m6t2Score.txt",
                "m7Map.png",
                "m7Map.txt",
                "m7MapGametype.png",
                "m7MapGametype.txt",
                "m7MapWin.png",
                "m7MapWin.txt",
                "m7t1Score.txt",
                "m7t2Score.txt",
                "MessageBox.txt",
                "t1Logo.png",
                "t1Logo.txt",
                "t1Name.txt",
                "t1Color.txt",
                "t1p1Hero.png",
                "t1p1Hero.txt",
                "t1p1Name.txt",
                "t1p1Role.png",
                "t1p1Role.txt",
                "t1p1Info.txt",
                "t1p1Image.png",
                "t1p2Hero.png",
                "t1p2Hero.txt",
                "t1p2Name.txt",
                "t1p2Role.png",
                "t1p2Role.txt",
                "t1p2Info.txt",
                "t1p2Image.png",
                "t1p3Hero.png",
                "t1p3Hero.txt",
                "t1p3Name.txt",
                "t1p3Role.png",
                "t1p3Role.txt",
                "t1p3Info.txt",
                "t1p3Image.png",
                "t1p4Hero.png",
                "t1p4Hero.txt",
                "t1p4Name.txt",
                "t1p4Role.png",
                "t1p4Role.txt",
                "t1p4Info.txt",
                "t1p4Image.png",
                "t1p5Hero.png",
                "t1p5Hero.txt",
                "t1p5Name.txt",
                "t1p5Role.png",
                "t1p5Role.txt",
                "t1p5Info.txt",
                "t1p5Image.png",
                "t1p6Hero.png",
                "t1p6Hero.txt",
                "t1p6Name.txt",
                "t1p6Role.png",
                "t1p6Role.txt",
                "t1p6Info.txt",
                "t1p6Image.png",
                "t1Score.txt",
                "t1Side.png",
                "t1SR.txt",
                "t2Logo.txt",
                "t2Logo.png",
                "t2Name.txt",
                "t2Color.txt",
                "t2p1Hero.png",
                "t2p1Hero.txt",
                "t2p1Name.txt",
                "t2p1Role.png",
                "t2p1Role.txt",
                "t2p1Info.txt",
                "t2p1Image.png",
                "t2p2Hero.png",
                "t2p2Hero.txt",
                "t2p2Name.txt",
                "t2p2Role.png",
                "t2p2Role.txt",
                "t2p2Info.txt",
                "t2p2Image.png",
                "t2p3Hero.png",
                "t2p3Hero.txt",
                "t2p3Name.txt",
                "t2p3Role.png",
                "t2p3Role.txt",
                "t2p3Info.txt",
                "t2p3Image.png",
                "t2p4Hero.png",
                "t2p4Hero.txt",
                "t2p4Name.txt",
                "t2p4Role.png",
                "t2p4Role.txt",
                "t2p4Info.txt",
                "t2p4Image.png",
                "t2p5Hero.png",
                "t2p5Hero.txt",
                "t2p5Name.txt",
                "t2p5Role.png",
                "t2p5Role.txt",
                "t2p5Info.txt",
                "t2p5Image.png",
                "t2p6Hero.png",
                "t2p6Hero.txt",
                "t2p6Name.txt",
                "t2p6Role.png",
                "t2p6Role.txt",
                "t2p6Info.txt",
                "t2p6Image.png",
                "t2Score.txt",
                "t2Side.png",
                "t2SR.txt"
            };

            GeneralFiles = new List<string>
            {
                "message.txt",
                "host.txt",
                "caster1.txt",
                "caster2.txt",
                "analyst1.txt",
                "analyst2.txt",
                "utility1.txt", //6
                "utility2.txt",
                "utility3.txt",
                "utility4.txt",
                "utility5.txt",
                "utility6.txt",
                "utility7.txt",
                "utility8.txt",
                "utility9.txt",
                "utility10.txt",
                "utility11.txt",
                "utility12.txt",
                "utility13.txt",
                "utility14.txt",
                "utility15.txt",
                "utility16.txt",
                "utility17.txt",
                "utility18.txt",
                "utility19.txt",
                "utility20.txt",
                "utility21.txt",
                "utility22.txt",
                "image1Path.txt", //28
                "image2Path.txt",
                "image3Path.txt",
                "image4Path.txt",
                "image5Path.txt",
                "image6Path.txt",
                "image7Path.txt",
                "image8Path.txt",
                "image9Path.txt",
                "image10Path.txt",
                "image11Path.txt",
                "image12Path.txt",
                "image13Path.txt",
                "image14Path.txt",
                "image15Path.txt",
                "image16Path.txt",
                "image17Path.txt",
                "image18Path.txt",
                "image19Path.txt",
                "image20Path.txt",
                "image21Path.txt",
                "image22Path.txt",
                "image23Path.txt",
                "image24Path.txt",
                "image25Path.txt",
                "image26Path.txt",
                "image27Path.txt",
                "image28Path.txt",
                "colorTheme1.txt", //56
                "colorTheme2.txt",
                "colorTheme3.txt",
                "image1.png", //59
                "image2.png",
                "image3.png",
                "image4.png",
                "image5.png",
                "image6.png",
                "image7.png",
                "image8.png",
                "image9.png",
                "image10.png",
                "image11.png",
                "image12.png",
                "image13.png",
                "image14.png",
                "image15.png",
                "image16.png",
                "image17.png",
                "image18.png",
                "image19.png",
                "image20.png",
                "image21.png",
                "image22.png",
                "image23.png",
                "image24.png",
                "image25.png",
                "image26.png",
                "image27.png",
                "image28.png",
                "colorTheme4.txt", //87
                "colorTheme5.txt",
                "colorTheme6.txt",
                "colorTheme7.txt", 
                "colorTheme8.txt"
            };

            PlayerFiles = new List<string>
            {
                "hero1.png",
                "hero1.txt",
                "hero2.png",
                "hero2.txt",
                "hero3.png",
                "hero3.txt",
                "logo.png",
                "logo.txt",
                "image.png",
                "image.txt",
                "player.txt",
                "role.png",
                "role.txt",
                "sr.txt",
                "team.txt",
                "info.txt"
            };

            ReplayFiles = new List<string>
            {
                "Replay Replay.mp4",
                "Desk Replay.mp4",
                "ReplayPath.txt"
            };

            SettingFiles = new List<string>
            {

            };

            BracketFiles = new List<string>
            {
                "bracketTeam1.txt",
                "bracketTeam2.txt",
                "bracketTeam3.txt",
                "bracketTeam4.txt",
                "bracketTeam5.txt",
                "bracketTeam6.txt",
                "bracketTeam7.txt",
                "bracketTeam8.txt",
                "bracketTeam9.txt",
                "bracketTeam10.txt",
                "bracketTeam11.txt",
                "bracketTeam12.txt",
                "bracketTeam13.txt",
                "bracketTeam14.txt",
                "bracketTeam15.txt",
                "bracketTeam16.txt",

                "bracketTeam1.png",
                "bracketTeam2.png",
                "bracketTeam3.png",
                "bracketTeam4.png",
                "bracketTeam5.png",
                "bracketTeam6.png",
                "bracketTeam7.png",
                "bracketTeam8.png",
                "bracketTeam9.png",
                "bracketTeam10.png",
                "bracketTeam11.png",
                "bracketTeam12.png",
                "bracketTeam13.png",
                "bracketTeam14.png",
                "bracketTeam15.png",
                "bracketTeam16.png",

                "bracketR1M1Team1.txt",
                "bracketR1M1Team2.txt",
                "bracketR1M2Team1.txt",
                "bracketR1M2Team2.txt",
                "bracketR1M3Team1.txt",
                "bracketR1M3Team2.txt",
                "bracketR1M4Team1.txt",
                "bracketR1M4Team2.txt",
                "bracketR1M5Team1.txt",
                "bracketR1M5Team2.txt",
                "bracketR1M6Team1.txt",
                "bracketR1M6Team2.txt",
                "bracketR1M7Team1.txt",
                "bracketR1M7Team2.txt",
                "bracketR1M8Team1.txt",
                "bracketR1M8Team2.txt",
                "bracketQFM1Team1.txt",
                "bracketQFM1Team2.txt",
                "bracketQFM2Team1.txt",
                "bracketQFM2Team2.txt",
                "bracketQFM3Team1.txt",
                "bracketQFM3Team2.txt",
                "bracketQFM4Team1.txt",
                "bracketQFM4Team2.txt",
                "bracketSFM1Team1.txt",
                "bracketSFM1Team2.txt",
                "bracketSFM2Team1.txt",
                "bracketSFM2Team2.txt",
                "bracketWFM1Team1.txt",
                "bracketWFM1Team2.txt",
                "bracketLR1M1Team1.txt",
                "bracketLR1M1Team2.txt",
                "bracketLR1M2Team1.txt",
                "bracketLR1M2Team2.txt",
                "bracketLR1M3Team1.txt",
                "bracketLR1M3Team2.txt",
                "bracketLR1M4Team1.txt",
                "bracketLR1M4Team2.txt",
                "bracketLR2M1Team1.txt",
                "bracketLR2M1Team2.txt",
                "bracketLR2M2Team1.txt",
                "bracketLR2M2Team2.txt",
                "bracketLR2M3Team1.txt",
                "bracketLR2M3Team2.txt",
                "bracketLR2M4Team1.txt",
                "bracketLR2M4Team2.txt",
                "bracketLR3M1Team1.txt",
                "bracketLR3M1Team2.txt",
                "bracketLR3M2Team1.txt",
                "bracketLR3M2Team2.txt",
                "bracketLR4M1Team1.txt",
                "bracketLR4M1Team2.txt",
                "bracketLR4M2Team1.txt",
                "bracketLR4M2Team2.txt",
                "bracketLSFM1Team1.txt",
                "bracketLSFM1Team2.txt",
                "bracketLFM1Team1.txt",
                "bracketLFM1Team2.txt",
                "bracketGF1M1Team1.txt",
                "bracketGF1M1Team2.txt",
                "bracketGF2M1Team1.txt",
                "bracketGF2M1Team2.txt",
                "bracketTPM1Team1.txt",
                "bracketTPM1Team2.txt",
                "bracketR1M1Score1.txt",
                "bracketR1M1Score2.txt",
                "bracketR1M2Score1.txt",
                "bracketR1M2Score2.txt",
                "bracketR1M3Score1.txt",
                "bracketR1M3Score2.txt",
                "bracketR1M4Score1.txt",
                "bracketR1M4Score2.txt",
                "bracketR1M5Score1.txt",
                "bracketR1M5Score2.txt",
                "bracketR1M6Score1.txt",
                "bracketR1M6Score2.txt",
                "bracketR1M7Score1.txt",
                "bracketR1M7Score2.txt",
                "bracketR1M8Score1.txt",
                "bracketR1M8Score2.txt",
                "bracketQFM1Score1.txt",
                "bracketQFM1Score2.txt",
                "bracketQFM2Score1.txt",
                "bracketQFM2Score2.txt",
                "bracketQFM3Score1.txt",
                "bracketQFM3Score2.txt",
                "bracketQFM4Score1.txt",
                "bracketQFM4Score2.txt",
                "bracketSFM1Score1.txt",
                "bracketSFM1Score2.txt",
                "bracketSFM2Score1.txt",
                "bracketSFM2Score2.txt",
                "bracketWFM1Score1.txt",
                "bracketWFM1Score2.txt",
                "bracketLR1M1Score1.txt",
                "bracketLR1M1Score2.txt",
                "bracketLR1M2Score1.txt",
                "bracketLR1M2Score2.txt",
                "bracketLR1M3Score1.txt",
                "bracketLR1M3Score2.txt",
                "bracketLR1M4Score1.txt",
                "bracketLR1M4Score2.txt",
                "bracketLR2M1Score1.txt",
                "bracketLR2M1Score2.txt",
                "bracketLR2M2Score1.txt",
                "bracketLR2M2Score2.txt",
                "bracketLR2M3Score1.txt",
                "bracketLR2M3Score2.txt",
                "bracketLR2M4Score1.txt",
                "bracketLR2M4Score2.txt",
                "bracketLR3M1Score1.txt",
                "bracketLR3M1Score2.txt",
                "bracketLR3M2Score1.txt",
                "bracketLR3M2Score2.txt",
                "bracketLR4M1Score1.txt",
                "bracketLR4M1Score2.txt",
                "bracketLR4M2Score1.txt",
                "bracketLR4M2Score2.txt",
                "bracketLSFM1Score1.txt",
                "bracketLSFM1Score2.txt",
                "bracketLFM1Score1.txt",
                "bracketLFM1Score2.txt",
                "bracketGF1M1Score1.txt",
                "bracketGF1M1Score2.txt",
                "bracketGF2M1Score1.txt",
                "bracketGF2M1Score2.txt",
                "bracketTPM1Score1.txt",
                "bracketTPM1Score2.txt",

                "bracketTeam1Path.txt",
                "bracketTeam2Path.txt",
                "bracketTeam3Path.txt",
                "bracketTeam4Path.txt",
                "bracketTeam5Path.txt",
                "bracketTeam6Path.txt",
                "bracketTeam7Path.txt",
                "bracketTeam8Path.txt",
                "bracketTeam9Path.txt",
                "bracketTeam10Path.txt",
                "bracketTeam11Path.txt",
                "bracketTeam12Path.txt",
                "bracketTeam13Path.txt",
                "bracketTeam14Path.txt",
                "bracketTeam15Path.txt",
                "bracketTeam16Path.txt",

                "bracketTeam1W.txt",
                "bracketTeam1L.txt",
                "bracketTeam1MW.txt",
                "bracketTeam1ML.txt",
                "bracketTeam2W.txt",
                "bracketTeam2L.txt",
                "bracketTeam2MW.txt",
                "bracketTeam2ML.txt",
                "bracketTeam3W.txt",
                "bracketTeam3L.txt",
                "bracketTeam3MW.txt",
                "bracketTeam3ML.txt",
                "bracketTeam4W.txt",
                "bracketTeam4L.txt",
                "bracketTeam4MW.txt",
                "bracketTeam4ML.txt",
                "bracketTeam5W.txt",
                "bracketTeam5L.txt",
                "bracketTeam5MW.txt",
                "bracketTeam5ML.txt",
                "bracketTeam6W.txt",
                "bracketTeam6L.txt",
                "bracketTeam6MW.txt",
                "bracketTeam6ML.txt",
                "bracketTeam7W.txt",
                "bracketTeam7L.txt",
                "bracketTeam7MW.txt",
                "bracketTeam7ML.txt",
                "bracketTeam8W.txt",
                "bracketTeam8L.txt",
                "bracketTeam8MW.txt",
                "bracketTeam8ML.txt",
                "bracketTeam9W.txt",
                "bracketTeam9L.txt",
                "bracketTeam9MW.txt",
                "bracketTeam9ML.txt",
                "bracketTeam10W.txt",
                "bracketTeam10L.txt",
                "bracketTeam10MW.txt",
                "bracketTeam10ML.txt",
                "bracketTeam11W.txt",
                "bracketTeam11L.txt",
                "bracketTeam11MW.txt",
                "bracketTeam11ML.txt",
                "bracketTeam12W.txt",
                "bracketTeam12L.txt",
                "bracketTeam12MW.txt",
                "bracketTeam12ML.txt",
                "bracketTeam13W.txt",
                "bracketTeam13L.txt",
                "bracketTeam13MW.txt",
                "bracketTeam13ML.txt",
                "bracketTeam14W.txt",
                "bracketTeam14L.txt",
                "bracketTeam14MW.txt",
                "bracketTeam14ML.txt",
                "bracketTeam15W.txt",
                "bracketTeam15L.txt",
                "bracketTeam15MW.txt",
                "bracketTeam15ML.txt",
                "bracketTeam16W.txt",
                "bracketTeam16L.txt",
                "bracketTeam16MW.txt",
                "bracketTeam16ML.txt"
            };

        }

        /// <summary>
        /// Generates a list of all folders that exist
        /// </summary>
        private void GenerateFolderList()
        {
            FolderList = new List<string>
            {
                "\\Match1",
                "\\General",
                "\\Player",
                "\\Settings",
                "\\Replay",
                "\\Replay\\Playlist",
                "\\Series",
                "\\Team",
                "\\Temp",
                "\\Bracket",
                "\\Gametypes",
                "\\Heroes",
                "\\Maps",
                "\\Roles"
            };
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler that saves a copy of the most recent replay to the playlist folder
        /// </summary>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string filelessPath = Path.GetDirectoryName(e.FullPath);
            string copyPath = filelessPath + "\\Playlist";
            System.IO.DirectoryInfo di = new DirectoryInfo(copyPath);
            int count = di.GetFiles().Length;
            int prefix = count + 1;
            string previousfile = count.ToString() + "-" + e.Name;
            string copiedFile = prefix.ToString() + "-" + e.Name;
            string copiedFullPath = Path.Combine(copyPath, copiedFile);
            string previousFullPath = Path.Combine(copyPath, previousfile);

            if (!File.Exists(copiedFullPath))
            {
                if (count != 0)
                {
                    if (!IsFileEqual(new FileInfo(previousFullPath), new FileInfo(e.FullPath)))
                    {
                        Thread.Sleep(1000);
                        File.Copy(e.FullPath, copiedFullPath);
                        BeginInvoke(new Action(() => addReplay(copiedFullPath)));
                        using (StreamWriter sw = File.CreateText(path + FolderList[4] + "\\" + ReplayFiles[2]))
                        {
                            sw.WriteLine(copiedFullPath.Replace(path + FolderList[5] + "\\", ""));
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                    File.Copy(e.FullPath, copiedFullPath);
                    BeginInvoke(new Action(() => addReplay(copiedFullPath)));
                    using (StreamWriter sw = File.CreateText(path + FolderList[4] + "\\" + ReplayFiles[2]))
                    {
                        sw.WriteLine(copiedFullPath.Replace(path + FolderList[5] + "\\", ""));
                    }
                }
            }
        }

        /// <summary>
        /// Event handling when the selected team changes
        /// </summary>
        private void replayTeamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            replayPlayersCheckedListBox.Items.Clear();

            if (replayTeamComboBox.SelectedIndex == 1)
            {
                foreach (Player player in Match1.Home.Players)
                {
                    replayPlayersCheckedListBox.Items.Insert(0, player.Name);
                }
            }
            else if (replayTeamComboBox.SelectedIndex == 2)
            {
                foreach (Player player in Match1.Away.Players)
                {
                    replayPlayersCheckedListBox.Items.Insert(0, player.Name);
                }
            }
            else
            {
                foreach (Player player in Match1.Home.Players)
                {
                    replayPlayersCheckedListBox.Items.Insert(0, player.Name);
                }
                foreach (Player player in Match1.Away.Players)
                {
                    replayPlayersCheckedListBox.Items.Insert(0, player.Name);
                }
            }
            if (replayListView.SelectedItems.Count > 0)
            {
                Replay selctedReplay = Replays[replayListView.SelectedItems[0].Index];
                if (replayTeamComboBox.Text == Match1.Home.Name)
                {
                    selctedReplay.Team = Match1.Home;
                }
                else if(replayTeamComboBox.Text == Match1.Away.Name)
                {
                    selctedReplay.Team = Match1.Away;
                }
                else
                {
                    selctedReplay.Team = new Team();
                }
                
                replayListView.SelectedItems[0].SubItems[1].Text = replayTeamComboBox.Text;
            }
        }

        /// <summary>
        /// Event handling when the selected map changes
        /// </summary>
        private void replayMapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                Replay selctedReplay = Replays[replayListView.SelectedItems[0].Index];

                if (replayMapComboBox.Text == Match1.Games[0].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[0].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[1].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[1].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[1].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[1].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[2].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[2].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[3].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[3].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[4].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[4].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[5].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[5].Map;
                }
                else if (replayMapComboBox.Text == Match1.Games[6].Map.Name)
                {
                    selctedReplay.Map = Match1.Games[6].Map;
                }
                else
                {
                    selctedReplay.Map = new Map();
                }
                replayListView.SelectedItems[0].SubItems[2].Text = replayMapComboBox.Text;
            }
        }

        /// <summary>
        /// Event handling when the description changes
        /// </summary>
        private void replayRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                Replay selctedReplay = Replays[replayListView.SelectedItems[0].Index];
                selctedReplay.Description = replayRichTextBox.Text;
                replayListView.SelectedItems[0].SubItems[3].Text = replayRichTextBox.Text;
            }
        }

        /// <summary>
        /// Event handling when the selected players change
        /// </summary>
        private void playersCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                Replays[replayListView.SelectedItems[0].Index].Players.Clear();
                if (replayTeamComboBox.SelectedIndex == 1)
                {
                    foreach (Player player in Match1.Home.Players)
                    {
                        if (replayPlayersCheckedListBox.CheckedItems.Contains(player.Name.Trim()))
                        {
                            Replays[replayListView.SelectedItems[0].Index].Players.Add(player);
                        }
                    }
                }
                else if (replayTeamComboBox.SelectedIndex == 2)
                {
                    foreach (Player player in Match1.Away.Players)
                    {
                        if (replayPlayersCheckedListBox.CheckedItems.Contains(player.Name.Trim()))
                        {
                            Replays[replayListView.SelectedItems[0].Index].Players.Add(player);
                        }
                    }
                }
                else
                {
                    foreach (Player player in Match1.Home.Players)
                    {
                        if (replayPlayersCheckedListBox.CheckedItems.Contains(player.Name.Trim()))
                        {
                            Replays[replayListView.SelectedItems[0].Index].Players.Add(player);
                        }
                    }
                    foreach (Player player in Match1.Away.Players)
                    {
                        if (replayPlayersCheckedListBox.CheckedItems.Contains(player.Name.Trim()))
                        {
                            Replays[replayListView.SelectedItems[0].Index].Players.Add(player);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handling when the selected replay changes
        /// </summary>
        private void replayListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (replayListView.SelectedItems.Count > 0)
            {
                Replay selctedReplay = Replays[replayListView.SelectedItems[0].Index];

                if (selctedReplay.Description != null)
                {
                    replayRichTextBox.Text = selctedReplay.Description;
                }
                else
                {
                    replayRichTextBox.Text = "";
                }
                if (selctedReplay.Team != null)
                {
                    replayTeamComboBox.SelectedIndex = replayTeamComboBox.FindStringExact(selctedReplay.Team.Name);
                }
                else
                {
                    replayTeamComboBox.SelectedIndex = 0;
                }
                if (selctedReplay.Map != null)
                {
                    replayMapComboBox.SelectedIndex = replayMapComboBox.FindStringExact(selctedReplay.Map.Name);
                }
                else
                {
                    replayMapComboBox.SelectedIndex = 0;
                }
                if(selctedReplay.Players.Count != 0)
                {
                    foreach(Player player in selctedReplay.Players)
                    {
                        if (replayPlayersCheckedListBox.Items.Contains(player.Name))
                        {
                            replayPlayersCheckedListBox.SetItemCheckState(replayPlayersCheckedListBox.Items.IndexOf(player.Name), CheckState.Checked);
                        }
                    }
                }
                else
                {
                    foreach(int i in replayPlayersCheckedListBox.CheckedIndices)
                    {
                        replayPlayersCheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
                    }
                    
                }
                
            }
        }

        /// <summary>
        /// Prevents Numeric Up Downs from being scrollable to increase/decrease
        /// </summary>
        /// <param name="sender">Numeric Up Down being scrolled on</param>
        /// <param name="e">Event</param>
        private void preventMouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        /// <summary>
        /// Shows the image path for an image button in a tool tip on mouse over
        /// </summary>
        /// <param name="sender">Button being mouse overed</param>
        /// <param name="e">Event</param>
        private void ToolTipMouseHover(object sender, EventArgs e)
        {
            ButtonToolTip.SetToolTip(((Button)sender), ((Button)sender).AccessibleDescription);
        }

        #endregion

        #region Bracket Methods

        /// <summary>
        /// Link the bracket combo box binding source to the list of teams in the bracket
        /// </summary>
        private void linkBracketToTeams()
        {
            bracketR1M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M3Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M3Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M4Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M4Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M5Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M5Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M6Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M6Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M7Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M7Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M8Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketR1M8Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM3Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM3Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM4Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketQFM4Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketSFM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketSFM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketSFM2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketSFM2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketWFM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketWFM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M3Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M3Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M4Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR1M4Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M3Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M3Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M4Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR2M4Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR3M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR3M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR3M2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR3M2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR4M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR4M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR4M2Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLR4M2Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLSFM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLSFM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLFM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketLFM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketGF1M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketGF1M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketGF2M1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketGF2M1Team2.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketTPM1Team1.DataSource = new BindingSource { DataSource = BracketTeams };
            bracketTPM1Team2.DataSource = new BindingSource { DataSource = BracketTeams };

            bracketR1M1Team1.DisplayMember = "Name";
            bracketR1M1Team2.DisplayMember = "Name";
            bracketR1M2Team1.DisplayMember = "Name";
            bracketR1M2Team2.DisplayMember = "Name";
            bracketR1M3Team1.DisplayMember = "Name";
            bracketR1M3Team2.DisplayMember = "Name";
            bracketR1M4Team1.DisplayMember = "Name";
            bracketR1M4Team2.DisplayMember = "Name";
            bracketR1M5Team1.DisplayMember = "Name";
            bracketR1M5Team2.DisplayMember = "Name";
            bracketR1M6Team1.DisplayMember = "Name";
            bracketR1M6Team2.DisplayMember = "Name";
            bracketR1M7Team1.DisplayMember = "Name";
            bracketR1M7Team2.DisplayMember = "Name";
            bracketR1M8Team1.DisplayMember = "Name";
            bracketR1M8Team2.DisplayMember = "Name";
            bracketQFM1Team1.DisplayMember = "Name";
            bracketQFM1Team2.DisplayMember = "Name";
            bracketQFM2Team1.DisplayMember = "Name";
            bracketQFM2Team2.DisplayMember = "Name";
            bracketQFM3Team1.DisplayMember = "Name";
            bracketQFM3Team2.DisplayMember = "Name";
            bracketQFM4Team1.DisplayMember = "Name";
            bracketQFM4Team2.DisplayMember = "Name";
            bracketSFM1Team1.DisplayMember = "Name";
            bracketSFM1Team2.DisplayMember = "Name";
            bracketSFM2Team1.DisplayMember = "Name";
            bracketSFM2Team2.DisplayMember = "Name";
            bracketWFM1Team1.DisplayMember = "Name";
            bracketWFM1Team2.DisplayMember = "Name";
            bracketLR1M1Team1.DisplayMember = "Name";
            bracketLR1M1Team2.DisplayMember = "Name";
            bracketLR1M2Team1.DisplayMember = "Name";
            bracketLR1M2Team2.DisplayMember = "Name";
            bracketLR1M3Team1.DisplayMember = "Name";
            bracketLR1M3Team2.DisplayMember = "Name";
            bracketLR1M4Team1.DisplayMember = "Name";
            bracketLR1M4Team2.DisplayMember = "Name";
            bracketLR2M1Team1.DisplayMember = "Name";
            bracketLR2M1Team2.DisplayMember = "Name";
            bracketLR2M2Team1.DisplayMember = "Name";
            bracketLR2M2Team2.DisplayMember = "Name";
            bracketLR2M3Team1.DisplayMember = "Name";
            bracketLR2M3Team2.DisplayMember = "Name";
            bracketLR2M4Team1.DisplayMember = "Name";
            bracketLR2M4Team2.DisplayMember = "Name";
            bracketLR3M1Team1.DisplayMember = "Name";
            bracketLR3M1Team2.DisplayMember = "Name";
            bracketLR3M2Team1.DisplayMember = "Name";
            bracketLR3M2Team2.DisplayMember = "Name";
            bracketLR4M1Team1.DisplayMember = "Name";
            bracketLR4M1Team2.DisplayMember = "Name";
            bracketLR4M2Team1.DisplayMember = "Name";
            bracketLR4M2Team2.DisplayMember = "Name";
            bracketLSFM1Team1.DisplayMember = "Name";
            bracketLSFM1Team2.DisplayMember = "Name";
            bracketLFM1Team1.DisplayMember = "Name";
            bracketLFM1Team2.DisplayMember = "Name";
            bracketGF1M1Team1.DisplayMember = "Name";
            bracketGF1M1Team2.DisplayMember = "Name";
            bracketGF2M1Team1.DisplayMember = "Name";
            bracketGF2M1Team2.DisplayMember = "Name";
            bracketTPM1Team1.DisplayMember = "Name";
            bracketTPM1Team2.DisplayMember = "Name";

        }

        /// <summary>
        /// Set's the list of teams in the bracket combo box with the teams in teams list
        /// </summary>
        private void setBracketTeams()
        {
            BracketTeams.Clear();
            BracketTeams.Add(new Team());
            BracketTeams.Add(new Team(bracketTeam1.Text, path + FolderList[9] + BracketFiles[16]));
            BracketTeams.Add(new Team(bracketTeam2.Text, path + FolderList[9] + BracketFiles[17]));
            BracketTeams.Add(new Team(bracketTeam3.Text, path + FolderList[9] + BracketFiles[18]));
            BracketTeams.Add(new Team(bracketTeam4.Text, path + FolderList[9] + BracketFiles[19]));
            BracketTeams.Add(new Team(bracketTeam5.Text, path + FolderList[9] + BracketFiles[20]));
            BracketTeams.Add(new Team(bracketTeam6.Text, path + FolderList[9] + BracketFiles[21]));
            BracketTeams.Add(new Team(bracketTeam7.Text, path + FolderList[9] + BracketFiles[22]));
            BracketTeams.Add(new Team(bracketTeam8.Text, path + FolderList[9] + BracketFiles[23]));
            BracketTeams.Add(new Team(bracketTeam9.Text, path + FolderList[9] + BracketFiles[24]));
            BracketTeams.Add(new Team(bracketTeam10.Text, path + FolderList[9] + BracketFiles[25]));
            BracketTeams.Add(new Team(bracketTeam11.Text, path + FolderList[9] + BracketFiles[26]));
            BracketTeams.Add(new Team(bracketTeam12.Text, path + FolderList[9] + BracketFiles[27]));
            BracketTeams.Add(new Team(bracketTeam13.Text, path + FolderList[9] + BracketFiles[28]));
            BracketTeams.Add(new Team(bracketTeam14.Text, path + FolderList[9] + BracketFiles[29]));
            BracketTeams.Add(new Team(bracketTeam15.Text, path + FolderList[9] + BracketFiles[30]));
            BracketTeams.Add(new Team(bracketTeam16.Text, path + FolderList[9] + BracketFiles[31]));
        }

        #endregion

        #region Custom Object Methods

        /// <summary>
        /// Method to set the Gametype list to default
        /// </summary>
        private void DefaultGametypes()
        {
            GametypeList.Clear();
            DirectoryWipe(path + FolderList[10]);
            foreach (Gametype gametype in DefaultGametypeList)
            {
                CreateCustomGametype(gametype);
            }
        }

        /// <summary>
        /// Method to set the Hero list to default
        /// </summary>
        private void DefaultHeroes()
        {
            HeroList.Clear();
            DirectoryWipe(path + FolderList[11]);
            foreach (Hero hero in DefaultHeroList)
            {
                CreateCustomHero(hero);
            }
        }

        /// <summary>
        /// Method to set the Map list to default
        /// </summary>
        private void DefaultMaps()
        {
            MapList.Clear();
            DirectoryWipe(path + FolderList[12]);
            foreach (Map map in DefaultMapList)
            {
                CreateCustomMap(map);
            }
        }

        /// <summary>
        /// Method to set the Role list to default
        /// </summary>
        private void DefaultRoles()
        {
            RoleList.Clear();
            DirectoryWipe(path + FolderList[13]);
            foreach (Role role in DefaultRoleList)
            {
                CreateCustomRole(role);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LinkHeroList()
        {
            m1t1p1Hero.Items.Clear();
            m1t1p2Hero.Items.Clear();
            m1t1p3Hero.Items.Clear();
            m1t1p4Hero.Items.Clear();
            m1t1p5Hero.Items.Clear();
            m1t1p6Hero.Items.Clear();

            m1t2p1Hero.Items.Clear();
            m1t2p2Hero.Items.Clear();
            m1t2p3Hero.Items.Clear();
            m1t2p4Hero.Items.Clear();
            m1t2p5Hero.Items.Clear();
            m1t2p6Hero.Items.Clear();

            playerHeroBox1.Items.Clear();
            playerHeroBox2.Items.Clear();
            playerHeroBox3.Items.Clear();

            m1t1p1Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t1p2Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t1p3Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t1p4Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t1p5Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t1p6Hero.DataSource = new BindingSource { DataSource = HeroList };

            m1t2p1Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t2p2Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t2p3Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t2p4Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t2p5Hero.DataSource = new BindingSource { DataSource = HeroList };
            m1t2p6Hero.DataSource = new BindingSource { DataSource = HeroList };

            playerHeroBox1.DataSource = new BindingSource { DataSource = HeroList };
            playerHeroBox2.DataSource = new BindingSource { DataSource = HeroList };
            playerHeroBox3.DataSource = new BindingSource { DataSource = HeroList };

            m1t1p1Hero.DisplayMember = "Name";
            m1t1p2Hero.DisplayMember = "Name";
            m1t1p3Hero.DisplayMember = "Name";
            m1t1p4Hero.DisplayMember = "Name";
            m1t1p5Hero.DisplayMember = "Name";
            m1t1p6Hero.DisplayMember = "Name";

            m1t2p1Hero.DisplayMember = "Name";
            m1t2p2Hero.DisplayMember = "Name";
            m1t2p3Hero.DisplayMember = "Name";
            m1t2p4Hero.DisplayMember = "Name";
            m1t2p5Hero.DisplayMember = "Name";
            m1t2p6Hero.DisplayMember = "Name";

            playerHeroBox1.DisplayMember = "Name";
            playerHeroBox2.DisplayMember = "Name";
            playerHeroBox3.DisplayMember = "Name";
        }

        /// <summary>
        /// 
        /// </summary>
        private void LinkMapList()
        {
            m1m1Map.Items.Clear();
            m1m2Map.Items.Clear();
            m1m3Map.Items.Clear();
            m1m4Map.Items.Clear();
            m1m5Map.Items.Clear();
            m1m6Map.Items.Clear();
            m1m7Map.Items.Clear();

            m1m1Map.DataSource = new BindingSource { DataSource = MapList };
            m1m2Map.DataSource = new BindingSource { DataSource = MapList };
            m1m3Map.DataSource = new BindingSource { DataSource = MapList };
            m1m4Map.DataSource = new BindingSource { DataSource = MapList };
            m1m5Map.DataSource = new BindingSource { DataSource = MapList };
            m1m6Map.DataSource = new BindingSource { DataSource = MapList };
            m1m7Map.DataSource = new BindingSource { DataSource = MapList };

            m1m1Map.DisplayMember = "Name";
            m1m2Map.DisplayMember = "Name";
            m1m3Map.DisplayMember = "Name";
            m1m4Map.DisplayMember = "Name";
            m1m5Map.DisplayMember = "Name";
            m1m6Map.DisplayMember = "Name";
            m1m7Map.DisplayMember = "Name";
        }

        /// <summary>
        /// 
        /// </summary>
        private void LinkRoleList()
        {
            m1t1p1Role.Items.Clear();
            m1t1p2Role.Items.Clear();
            m1t1p3Role.Items.Clear();
            m1t1p4Role.Items.Clear();
            m1t1p5Role.Items.Clear();
            m1t1p6Role.Items.Clear();

            m1t2p1Role.Items.Clear();
            m1t2p2Role.Items.Clear();
            m1t2p3Role.Items.Clear();
            m1t2p4Role.Items.Clear();
            m1t2p5Role.Items.Clear();
            m1t2p6Role.Items.Clear();

            playerRoleBox.Items.Clear();

            m1t1p1Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t1p2Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t1p3Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t1p4Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t1p5Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t1p6Role.DataSource = new BindingSource { DataSource = RoleList };

            m1t2p1Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t2p2Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t2p3Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t2p4Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t2p5Role.DataSource = new BindingSource { DataSource = RoleList };
            m1t2p6Role.DataSource = new BindingSource { DataSource = RoleList };

            playerRoleBox.DataSource = new BindingSource { DataSource = RoleList };

            m1t1p1Role.DisplayMember = "Name";
            m1t1p2Role.DisplayMember = "Name";
            m1t1p3Role.DisplayMember = "Name";
            m1t1p4Role.DisplayMember = "Name";
            m1t1p5Role.DisplayMember = "Name";
            m1t1p6Role.DisplayMember = "Name";

            m1t2p1Role.DisplayMember = "Name";
            m1t2p2Role.DisplayMember = "Name";
            m1t2p3Role.DisplayMember = "Name";
            m1t2p4Role.DisplayMember = "Name";
            m1t2p5Role.DisplayMember = "Name";
            m1t2p6Role.DisplayMember = "Name";

            playerRoleBox.DisplayMember = "Name";
        }

        /// <summary>
        /// Saves custom Gametype to Gametype directory
        /// </summary>
        /// <param name="gametype">Gametype being created</param>
        private void CreateCustomGametype(Gametype gametype)
        {
            if (!gametype.Name.Equals(""))
            {
                var serializer = new XmlSerializer(gametype.GetType());
                string filePath = path + FolderList[10] + "\\" + gametype.Name.Replace(":", "-").Replace(" ", "_") + ".gametype";
                using (var writer = XmlWriter.Create(filePath))
                {
                    gametype.IconPath = filePath.Replace(".gametype", ".png");
                    gametype.Icon.Save(gametype.IconPath);
                    gametype.IconPath = gametype.IconPath.Replace(path, "");
                    serializer.Serialize(writer, gametype);
                }
            }
            GametypeList.Add(gametype);
        }

        /// <summary>
        /// Saves custom Hero to Hero directory
        /// </summary>
        /// <param name="hero">Hero being created</param>
        public void CreateCustomHero (Hero hero)
        {
            if (!hero.Name.Equals(""))
            {
                var serializer = new XmlSerializer(hero.GetType());
                string filePath = path + FolderList[11] + "\\" + hero.Name.Replace(":", "-").Replace(" ", "_") + ".hero";
                using (var writer = XmlWriter.Create(filePath))
                {
                    hero.IconPath = filePath.Replace(".hero", ".png");
                    hero.Icon.Save(hero.IconPath);
                    hero.IconPath = hero.IconPath.Replace(path, "");
                    serializer.Serialize(writer, hero);
                }
            }
            HeroList.Add(hero);
            LoadPlayerHeroes();
        }

        /// <summary>
        /// Saves custom Map to Map directory
        /// </summary>
        /// <param name="map">Map being created</param>
        public void CreateCustomMap(Map map)
        {
            if (!map.Name.Equals(""))
            {
                var serializer = new XmlSerializer(map.GetType());
                string filePath = path + FolderList[12] + "\\" + map.Name.Replace(":", "-").Replace(" ", "_") + ".map";
                using (var writer = XmlWriter.Create(filePath))
                {
                    map.IconPath = filePath.Replace(".map", ".png");
                    map.Icon.Save(map.IconPath);
                    map.IconPath = map.IconPath.Replace(path, "");
                    serializer.Serialize(writer, map);
                }
            }
            MapList.Add(map);
            LoadMapsPlayed();
        }

        /// <summary>
        /// Saves custom Role to Role directory
        /// </summary>
        /// <param name="role">Role being created</param>
        public void CreateCustomRole(Role role)
        {
            if (!role.Name.Equals(""))
            {
                var serializer = new XmlSerializer(role.GetType());
                string filePath = path + FolderList[13] + "\\" + role.Name.Replace(":", "-").Replace(" ", "_") + ".role";
                using (var writer = XmlWriter.Create(filePath))
                {
                    role.IconPath = filePath.Replace(".role", ".png");
                    role.Icon.Save(role.IconPath);
                    role.IconPath = role.IconPath.Replace(path, "");
                    serializer.Serialize(writer, role);
                }
            }
            RoleList.Add(role);
            LoadPlayerRoles();
        }

        /// <summary>
        /// Edits the selected gametype
        /// </summary>
        /// <param name="gametype">Gametype being editted</param>
        public void EditCustomGametype(Gametype gametype)
        {

        }

        /// <summary>
        /// Edits the selected gametype
        /// </summary>
        /// <param name="hero">Gametype being editted</param>
        public void EditCustomHero(Hero hero)
        {

        }

        /// <summary>
        /// Edits the selected map
        /// </summary>
        /// <param name="map">Map being editted</param>
        public void EditCustomMap(Map map)
        {

        }

        /// <summary>
        /// Edits the selected role
        /// </summary>
        /// <param name="role">Role being editted</param>
        public void EditCustomRole(Role role)
        {
            RoleList.Remove(role);
        }

        /// <summary>
        /// Deletes the selected gametype
        /// </summary>
        /// <param name="gametype">Gametype being deleted</param>
        public void DeleteCustomGametype(Gametype gametype)
        {

        }

        /// <summary>
        /// Deletes the selected hero
        /// </summary>
        /// <param name="hero">Gametype being deleted</param>
        public void DeleteCustomHero(Hero hero)
        {

        }

        /// <summary>
        /// Deletes the selected map
        /// </summary>
        /// <param name="map">Map being deleted</param>
        public void DeleteCustomMap(Map map)
        {

        }

        /// <summary>
        /// Deletes the selected role
        /// </summary>
        /// <param name="role">Role being deleted</param>
        public void DeleteCustomRole(Role role)
        {

        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Watches the defined path to see if the replay file has been motified and kicks of en event if it has
        /// </summary>
        /// <param name="path">Path that contains file being watched</param>
        private void CreateFileWatcher(String path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "Replay Replay.mp4";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Method to add replay to the replay list
        /// </summary>
        /// <param name="path"></param>
        private void addReplay(string path)
        {
            FileInfo fi = new FileInfo(path);
            ShellFile shellFile = ShellFile.FromFilePath(fi.FullName);
            Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap;
            Replay replay = new Replay();
            replay.Players = new List<Player>();
            replay.ReplayPath = fi.FullName;
            replay.Name = fi.Name;
            replay.Thumbnail = shellThumb;
            Replays.Add(replay);
            replayImageList.Images.Add(shellThumb);
            ListViewItem item = new ListViewItem();
            item.Text = fi.Name;
            item.ImageIndex = replayImageList.Images.Count - 1;
            item.SubItems.Add("");
            item.SubItems.Add("");
            item.SubItems.Add("");
            item.SubItems.Add(fi.CreationTime.ToShortTimeString());
            replayListView.Items.Add(item);
            replayListView.Refresh();
        }

        /// <summary>
        /// Wipes the directory that is used as a parameter.
        /// </summary>
        /// <param name="directoryPath">Directory being wiped</param>
        public void DirectoryWipe(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                subDirectory.Delete(true);
            }
        }
        
        /// <summary>
        /// Turns an image to grayscale
        /// </summary>
        public Bitmap GrayScale(Bitmap image)
        {
            //get image dimension
            int width = image.Width;
            int height = image.Height;

            //color of pixel
            Color p;

            //grayscale
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //get pixel value
                    p = image.GetPixel(x, y);

                    //extract pixel component ARGB
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    //find average
                    int avg = (r + g + b) / 3;

                    //set new pixel value
                    image.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }

            return image;
        }

        #endregion
















        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://raw.githubusercontent.com/GitProductions/BGG/main/README.txt";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(content); // Display the content in a message box
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

    
        private void Hero_SelectionChanged(object sender, EventArgs e)
        {
            if (isInitializing)
            {
                return;
            }

            // Cast sender to ComboBox
            ComboBox comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedItem is OW_Scoreboard_Tool.Entities.Hero selectedHero)
            {
                // Get the 'Name' property of the selected Hero
                string selectedHeroName = selectedHero.Name;
                string selectedHeroImagePath = selectedHero.IconPath;

                // Get the current working directory and combine with the relative path
                string currentDirectory = Directory.GetCurrentDirectory();
                string absolutePath = Path.Combine(currentDirectory, selectedHeroImagePath.TrimStart('\\'));

                // Construct the PictureBox name
                string pictureBoxName = comboBox.Name + "Icon";

                // Set the image using the shared method
                SetHeroImage(pictureBoxName, absolutePath);
            }
            else
            {
                MessageBox.Show("No hero selected or invalid selection.");
            }
        }



        private void SetHeroImage(string pictureBoxName, string imagePath)
        {
            // Find the PictureBox control by name
            PictureBox pictureBox = this.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

            if (pictureBox != null)
            {
                // Check if the image path is valid and the file exists
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // Set the PictureBox image using the provided image path
                    pictureBox.Image = Image.FromFile(imagePath);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    // we could have a status bar that displays errors if they occur??
                    //MessageBox.Show("Image file not found or path is invalid: " + imagePath);
                }
            }
            else
            {
                MessageBox.Show($"PictureBox with name {pictureBoxName} not found.");
            }
        }

        private void LBL_SET_HK_UPDATE_Click(object sender, EventArgs e)
        {

        }

        private void INPUT_SET_HK_UPDATE_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            ButtonToolTip.SetToolTip(((Button)sender), "Oh helo there");

        }




        //private bool isShiftPressed = false;
        //private bool isCtrlPressed = false;
        //private bool isAltPressed = false;

        //private void SetHotkey1_Click(object sender, EventArgs e)
        //{
        //    SetHotkey(sender, e);
        //}

        //private void SetHotkey(object sender, EventArgs e)
        //{
        //    if (!isListeningForKey)
        //    {
        //        isListeningForKey = true;
        //        INPUT_SetHotkey1.Text = "Press any key combination...";
        //        this.KeyPreview = true;
        //        this.KeyDown += Form1_KeyDown;
        //        this.KeyUp += Form1_KeyUp;
        //    }
        //    else
        //    {
        //        StopListening();
        //    }
        //}

        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (isListeningForKey)
        //    {
        //        // Check for modifier keys
        //        if (e.KeyCode == Keys.ShiftKey)
        //            isShiftPressed = true;
        //        if (e.KeyCode == Keys.ControlKey)
        //            isCtrlPressed = true;
        //        if (e.KeyCode == Keys.Menu)  // Alt is recognized as 'Menu' in Windows Forms
        //            isAltPressed = true;

        //        // Check if it's a non-modifier key
        //        if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu)
        //        {
        //            // Build the key combination string
        //            string hotkey = "";
        //            if (isCtrlPressed) hotkey += "Ctrl + ";
        //            if (isShiftPressed) hotkey += "Shift + ";
        //            if (isAltPressed) hotkey += "Alt + ";
        //            hotkey += e.KeyCode.ToString();

        //            INPUT_SetHotkey1.Text = hotkey;
        //        }
        //    }
        //}

        //private void Form1_KeyUp(object sender, KeyEventArgs e)
        //{
        //    // Reset modifier keys when released
        //    if (e.KeyCode == Keys.ShiftKey)
        //        isShiftPressed = false;
        //    if (e.KeyCode == Keys.ControlKey)
        //        isCtrlPressed = false;
        //    if (e.KeyCode == Keys.Menu) // Alt is recognized as 'Menu' in Windows Forms
        //        isAltPressed = false;

        //    // Stop listening only after all keys are released
        //    if (!isShiftPressed && !isCtrlPressed && !isAltPressed)
        //    {
        //        StopListening();
        //    }
        //}

        //private void StopListening()
        //{
        //    isListeningForKey = false;
        //    this.KeyPreview = false;
        //    this.KeyDown -= Form1_KeyDown;
        //    this.KeyUp -= Form1_KeyUp;
        //}
    }

}