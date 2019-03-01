using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2017 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Demos
{
	/// <summary>
	/// Allows the user to select from a list of webcams and creates a capture instance per webcam allowing each webcam to be recorded
	/// </summary>
	public class WebcamCaptureDemo : MonoBehaviour
	{
		private class Instance
		{
			public string name;
			public WebCamTexture texture;
			public CaptureFromTexture capture;
			public CaptureGUI gui;
		}

		[SerializeField]
		private GUISkin _skin;

		[SerializeField]
		private GameObject _prefab;

		[SerializeField]
		private int _webcamResolutionWidth = 640;

		[SerializeField]
		private int _webcamResolutionHeight = 480;

		[SerializeField]
		private int _webcamFrameRate = 30;

		// State
		private Instance[] _instances;
		private int _selectedWebcamIndex;

		private void Start()
		{
			// Create instance data per webcam
			int numCams = WebCamTexture.devices.Length;
			_instances = new Instance[numCams];
			for (int i = 0; i < numCams; i++)
			{
				GameObject go = (GameObject)GameObject.Instantiate(_prefab);

				Instance instance = new Instance();
				instance.name = WebCamTexture.devices[i].name;
				instance.capture = go.GetComponent<CaptureFromTexture>();
				instance.capture._autoFilenamePrefix = "Demo4Webcam-" + i;
				instance.gui = go.GetComponent<CaptureGUI>();
				instance.gui._showUI = false;

				_instances[i] = instance;
			}

			if (numCams > 0)
			{
				Change(0);
			}
		}

		private void StartWebcam(Instance instance)
		{
			// NOTE: WebcamTexture can be slow for high resolutions, this can cause issues with audio-video sync.
			// Our plugins AVPro Live Camera or AVPro DeckLink can be used to capture high resolution devices
			instance.texture = new WebCamTexture(instance.name, _webcamResolutionWidth, _webcamResolutionHeight, _webcamFrameRate);
			instance.texture.Play();
			if (instance.texture.isPlaying)
			{
				instance.capture.SetSourceTexture(instance.texture);
			}
			else
			{
				StopWebcam(instance);
			}
		}

		private void StopWebcam(Instance instance)
		{
			if (instance.texture != null)
			{
				if (instance.capture != null && instance.capture.IsCapturing())
				{
					instance.capture.SetSourceTexture(null);
					instance.capture.StopCapture();
				}

				instance.texture.Stop();
				Destroy(instance.texture);
				instance.texture = null;
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < _instances.Length; i++)
			{
				StopWebcam(_instances[i]);
			}
		}

		private void Change(int index)
		{
			_selectedWebcamIndex = index;
			for (int j = 0; j < _instances.Length; j++)
			{
				_instances[j].gui._showUI = (j == _selectedWebcamIndex);
			}
		}

		private void OnGUI()
		{
			GUI.skin = _skin;
			GUILayout.BeginArea(new Rect(Screen.width - 512, 0, 512, Screen.height));
			GUILayout.BeginVertical();

			GUILayout.Label("Select webcam:");

			for (int i = 0; i < _instances.Length; i++)
			{
				Instance webcam = _instances[i];

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

				if (_selectedWebcamIndex == i)
				{
					GUILayout.Label("->", GUILayout.Width(32f));
				}
				else
				{
					GUILayout.Label(" ", GUILayout.Width(32f));
				}

				if (webcam.capture.IsCapturing())
				{
					float t = Mathf.PingPong(Time.timeSinceLevelLoad, 0.25f) * 4f;
					GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.white, t);
					GUI.color = Color.Lerp(Color.red, Color.white, t);

				}

				if (GUILayout.Button(webcam.name, GUILayout.Width(200), GUILayout.ExpandWidth(true)))
				{
					Change(i);
				}
				GUI.backgroundColor = Color.white;
				GUI.color = Color.white;

				if (webcam.texture == null)
				{
					if (GUILayout.Button("Play", GUILayout.Width(64f)))
					{
						StartWebcam(webcam);
						Change(i);
					}
				}
				else
				{
					if (GUILayout.Button("Stop", GUILayout.Width(64f)))
					{
						StopWebcam(webcam);
						Change(i);
					}
				}

				if (webcam.texture != null)
				{
					Rect camRect = GUILayoutUtility.GetRect(256, 256.0f / (webcam.texture.width / (float)webcam.texture.height));
					GUI.DrawTexture(camRect, webcam.texture);
				}
				else
				{
					GUILayout.Label("No signal...", GUILayout.MinWidth(256.0f), GUILayout.MaxWidth(256.0f), GUILayout.ExpandWidth(false));
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}