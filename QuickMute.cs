/* 
QuickMute
Copyright 2015 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using System.IO;
using UnityEngine;

namespace QuickMute {
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class QuickMute : MonoBehaviour {
		public static string VERSION = "1.00";
		public static string MOD = "QuickMute";

		private static bool isdebug = true;

		private string File_settings = KSPUtil.ApplicationRootPath + "GameData/" + MOD + "/Config.txt";

		private Texture StockToolBar_Texture_sound = (Texture)GameDatabase.Instance.GetTexture (MOD + "/Textures/StockToolBar_sound", false);
		private Texture StockToolBar_Texture_mute = (Texture)GameDatabase.Instance.GetTexture (MOD + "/Textures/StockToolBar_mute", false);
		private string BlizzyToolBar_TexturePath_sound = MOD + "/Textures/BlizzyToolBar_sound";
		private string BlizzyToolBar_TexturePath_mute = MOD + "/Textures/BlizzyToolBar_mute";
		private Texture Icon_Texture_sound = (Texture)GameDatabase.Instance.GetTexture (MOD + "/Textures/Icon_sound", false);
		private Texture Icon_Texture_mute = (Texture)GameDatabase.Instance.GetTexture (MOD + "/Textures/Icon_mute", false);
		private ApplicationLauncherButton StockToolBar_Button;
		private IButton BlizzyToolBar_Button;

		private DateTime Mute_Icon_Temp = new DateTime(0);

		[Persistent]
		private string Key = "f10";
		[Persistent]
		private bool StockToolBar = true;
		[Persistent]
		private bool BlizzyToolBar = true;
		[Persistent]
		private bool Muted = false;

		[KSPField(isPersistant = true)]
		private static bool first = true;

		private bool isBlizzyToolBar {
			get {
				return (ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null);
			}
		}

		private void Awake() {
			Load ();
			if (first && Muted) {
				Mute (true);
			}
			first = false;
			GameEvents.onGUIApplicationLauncherReady.Add (OnGUIApplicationLauncherReady);
			if (BlizzyToolBar && HighLogic.LoadedSceneIsGame) {
				BlizzyToolBar_Init ();
			}
		}

		private void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (OnGUIApplicationLauncherReady);
			StockToolBar_Destroy ();
			if (BlizzyToolBar && HighLogic.LoadedSceneIsGame) {
				BlizzyToolBar_Destroy ();
			}
		}

		private Texture Icon_Texture {
			get {
				return (Muted ? Icon_Texture_mute : Icon_Texture_sound );
			}
		}

		// GESTION DES TOOLBARS
		private Texture StockToolBar_Texture {
			get {
				return (Muted ? StockToolBar_Texture_mute : StockToolBar_Texture_sound );
			}
		}

		private string BlizzyToolBar_TexturePath {
			get {
				return (Muted ? BlizzyToolBar_TexturePath_mute : BlizzyToolBar_TexturePath_sound);
			}
		}

		private void OnGUIApplicationLauncherReady() {
			if (StockToolBar) {
				StockToolBar_Init ();
			}
		}
		private void BlizzyToolBar_Init() {
			if (isBlizzyToolBar && BlizzyToolBar_Button == null) {
				BlizzyToolBar_Button = ToolbarManager.Instance.add("QuickMute", "QuickMute");
				BlizzyToolBar_Button.TexturePath = BlizzyToolBar_TexturePath;
				BlizzyToolBar_Button.ToolTip = "QuickMute";
				BlizzyToolBar_Button.OnClick += (e) => Mute();
			}
		}
		private void BlizzyToolBar_Destroy() {
			if (isBlizzyToolBar && BlizzyToolBar_Button != null) {
				BlizzyToolBar_Button.Destroy ();
			}
		}
		private void StockToolBar_Init() {
			if (StockToolBar_Button == null && ApplicationLauncher.Ready) {
				StockToolBar_Button = ApplicationLauncher.Instance.AddModApplication (Mute, Mute, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, StockToolBar_Texture);
			}
		}
		private void StockToolBar_Destroy() {
			if (StockToolBar_Button != null) {
				ApplicationLauncher.Instance.RemoveModApplication (StockToolBar_Button);
				StockToolBar_Button = null;
			}
		}

		// COMMANDES
		public void Mute() {
			Mute (!Muted);
			Mute_Icon_Temp = DateTime.Now;
			Save ();
		}
		public void Mute(bool mute) {
			Muted = mute;
			var _audios = Resources.FindObjectsOfTypeAll (typeof(AudioSource));
			foreach (AudioSource _audio in _audios) {
				_audio.mute = Muted;
			}
			if (ApplicationLauncher.Ready && StockToolBar_Button != null) {
				StockToolBar_Button.SetTexture (StockToolBar_Texture);
				if (Muted && StockToolBar_Button.State == RUIToggleButton.ButtonState.FALSE) {
					StockToolBar_Button.SetTrue (false);
				}
				if (!Muted && StockToolBar_Button.State == RUIToggleButton.ButtonState.TRUE) {
					StockToolBar_Button.SetFalse (false);
				}
			}
			if (isBlizzyToolBar && BlizzyToolBar_Button != null) {
				BlizzyToolBar_Button.TexturePath = BlizzyToolBar_TexturePath;
			}
			Log ((Muted ? "Mute" : "Unmute"));
		}

		// GESTION DE LA TOUCHE EXIT
		private void Update() {
			if (ApplicationLauncher.Ready && StockToolBar_Button != null) {
				if (Muted && StockToolBar_Button.State == RUIToggleButton.ButtonState.FALSE) {
					StockToolBar_Button.SetTrue (false);
				}
			}
			if (Input.GetKeyDown (Key)) {
				Mute ();
			}
		}

		// AFFICHAGE DE L'INTERFACE
		private void OnGUI() {
			DateTime _date = new DateTime (0);
			if (Mute_Icon_Temp != _date) {
				GUILayout.BeginArea (new Rect ((Screen.width - 96) / 2, (Screen.height - 96) / 2, 96, 96), Icon_Texture);
				GUILayout.EndArea ();
				if ((DateTime.Now - Mute_Icon_Temp).TotalSeconds > 5) {
					Mute_Icon_Temp = _date;
				}
			}
		}

		// GESTION DE LA CONFIGURATION
		public void Save() {
			ConfigNode _temp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
			_temp.Save(File_settings);
			Log("Save");
		}
		public void Load() {
			if (File.Exists (File_settings)) {
				ConfigNode _temp = ConfigNode.Load (File_settings);
				if (_temp.HasValue ("StockToolBar") && _temp.HasValue ("BlizzyToolBar") && _temp.HasValue ("Key") && _temp.HasValue ("Muted")) {
					ConfigNode.LoadObjectFromConfig (this, _temp);
					Log("Load");
					return;
				}
			}
			Save ();
		}

		// AFFICHAGE DES MESSAGES SUR LA CONSOLE
		private static void Log(string String) {
			if (isdebug) {
				Debug.Log (MOD + "(" + VERSION + "): " + String);
			}
		}
	}
}