using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

public class SettingInput : MonoBehaviour
{
	private void OnValidate()
	{
        if (this.inputfield == null)
		{
			this.inputfield = this.gameObject.GetComponent<InputField>();
		}
	}

	private void OnEnable()
	{
        this.inputfield = this.gameObject.GetComponent<InputField>();
		if (saveToGameData)
		{
			if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				this.inputfield.text = GameData.g.ReadInt(this.key).ToString();
			}
			else
			{
				string currentPrefixAndValue = Util.FindIntKeyByPrefix(this.key);
				if (currentPrefixAndValue != "")
				{
					this.inputfield.text = currentPrefixAndValue.Remove(0,this.key.Length);
				}
				else
				{
					this.inputfield.text = "";
				}
			}	
		}
		else
		{
			if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				this.inputfield.text = Settings.s.ReadInt(this.key).ToString();
			}
			else
			{
				this.inputfield.text = Settings.s.ReadString(this.key);
			}
		}
		this.setInitialSetting = true;
	}

	public void OnValueChanged(string value)
	{
        this.inputfield.textComponent.text = this.inputfield.text;
		if (!this.setInitialSetting)
		{
			return;
		}
		if (saveToGameData)
		{
			if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				GameData.g.Write(this.key, int.Parse(this.inputfield.text));
			}
			else
			{
				Util.RemoveIntKeysByPrefix(this.key);
				GameData.g.Write(this.key + this.inputfield.text, 0);
			}
		}
		else
		{
			if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				Settings.s.Write(this.key, int.Parse(this.inputfield.text));
			}
			else
			{
				Settings.s.Write(this.key, this.inputfield.text);
			}
		}
		Settings.s.LoadSettings();
	}

	public string key;

	public InputField inputfield;

	private bool setInitialSetting;

	public bool saveToGameData;
}