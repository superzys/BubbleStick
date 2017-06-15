using System;
using UnityEngine;

public class PowerController : MonoBehaviour
{
	[SerializeField]
	public GameObject m_RotateObj;

	[SerializeField]
	private ParticleEmitter m_EffectBig;

	[SerializeField]
	private ParticleEmitter m_EffectSmall;

	[SerializeField]
	private GameObject m_FingerLight;

	[SerializeField]
	private SlimeController m_SlimeController;

	[SerializeField]
	private GameObject skill;

	[SerializeField]
	private GameObject skill1;

	[SerializeField]
	private GameObject skill2;

	[SerializeField]
	private GameObject skill3;

	[SerializeField]
	private GameObject skill4;

	[SerializeField]
	private GameObject skill1Invalid;

	[SerializeField]
	private GameObject skill2Invalid;

	[SerializeField]
	private GameObject skill3Invalid;

	[SerializeField]
	private GameObject skill4Invalid;

	private float m_FingerLightScale = 1f;

	private Vector3 m_BasePos;

	private Vector3 m_ToPos;

	private void Start()
	{
		GameObject arg_CE_0 = this.m_FingerLight.gameObject;
		object[] expr_12 = new object[16];
		expr_12[0] = "name";
		expr_12[1] = "scale";
		expr_12[2] = "from";
		expr_12[3] = 0f;
		expr_12[4] = "to";
		expr_12[5] = 1f;
		expr_12[6] = "time";
		expr_12[7] = 0.4f;
		expr_12[8] = "easeType";
		expr_12[9] = iTween.EaseType.easeInOutQuart;
		expr_12[10] = "loopType";
		expr_12[11] = iTween.LoopType.pingPong;
		expr_12[12] = "onupdate";
		expr_12[13] = new Action<float>(delegate(float nowValue)
		{
			float num = (nowValue - 0.5f) / 4f;
			this.m_FingerLight.transform.localScale = new Vector3(this.m_FingerLightScale + num, this.m_FingerLightScale + num, 0f);
		});
		expr_12[14] = "oncomplete";
		expr_12[15] = new Action(delegate
		{
		});
		iTween.ValueTo(arg_CE_0, iTween.Hash(expr_12));
	}

	private void Update()
	{
	}

	public void StartController(Vector3 iBasePos)
	{
		this.m_BasePos = iBasePos;
		this.m_ToPos = iBasePos;
		base.transform.localPosition = this.m_BasePos;
		this.m_SlimeController.FadeInAnim();
		this.m_RotateObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.m_FingerLightScale = 0f;
		this.m_FingerLight.transform.localPosition = Vector3.zero;
		this.m_EffectBig.transform.localPosition = Vector3.zero;
		this.m_EffectSmall.transform.localPosition = Vector3.zero;
		this.m_EffectBig.worldVelocity = Vector3.zero;
		this.m_EffectSmall.worldVelocity = Vector3.zero;
		this.m_EffectBig.emit = true;
		this.m_EffectSmall.emit = true;
		iTween.StopByName(this.m_FingerLight.gameObject, "color");
		GameObject arg_1A1_0 = this.m_FingerLight.gameObject;
		object[] expr_F7 = new object[14];
		expr_F7[0] = "name";
		expr_F7[1] = "color";
		expr_F7[2] = "from";
		expr_F7[3] = 0f;
		expr_F7[4] = "to";
		expr_F7[5] = 1f;
		expr_F7[6] = "time";
		expr_F7[7] = 0.1f;
		expr_F7[8] = "easeType";
		expr_F7[9] = iTween.EaseType.linear;
		expr_F7[10] = "onupdate";
		expr_F7[11] = new Action<float>(delegate(float nowValue)
		{
//			this.m_FingerLight.renderer.material.color = this.ColorAlphaChange(Color.white, nowValue);
		});
		expr_F7[12] = "oncomplete";
		expr_F7[13] = new Action(delegate
		{
		});
		iTween.ValueTo(arg_1A1_0, iTween.Hash(expr_F7));
	}

	public void MoveController(Vector3 iMovePos)
	{
		this.m_ToPos = iMovePos;
		Vector3 vector = this.m_ToPos - this.m_BasePos;
		float num = Vector3.Distance(this.m_BasePos, this.m_ToPos);
		float num2 = Vector3.Angle(Vector3.up, vector) * Mathf.Sign(vector.x);
		this.m_SlimeController.m_TargetVector = new Vector3(0f, num / this.m_SlimeController.transform.localScale.y, 0f);
		this.m_RotateObj.transform.localRotation = Quaternion.Euler(0f, 0f, num2 * -1f);
//		this.m_FingerLight.renderer.material.color = this.ColorAlphaChange(Color.white, 1f);
		this.m_FingerLightScale = Mathf.Min(num / 150f, 1f);
		this.m_FingerLight.transform.localPosition = new Vector3(0f, num, 0f);
		this.m_EffectBig.transform.localPosition = new Vector3(0f, num, 0f);
		this.m_EffectSmall.transform.localPosition = new Vector3(0f, num, 0f);
		this.m_EffectBig.worldVelocity = vector * -1f * 0.8f;
		this.m_EffectSmall.worldVelocity = vector * -1f * 0.9f;
		this.m_SlimeController.SetVisible(true);
	}

