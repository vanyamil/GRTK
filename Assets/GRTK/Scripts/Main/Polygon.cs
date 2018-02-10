using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GRTK
{
	public enum PolygonType { Discard, LevelExterior, LevelHoles }
    // Class to represent a closed polygon. This is serializable so we can
    // store it in a scriptable object (LevelGeometry) to save our
    // results from compiling geometry
    public class Polygon : MonoBehaviour
    {
        public bool Visualize = true;

        // the raw data that represents these polygons.
        public List<Vector2> verticies = new List<Vector2>();

		// The type of polygon for the purpose of GeoJson
		public PolygonType type = PolygonType.Discard;

        public void SetVerticies(List<Vector2> verticies)
        {
            this.verticies = verticies;
        }

        // Adds a vertex as a neighbor to the last and first vertex in the loop
        public void AppendVertex(Vector2 vert)
        {
            verticies.Add(vert);
        }

        // Removes a given vertex
        public void RemoveVertex(Vector2 vert)
        {
            verticies.Remove(vert);
        }

        // Returns this polygon in a way unity can process with meshes
        public Vector2[] GetRaw2()
        {
            return verticies.ToArray();
        }

        public Vector3[] GetRaw3()
        {
            List<Vector3> vec3 = new List<Vector3>();
            foreach (Vector2 vertex in verticies)
            {
                vec3.Add(new Vector3(vertex.x, vertex.y, 0));
            }

            return vec3.ToArray();
		}

		// Returns a JSON string encoding the polygon with exterior boundary and interiors as holes under the GeoJSON format.
		public static string ToJson(Polygon exterior, List<Polygon> interiors) {
			List<List<Vector2>> coordinates = new List<List<Vector2>> ();
			coordinates.Add (exterior.verticies);
			foreach(Polygon p in interiors) 
				coordinates.Add(p.verticies);
			
			string json = "{\"type\": \"Polygon\", \"coordinates\": [";
			foreach (List<Vector2> polyline in coordinates) {
				json += "[";
				foreach (Vector2 v in polyline) {
					json += string.Format ("[{0}, {1}]", v.x, v.y);
					if (v != polyline [polyline.Count - 1])
						json += ",";
				}
				json += "]";
				if (polyline != coordinates [coordinates.Count - 1])
					json += ",";
			}
			return json + "]}";
		}

        #region Editor
        private void OnDrawGizmosSelected()
        {
            if (Visualize)
            {
                Vector3[] poly = GetRaw3();
                int i;
                for (i = 0; i < poly.Length; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(poly[i], 0.1f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(poly[i], poly[(i + 1) % poly.Length]);
                }
            }
        }
        #endregion
    }
}