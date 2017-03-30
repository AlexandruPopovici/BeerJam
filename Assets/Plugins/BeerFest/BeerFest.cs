using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

// Populate this with information about your Game, so it appears in the Banner
public static class GameInfo
{
  // Name of your Game
  public static String GameName   = "Barley Monsta";
  // Your name!
  public static String GameAuthor = "Alex";
  // Starting Scene, or leave Blank to use the existing
  public static String StartScene = "";
  // Show debug controller and size information on the screen.
  public static bool   ShowDebug  = false;
  // Show debug controller and size information on the screen. Pressed by the 'Back' button.
  public static bool   ShowHelp   = false;
  // Show help banner at the bottom right of the screen. Controlled by you!
  public static bool   ShowHelpBanner = true;
  // Powers with cooldown
  public static bool ShowPowers = true;
  // Just write them here
  public static bool TracePowerState = true;
  public static bool DigPowetState = true;



  // Displayed in the Help Screen
  public static String[] HelpText = new string[] {
    "You're out of beer! You need to break into the Barley Monsta's stash to get some.",
    "Once you have it, run for the exit! The Barley Monsta is hunting you.",
    "There is no way to kill it. You need to avoid it. It's faster than you.",
    "Using 'Trace' will always show you the direction in which you need to head."
  };
  // Display in the Control list. 
  // See the Icons in the Resources/BeerFest/Icons for names, or Blank for none
  public static String[] HelpControls = new String[] {
    "Stick",      "Move",
    "Action1",    "Trace",
    "Action2",    "Dig",
  };
}

// This is just a simple helper class that wraps up some of the InControl Gamepad, and Unity
// Key controls. So it can work with either.
//
// Feel free to change the Keyboard controls, but please leave the GamePad as is.
//
// Example Usage:
//
//  if (Controls.Action1)
//   Fire();
//
//  float moveHorz = Controls.LeftStickX
//
public static class Controls
{
  // See:
  // http://www.gallantgames.com/pages/incontrol-standardized-controls
  // For a picture of the layout of the controller.
 
  public static bool Action1
  {
    get { return InputManager.ActiveDevice.Action1 || Input.GetKey(KeyCode.K); }
  }

  public static bool Action2
  {
    get { return InputManager.ActiveDevice.Action2 || Input.GetKey(KeyCode.L); }
  }

  public static bool Action3
  {
    get { return InputManager.ActiveDevice.Action3 || Input.GetKey(KeyCode.J); }
  }

  public static bool Action4
  {
    get { return InputManager.ActiveDevice.Action4 || Input.GetKey(KeyCode.I); }
  }

  public static bool DPadUp
  {
    get { return InputManager.ActiveDevice.DPadUp || Input.GetKey(KeyCode.W); }
  }

  public static bool DPadRight
  {
    get { return InputManager.ActiveDevice.DPadRight || Input.GetKey(KeyCode.D); }
  }

  public static bool DPadDown
  {
    get { return InputManager.ActiveDevice.DPadDown || Input.GetKey(KeyCode.S); }
  }

  public static bool DPadLeft
  {
    get { return InputManager.ActiveDevice.DPadLeft || Input.GetKey(KeyCode.A); }
  }

  public static bool LeftBumper
  {
    get { return InputManager.ActiveDevice.LeftBumper.IsPressed || Input.GetKey(KeyCode.Alpha1); }
  }

  public static bool RightBumper
  {
    get { return InputManager.ActiveDevice.RightBumper.IsPressed || Input.GetKey(KeyCode.Alpha2); }
  }
  
  // Reloads a Scene, effectivetly 'restarting the game' for another Player.
  public static bool Reset
  {
    get { return InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyUp(KeyCode.Escape); }
  }
  
  // Shows a help overlay over the screen, explaining the game and controls.
  public static bool Help
  {
    get { return InputManager.ActiveDevice.Action4.WasReleased || Input.GetKeyUp(KeyCode.Tab); }
  }

