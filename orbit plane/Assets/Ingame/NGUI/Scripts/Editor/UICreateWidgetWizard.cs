﻿//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Widget Creation Wizard
/// </summary>

public class UICreateWidgetWizard : EditorWindow
{
	public enum WidgetType
	{
		Label,
		Sprite,
		SlicedSprite,
		TiledSprite,
		FilledSprite,
		SimpleTexture,
		Button,
		ImageButton,
		Checkbox,
		ProgressBar,
		Slider,
		Input,
		PopupList,
		PopupMenu,
	}

	static WidgetType mType = WidgetType.Button;
	static string mSprite = "";
	static string mSliced = "";
	static string mTiled = "";
	static string mFilled = "";
	static string mButton = "";
	static string mImage0 = "";
	static string mImage1 = "";
	static string mImage2 = "";
	static string mSliderBG = "";
	static string mSliderFG = "";
	static string mSliderTB = "";
	static string mCheckBG = "";
	static string mCheck = "";
	static string mInputBG = "";
	static string mListFG = "";
	static string mListBG = "";
	static string mListHL = "";
	static Color mColor = Color.white;
	static bool mLoaded = false;

	/// <summary>
	/// Save the specified string into player prefs.
	/// </summary>

	static void SaveString (string field, string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			PlayerPrefs.DeleteKey(field);
		}
		else
		{
			PlayerPrefs.SetString(field, val);
		}
	}

	/// <summary>
	/// Load the specified string from player prefs.
	/// </summary>

	static string LoadString (string field) { string s = PlayerPrefs.GetString(field); return (string.IsNullOrEmpty(s)) ? "" : s; }

	/// <summary>
	/// Save all serialized values in player prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Save ()
	{
		PlayerPrefs.SetInt("NGUI Widget Type", (int)mType);
		PlayerPrefs.SetInt("NGUI Color", NGUIMath.ColorToInt(mColor));

		SaveString("NGUI Sprite", mSprite);
		SaveString("NGUI Sliced", mSliced);
		SaveString("NGUI Tiled", mTiled);
		SaveString("NGUI Filled", mFilled);
		SaveString("NGUI Button", mButton);
		SaveString("NGUI Image 0", mImage0);
		SaveString("NGUI Image 1", mImage1);
		SaveString("NGUI Image 2", mImage2);
		SaveString("NGUI CheckBG", mCheckBG);
		SaveString("NGUI Check", mCheck);
		SaveString("NGUI SliderBG", mSliderBG);
		SaveString("NGUI SliderFG", mSliderFG);
		SaveString("NGUI SliderTB", mSliderTB);
		SaveString("NGUI InputBG", mInputBG);
		SaveString("NGUI ListFG", mListFG);
		SaveString("NGUI ListBG", mListBG);
		SaveString("NGUI ListHL", mListHL);

		PlayerPrefs.Save();
	}

	/// <summary>
	/// Load all serialized values from Player Prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Load ()
	{
		mType = (WidgetType)PlayerPrefs.GetInt("NGUI Widget Type", 0);

		int color = PlayerPrefs.GetInt("NGUI Color", -1);
		if (color != -1) mColor = NGUIMath.IntToColor(color);

		mSprite		= LoadString("NGUI Sprite");
		mSliced		= LoadString("NGUI Sliced");
		mTiled		= LoadString("NGUI Tiled");
		mFilled		= LoadString("NGUI Filled");
		mButton		= LoadString("NGUI Button");
		mImage0		= LoadString("NGUI Image 0");
		mImage1		= LoadString("NGUI Image 1");
		mImage2		= LoadString("NGUI Image 2");
		mCheckBG	= LoadString("NGUI CheckBG");
		mCheck		= LoadString("NGUI Check");
		mSliderBG	= LoadString("NGUI SliderBG");
		mSliderFG	= LoadString("NGUI SliderFG");
		mSliderTB	= LoadString("NGUI SliderTB");
		mInputBG	= LoadString("NGUI InputBG");
		mListFG		= LoadString("NGUI ListFG");
		mListBG		= LoadString("NGUI ListBG");
		mListHL		= LoadString("NGUI ListHL");
	}

	/// <summary>
	/// Atlas selection function.
	/// </summary>

	void OnSelectAtlas (MonoBehaviour obj)
	{
		UISettings.atlas = obj as UIAtlas;
		Repaint();
	}

	/// <summary>
	/// Font selection function.
	/// </summary>

	void OnSelectFont (MonoBehaviour obj)
	{
		UISettings.font = obj as UIFont;
		Repaint();
	}

	/// <summary>
	/// Convenience function -- creates the "Add To" button and the parent object field to the right of it.
	/// </summary>

	bool ShouldCreate (GameObject go, bool isValid)
	{
		GUI.color = isValid ? Color.green : Color.grey;

		GUILayout.BeginHorizontal();
		bool retVal = GUILayout.Button("Add To", GUILayout.Width(76f));
		GUI.color = Color.white;
		GameObject sel = EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(140f)) as GameObject;
		GUILayout.Label("Select the parent in the Hierarchy View", GUILayout.MinWidth(10000f));
		GUILayout.EndHorizontal();

		if (sel != go) Selection.activeGameObject = sel;

		if (retVal && isValid)
		{
			NGUIEditorTools.RegisterUndo("Add a Widget");
			return true;
		}
		return false;
	}

	/// <summary>
	/// Label creation function.
	/// </summary>

	void CreateLabel (GameObject go)
	{
		GUILayout.BeginHorizontal();
		Color c = EditorGUILayout.ColorField("Color", mColor, GUILayout.Width(220f));
		GUILayout.Label("Color tint the label will start with");
		GUILayout.EndHorizontal();

		if (mColor != c)
		{
			mColor = c;
			Save();
		}

		if (ShouldCreate(go, UISettings.font != null))
		{
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = UISettings.font;
			lbl.text = "New Label";
			lbl.color = mColor;
			lbl.MakePixelPerfect();
			Selection.activeGameObject = lbl.gameObject;
		}
	}

	/// <summary>
	/// Sprite creation function.
	/// </summary>

	void CreateSprite<T> (GameObject go, ref string field) where T : UISprite
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string sp = UISpriteInspector.SpriteField(UISettings.atlas, "Sprite", field, GUILayout.Width(200f));

			if (!string.IsNullOrEmpty(sp))
			{
				GUILayout.Space(20f);
				GUILayout.Label("Sprite that will be created");
			}
			GUILayout.EndHorizontal();

			if (sp != field)
			{
				field = sp;
				Save();
			}
		}

		if (ShouldCreate(go, UISettings.atlas != null))
		{
			T sprite = NGUITools.AddWidget<T>(go);
			sprite.name = sprite.name + " (" + field + ")";
			sprite.atlas = UISettings.atlas;
			sprite.spriteName = field;
			sprite.MakePixelPerfect();
			Selection.activeGameObject = sprite.gameObject;
		}
	}

	/// <summary>
	/// UI Texture doesn't do anything other than creating the widget.
	/// </summary>

	void CreateSimpleTexture (GameObject go)
	{
		if (ShouldCreate(go, true))
		{
			UITexture tex = NGUITools.AddWidget<UITexture>(go);
			Selection.activeGameObject = tex.gameObject;
		}
	}

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateButton (GameObject go)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string bg = UISpriteInspector.SpriteField(UISettings.atlas, "Background", mButton, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sliced Sprite for the background");
			GUILayout.EndHorizontal();
			if (mButton != bg) { mButton = bg; Save(); }
		}

		if (ShouldCreate(go, UISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Button";

			UISlicedSprite bg = NGUITools.AddWidget<UISlicedSprite>(go);
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = UISettings.atlas;
			bg.spriteName = mButton;
			bg.transform.localScale = new Vector3(150f, 40f, 1f);
			bg.MakePixelPerfect();

			if (UISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = UISettings.font;
				lbl.text = go.name;
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIButtonColor>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>();
			go.AddComponent<UIButtonOffset>();
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateImageButton (GameObject go)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string bg = UISpriteInspector.SpriteField(UISettings.atlas, "Normal", mImage0, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Normal state sprite");
			GUILayout.EndHorizontal();
			if (mImage0 != bg) { mImage0 = bg; Save(); }

			GUILayout.BeginHorizontal();
			bg = UISpriteInspector.SpriteField(UISettings.atlas, "Hover", mImage1, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Hover state sprite");
			GUILayout.EndHorizontal();
			if (mImage1 != bg) { mImage1 = bg; Save(); }

			GUILayout.BeginHorizontal();
			bg = UISpriteInspector.SpriteField(UISettings.atlas, "Pressed", mImage2, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Pressed state sprite");
			GUILayout.EndHorizontal();
			if (mImage2 != bg) { mImage2 = bg; Save(); }
		}

		if (ShouldCreate(go, UISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Image Button";

			UIAtlas.Sprite sp = UISettings.atlas.GetSprite(mImage0);
			UISprite sprite = (sp.inner == sp.outer) ? NGUITools.AddWidget<UISprite>(go) :
				(UISprite)NGUITools.AddWidget<UISlicedSprite>(go);

			sprite.name = "Background";
			sprite.depth = depth;
			sprite.atlas = UISettings.atlas;
			sprite.spriteName = mImage0;
			sprite.transform.localScale = new Vector3(150f, 40f, 1f);
			sprite.MakePixelPerfect();

			if (UISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = UISettings.font;
				lbl.text = go.name;
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			UIImageButton ib = go.AddComponent<UIImageButton>();
			ib.target		 = sprite;
			ib.normalSprite  = mImage0;
			ib.hoverSprite	 = mImage1;
			ib.pressedSprite = mImage2;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Checkbox creation function.
	/// </summary>

	void CreateCheckbox (GameObject go)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string bg = UISpriteInspector.SpriteField(UISettings.atlas, "Background", mCheckBG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sliced Sprite for the background");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			string ck = UISpriteInspector.SpriteField(UISettings.atlas, "Checkmark", mCheck, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sprite visible when the state is 'checked'");
			GUILayout.EndHorizontal();

			if (mCheckBG != bg || mCheck != ck)
			{
				mCheckBG = bg;
				mCheck = ck;
				Save();
			}
		}

		if (ShouldCreate(go, UISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Checkbox";

			UISlicedSprite bg = NGUITools.AddWidget<UISlicedSprite>(go);
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = UISettings.atlas;
			bg.spriteName = mCheckBG;
			bg.transform.localScale = new Vector3(26f, 26f, 1f);
			bg.MakePixelPerfect();

			UISprite fg = NGUITools.AddWidget<UISprite>(go);
			fg.name = "Checkmark";
			fg.atlas = UISettings.atlas;
			fg.spriteName = mCheck;
			fg.MakePixelPerfect();

			if (UISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = UISettings.font;
				lbl.text = go.name;
				lbl.pivot = UIWidget.Pivot.Left;
				lbl.transform.localPosition = new Vector3(16f, 0f, 0f);
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UICheckbox>().checkSprite = fg;
			go.AddComponent<UIButtonColor>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>().tweenTarget = bg.transform;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Progress bar creation function.
	/// </summary>

	void CreateSlider (GameObject go, bool slider)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string bg = UISpriteInspector.SpriteField(UISettings.atlas, "Background", mSliderBG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sprite for the background (empty bar)");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			string fg = UISpriteInspector.SpriteField(UISettings.atlas, "Foreground", mSliderFG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sprite for the foreground (full bar)");
			GUILayout.EndHorizontal();

			string tb = mSliderTB;

			if (slider)
			{
				GUILayout.BeginHorizontal();
				tb = UISpriteInspector.SpriteField(UISettings.atlas, "Thumb", mSliderTB, GUILayout.Width(200f));
				GUILayout.Space(20f);
				GUILayout.Label("Sprite for the thumb indicator");
				GUILayout.EndHorizontal();
			}

			if (mSliderBG != bg || mSliderFG != fg || mSliderTB != tb)
			{
				mSliderBG = bg;
				mSliderFG = fg;
				mSliderTB = tb;
				Save();
			}
		}

		if (ShouldCreate(go, UISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = slider ? "Slider" : "Progress Bar";

			// Background sprite
			UIAtlas.Sprite bgs = UISettings.atlas.GetSprite(mSliderBG);
			UISprite back = (bgs.inner == bgs.outer) ?
				(UISprite)NGUITools.AddWidget<UISprite>(go) :
				(UISprite)NGUITools.AddWidget<UISlicedSprite>(go);

			back.name = "Background";
			back.depth = depth;
			back.pivot = UIWidget.Pivot.Left;
			back.atlas = UISettings.atlas;
			back.spriteName = mSliderBG;
			back.transform.localScale = new Vector3(200f, 30f, 1f);
			back.MakePixelPerfect();

			// Fireground sprite
			UIAtlas.Sprite fgs = UISettings.atlas.GetSprite(mSliderFG);
			UISprite front = (fgs.inner == fgs.outer) ?
				(UISprite)NGUITools.AddWidget<UIFilledSprite>(go) :
				(UISprite)NGUITools.AddWidget<UISlicedSprite>(go);

			front.name = "Foreground";
			front.pivot = UIWidget.Pivot.Left;
			front.atlas = UISettings.atlas;
			front.spriteName = mSliderFG;
			front.transform.localScale = new Vector3(200f, 30f, 1f);
			front.MakePixelPerfect();

			// Add a collider
			if (slider) NGUITools.AddWidgetCollider(go);

			// Add the slider script
			UISlider uiSlider = go.AddComponent<UISlider>();
			uiSlider.foreground = front.transform;
			uiSlider.fullSize = front.transform.localScale;

			// Thumb sprite
			if (slider)
			{
				UIAtlas.Sprite tbs = UISettings.atlas.GetSprite(mSliderTB);
				UISprite thb = (tbs.inner == tbs.outer) ?
					(UISprite)NGUITools.AddWidget<UISprite>(go) :
					(UISprite)NGUITools.AddWidget<UISlicedSprite>(go);

				thb.name = "Thumb";
				thb.atlas = UISettings.atlas;
				thb.spriteName = mSliderTB;
				thb.transform.localPosition = new Vector3(200f, 0f, 0f);
				thb.transform.localScale = new Vector3(20f, 40f, 1f);
				thb.MakePixelPerfect();

				NGUITools.AddWidgetCollider(thb.gameObject);
				thb.gameObject.AddComponent<UIButtonColor>();
				thb.gameObject.AddComponent<UIButtonScale>();

				uiSlider.thumb = thb.transform;
			}
			uiSlider.rawValue = 0.75f;

			// Select the slider
			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Input field creation function.
	/// </summary>

	void CreateInput (GameObject go)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string bg = UISpriteInspector.SpriteField(UISettings.atlas, "Background", mInputBG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sliced Sprite for the background");
			GUILayout.EndHorizontal();
			if (mInputBG != bg) { mInputBG = bg; Save(); }
		}

		if (ShouldCreate(go, UISettings.atlas != null && UISettings.font != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Input";

			float padding = 3f;

			UISlicedSprite bg = NGUITools.AddWidget<UISlicedSprite>(go);
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = UISettings.atlas;
			bg.spriteName = mInputBG;
			bg.pivot = UIWidget.Pivot.Left;
			bg.transform.localScale = new Vector3(400f, UISettings.font.size + padding * 2f, 1f);
			bg.MakePixelPerfect();

			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = UISettings.font;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.transform.localPosition = new Vector3(padding, 0f, 0f);
			lbl.multiLine = false;
			lbl.supportEncoding = false;
			lbl.lineWidth = Mathf.RoundToInt(400f - padding * 2f);
			lbl.text = "You can type here";
			lbl.MakePixelPerfect();

			// Add a collider to the background
			NGUITools.AddWidgetCollider(bg.gameObject);

			// Add an input script to the background and have it point to the label
			UIInput input = bg.gameObject.AddComponent<UIInput>();
			input.label = lbl;

			// Update the selection
			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Create a popup list or a menu.
	/// </summary>

	void CreatePopup (GameObject go, bool isDropDown)
	{
		if (UISettings.atlas != null)
		{
			GUILayout.BeginHorizontal();
			string sprite = UISpriteInspector.SpriteField(UISettings.atlas, "Foreground", mListFG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Foreground sprite (shown on the button)");
			GUILayout.EndHorizontal();
			if (mListFG != sprite) { mListFG = sprite; Save(); }

			GUILayout.BeginHorizontal();
			sprite = UISpriteInspector.SpriteField(UISettings.atlas, "Background", mListBG, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Background sprite (envelops the options)");
			GUILayout.EndHorizontal();
			if (mListBG != sprite) { mListBG = sprite; Save(); }

			GUILayout.BeginHorizontal();
			sprite = UISpriteInspector.SpriteField(UISettings.atlas, "Highlight", mListHL, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Sprite used to highlight the selected option");
			GUILayout.EndHorizontal();
			if (mListHL != sprite) { mListHL = sprite; Save(); }
		}

		if (ShouldCreate(go, UISettings.atlas != null && UISettings.font != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = isDropDown ? "Popup List" : "Popup Menu";

			// Background sprite
			UISprite sprite = NGUITools.AddSprite(go, UISettings.atlas, mListFG);
			sprite.depth = depth;
			sprite.atlas = UISettings.atlas;
			sprite.transform.localScale = new Vector3(150f, 34f, 1f);
			sprite.pivot = UIWidget.Pivot.Left;
			sprite.MakePixelPerfect();

			UIAtlas.Sprite sp = UISettings.atlas.GetSprite(mListFG);
			float padding = Mathf.Max(4f, sp.inner.xMin - sp.outer.xMin);

			// Text label
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = UISettings.font;
			lbl.text = go.name;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.cachedTransform.localPosition = new Vector3(padding, 0f, 0f);
			lbl.MakePixelPerfect();

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the popup list
			UIPopupList list = go.AddComponent<UIPopupList>();
			list.atlas = UISettings.atlas;
			list.font = UISettings.font;
			list.backgroundSprite = mListBG;
			list.highlightSprite = mListHL;
			list.padding = new Vector2(padding, Mathf.RoundToInt(padding * 0.5f));
			if (isDropDown) list.textLabel = lbl;
			for (int i = 0; i < 5; ++i) list.items.Add(isDropDown ? ("List Option " + i) : ("Menu Option " + i));

			// Add the scripts
			go.AddComponent<UIButtonColor>().tweenTarget = sprite.gameObject;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	/// <summary>
	/// Repaint the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		// Load the saved preferences
		if (!mLoaded) { mLoaded = true; Load(); }

		EditorGUIUtility.LookLikeControls(80f);
		GameObject go = NGUIEditorTools.SelectedRoot();

		if (go == null)
		{
			GUILayout.Label("You must create a UI first.");
			
			if (GUILayout.Button("Open the New UI Wizard"))
			{
				EditorWindow.GetWindow<UICreateNewUIWizard>(false, "New UI", true);
			}
		}
		else
		{
			GUILayout.Space(4f);

			GUILayout.BeginHorizontal();
			ComponentSelector.Draw<UIAtlas>(UISettings.atlas, OnSelectAtlas, GUILayout.Width(140f));
			GUILayout.Label("Texture atlas used by widgets", GUILayout.MinWidth(10000f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			ComponentSelector.Draw<UIFont>(UISettings.font, OnSelectFont, GUILayout.Width(140f));
			GUILayout.Label("Font used by labels", GUILayout.MinWidth(10000f));
			GUILayout.EndHorizontal();

			GUILayout.Space(-2f);
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			WidgetType wt = (WidgetType)EditorGUILayout.EnumPopup("Template", mType, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Select a widget template to use");
			GUILayout.EndHorizontal();

			if (mType != wt) { mType = wt; Save(); }

			switch (mType)
			{
				case WidgetType.Label:			CreateLabel(go); break;
				case WidgetType.Sprite:			CreateSprite<UISprite>(go, ref mSprite); break;
				case WidgetType.SlicedSprite:	CreateSprite<UISlicedSprite>(go, ref mSliced); break;
				case WidgetType.TiledSprite:	CreateSprite<UITiledSprite>(go, ref mTiled); break;
				case WidgetType.FilledSprite:	CreateSprite<UIFilledSprite>(go, ref mFilled); break;
				case WidgetType.SimpleTexture:	CreateSimpleTexture(go); break;
				case WidgetType.Button:			CreateButton(go); break;
				case WidgetType.ImageButton:	CreateImageButton(go); break;
				case WidgetType.Checkbox:		CreateCheckbox(go); break;
				case WidgetType.ProgressBar:	CreateSlider(go, false); break;
				case WidgetType.Slider:			CreateSlider(go, true); break;
				case WidgetType.Input:			CreateInput(go); break;
				case WidgetType.PopupList:		CreatePopup(go, true); break;
				case WidgetType.PopupMenu:		CreatePopup(go, false); break;
			}
		}
	}
}