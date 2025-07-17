using GatorRando.Archipelago;
using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

public class SettingInput : MonoBehaviour
{
	private void OnValidate()
	{
        if (inputfield == null)
		{
			inputfield = gameObject.GetComponent<InputField>();
		}
	}

	private void OnEnable()
	{
        inputfield = gameObject.GetComponent<InputField>();
		if (saveAsLastConnection)
		{
			inputfield.text = SaveManager.DisplayLastConnectionData(key);
		}
		else
		{
			if (inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				inputfield.text = Settings.s.ReadInt(key).ToString();
			}
			else
			{
				inputfield.text = Settings.s.ReadString(key);
			}
		}
		setInitialSetting = true;
	}

	public void OnValueChanged(string value)
	{
        inputfield.textComponent.text = inputfield.text;
		if (!setInitialSetting)
		{
			return;
		}
		if (saveAsLastConnection)
		{
			SaveManager.UpdateLastConnectionData(key, inputfield.text);
		}
		else
		{
			if (inputfield.contentType == InputField.ContentType.IntegerNumber)
			{
				Settings.s.Write(key, int.Parse(inputfield.text));
			}
			else
			{
				Settings.s.Write(key, inputfield.text);
			}
		}
		Settings.s.LoadSettings();
	}

	public string key;

	public InputField inputfield;

	private bool setInitialSetting;

	public bool saveAsLastConnection;
}