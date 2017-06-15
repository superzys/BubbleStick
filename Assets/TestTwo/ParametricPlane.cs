using System;
using UnityEngine;

[AddComponentMenu("Primitives/Plane"), ExecuteInEditMode]
[Serializable]
public class ParametricPlane : ParametricPrimitive
{
	public float _height = 1f;

	public float _width = 1f;

	protected float height = 1f;

	protected float width = 1f;

	protected float demiHeight;

	protected float demiWidth;

	protected Vector3 topLeft;

	protected Vector3 topRight;

	protected Vector3 bottomLeft;

	protected Vector3 bottomRight;

	public void CreateMesh()
	{
		if (this.subdivisionsHeight != this._subdivisionsHeight || this.subdivisionsWidth != this._subdivisionsWidth || this.align != this._align || this.invert != this._invert || this.invertNormal != this._invertNormal || this.width != this._width || this.height != this._height)
		{
			this.subdivisionsHeight = this._subdivisionsHeight;
			this.subdivisionsWidth = this._subdivisionsWidth;
			this.align = this._align;
			this.invert = this._invert;
			this.invertNormal = this._invertNormal;
			this.width = this._width;
			this.height = this._height;
			this.ShowMesh();
		}
	}

	protected void Update()
	{
		this.CreateMesh();
	}

	public override void Reset()
	{
		base.Reset();
		this._height = 1f;
		this._width = 1f;
	}

	public override void ShowMesh()
	{
		if (this.subdivisionsWidth < 1)
		{
			this.subdivisionsWidth = 1;
		}
		if (this.subdivisionsHeight < 1)
		{
			this.subdivisionsHeight = 1;
		}
		if (this.height < 0f)
		{
			this.height = 0f;
		}
		if (this.width < 0f)
		{
			this.width = 0f;
		}
		this.demiWidth = this.width / 2f;
		this.demiHeight = this.height / 2f;
		switch (this.align)
		{
		case ParametricPrimitive.eAlign.alignX:
			this.topLeft = (this.invert ? new Vector3(0f, this.demiHeight, this.demiWidth) : new Vector3(0f, this.demiHeight, -this.demiWidth));
			this.topRight = (this.invert ? new Vector3(0f, this.demiHeight, -this.demiWidth) : new Vector3(0f, this.demiHeight, this.demiWidth));
			this.bottomLeft = (this.invert ? new Vector3(0f, -this.demiHeight, this.demiWidth) : new Vector3(0f, -this.demiHeight, -this.demiWidth));
			this.bottomRight = (this.invert ? new Vector3(0f, -this.demiHeight, -this.demiWidth) : new Vector3(0f, -this.demiHeight, this.demiWidth));
//			goto IL_3DF;
			break;
		case ParametricPrimitive.eAlign.alignY:
//			IL_A0:
			this.topLeft = (this.invert ? new Vector3(this.demiWidth, 0f, this.demiHeight) : new Vector3(-this.demiWidth, 0f, this.demiHeight));
			this.topRight = (this.invert ? new Vector3(-this.demiWidth, 0f, this.demiHeight) : new Vector3(this.demiWidth, 0f, this.demiHeight));
			this.bottomLeft = (this.invert ? new Vector3(this.demiWidth, 0f, -this.demiHeight) : new Vector3(-this.demiWidth, 0f, -this.demiHeight));
			this.bottomRight = (this.invert ? new Vector3(-this.demiWidth, 0f, -this.demiHeight) : new Vector3(this.demiWidth, 0f, -this.demiHeight));
//			goto IL_3DF;
			break;
		case ParametricPrimitive.eAlign.alignZ:
			this.topLeft = (this.invert ? new Vector3(-this.demiWidth, this.demiHeight, 0f) : new Vector3(this.demiWidth, this.demiHeight, 0f));
			this.topRight = (this.invert ? new Vector3(this.demiWidth, this.demiHeight, 0f) : new Vector3(-this.demiWidth, this.demiHeight, 0f));
			this.bottomLeft = (this.invert ? new Vector3(-this.demiWidth, -this.demiHeight, 0f) : new Vector3(this.demiWidth, -this.demiHeight, 0f));
			this.bottomRight = (this.invert ? new Vector3(this.demiWidth, -this.demiHeight, 0f) : new Vector3(-this.demiWidth, -this.demiHeight, 0f));
//			goto IL_3DF;
			break;
		}
//		goto IL_A0;
//		IL_3DF:
		this.normal = Vector3.Cross(Vector3.Normalize(this.topLeft - this.bottomLeft), Vector3.Normalize(this.bottomRight - this.bottomLeft));
		this.normal *= ((!this.invertNormal) ? 1f : -1f);
		this.newVertices.Clear();
		this.newTriangles.Clear();
		this.newUV.Clear();
		this.newNormals.Clear();
		this.mesh.Clear();
		float num = this.width / (float)this.subdivisionsWidth;
		float num2 = this.height / (float)this.subdivisionsHeight;
		Vector3 a = Vector3.Normalize(this.bottomLeft - this.topLeft);
		Vector3 a2 = Vector3.Normalize(this.topRight - this.topLeft);
		for (int i = 0; i <= this.subdivisionsHeight; i++)
		{
			for (int j = 0; j <= this.subdivisionsWidth; j++)
			{
				this.newVertices.Add(this.topLeft + (float)j * num * a2 + (float)i * num2 * a);
				this.newUV.Add(new Vector2((float)j / (float)this.subdivisionsWidth, 1f - (float)i / (float)this.subdivisionsHeight));
				this.newNormals.Add(this.normal);
			}
		}
		for (int k = 0; k < this.subdivisionsHeight; k++)
		{
			for (int l = 0; l < this.subdivisionsWidth; l++)
			{
				this.newTriangles.Add(l + (k + 1) * (this.subdivisionsWidth + 1));
				if (!this.invertNormal)
				{
					this.newTriangles.Add(l + k * (this.subdivisionsWidth + 1));
					this.newTriangles.Add(l + 1 + k * (this.subdivisionsWidth + 1));
				}
				else
				{
					this.newTriangles.Add(l + 1 + k * (this.subdivisionsWidth + 1));
					this.newTriangles.Add(l + k * (this.subdivisionsWidth + 1));
				}
				this.newTriangles.Add(l + (k + 1) * (this.subdivisionsWidth + 1));
				if (!this.invertNormal)
				{
					this.newTriangles.Add(l + 1 + k * (this.subdivisionsWidth + 1));
					this.newTriangles.Add(l + 1 + (k + 1) * (this.subdivisionsWidth + 1));
				}
				else
				{
					this.newTriangles.Add(l + 1 + (k + 1) * (this.subdivisionsWidth + 1));
					this.newTriangles.Add(l + 1 + k * (this.subdivisionsWidth + 1));
				}
			}
		}
		this.mesh.vertices = this.newVertices.ToArray();
		this.mesh.triangles = this.newTriangles.ToArray();
		this.mesh.uv = this.newUV.ToArray();
		this.mesh.normals = this.newNormals.ToArray();
		this.meshFilter.mesh = this.mesh;
	}

	protected override string getName()
	{
		return "ParametricPlane";
	}
}
