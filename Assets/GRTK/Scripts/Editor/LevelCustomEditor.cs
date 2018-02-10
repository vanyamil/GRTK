﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace GRTK
{
    [CustomEditor(typeof(Level))]
    public class LevelCustomEditor : Editor
    {
        static bool BoundariesVisible = true;

		/// <summary>
		/// Exports the polygons to JSON under the GeoJSON format.
		/// </summary>
		void ExportPolygonsToJSON(Level level) 
		{
			// Get the original polygons
			List<Polygon> list = new List<Polygon> ();
			level.GetComponents(list);

			// Find the level bounds - rightfully assuming only one
			Polygon exterior = list.Find(p => p.type == PolygonType.LevelExterior);
			if (exterior == null) {
				EditorUtility.DisplayDialog (
					"Warning : no level bounds found",
					"Please make sure that the boundary polygons have been built and at least one has been assigned the type 'Level Exterior'.",
					"Understood!");
				return;
			}
			// Find the level holes
			List<Polygon> interiors = list.FindAll(p => p.type == PolygonType.LevelHoles);

			// And finally save the file
			string exportPath = EditorUtility.SaveFilePanel (
				"Export to GeoJSON format",
				"",
				"Boundary.json",
				"json");

			if (exportPath.Length != 0) {
				System.IO.File.WriteAllText (exportPath, Polygon.ToJson(exterior, interiors));
			}
		}

        public override void OnInspectorGUI()
        {
            // INITIALIZATION
            Level level = target as Level;

            // DRAW THE UI
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool addButtonResult = GUILayout.Button("Add Boundary", GUILayout.Width(300), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool hideButtonResult = GUILayout.Button("Hide/Show Boundaries", GUILayout.Width(300), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool resetButtonResult = GUILayout.Button("Reset", GUILayout.Width(300), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool buildButtonResult = GUILayout.Button("Build", GUILayout.Width(300), GUILayout.Height(30));
            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			bool exportButtonResult = GUILayout.Button("Export to GeoJSON", GUILayout.Width(300), GUILayout.Height(30));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            // HANDLE THE INPUT
			if (buildButtonResult) {
				level.CompileLevel ();
			} else if (addButtonResult) {
				// Create a new GameObject with a Level attached
				GameObject newBoundary = GameObject.CreatePrimitive (PrimitiveType.Cube);
				newBoundary.name = "Boundary";
				newBoundary.AddComponent<Boundary> ();

				// Ensure it gets reparented if this was a context click (otherwise does nothing)
				GameObjectUtility.SetParentAndAlign (newBoundary, ((Level)target).gameObject);

				// Register the creation in the undo system
				Undo.RegisterCreatedObjectUndo (newBoundary, "Create " + newBoundary.name);
			} else if (resetButtonResult) {
				// Destroy all child polygons
				foreach (Polygon poly in level.gameObject.GetComponents<Polygon>()) {
					DestroyImmediate (poly);
				}
				GUIUtility.ExitGUI ();
			} else if (hideButtonResult) {
				// toggle visibility of all child boundaries
				if (BoundariesVisible)
					BoundariesVisible = false;
				else
					BoundariesVisible = true;
				foreach (Transform child in level.transform) {
					child.gameObject.SetActive (BoundariesVisible);
				}
			} else if (exportButtonResult) {
				ExportPolygonsToJSON (level);
			}
        }
    }
}
