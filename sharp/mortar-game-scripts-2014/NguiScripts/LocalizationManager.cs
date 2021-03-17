using UnityEngine;
using System.Collections;

public class LocalizationManager : RequiredMonoSingleton<LocalizationManager>
{
    protected override void Awake ()
    {
        base.Awake();
	    Localization.language = _language;
	}

    [SerializeField]
    [ContextMenuItem("Change Language", "ChangeLanguage")]
    private string _language;  
    
    private void ChangeLanguage()
    {
        if (Localization.knownLanguages[0] != Localization.language)
        {
            Localization.language = Localization.knownLanguages[0];
        }
        else
        {
            Localization.language = Localization.knownLanguages[1];
        }
        _language = Localization.language;
    }
}
