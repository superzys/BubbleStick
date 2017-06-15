using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class VirtualStick : MonoBehaviour
{
	[SerializeField]
	private SlimeController m_SlimeController;



	public delegate void MoveCallBack (Vector2 dirVer, bool isRun);

	public delegate void StopMoveCallBack ();

	public delegate void AttackCallBack (Vector2 dirVer);


	public delegate void CastSkillCallBack (int skillID);

	public  MoveCallBack OnMoveCall;
	public  StopMoveCallBack OnStopMoveCall;
	public  AttackCallBack OnAttackCall;
	public  CastSkillCallBack CastSkillCall;
	/// <summary>
	/// T//////////////////////////////////////////////////////////////////////////////////////////////	/// </summary>

	private bool isPress = false;
	private bool isShowStick = false;
	private bool isMoved = false;
	private bool isInSkillArea = false;
	private bool isShowSkillCmp = false;

	private bool isCounting = false;
	private int countNum;

	public int MaxWaiteTime = 50;
	float minMoveDir = 20f;
	// 偏移量 小于此值 不进行移动;
	float walkRunDir = 211f;
	/// <summary>
	/// The skill area.
	/// 技能检测范围;
	/// </summary>
	public int skillArea = 20;

	public Transform touchCmp;
	public Transform skillCmp;
	public Transform stickCmp;
	public Transform TipCmp;

	public Transform MouseSprite;

//	public Transform m_skill_1;
//	public Transform m_skill_2;
//	public Transform m_skill_3;

	Vector2 touchpos;
	Vector2 touchWorldpos;
	Vector3 oldTouchPos;
	Vector3 oldWorldPos;
	float oldZ = 0f;

	public Transform DirArea;


	//	public Panel_gameBtn_ctrl gmCtr;
	//---------------------   轨迹展示
	//最大距离
	private const float MaxDis = 3f;


	bool isRun = false;
	public bool IsCanShowSkill = true;
	//是否点击到
	private bool m_IsClick;
	//是否在移动
	private bool m_IsMove;
	//链接的mesh的点
	private Vector3[] m_Vertices;

	#region 技能

	Transform skill_Tem;


	List<OneSkillUI> skillUIArr = new List<OneSkillUI>();

	public List<int> m_SkillIds = new List<int> ();

	/// <summary>
	/// The max skill gap.
	/// 最大间隙，技能之间的距离不能超过这个;
	/// </summary>
	public int MaxSkillGap = 120;
	/// <summary>
	/// The skill start z.
	/// 第一个技能开始的角度;
	/// </summary>
	public int SkillStartZ = 240;

	int skillUIGap = 120;

	public float EffectiveNum = 0.5f;

	#endregion
	// Use this for initialization
	void Awake ()
	{
		oldZ = stickCmp.position.z;
		if (stickCmp == null) {
			Debug.LogError (" stickCmp is null ");
		}

		if (skillCmp != null) {
			skill_Tem = skillCmp.Find ("Skill_Tem");
			if(skill_Tem != null)
			{
				skill_Tem.gameObject.SetActive (false);
			}

		}
	}


	// Use this for initialization
	void Start ()
	{
		if (skill_Tem != null && m_SkillIds.Count > 0) {
			skillUIArr = new List<OneSkillUI> ();
			 skillUIGap = 360 / m_SkillIds.Count;
			if(skillUIGap > MaxSkillGap)
			{
				skillUIGap = MaxSkillGap;
			}
			for (int i = 0; i < m_SkillIds.Count; i++) {
				int skill = m_SkillIds[i];
				OneSkillUI skillUI = InstantiateObjFun.AddOneObjToParent < OneSkillUI> (skill_Tem.gameObject, skillCmp);
				skillUI.transform.localRotation =Quaternion.Euler(0, 0, SkillStartZ - i * skillUIGap);
//					new Quaternion (oldRat.x, oldRat.y, oldRat.z + i * gap, oldRat.w);
				skillUI.gameObject.SetActive (true);
				skillUIArr.Add (skillUI);
			}
		}

		ResetClickDirArea ();

		if (TipCmp != null) {
			Transform top = TipCmp.Find ("Top");
			Transform end = TipCmp.Find ("End");
			walkRunDir = Vector3.Distance (top.localPosition, end.localPosition);
		}
	}

	// Update is called once per frame
	/// <summary>
	/// Update this instance.
	/// 计时。如果点击位置在初始位置周边一个范围内开始计时;
	/// 
	/// </summary>
	void Update ()
	{
		isInSkillArea = false;
		isMoved = false;

		Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 0f;
		isRun = false;
		//点击按下后显示起点终点与轨迹;
		if (isPress) {

			touchpos = Input.mousePosition;

			touchWorldpos = pos;
			float dirX = touchpos.x - oldTouchPos.x;
			float dirY = touchpos.y - oldTouchPos.y;
			//---此部分是圆形区域检测
			float px = 750f / (float)Screen.width;

			float dir = Vector3.Distance (touchpos, oldTouchPos) * px;
			if (dir >= minMoveDir) {
				isMoved = true;
			}
			if (dir >= walkRunDir) {
//				Debug.LogError ("zys  dir  "+dir +"  walkRunDir " + walkRunDir);
				isRun = true;
//				mStick.isRun = isRun;
			} else {
//				mStick.isRun = isRun;
			}

			this.m_SlimeController.m_TargetVector = new Vector3 (0f, dir / this.m_SlimeController.transform.localScale.y, 0f);

			this.m_SlimeController.SetVisible (true);
			///---------------  此部分是矩形区域检测

			if (!isShowSkillCmp && isPress && IsCanShowSkill) {
				DirArea.gameObject.SetActive (true);
				if (dir < skillArea) {
					isInSkillArea = true;
				}
				if (isInSkillArea) {
					countNum++;
					if (countNum >= MaxWaiteTime) {
						isShowSkillCmp = true;
						skillCmp.gameObject.SetActive (true);
					}
				} else {
					countNum = 0;
				}


//				Debug.Log (angle);

			}
			if (isShowSkillCmp) {//技能显示了 根据两点位置算当前选中的技能;
				
			}

			Vector3 curTouchPos = Input.mousePosition;
			Vector3 curWorldPos = UICamera.mainCamera.ScreenToWorldPoint (curTouchPos);

			Vector3 atanPos = curWorldPos - oldWorldPos;

			float angle = Mathf.Atan2 (atanPos.y, atanPos.x) * 180 / Mathf.PI;
			DirArea.localEulerAngles = new Vector3 (0, 0, -90f + angle);


			MouseSprite.transform.position = curWorldPos;

			Vector2 dirVer = new Vector2 (dirX, dirY);
//			if(mStick != null)
//			mStick.UpdateDir (dirVer);

			if (OnMoveCall != null) {
				OnMoveCall (dirVer, isRun);
			}

		}
	}

	/// <summary>
	/// Raises the press event.
	/// 如果摇杆没 出来的话在当前位置显示摇杆；
	/// 
	/// </summary>
	/// <param name="isPress">If set to <c>true</c> is press.</param>
	void OnPress (bool isPress)
	{ 
		this.isPress = isPress;
		if (isPress) {
			ClickDown ();
		} else {
			ClickUp ();
		}
	}

	/// 
	//辅助 发起时的 位置 检测;
	bool can_posi_allow = false;
	Ray ray;

	/// <summary>
	/// Clicks down.
	/// 点击按下;
	/// </summary>
	void ClickDown ()
	{
		if (!isShowStick) {
			isShowStick = true;
			oldTouchPos = Input.mousePosition;

			oldWorldPos = UICamera.mainCamera.ScreenToWorldPoint (oldTouchPos);

			Vector3 last = UICamera.mainCamera.ScreenToViewportPoint (Input.mousePosition);


			MouseSprite.gameObject.SetActive (true);
			stickCmp.gameObject.SetActive (true);
			TipCmp.gameObject.SetActive (true);
//			if(OldStickTrans != null)
//			{
//				OldStickTrans.gameObject.SetActive (true);
//			}
			touchCmp.position = oldWorldPos;
			skillCmp.position = oldWorldPos;
			TipCmp.position = oldWorldPos;
			DirArea.position = oldWorldPos;

			this.m_SlimeController.FadeInAnim ();
		}
	}

	/// <summary>
	/// Clicks up.
	/// 点击弹起;
	/// </summary>
	void ClickUp ()
	{
		//-----------------  进行技能轨迹和技能位置检测用来放技能;
//		if(mStick != null)
//		mStick.EndPress ();
//		if(gmCtr != null && !isMoved && !isShowSkillCmp)
//		{
//			gmCtr.btn_killerAttack ();
//		}

		if (!isMoved && !isShowSkillCmp) {
			this.m_SlimeController.CrushAnim ();
		} else {

			float num = Vector3.Distance (this.oldTouchPos, this.touchpos);
			this.m_SlimeController.FadeOutAnim (num > 5f);
		}
		this.m_SlimeController.m_TargetVector = Vector3.zero;

		if (!isMoved && !isShowSkillCmp) {
			if (OnAttackCall != null) {
				OnAttackCall (touchWorldpos);
			}
		} else {
			if (OnStopMoveCall != null) {
				OnStopMoveCall ();
			}

		}


		if (isShowSkillCmp && skillUIArr.Count >0) {
			bool canUseSkill = false;
			OneSkillUI temSkill = skillUIArr [0];
			if (temSkill.isPlaying()) {
				canUseSkill = false;
			} else {
				canUseSkill = true;
			}

			float num = Vector3.Distance (this.oldTouchPos, this.touchpos);
			if (num < 80) {
				canUseSkill = false;
			}

			float rad = DirArea.localEulerAngles.z;
			int Irad = (180 + (int)rad )%360;
			int useSkilIndex = -1;

			int startR = ( SkillStartZ + skillUIGap / 2);//从哪个角度开始第一个;

			int curMouseR = startR - (int)Irad ;
			curMouseR = (curMouseR + 360)%360;

			int chooseIndex = curMouseR / skillUIGap;//命中第几区间;

			int morR =  curMouseR % skillUIGap;//在某区间内多少角度;
			int minGap = Mathf.Abs (morR - skillUIGap / 2);//某区间与核心位置绝对值;

			if( minGap <( EffectiveNum * skillUIGap /2) )
			{
				useSkilIndex = chooseIndex;
			}
			if (useSkilIndex >= 0) {
				if (m_SkillIds.Count > useSkilIndex) {
					int skill = m_SkillIds [useSkilIndex ];		

					if(CastSkillCall != null)
						CastSkillCall (skill);
					
				} else {
					Debug.LogWarning ("该位置么有技能 ---index--- " + useSkilIndex);
				}
			}
		}

		MouseSprite.gameObject.SetActive (false);
		TipCmp.gameObject.SetActive (false);
//		skillCmp.gameObject.SetActive (false);
//		stickCmp.gameObject.SetActive (false);
//		TipCmp.gameObject.SetActive (false);

//		if(OldStickTrans != null)
//		{
//			OldStickTrans.gameObject.SetActive (false);
//		}

//		oldTouchPos = null;
		//------  轨迹重置;
		ResetClickDirArea ();

	}



	/// <summary>
	/// Resets the click dir area.
	/// 重置退拽轨迹;
	/// </summary>
	void ResetClickDirArea ()
	{

		this.isMoved = false;
		this.isCounting = false;
		this.isShowStick = false;
		this.countNum = 0;
		isInSkillArea = false;
		isShowSkillCmp = false;

		for (int i = 0; i < skillUIArr.Count; i++) {
			OneSkillUI temSkill = skillUIArr [i];

			temSkill.Play ();
		}
		skillCmp.gameObject.SetActive (false);

	}

	public void EndHideBubble ()
	{
		stickCmp.gameObject.SetActive (false);
	}


}
