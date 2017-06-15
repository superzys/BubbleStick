using System;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
	private int subdivionsWidth;

	private int subdivionsHeight;

	private int m_PointX;

	private int m_PointY;

	public Vector3 m_TargetVector = Vector3.zero;

	private Vector3[] m_FirstVectors;

	private Vector3[,] m_NowVectors;

	private Vector3 m_StartPosition = Vector3.zero;

	public bool m_AttackFlag = true;

	private MeshFilter meshFilter;

	public VirtualStick mVSCat;

	public float moveV = 0.06f;

	public float randVR = 0.1f;

	private float dt;

	private bool isPlayingAnim;

	private readonly Vector3 DEFAULT_SCALE = new Vector3 (1f, 1f, 1f);

	private readonly Vector3 MAX_SCALE = new Vector3 (1.7f, 1.7f, 1.7f);

	private readonly Vector3 MIN_SCALE = new Vector3 (0.8f, 0.8f, 0.8f);

	private void Awake ()
	{
		this.meshFilter = base.gameObject.GetComponent<MeshFilter> ();
	}

	private void Start ()
	{
		base.GetComponent<ParametricPlane> ().CreateMesh ();
		this.init ();
	}

	private void OnDisable ()
	{
		iTween.Stop (base.gameObject);
	}

	private void Update ()
	{
		if (this.subdivionsHeight > 0 && this.subdivionsWidth > 0) {
			Vector3[] vertices = this.meshFilter.mesh.vertices;
			if (this.m_TargetVector.y < 0.5f) {
				this.m_TargetVector = Vector3.zero;
			}
			this.SmoothingFilter ();
			for (int i = 0; i < this.subdivionsHeight; i++) {
				for (int j = 0; j < this.subdivionsWidth; j++) {
					int index = this.GetIndex (i, j);
					if (vertices.Length > index) {
						vertices [index] = this.m_NowVectors [i, j];
					}
			

				}
			} 
			this.meshFilter.mesh.vertices = vertices;
		}
	}

	private void init ()
	{
		this.subdivionsWidth = base.GetComponent<ParametricPlane> ()._subdivisionsWidth + 1;
		this.subdivionsHeight = base.GetComponent<ParametricPlane> ()._subdivisionsHeight + 1;
		this.m_PointX = this.subdivionsWidth / 2;
		this.m_PointY = 0;
		this.m_FirstVectors = this.meshFilter.mesh.vertices;
		this.m_NowVectors = new Vector3[this.subdivionsHeight, this.subdivionsWidth];
		for (int i = 0; i < this.subdivionsHeight; i++) {
			for (int j = 0; j < this.subdivionsWidth; j++) {
				int index = this.GetIndex (i, j);
				this.m_NowVectors [i, j] = this.m_FirstVectors [index];
			}
		}
	}

	private void CrushPolygon ()
	{
		this.dt = 0f;
		float num = UnityEngine.Random.Range (5f, 5f + this.randVR);
		float num2 = UnityEngine.Random.Range (0f, 100f);
		for (int i = 0; i < this.subdivionsHeight; i++) {
			for (int j = 0; j < this.subdivionsWidth; j++) {
				int num3 = i * this.subdivionsWidth + j;
				Vector3 vector = this.m_NowVectors [i, j];
				float num4 = Mathf.Atan2 (vector.y, vector.x);
				float num5 = this.moveV * (Mathf.Sin (num4 * num + num2) + 1f) / 2f;
				vector.x = this.m_FirstVectors [num3].x + Mathf.Cos (num4) * num5;
				vector.y = this.m_FirstVectors [num3].y + Mathf.Sin (num4) * num5;
				this.m_NowVectors [i, j] = vector;
			}
		}
	}

	private void ButtonPolygon (float rate)
	{
		this.dt = 0f;
		float num = 4f;
		float num2 = 45f;
		this.moveV = 0.5f * rate;
		base.transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
		for (int i = 0; i < this.subdivionsHeight; i++) {
			for (int j = 0; j < this.subdivionsWidth; j++) {
				int num3 = i * this.subdivionsWidth + j;
				Vector3 vector = this.m_NowVectors [i, j];
				float num4 = Mathf.Atan2 (vector.y, vector.x);
				float num5 = num4 * 57.29578f;
				if (num5 >= -45f && 135f >= num5) {
					float num6 = this.moveV * (Mathf.Sin (num4 * num + num2) + 1f) / 2f;
					vector.x = this.m_FirstVectors [num3].x + Mathf.Cos (num4) * num6;
					vector.y = this.m_FirstVectors [num3].y + Mathf.Sin (num4) * num6;
					this.m_NowVectors [i, j] = vector;
				}
			}
		}
		this.SmoothingFilter ();
	}

	private void SmoothingFilter ()
	{
		for (int i = 0; i < this.subdivionsHeight; i++) {
			for (int j = 0; j < this.subdivionsWidth; j++) {
				int index = this.GetIndex (i, j);
				Vector3 vector = this.m_NowVectors [i, j];
				if (this.m_TargetVector != Vector3.zero) {
					float num = Mathf.Sqrt ((float)((this.m_PointX - j) * (this.m_PointX - j) + (this.m_PointY - i) * (this.m_PointY - i)));
					if (num == 0f) {
						num = 1f;
					}
					int index2 = this.GetIndex (this.m_PointY, this.m_PointX);
					float num2 = this.m_TargetVector.x - this.m_FirstVectors [index2].x;
					float num3 = this.m_TargetVector.y - this.m_FirstVectors [index2].y;
					vector.x = this.m_FirstVectors [index].x + num2 / num;
					vector.y = this.m_FirstVectors [index].y + num3 / num;
					this.dt = 0f;
					if (this.m_StartPosition == Vector3.zero) {
						this.m_StartPosition = base.transform.localPosition;
					}
				} else {
					vector.x = this.m_FirstVectors [index].x * this.dt + this.m_NowVectors [i, j].x * (1f - this.dt);
					vector.y = this.m_FirstVectors [index].y * this.dt + this.m_NowVectors [i, j].y * (1f - this.dt);
					if (this.m_StartPosition != Vector3.zero) {
						this.m_StartPosition = Vector3.zero;
					}
				}
				this.m_NowVectors [i, j] = vector;
			}
		}
		if (this.dt < 1f) {
			this.dt += 0.075f;
		}
	}

	private int GetIndex (int y, int x)
	{
		return y * this.subdivionsWidth + x;
	}

	private Vector3 GetScaleVector (Vector3 vector, Vector3 scale)
	{
		return new Vector3 (vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
	}

	/// <summary>
	/// Fades the in animation.
	/// 寮濮嬫樉绀国
	/// </summary>
	public void FadeInAnim ()
	{
//		Debug.LogWarning ("FadeInAnim");
		this.isPlayingAnim = false;
		float animTime = 0.5f;
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MIN_SCALE;
		this.gameObject.transform.localScale = this.DEFAULT_SCALE;
		base.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, 150);

		iTween.StopByName (base.gameObject, "controllerScale");
		iTween.StopByName (base.gameObject, "controllerColor");
		iTween.StopByName (base.gameObject, "controllerMove");
		iTween.StopByName (base.gameObject, "colorAnim");

		base.transform.localPosition = Vector3.zero;
		iTween.ValueTo (base.gameObject, iTween.Hash (new object[] {
			"name",
			"controllerScale",
			"from",
			0f,
			"to",
			1f,
			"time",
			animTime * 0.15f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"onupdate",
			"",
//			new Action<float>(delegate(float nowValue)
//			{
//				this.gameObject.transform.localScale = Vector3.Lerp(fromScale, toScale, nowValue);
//			}),
			"oncomplete",
			""
//			new Action(delegate
//			{
//				GameObject arg_C3_0 = this.gameObject;
//				object[] expr_12 = new object[14];
//				expr_12[0] = "name";
//				expr_12[1] = "controllerScale";
//				expr_12[2] = "from";
//				expr_12[3] = 0f;
//				expr_12[4] = "to";
//				expr_12[5] = 1f;
//				expr_12[6] = "time";
//				expr_12[7] = animTime * 0.85f;
//				expr_12[8] = "easeType";
//				expr_12[9] = iTween.EaseType.easeOutElastic;
//				expr_12[10] = "onupdate";
//				expr_12[11] = new Action<float>(delegate(float nowValue)
//				{
//					this.gameObject.transform.localScale = Vector3.Lerp(toScale, fromScale, nowValue);
//				});
//				expr_12[12] = "oncomplete";
//				expr_12[13] = new Action(delegate
//				{
//				});
//				iTween.ValueTo(arg_C3_0, iTween.Hash(expr_12));
//			})
		}));
	}

	void FadeInAnimonupdate (float nowValue)
	{
//		Debug.Log ("FadeInAnimonupdate");
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MIN_SCALE;
		this.gameObject.transform.localScale = Vector3.Lerp(fromScale, toScale, nowValue);
	}
	void FadeInAnimonupdate2 (float nowValue)
	{
		//		Debug.Log ("FadeInAnimonupdate");
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MIN_SCALE;
		this.gameObject.transform.localScale = Vector3.Lerp(toScale, fromScale, nowValue);
	}

	void FadeInAnimoncomplete ()
	{
//		Debug.Log ("FadeInAnimoncomplete");
		float animTime = 0.5f;
//		Vector3 fromScale = this.DEFAULT_SCALE;
//		Vector3 toScale = this.MIN_SCALE;
		GameObject arg_C3_0 = this.gameObject;
		object[] expr_12 = new object[14];
		expr_12 [0] = "name";
		expr_12 [1] = "controllerScale";
		expr_12 [2] = "from";
		expr_12 [3] = 0f;
		expr_12 [4] = "to";
		expr_12 [5] = 1f;
		expr_12 [6] = "time";
		expr_12 [7] = animTime * 0.85f;
		expr_12 [8] = "easeType";
		expr_12 [9] = iTween.EaseType.easeOutElastic;
		expr_12 [10] = "onupdate";
		expr_12 [11] = "FadeInAnimonupdate2";
//		expr_12[11] = new Action<float>(delegate(float nowValue)
//		{
//			this.gameObject.transform.localScale = Vector3.Lerp(toScale, fromScale, nowValue);
//		});
		expr_12 [12] = "oncomplete";
		expr_12 [13] = "";
//		expr_12[13] = new Action(delegate
//		{
//		});
		iTween.ValueTo (arg_C3_0, iTween.Hash (expr_12));
	}






	public void FadeOutAnim (bool isMove)
	{
//		Debug.LogWarning ("FadeOutAnim  ----- " + isMove);
		this.isPlayingAnim = false;
		float num = 0.5f;
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MIN_SCALE;
		Vector3 fromPos = Vector3.zero;
		Vector3 toPos = new Vector3 (0f, -30f, 0f);
		base.transform.localPosition = Vector3.zero;
		iTween.StopByName (base.gameObject, "colorAnim");
		iTween.ValueTo (base.gameObject, iTween.Hash (new object[] {
			"name",
			"colorAnim",
			"from",
			0f,
			"to",
			1f,
			"time",
			0.3f,
			"delay",
			0.2f,
			"easeType",
			iTween.EaseType.linear,
			"onupdate",
			"colorAnimupdate",
//			new Action<float>(delegate(float nowValue)
//			{
//				Color color = this.gameObject.GetComponent<Renderer>().material.color;
//				this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color(color.r, color.g, color.b, color.a), new Color(1f, 1f, 1f, 0f), nowValue);
//			}),
			"oncomplete",
			"colorAnimOnComplete"
//			new Action(delegate
//			{
//				this.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
//			})
		}));
		
		if (!isMove) {
			FadeOutAnimEnd ();
			return;
		}
		iTween.StopByName (base.gameObject, "controllerScale");
		GameObject arg_203_0 = base.gameObject;
		object[] expr_15D = new object[14];
		expr_15D [0] = "name";
		expr_15D [1] = "controllerScale";
		expr_15D [2] = "from";
		expr_15D [3] = 0f;
		expr_15D [4] = "to";
		expr_15D [5] = 1f;
		expr_15D [6] = "time";
		expr_15D [7] = num;
		expr_15D [8] = "easeType";
		expr_15D [9] = iTween.EaseType.easeOutElastic;
		expr_15D [10] = "onupdate";
		expr_15D [11] = "controllerScaleOnUpdata";
//		expr_15D[11] = new Action<float>(delegate(float nowValue)
//		{
//			this.gameObject.transform.localScale = Vector3.Lerp(fromScale, toScale, nowValue);
//		});
		expr_15D [12] = "oncomplete";
		expr_15D [13] = "";
//		expr_15D[13] = new Action(delegate
//		{
//		});
		iTween.ValueTo (arg_203_0, iTween.Hash (expr_15D));
		iTween.StopByName (base.gameObject, "controllerMove");
		iTween.ValueTo (base.gameObject, iTween.Hash (new object[] {
			"name",
			"controllerMove",
			"from",
			0f,
			"to",
			1f,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"onupdate",
			"controllerMoveOnupdata",
//			new Action<float>(delegate(float nowValue)
//			{
//				this.gameObject.transform.localPosition = Vector3.Lerp(fromPos, toPos, nowValue);
//			}),
			"oncomplete",
			"FadeOutAnimoncomplete"
//			new Action(delegate
//			{
//				GameObject arg_BC_0 = this.gameObject;
//				object[] expr_12 = new object[14];
//				expr_12[0] = "name";
//				expr_12[1] = "controllerMove";
//				expr_12[2] = "from";
//				expr_12[3] = 0f;
//				expr_12[4] = "to";
//				expr_12[5] = 1f;
//				expr_12[6] = "time";
//				expr_12[7] = 0.4f;
//				expr_12[8] = "easeType";
//				expr_12[9] = iTween.EaseType.easeOutElastic;
//				expr_12[10] = "onupdate";
//				expr_12[11] = new Action<float>(delegate(float nowValue)
//				{
//					this.gameObject.transform.localPosition = Vector3.Lerp(toPos, fromPos, nowValue);
//				});
//				expr_12[12] = "oncomplete";
//				expr_12[13] = new Action(delegate
//				{
//				});
//				iTween.ValueTo(arg_BC_0, iTween.Hash(expr_12));
//			})
		}));
	}

	void colorAnimOnComplete()
	{
		this.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
	}
	void colorAnimupdate (float nowValue)
	{
//		Debug.Log ("FadeInAnimonupdate");
//		Vector3 fromScale = this.DEFAULT_SCALE;
//		Vector3 toScale = this.MIN_SCALE;
		Color color = this.gameObject.GetComponent<Renderer> ().material.color;
		this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (new Color (color.r, color.g, color.b, color.a), new Color (1f, 1f, 1f, 0f), nowValue);
		//		this.gameObject.transform.localScale = Vector3.Lerp(fromScale, toScale, nowValue);
	}
	void controllerScaleOnUpdata(float nowValue)
	{
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MIN_SCALE;
		this.gameObject.transform.localScale = Vector3.Lerp(fromScale, toScale, nowValue);
	}
	void controllerMoveOnupdata(float nowValue)
	{
		Vector3 fromPos = Vector3.zero;
		Vector3 toPos = new Vector3 (0f, -30f, 0f);
		this.gameObject.transform.localPosition = Vector3.Lerp(fromPos, toPos, nowValue);
	}
	void controllerMoveOnupdata2(float nowValue)
	{
		Vector3 fromPos = Vector3.zero;
		Vector3 toPos = new Vector3 (0f, -30f, 0f);
		this.gameObject.transform.localPosition = Vector3.Lerp(toPos, fromPos, nowValue);
	}
	void FadeOutAnimoncomplete ()
	{
//		Debug.Log ("FadeInAnimoncomplete");
		float animTime = 0.5f;
		//		Vector3 fromScale = this.DEFAULT_SCALE;
		//		Vector3 toScale = this.MIN_SCALE;
		GameObject arg_C3_0 = this.gameObject;
		object[] expr_12 = new object[14];
		expr_12 [0] = "name";
		expr_12 [1] = "controllerMove";
		expr_12 [2] = "from";
		expr_12 [3] = 0f;
		expr_12 [4] = "to";
		expr_12 [5] = 1f;
		expr_12 [6] = "time";
		expr_12 [7] = 0.4f;
		expr_12 [8] = "easeType";
		expr_12 [9] = iTween.EaseType.easeOutElastic;
		expr_12 [10] = "onupdate";
		expr_12 [11] = "controllerMoveOnupdata2";
		//		expr_12[11] = new Action<float>(delegate(float nowValue)
		//		{
		//			this.gameObject.transform.localScale = Vector3.Lerp(toScale, fromScale, nowValue);
		//		});
		expr_12 [12] = "oncomplete";
		expr_12 [13] = "FadeOutAnimEnd";
		//		expr_12[13] = new Action(delegate
		//		{
		//		});
		iTween.ValueTo (arg_C3_0, iTween.Hash (expr_12));
	}

	void FadeOutAnimEnd ()
	{
		this.gameObject.transform.localScale = this.DEFAULT_SCALE;
		Debug.Log ("FadeOutAnimEndcall");
		if (mVSCat != null) {
			mVSCat.EndHideBubble ();
		}
	}

	void CrushAnimEnd()
	{
		if (mVSCat != null) {
			mVSCat.EndHideBubble ();
		}
	}


	public void SetVisible (bool isVisible)
	{
		if (isVisible) {
			base.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, 150);
		} else {
			base.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, 0);
		}
	}

	public void ButtonAnim (float rate)
	{
		iTween.StopByName (base.gameObject, "controllerScale");
		iTween.StopByName (base.gameObject, "colorAnim");
		base.transform.localPosition = Vector3.zero;
		this.ButtonPolygon (rate);
	}

	public void CrushAnim ()
	{
		Debug.LogWarning ("CrushAnim");
		this.isPlayingAnim = false;
		float num = 0.3f;
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MAX_SCALE;
		iTween.StopByName (base.gameObject, "controllerScale");
		base.transform.localPosition = Vector3.zero;
		this.CrushPolygon ();
		iTween.ValueTo (base.gameObject, iTween.Hash (new object[] {
			"name",
			"controllerScale",
			"from",
			0f,
			"to",
			1f,
			"time",
			num,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"onupdate",
			"CrushAnimonupdate",
			"oncomplete",
			"CrushAnimEnd"
//			new Action<float>(delegate(float nowValue)
//			{
//				this.gameObject.transform.localScale = Vector3.Lerp(toScale, fromScale, nowValue);
//			})
		}));
	}

	void CrushAnimonupdate (float nowValue)
	{
		Vector3 fromScale = this.DEFAULT_SCALE;
		Vector3 toScale = this.MAX_SCALE;

		Vector3 fromRota = new Vector3 (0, 0, 0);
		Vector3 toRota = new Vector3 (0, 0, 180);

//		Debug.Log ("nowValue ---- " + nowValue);

//		this.gameObject.transform.parent.localEulerAngles = Vector3.Lerp (toRota, fromRota, nowValue);

		this.gameObject.transform.localScale = Vector3.Lerp (toScale, fromScale, nowValue);
	}


	public void ScaleUpDownAnim ()
	{
		Debug.LogWarning ("ScaleUpDownAnim");
		this.isPlayingAnim = false;
		iTween.StopByName (base.gameObject, "controllerScale");
		base.gameObject.transform.localScale = this.DEFAULT_SCALE;
		base.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, 150);
		Vector3 fromScale = new Vector3 (300f, 300f, 1f);
		Vector3 toScale = new Vector3 (800f, 800f, 1f);
		iTween.ValueTo (base.gameObject, iTween.Hash (new object[] {
			"name",
			"controllerScale",
			"from",
			0f,
			"to",
			1f,
			"time",
			0.2f,
			"easeType",
			iTween.EaseType.easeInQuad,
			"onupdate",
			new Action<float> (delegate(float nowValue) {
				this.gameObject.transform.localScale = Vector3.Lerp (fromScale, toScale, nowValue);
			}),
			"oncomplete",
			new Action (delegate {
				GameObject arg_BC_0 = this.gameObject;
				object[] expr_12 = new object[14];
				expr_12 [0] = "name";
				expr_12 [1] = "controllerScale";
				expr_12 [2] = "from";
				expr_12 [3] = 0f;
				expr_12 [4] = "to";
				expr_12 [5] = 1f;
				expr_12 [6] = "time";
				expr_12 [7] = 0.6f;
				expr_12 [8] = "easeType";
				expr_12 [9] = iTween.EaseType.easeOutElastic;
				expr_12 [10] = "onupdate";
				expr_12 [11] = new Action<float> (delegate(float nowValue) {
					this.gameObject.transform.localScale = Vector3.Lerp (toScale, fromScale, nowValue);
				});
				expr_12 [12] = "oncomplete";
				expr_12 [13] = new Action (delegate {
				});
				iTween.ValueTo (arg_BC_0, iTween.Hash (expr_12));
				GameObject arg_194_0 = this.gameObject;
				object[] expr_D3 = new object[16];
				expr_D3 [0] = "name";
				expr_D3 [1] = "controllerColor";
				expr_D3 [2] = "from";
				expr_D3 [3] = 0f;
				expr_D3 [4] = "to";
				expr_D3 [5] = 1f;
				expr_D3 [6] = "time";
				expr_D3 [7] = 0.3f;
				expr_D3 [8] = "delay";
				expr_D3 [9] = 0.1f;
				expr_D3 [10] = "easeType";
				expr_D3 [11] = iTween.EaseType.linear;
				expr_D3 [12] = "onupdate";
				expr_D3 [13] = new Action<float> (delegate(float nowValue) {
					this.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, (byte)(150f - 150f * nowValue));
				});
				expr_D3 [14] = "oncomplete";
				expr_D3 [15] = new Action (delegate {
				});
				iTween.ValueTo (arg_194_0, iTween.Hash (expr_D3));
			})
		}));
	}

	public void ScaleUpAim ()
	{
		Debug.LogWarning ("ScaleUpAim");
		if (this.isPlayingAnim) {
			return;
		}
		base.gameObject.GetComponent<Renderer> ().material.color = new Color32 (255, 255, 255, 150);
		this.isPlayingAnim = true;
		iTween.StopByName (base.gameObject, "controllerScale");
		Vector3 beforeScale = base.gameObject.transform.localScale;
		GameObject arg_130_0 = base.gameObject;
		object[] expr_86 = new object[14];
		expr_86 [0] = "name";
		expr_86 [1] = "controllerScale";
		expr_86 [2] = "from";
		expr_86 [3] = 0f;
		expr_86 [4] = "to";
		expr_86 [5] = 1f;
		expr_86 [6] = "time";
		expr_86 [7] = 0.2f;
		expr_86 [8] = "easeType";
		expr_86 [9] = iTween.EaseType.linear;
		expr_86 [10] = "onupdate";
		expr_86 [11] = new Action<float> (delegate(float nowValue) {
			this.gameObject.transform.localScale = Vector3.Lerp (beforeScale, this.MAX_SCALE, nowValue);
		});
		expr_86 [12] = "oncomplete";
		expr_86 [13] = new Action (delegate {
		});
		iTween.ValueTo (arg_130_0, iTween.Hash (expr_86));
	}
}