  public static float LeftStickX
  {
    get { return InputManager.ActiveDevice.LeftStickX.Value; }
  }
  
  public static float LeftStickY
  {
    get { return InputManager.ActiveDevice.LeftStickY.Value; }
  }
  
  public static float RightStickX
  {
    get { return InputManager.ActiveDevice.RightStickX.Value; }
  }
  
  public static float RightStickY
  {
    get { return InputManager.ActiveDevice.RightStickY.Value; }
  }
  
  public static float LeftTrigger
  {
    get { return InputManager.ActiveDevice.LeftTrigger.Value; }
  }
  
  public static float RightTrigger
  {
    get { return InputManager.ActiveDevice.RightTrigger.Value; }
  }

}

#region Super Secret!

public class BeerFest : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void OnBeforeSceneLoadRuntimeMethod()
  {
    GameObject beerFest = GameObject.Find("__BeerFest");

    if (beerFest == null)
    {
      beerFest = new GameObject("__BeerFest");
      GameObject.DontDestroyOnLoad(beerFest);
      This = beerFest.AddComponent<BeerFest>();
      ControlsMgr = beerFest.AddComponent<InControlManager>();
      ControlsMgr.logDebugInfo = false;
      ControlsMgr.dontDestroyOnLoad = true;
    }
    else
    {
      This = beerFest.GetComponent<BeerFest>();
      ControlsMgr = beerFest.GetComponent<InControlManager>();
    }
  }

  static BeerFest         This;
  static InControlManager ControlsMgr;

  public static InputDevice ActiveDevice { get { return InputManager.ActiveDevice; } }


  public static void ResetGame()
  {
    if (String.IsNullOrEmpty(GameInfo.StartScene))
    {
      GameInfo.StartScene = SceneManager.GetActiveScene().name;
    }
    Debug.Log("Starting Game Reset!");
    GameInfo.ShowHelp = false;
    GameInfo.ShowHelpBanner = true;
    SceneManager.LoadScene(GameInfo.StartScene);
    Debug.Log("Game Reseted!");
  }
  
  const float kAlpha = 0.85f;
  const int   KFontOffset = -7;
  
  static GUIStyle  labelStyle, labelStyleCentre, authorStyle;
  static Texture2D pdgjLogo;
  static Dictionary<String, Texture2D> icons; 
  static KeyCode[] keyCodes;

  void Awake()
  {
    keyCodes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
  }

  void Start ()
  {
    labelStyle = new GUIStyle();
    labelStyle.font = (Font) Resources.Load("BeerFest/Roboto-Black");
    labelStyle.fontSize = 36;
    labelStyle.padding = new RectOffset(0,0,0,0);
    labelStyle.margin = new RectOffset(0,0,0,0);
    labelStyle.border = new RectOffset(0,0,0,0);
    labelStyle.alignment = TextAnchor.UpperLeft;
    labelStyle.normal.textColor = new Color(1, 1, 1, 1);

    pdgjLogo = (Texture2D) Resources.Load("BeerFest/pdgj");
    icons = new Dictionary<string, Texture2D>(10);

    icons.Add("Action1", (Texture2D) Resources.Load("BeerFest/Icons/Action1"));
    icons.Add("Action2", (Texture2D) Resources.Load("BeerFest/Icons/Action2"));
    icons.Add("Action3", (Texture2D) Resources.Load("BeerFest/Icons/Action3"));
    icons.Add("Action4", (Texture2D) Resources.Load("BeerFest/Icons/Action4"));
    icons.Add("Down", (Texture2D) Resources.Load("BeerFest/Icons/Down"));
    icons.Add("DPad", (Texture2D) Resources.Load("BeerFest/Icons/DPad"));
    icons.Add("Help", (Texture2D) Resources.Load("BeerFest/Icons/Help"));
    icons.Add("Left", (Texture2D) Resources.Load("BeerFest/Icons/Left"));
    icons.Add("Reset", (Texture2D) Resources.Load("BeerFest/Icons/Reset"));
    icons.Add("Right", (Texture2D) Resources.Load("BeerFest/Icons/Right"));
    icons.Add("Stick", (Texture2D) Resources.Load("BeerFest/Icons/Stick"));
    icons.Add("Up", (Texture2D) Resources.Load("BeerFest/Icons/Up"));

    authorStyle = new GUIStyle(labelStyle);
    authorStyle.fontSize = 24;
    
    labelStyleCentre = new GUIStyle(labelStyle);
    labelStyleCentre.alignment = TextAnchor.UpperCenter;
  }
  
  void Update()
  {
    if (pdgjLogo == null)
    {
      // Naughty by works. :)
      Awake();
      Start();
    }

    if (Controls.Reset)
    {
      ResetGame();
    }

    if (Controls.Help)
    {
      GameInfo.ShowHelp = !GameInfo.ShowHelp;
    }
  }

  void OnGUI()
  {
    if (pdgjLogo == null)
    {
      // Naughty by works. :)
      Awake();
      Start();
    }

    ShowLeftBanner();
    
    if (GameInfo.ShowHelp)
    {
      ShowHelp();
    }
    
    if (GameInfo.ShowHelp || GameInfo.ShowHelpBanner)
    {
      ShowRightBanner(GameInfo.ShowHelp);
    }

    if (GameInfo.ShowDebug)
    {
      ShowDebug();
    }

    if (GameInfo.ShowPowers)
    {
        ShowPowers(GameInfo.TracePowerState, GameInfo.DigPowetState);
    }
  }
  
  void ShowLeftBanner()
  {
    int boxW = Screen.width / 3;
    int boxH = 64;

    int boxX = 0;
    int boxY = Screen.height - boxH;

    GUI.color = new Color(0, 0, 0, kAlpha);
    GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);
    
    GUI.color = new Color(1, 1, 1, 1);
    GUI.DrawTexture(new Rect(boxX + 4, boxY + 4, boxH - 8, boxH - 8), pdgjLogo);
    
    GUI.color = new Color(1, 1, 1, 1);
    GUI.contentColor = Color.white;

    Heading(boxX + boxH, boxY, boxW, GameInfo.GameName.ToUpper());
    Text(boxX + boxH, boxY + KFontOffset + 37, boxW, GameInfo.GameAuthor.ToUpper());
  }
  
  void ShowRightBanner(bool isShowingHelp)
  {
    int boxW = Screen.width / 3;
    int boxH = 64;

    int boxX = Screen.width - boxW;
    int boxY = Screen.height - boxH;

    GUI.color = new Color(0, 0, 0, kAlpha);
    GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);
    
    if (isShowingHelp)
    {
      GUI.color = new Color(1, 1, 1, 1);
      GUI.contentColor = Color.white;

      IconText(boxX + boxW / 8, boxY + 20, boxW / 2, "Action4", "CLOSE HELP");
    }
    else
    {
      GUI.color = new Color(1, 1, 1, 1);
      GUI.contentColor = Color.white;

      IconText(boxX + boxW / 8, boxY + 20, boxW / 2, "Action4", "HELP");
      IconText(boxX + boxW / 2 + boxW / 8, boxY + 20, boxW / 2, "Reset", "RESET");
    }
  }

  public static void Text(int x, int y, int w, String text)
  {
    GUI.Label(new Rect(x, y, w, 40), text, authorStyle);
  }
  
  public static void Heading(int x, int y, int w, String text)
  {
    GUI.Label(new Rect(x, y + KFontOffset, w, 40), text, labelStyle);
  }

  public static void HeadingCenter(int x, int y, int w, String text)
  {
    GUI.Label(new Rect(x, y, w, 40), text, labelStyleCentre);
  }

  public static void IconText(int x, int y, int w, String icon, String text)
  {
    Texture2D iconTexture = null;
    if (icons.TryGetValue(icon, out iconTexture))
    {
      GUI.DrawTexture(new Rect(x, y, 32, 32), iconTexture);
      x += 48;
    }
    else
    {
      Vector3 sz = labelStyle.CalcSize(new GUIContent(text));
      Text(x, y, (int) sz.x, icon);
      x += (int) sz.x;
    }

    Text(x, y, w, text);

  }
  
  void ShowHelp()
  {
    int boxW = Screen.width / 2;
    int boxH = Screen.height / 2;

    int boxX = Screen.width / 2 - boxW / 2;
    int boxY = (Screen.height / 2 - boxH / 2);
    
    GUI.color = new Color(0, 0, 0, kAlpha);
    GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);
    
    GUI.color = new Color(1, 1, 1, 1);
    GUI.contentColor = Color.white;
    HeadingCenter(boxX, boxY, boxW, "HELP");

    int y = boxY + 50;

    foreach(var line in GameInfo.HelpText)
    {
      Text(boxX + 20, y, boxW - 20, line);
      y += 30;
    }
    
    y += 30;

    for(int i=0;i < GameInfo.HelpControls.Length;i+=2)
    {
      string iconName = GameInfo.HelpControls[i + 0];
      string caption = GameInfo.HelpControls[i + 1];
      IconText(boxX + 16, y, boxW, iconName, caption);
      y += 40;
    }

  }

  void ShowDebug()
  {
    GUI.color = new Color(1, 1, 1, 1);
    GUI.contentColor = Color.white;

    int y = 0;
    GUI.Label(new Rect(10, y, 1000, 200), String.Format("Screen: {0} x {1}", Screen.width, Screen.height) );
    y += 15;
    
    if (InputManager.Devices != null)
    {
      foreach(var device in InputManager.Devices)
      {
        GUI.Label(new Rect(10, y, 1000, 200), String.Format("Device: {0}", device.Name) );
        y += 15;
      }
    }

    StringBuilder sb = new StringBuilder(512);
    sb.AppendFormat("Sticks: {0:0.00}, {1:0.00} | {2:0.00}, {3:0.00} ", Controls.LeftStickX, Controls.LeftStickY,  Controls.RightStickX, Controls.RightStickY);
    sb.AppendFormat("Triggers: {0:0.00}, {1:0.00} ", Controls.LeftTrigger, Controls.RightTrigger);
        
    if (Controls.LeftBumper)
      sb.Append("Left-Bumper ");
          
    if (Controls.RightBumper)
      sb.Append("Right-Bumper ");
          
    if (Controls.Action1)
      sb.Append("Action-1 ");
          
    if (Controls.Action2)
      sb.Append("Action-2 ");

    if (Controls.Action3)
      sb.Append("Action-3 ");

    if (Controls.Action4)
      sb.Append("Action-4 ");

    if (Controls.DPadUp)
      sb.Append("Up ");
          
    if (Controls.DPadRight)
      sb.Append("Right ");

    if (Controls.DPadDown)
      sb.Append("Down ");

    if (Controls.DPadLeft)
      sb.Append("Left ");

    foreach(var kc in keyCodes)
    {
      if (Input.GetKey(kc))
        sb.AppendFormat("Button-{0} ", kc);
    }

    GUI.Label(new Rect(40, y, 1000, 200), sb.ToString() );
    
    y += 15;
  }

    void ShowPowers(bool traceState, bool digState)
    {
        Color inactive = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        Color active = Color.white;

        int boxW = Screen.width / 4;
        int boxH = 64;

        int boxX = Screen.width/2 - boxW/2;
        int boxY = Screen.height - boxH;
        
        GUI.color = traceState ? active : inactive;
        IconText(boxX + boxW / 8, boxY + 20, boxW / 2, "Action1", "TRACE");
        GUI.color = digState ? active : inactive;
        IconText(boxX + boxW / 2 + boxW / 8, boxY + 20, boxW / 2, "Action2", "DIG");
        GUI.color = active;
    }
}

#endregion
