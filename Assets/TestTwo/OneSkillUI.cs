using UnityEngine;
using System.Collections;

public class OneSkillUI : MonoBehaviour
{

	protected int skill;

	public int Skill {
		get {
			return skill;
		}
		set {
			skill = value;
			ShowData ();
		}
	}

//	UISpriteAnimation Img_Tween;
//	UISprite Img_SkillIcon;

	void Awake ()
	{
		Transform find = transform.Find ("Img_Tween");
//		if (find != null) {
//			Img_Tween = find.GetComponent<UISpriteAnimation> ();
//		}
//		find = transform.Find ("Img_SkillIcon");
//		if (find != null) {
//			Img_SkillIcon = find.GetComponent<UISprite> ();
//		}
		ShowData ();
	}
	// Use this for initialization
	void Start ()
	{
	
	}

	void ShowData ()
	{
//		if (Img_SkillIcon != null && skill != null) {
//
//		}
	}
	/// <summary>
	/// Ises the playing.
	/// 是否正在播放;
	/// </summary>
	/// <returns><c>true</c>, if playing was ised, <c>false</c> otherwise.</returns>
	public bool isPlaying()
	{
//		if(Img_Tween != null)
//		{
//			return Img_Tween.isPlaying;
//		}
		return false;
	}

	public void Play()
	{
//		if(Img_Tween != null)
//		{
//			Img_Tween.ResetToBeginning ();
//			Img_Tween.Play ();
//		}
	}

}
