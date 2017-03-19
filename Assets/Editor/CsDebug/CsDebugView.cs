using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class CsDebugView : EditorWindow
{
	public Dictionary<CsDComponent, CsDLevel> DebugLevel;
	public CsDLogTarget LogTarget;

	public CsDebugView()
	{
		
	}

	void OnEnable()
	{
		DebugLevel = CsDebug.LoadDebugSetting(out LogTarget);
		if(DebugLevel == null)
		{
			Debug.LogError("Failed to load debug settings. Using default.");
			DebugLevel = new Dictionary<CsDComponent, CsDLevel>();
			CsDComponent [] components = (CsDComponent[])Enum.GetValues(typeof(CsDComponent));

			foreach(CsDComponent component in components)
			{
				DebugLevel.Add(component, CsDLevel.Default);
			}
		}
	}

	[MenuItem("Window/CsDebug")]



	public static void ShowWindow()
	{
		
		EditorWindow.GetWindow(typeof(CsDebugView));
	}



	void OnGUI()
	{
		GUILayout.Label("Debug Level Settings", EditorStyles.boldLabel);

		CsDComponent [] components = (CsDComponent[])Enum.GetValues(typeof(CsDComponent));

		foreach(CsDComponent component in components)
		{
			DebugLevel[component] = (CsDLevel) EditorGUILayout.EnumPopup(component.ToString(), DebugLevel[component]);
		}

		GUILayout.Label("Debug Log Target", EditorStyles.boldLabel);

		LogTarget = (CsDLogTarget) EditorGUILayout.EnumPopup("Log Target", LogTarget);


		if(GUILayout.Button("Save", GUILayout.Width(50)))
		{
			if(CsDebug.SaveDebugSetting(DebugLevel, LogTarget))
			{
				Debug.Log("Debug level setting saved successfully!");
			}
			else
			{
				Debug.LogError("Failed to save debug level setting!");
			}
		}

	}




}
