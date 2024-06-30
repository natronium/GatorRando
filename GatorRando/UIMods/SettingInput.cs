using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

public class SettingInput : MonoBehaviour
{
	// Token: 0x06000E2B RID: 3627 RVA: 0x0004441F File Offset: 0x0004261F
	private void OnValidate()
	{
        if (this.inputfield == null)
		{
			this.inputfield = this.gameObject.GetComponent<InputField>();
		}
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0004443B File Offset: 0x0004263B
	private void OnEnable()
	{
        this.inputfield = this.gameObject.GetComponent<InputField>();
        if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
        {
            this.inputfield.text = Settings.s.ReadInt(this.key).ToString();
        }
        else
        {
            this.inputfield.text = Settings.s.ReadString(this.key);
        }
		this.setInitialSetting = true;
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x0004446A File Offset: 0x0004266A
	public void OnValueChanged(string value)
	{
        this.inputfield.textComponent.text = this.inputfield.text;
		if (!this.setInitialSetting)
		{
			return;
		}
        if (this.inputfield.contentType == InputField.ContentType.IntegerNumber)
        {
            Settings.s.Write(this.key, int.Parse(this.inputfield.text));
        }
        else
        {
            Settings.s.Write(this.key, this.inputfield.text);
        }
		Settings.s.LoadSettings();
	}

	// Token: 0x04001292 RID: 4754
	public string key;

	// Token: 0x04001293 RID: 4755
	public InputField inputfield;

	// Token: 0x04001294 RID: 4756
	private bool setInitialSetting;
}