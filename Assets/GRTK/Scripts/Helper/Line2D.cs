using UnityEngine;
using System.Collections;

namespace GRTK
{
    /// <summary>
    /// A class to represent a line segment and handle various interactions with line segments
    /// </summary>
    public class Line2D
    {
        public static float LEFT_EPS = 0.0001f;

        // The two points defining the line segment
        public Vector2 p1;
        public Vector2 p2;

        // Main constructor
        public Line2D(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

	// Returns the magnitude of a (cross) b
	protected static float cross(Vector2 a, Vector2 b) {
		return a.x * b.y - a.y * b.x;
	}

        // Certifies whether a line segment intersects another.
        public bool Intersect(Line2D other)
        {
			Vector2 temp = new Vector2();
			return Intersect (other, ref temp);
        }

		// Certifies whether a line segment intersects another and if so, writes the intersection point to the second argument.
        public bool Intersect(Line2D other, ref Vector2 intersectionPoint)
        {
            // Get the direction vectors
			Vector2 dir1 = p2 - p1, dir2 = other.p2 - other.p1;

			// Check if parallel - normalized direction vectors are equal or opposite equal
			if (dir1.normalized == dir2.normalized || dir1.normalized == (-dir2.normalized)) 
				return false;

			// Otherwise, solve the linear equation for s and t: this.p1 + t*dir1 = other.p1 + s*dir2.
			// If both are within the [0, 1] range, the segments intersect.

			 // We'll need the difference vector as well as the cross product magnitude
			Vector2 delta = other.p1 - p1;
			float crossV = cross (dir1, dir2);

			// Now get the interpolation values
			float t = cross (delta, dir2) / crossV;
			float s = cross (delta, dir1) / crossV;

			// If a value is within the [0, 1] range, its clamping in that range will yield the same value.
			if (Mathf.Clamp01 (t) == t && Mathf.Clamp01 (s) == s) {
				// Not sure how C# references work so using Set on the given argument
				intersectionPoint = p1 + (t * dir1);
				return true;
			}
			return false;
        }

        // Check if a point is to the left of the line (assuming orientation of line A to B means the line is going towards B)
        public bool Left(Vector2 point)
        {
            // Find the determinate of the point and the line
            // A = -(y2 - y1)
            // B = x2 - x1
            // C = -(A * x1 + B * y1)
            // D = A * xp + B * yp + C
            double A = -(p2.y - p1.y);
            double B = p2.x - p1.x;
            double C = -(A * p1.x + B * p1.y);
            double D = A * point.x + B * point.y + C;

            // if D > 0 then it's on the left
            return D > LEFT_EPS;
        }
    }
}
