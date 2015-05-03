/* 
Quick0
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
using System.Collections;
using UnityEngine;


namespace QuickMute {
	public class QMute : Quick {
	
		private string Icon_TexturePathSound = Quick.MOD + "/Textures/Icon_sound";
		private string Icon_TexturePathMute = Quick.MOD + "/Textures/Icon_mute";

		protected Texture2D Icon_Texture {
			get {
				return GameDatabase.Instance.GetTexture((QSettings.Instance.Muted ? Icon_TexturePathMute : Icon_TexturePathSound), false);
			}
		}

		protected bool Draw = false;

		protected bool Muted {
			get {
				return QSettings.Instance.Muted;
			}
			set {
				QSettings.Instance.Muted = value;
			}
		}

		public void Mute() {
			Mute (!Muted);
			Draw = true;
			StartCoroutine (Wait (5));
			QSettings.Instance.Save ();
		}

		private IEnumerator Wait(int seconds) {
			yield return new WaitForSeconds (seconds);
			Draw = false;
		}

		public void Mute(bool mute) {
			Muted = mute;
			QuickMute.BlizzyToolbar.Refresh ();
			QuickMute.StockToolbar.Refresh ();
			AudioSource[] _audios = (AudioSource[])Resources.FindObjectsOfTypeAll (typeof(AudioSource));
			foreach (AudioSource _audio in _audios) {
				_audio.mute = Muted;
			}
			Quick.Log ((Muted ? "Mute" : "Unmute"));
		}
	}
}