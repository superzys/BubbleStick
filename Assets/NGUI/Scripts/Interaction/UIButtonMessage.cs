//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Message (Legacy)")]
public class UIButtonMessage : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren = false;

	bool mStarted = false;

	void Start () { mStarted = true; }

	void OnEnable () { if (mStarted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (((isOver && trigger == Trigger.OnMouseOver) ||
				(!isOver && trigger == Trigger.OnMouseOut))) Send();
		}
	}



    //原版
	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (((isPressed && trigger == Trigger.OnPress) ||
				(!isPressed && trigger == Trigger.OnRelease))) Send();
		}
	}
    //改写的wumin
    void OnPress_new(bool isPressed)
    {
        Debug.Log("监听按下");
        if (trigger == Trigger.OnPress)
        {
            if (isPressed)
            {
                //  isPress = true;

                Debug.Log("监听按下");
            }
            else
            {
                //isPress = false;
            }
        }
        else if (trigger == Trigger.OnRelease)
        {
            //松开时发送消息  
            if (!isPressed)
            {
                Debug.Log("监听松开");
                Send();
            }
        }

    }

    void OnSelect (bool isSelected)
	{
		if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}

	void OnClick () { if (enabled && trigger == Trigger.OnClick) Send(); }

	void OnDoubleClick () { if (enabled && trigger == Trigger.OnDoubleClick) Send(); }

	void Send ()
	{
		if (string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;

		if (includeChildren)
		{
			Transform[] transforms = target.GetComponentsInChildren<Transform>();

			for (int i = 0, imax = transforms.Length; i < imax; ++i)
			{
				Transform t = transforms[i];
				t.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
