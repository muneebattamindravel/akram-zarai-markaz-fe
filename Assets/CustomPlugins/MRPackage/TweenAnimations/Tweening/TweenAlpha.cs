using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's alpha.
/// </summary>

public class TweenAlpha : UITweener
{
	[Space (15)]
	[Range (0f, 1f)]
	public float from = 1f;
	[Range (0f, 1f)]
	public float to = 1f;

	bool mCached = false;
	SpriteRenderer mRect;
	Material mMat;
	SpriteRenderer mSr;
	Text mText;
	Image mImage;
	CanvasGroup mCanvasGroup;

	[System.Obsolete ("Use 'value' instead")]
	public float alpha { get { return this.value; } set { this.value = value; } }

	void Cache ()
	{
		mCached = true;

		mCanvasGroup = GetComponent<CanvasGroup> ();

		if (mCanvasGroup != null)
			return;

		mImage = GetComponent<Image> ();

		if (mImage != null)
			return;

		mSr = GetComponent<SpriteRenderer> ();

		if (mSr != null)
			return;

		mText = GetComponent<Text> ();

		if (mText != null)
			return;





	}

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value {
		get {
			if (!mCached)
				Cache ();
			if (mText != null)
				return mText.color.a;
			if (mImage != null)
				return mImage.color.a;
			if (mSr != null)
				return mSr.color.a;
			if (mCanvasGroup != null)
				return mCanvasGroup.alpha;
			

			return mMat != null ? mMat.color.a : 1f;
		}
		set {
			if (!mCached)
				Cache ();

			if (mImage != null) {
				Color c = mImage.color;
				c.a = value;
				mImage.color = c;
			} else if (mSr != null) {
				Color c = mSr.color;
				c.a = value;
				mSr.color = c;
			} else if (mMat != null) {
				Color c = mMat.color;
				c.a = value;
				mMat.color = c;
			} else if (mText != null) {
				Color c = mText.color;
				c.a = value;
				mText.color = c;
			} else if (mCanvasGroup != null) {
				mCanvasGroup.alpha = value;
			}

		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = Mathf.Lerp (from, to, factor);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenAlpha Begin (GameObject go, float duration, float alpha)
	{
		TweenAlpha comp = UITweener.Begin<TweenAlpha> (go, duration);
		comp.from = comp.value;
		comp.to = alpha;

		if (duration <= 0f) {
			comp.Sample (1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	public override void SetStartToCurrentValue ()
	{
		from = value;
	}

	public override void SetEndToCurrentValue ()
	{
		to = value;
	}
}
