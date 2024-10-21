using UnityEngine;

namespace UnityEditor.Rendering.Universal
{
    [FilePath("UserSettings/DanbaidongUserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class DanbaidongUserSettings : ScriptableSingleton<DanbaidongUserSettings>
    {
        [SerializeField]
        bool m_WizardPopupAlreadyShownOnce = false;
        [SerializeField]
        int m_WizardActiveTab = 0;
        [SerializeField]
        bool m_WizardNeedRestartAfterChangingToDX12 = false;
        [SerializeField]
        bool m_WizardNeedToRunFixAllAgainAfterDomainReload = false;
        [SerializeField]
        bool m_WizardIsStartPopup = true;

        public static int wizardActiveTab
        {
            get => instance.m_WizardActiveTab;
            set
            {
                instance.m_WizardActiveTab = value;
                instance.Save();
            }
        }

        public static bool wizardPopupAlreadyShownOnce
        {
            get => instance.m_WizardPopupAlreadyShownOnce;
            set
            {
                instance.m_WizardPopupAlreadyShownOnce = value;
                instance.Save();
            }
        }

        public static bool wizardNeedToRunFixAllAgainAfterDomainReload
        {
            get => instance.m_WizardNeedToRunFixAllAgainAfterDomainReload;
            set
            {
                instance.m_WizardNeedToRunFixAllAgainAfterDomainReload = value;
                instance.Save();
            }
        }

        public static bool wizardNeedRestartAfterChangingToDX12
        {
            get => instance.m_WizardNeedRestartAfterChangingToDX12;
            set
            {
                instance.m_WizardNeedRestartAfterChangingToDX12 = value;
                instance.Save();
            }
        }

        public static bool wizardIsStartPopup
        {
            get => instance.m_WizardIsStartPopup;
            set
            {
                instance.m_WizardIsStartPopup = value;
                instance.Save();
            }
        }

        void Save()
            => Save(true);
    }
}
