// Copyright 2019 Vilar24

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class Example : AssetPostprocessor {

	private void OnPostprocessModel(GameObject g) {
		Apply(g.transform);
	}

	private void Apply(Transform t) {
		CreateAsymmetricalBlendShapes(t.gameObject, 0.04f);
		foreach (Transform child in t)
			Apply(child);
	}

	private void CreateAsymmetricalBlendShapes(GameObject gameObject, float blendDistance) {
		if (gameObject.GetComponent<SkinnedMeshRenderer>() == null) return;
		Mesh mesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		int cachedBlendShapeCount = mesh.blendShapeCount;
		float tempBlendDistance = blendDistance;
		for (int i = 0; i < cachedBlendShapeCount; i++) {
			if (mesh.GetBlendShapeName(i).Contains("LeftRight")) {
				tempBlendDistance = blendDistance;
				MatchCollection matches = Regex.Matches(mesh.GetBlendShapeName(i), ".*LeftRight(?<foo>[0-9]+)", RegexOptions.ExplicitCapture);
				foreach (Match match in matches) {
					tempBlendDistance = float.Parse(match.Groups["foo"].Value) * 0.01f;
				}	
				CreateAsymmetricalBlendShape(mesh, i, false, tempBlendDistance);
				CreateAsymmetricalBlendShape(mesh, i, true, tempBlendDistance);
			}
		}
	}

	private void CreateAsymmetricalBlendShape(Mesh mesh, int shapeIndex, bool right, float blendDistance) {
		float modelWidth = 0f;
		Vector3[] vertices = mesh.vertices;
		Vector3[] deltaVertices = new Vector3[mesh.vertices.Length];
		Vector3[] deltaNormals = new Vector3[mesh.vertices.Length];
		Vector3[] deltaTangents = new Vector3[mesh.vertices.Length];
		mesh.GetBlendShapeFrameVertices(shapeIndex, 0, deltaVertices, deltaNormals, deltaTangents);
		float rightFactor = 0;
		for (int i = 0; i < vertices.Length; i++) {
			if (modelWidth < vertices[i].x) modelWidth = vertices[i].x;
		}
		blendDistance = blendDistance * modelWidth;
		for (int i = 0; i < deltaVertices.Length; i++) {
			rightFactor = (vertices[i].x + blendDistance) / (blendDistance * 2f);
			rightFactor = Mathf.Clamp01(rightFactor);
			if (!right) rightFactor = 1f - rightFactor;
			deltaVertices[i] = deltaVertices[i] * rightFactor;
			deltaNormals[i] = deltaNormals[i] * rightFactor;
			deltaTangents[i] = deltaTangents[i] * rightFactor;
		}
		string[] separatingStrings = { "LeftRight" };
		mesh.AddBlendShapeFrame(mesh.GetBlendShapeName(shapeIndex).Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0] + (right ? "Right" : "Left"), 100, deltaVertices, deltaNormals, deltaTangents);
	}

}
#endif