using UnityEngine;
using System.Collections;

public static class StaticUtility
{



	//first-order intercept using absolute target position
	public static Vector3 FirstOrderIntercept
		(
			Vector3 shooterPosition,
			Vector3 shooterVelocity,
			float shotSpeed,
			Vector3 targetPosition,
			Vector3 targetVelocity
			)  
	{
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime
			(
				shotSpeed,
				targetRelativePosition,
				targetRelativeVelocity
				);
		return targetPosition + t*(targetRelativeVelocity);
	}

	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime
		(
			float shotSpeed,
			Vector3 targetRelativePosition,
			Vector3 targetRelativeVelocity
		) 
	{
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if(velocitySquared < 0.001f)
			return 0f;
		
		float a = velocitySquared - shotSpeed*shotSpeed;
		
		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f)
		{
			float t = -targetRelativePosition.sqrMagnitude/
				(
					2f*Vector3.Dot
					(
					targetRelativeVelocity,
					targetRelativePosition
					)
					);
			return Mathf.Max(t, 0f); //don't shoot back in time
		}
		
		float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b*b - 4f*a*c;
		
		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
			t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else
					return t1; //only t1 is positive
			} else
				return Mathf.Max(t2, 0f); //don't shoot back in time
		} else if (determinant < 0f) //determinant < 0; no intercept path
			return 0f;
		else //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
	}



	public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
	{
		switch (blendMode)
		{
		case BlendMode.Opaque:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = -1;
			break;
		case BlendMode.Cutout:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 2450;
			break;
		case BlendMode.Fade:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		case BlendMode.Transparent:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		}

	}

	//Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	//same plane, use ClosestPointsOnTwoLines() instead.
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{

		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		//is coplanar, and not parrallel
		if(Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
			return true;
		}
		else
		{
			intersection = Vector3.zero;
			return false;
		}
	}

	public static bool LineSegmentIntersection(out Vector2 intersection, Vector2 p, Vector2 r, Vector2 q, Vector2 s)
	{
		//Debug.Log(p + " " + (p + r) + " " + q + " " + (q + s));
		float top_u = TwoDCrossProduct(q - p, r);
		float bottom = TwoDCrossProduct(r, s);
		float top_t = TwoDCrossProduct(q - p, s);

		intersection = Vector2.zero;

		if(bottom != 0)
		{
			float t = (top_t / bottom);
			float u = (top_u / bottom);
			//Debug.Log("t = " + t + " u = " + u + " bottom = " + bottom); 
			if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
			{
				intersection = p + t * r;
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	public static float TwoDCrossProduct(Vector2 vec1, Vector2 vec2)
	{
		return vec1.x * vec2.y - vec1.y * vec2.x;
	}

	public static bool ContainsPoint (Vector2 [] polyPoints, Vector2 p, Vector3 initiator, out Vector3 borderPoint)
	{ 
		//Debug.Log("Begin contains point check");
		int j = polyPoints.Length-1; 
		bool inside = false; 
		borderPoint = Vector3.zero;
		bool intersectionFound = false;
		for(int i = 0; i < polyPoints.Length; j = i++) 
		{ 
			if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
				(p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
				inside = !inside; 

			//if(inside == false)
			{
				Vector2 intersection;
				Vector2 initiatorFlat = new Vector2(initiator.x, initiator.z);
				Vector2 vec1 = p - initiatorFlat;
				Vector2 vec2 = polyPoints[i] -polyPoints[j];

				Vector3 a = initiatorFlat + vec1;
				Vector3 vec2_3d = new Vector3(vec2.x, 0, vec2.y);
				//Debug.DrawLine(new Vector3(initiator.x, 0, initiator.z), new Vector3(a.x, 0, a.y), Color.blue, 0.04f);
				//Debug.DrawLine(new Vector3(polyPoints[j].x, 0, polyPoints[j].y), new Vector3(polyPoints[j].x, 0, polyPoints[j].y) + vec2_3d, Color.red, 0.04f);
				bool isIntersecting = LineSegmentIntersection(out intersection, initiatorFlat, vec1, polyPoints[j], vec2);
				if(isIntersecting)
				{
					intersectionFound = true;
					borderPoint = new Vector3(intersection.x, 0, intersection.y);

				}
			}
		} 

		if(!intersectionFound)
		{
			borderPoint = Vector3.zero;
		}


		return inside; 
	}

	public static bool ContainsPoint (Vector2 [] polyPoints, Vector2 p)
	{ 
		//Debug.Log("Begin contains point check");
		int j = polyPoints.Length-1; 
		bool inside = false; 

		for(int i = 0; i < polyPoints.Length; j = i++) 
		{ 
			if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
				(p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
				inside = !inside; 
		}

		return inside;
	}

	public static bool CompareIntWithOp(int value1, int value2, int op)
	{
		if(op == 2)
		{
			return value1 > value2;
		}
		else if(op == 1)
		{
			return value1 >= value2;
		}
		else if(op == 0)
		{
			return value1 == value2;
		}
		else if(op == -1)
		{
			return value1 <= value2;
		}
		else if(op == -2)
		{
			return value1 < value2;
		}

		return false;
	}


}