	public void EndController(bool iAttackFlag)
	{
		if (iAttackFlag)
		{
			this.m_SlimeController.CrushAnim();
		}
		else
		{
			float num = Vector3.Distance(this.m_BasePos, this.m_ToPos);
			this.m_SlimeController.FadeOutAnim(num > 5f);
		}
		this.m_SlimeController.m_TargetVector = Vector3.zero;
		this.m_EffectBig.emit = false;
		this.m_EffectSmall.emit = false;
//		this.m_FingerLight.renderer.material.color = this.ColorAlphaChange(Color.white, 0f);
		iTween.StopByName(this.m_FingerLight.gameObject, "color");
	}

	public void SetVisible(bool isVisible)
	{
		if (isVisible)
		{
//			this.m_FingerLight.renderer.material.color = this.ColorAlphaChange(Color.white, 1f);
			iTween.StopByName(this.m_FingerLight.gameObject, "color");
		}
		else
		{
			this.m_SlimeController.m_TargetVector = Vector3.zero;
//			this.m_FingerLight.renderer.material.color = this.ColorAlphaChange(Color.white, 0f);
			iTween.StopByName(this.m_FingerLight.gameObject, "color");
		}
		this.m_SlimeController.SetVisible(isVisible);
	}

	private Color ColorAlphaChange(Color iColor, float iAlpha)
	{
		return new Color(iColor.r, iColor.g, iColor.b, iAlpha);
	}

	public void SetSkillVisible(bool flag)
	{
		this.skill.SetActive(flag);
	}

//	public void SetSkillCategoryIcon(int slotNo, Refer.ActionSkillCategory category)
//	{
//		UISprite uISprite = null;
//		string spriteName = string.Empty;
//		switch (category)
//		{
//		case Refer.ActionSkillCategory.NearAttack:
//			spriteName = "btn_skill_nearattack";
//			break;
//		case Refer.ActionSkillCategory.FarAttack:
//			spriteName = "btn_skill_farattack";
//			break;
//		case Refer.ActionSkillCategory.Reinforcement:
//			spriteName = "btn_skill_reinforcement";
//			break;
//		case Refer.ActionSkillCategory.Heal:
//			spriteName = "btn_skill_heal";
//			break;
//		}
//		switch (slotNo)
//		{
//		case 0:
//			uISprite = this.skill1.GetComponent<UISprite>();
//			break;
//		case 1:
//			uISprite = this.skill2.GetComponent<UISprite>();
//			break;
//		case 2:
//			uISprite = this.skill3.GetComponent<UISprite>();
//			break;
//		case 3:
//			uISprite = this.skill4.GetComponent<UISprite>();
//			break;
//		}
//		uISprite.spriteName = spriteName;
//	}

	public void UpdateCharging(float rate)
	{
		this.m_RotateObj.transform.localRotation = Quaternion.Euler(0f, 0f, 35f);
//		this.m_SlimeController.renderer.material.color = Color.Lerp(new Color(1f, 1f, 1f, 0.5882353f), new Color(0.0509803928f, 0.619607866f, 0.796078444f, 0.8f), rate);
		this.m_SlimeController.ButtonAnim(rate);
	}

	public bool IsSkillActive()
	{
		return this.skill.activeInHierarchy;
	}

	public void SetSkillActive(int slotNo, bool flag)
	{
		switch (slotNo)
		{
		case 0:
			this.skill1.SetActive(flag);
			break;
		case 1:
			this.skill2.SetActive(flag);
			break;
		case 2:
			this.skill3.SetActive(flag);
			break;
		case 3:
			this.skill4.SetActive(flag);
			break;
		}
	}

	public void SetSkillInvalidActive(int slotNo, bool flag)
	{
		switch (slotNo)
		{
		case 0:
			this.skill1Invalid.SetActive(flag);
			break;
		case 1:
			this.skill2Invalid.SetActive(flag);
			break;
		case 2:
			this.skill3Invalid.SetActive(flag);
			break;
		case 3:
			this.skill4Invalid.SetActive(flag);
			break;
		}
	}
}
