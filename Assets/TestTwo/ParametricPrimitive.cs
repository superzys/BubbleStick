using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public abstract class ParametricPrimitive : MonoBehaviour
{
	public enum eAlign
	{
		alignX,
		alignY,
		alignZ
	}

	public bool isStatic = true;

	public int _subdivisionsHeight = 1;

	public int _subdivisionsWidth = 1;

	public ParametricPrimitive.eAlign _align = ParametricPrimitive.eAlign.alignY;

	public bool _invert;

	public bool _invertNormal;

	protected int subdivisionsHeight = 1;

	protected int subdivisionsWidth = 1;

	protected ParametricPrimitive.eAlign align = ParametricPrimitive.eAlign.alignY;

	protected bool invert;

	protected bool invertNormal;

	protected Vector3 normal;

	protected List<Vector3> newVertices;

	protected List<Vector3> newNormals;

	protected List<Vector2> newUV;

	protected List<int> newTriangles;

	protected MeshFilter meshFilter;

	protected Mesh mesh;

	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.mesh = new Mesh();
		this.newVertices = new List<Vector3>();
		this.newUV = new List<Vector2>();
		this.newNormals = new List<Vector3>();
		this.newTriangles = new List<int>();
		this.ShowMesh();
	}

	protected virtual string getName()
	{
		return "ParametricPrimitive";
	}

	public virtual void Reset()
	{
		this.isStatic = true;
		this._subdivisionsHeight = 1;
		this._subdivisionsWidth = 1;
		this._align = ParametricPrimitive.eAlign.alignY;
		this._invert = false;
		this._invertNormal = false;
	}

	public virtual void ShowMesh()
	{
	}
}
