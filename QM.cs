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
using UnityEngine;

namespace QuickMute {

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class QuickMute : QMute {

		internal static QuickMute Instance;
		[KSPField(isPersistant = true)] internal static QBlizzyToolbar BlizzyToolbar;
		[KSPField(isPersistant = true)] internal static QStockToolbar StockToolbar;

		private void Awake() {
			Instance = this;
			if (BlizzyToolbar == null) BlizzyToolbar = new QBlizzyToolbar ();
			if (StockToolbar == null) StockToolbar = new QStockToolbar ();
			GameEvents.onGUIApplicationLauncherDestroyed.Add (StockToolbar.AppLauncherDestroyed);
			GameEvents.onGameSceneLoadRequested.Add (StockToolbar.AppLauncherDestroyed);
			GameEvents.onGUIApplicationLauncherUnreadifying.Add (StockToolbar.AppLauncherDestroyed);
			GameEvents.onFlightReady.Add (OnFlightReady);
		}

		private void Start() {
		    QSettings.Instance.Load ();
			BlizzyToolbar.Start ();
			StartCoroutine (StockToolbar.AppLauncherReady ());
			if (Muted) {
				Mute (true);
			}
		}

		private void OnDestroy() {
			BlizzyToolbar.OnDestroy ();
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (StockToolbar.AppLauncherDestroyed);
			GameEvents.onGameSceneLoadRequested.Remove (StockToolbar.AppLauncherDestroyed);
			GameEvents.onGUIApplicationLauncherUnreadifying.Remove (StockToolbar.AppLauncherDestroyed);
			GameEvents.onFlightReady.Remove (OnFlightReady);
		}

		private void OnFlightReady() {
			if (QSettings.Instance.Muted) {
				Mute (true);
			}
		}

		private void Update() {
			if (Input.GetKeyDown (QSettings.Instance.Key)) {
				Mute ();
			}
		}

		private void OnGUI() {
			if (Draw) {
				GUILayout.BeginArea (new Rect ((Screen.width - 96) / 2, (Screen.height - 96) / 2, 96, 96), Icon_Texture);
				GUILayout.EndArea ();
			}
		}
	}
}