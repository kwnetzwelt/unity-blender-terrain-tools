using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Terra
{
	[CustomEditor(typeof(TerraGenerator))]
	public class TerraGeneratorInspector : UnityEditor.Editor
	{
		TerraGeneratorTask task;
		void OnEnable()
		{
			EditorApplication.update += OnUpdate;
		}

		void OnUpdate()
		{
			if(task != null)
			{
				if(task.done)
				{
					var terraGen = (target as TerraGenerator);
					TerraGeneratorTask.ApplyToTerrain(terraGen.GetComponent<Terrain>(), task);
					task = null;
				}
				Repaint();
			}
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = task == null;
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate")) {
				var settings = (target as TerraGenerator).settings.Copy();
				task = new TerraGeneratorTask(settings);
				task.Run();
				EditorUtility.SetDirty(target);
			}

			GUILayout.Box("", GUILayout.ExpandWidth(true));
			GUI.enabled = true;
			if (task != null) {
				Rect r = GUILayoutUtility.GetLastRect();
				EditorGUI.ProgressBar(r, task.progress, Mathf.RoundToInt(task.progress * 100) + "%");
			}
		}

	}
}