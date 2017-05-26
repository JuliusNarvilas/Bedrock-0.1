using Common.IO.Recources;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common.Text
{
    public class IntelligentTextSettings : ScriptableObject
    {
        public static readonly string LOCALIZATION_ID_GLOBAL = "global";
        public static readonly string LOCALIZATION_SAVE_ID = "currentLocalization";
        public delegate string CustomInsertProvider();
        private static IntelligentTextSettings s_Instance = null;

        public static IntelligentTextSettings FindInstance()
        {
            return Resources.Load<IntelligentTextSettings>("IntelligentTextSettings");
        }

#if UNITY_EDITOR
        public static void CreateInstance()
        {
            var resDir = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources"));
            if (!resDir.Exists)
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
            }
            UnityEditor.AssetDatabase.CreateAsset(s_Instance, "Assets/Resources/IntelligentTextSettings.asset");
        }
#endif

        public static IntelligentTextSettings Instance
        {
            get
            {
                if (s_Instance != null)
                {
                    return s_Instance;
                }
                s_Instance = FindInstance();
                if (s_Instance != null)
                {
                    s_Instance.Initialise();
                    return s_Instance;
                }

                s_Instance = CreateInstance<IntelligentTextSettings>();
#if UNITY_EDITOR
                CreateInstance();
                s_Instance = FindInstance();
#endif
                return s_Instance;
            }
        }

        [SerializeField]
        private TextAsset m_Localisations = null;
        
        private Dictionary<string, IntelligentTextLocalizationRecord> m_LocalizationsRecords = new Dictionary<string, IntelligentTextLocalizationRecord>();
        private Dictionary<int, IntelligentText> m_ActiveTextRegistry = new Dictionary<int, IntelligentText>();
        private Dictionary<string, CustomInsertProvider> m_CustomInserts = new Dictionary<string, CustomInsertProvider>();

        private IntelligentTextLocalizationRecord m_CurrentLocalizationRecord;
        private IntelligentTextLocalization m_CurrentLocalization = new IntelligentTextLocalization();


        public string CurrentLocalizationId
        {
            get { return m_CurrentLocalizationRecord.id; }
        }
        public string CurrentLocalizationDisplayName
        {
            get { return m_CurrentLocalizationRecord.displayName; }
        }
        public IEnumerable<IntelligentTextLocalizationRecord> LocalizationsRecords
        {
            get { return m_LocalizationsRecords.Values; }
        }

        public void SetCustomInsert(string i_Id, CustomInsertProvider i_Response)
        {
            m_CustomInserts[i_Id] = i_Response;
        }
        public bool RemoveCustomInsert(string i_Id)
        {
            return m_CustomInserts.Remove(i_Id);
        }


        public void RegisterText(IntelligentText i_Text)
        {
            m_ActiveTextRegistry[i_Text.GetInstanceID()] = i_Text;
            i_Text.Refresh();
        }
        public void UnregisterText(IntelligentText i_Text)
        {
            m_ActiveTextRegistry.Remove(i_Text.GetInstanceID());
        }

        public string GetInsert(string i_Id)
        {
            CustomInsertProvider customInsertFunc;
            if(m_CustomInserts.TryGetValue(i_Id, out customInsertFunc))
            {
                return customInsertFunc();
            }
            string result;
            if(m_CurrentLocalization.Inserts.TryGetValue(i_Id, out result))
            {
                return result;
            }
            Log.DebugLogError("IntelligentText Insert not found with id: {0}", i_Id);
            return string.Format("[{0}]", i_Id);
        }

        public IntelligentTextStyle GetStyle(string i_Id)
        {
            IntelligentTextStyle result;
            if (m_CurrentLocalization.Styles.TryGetValue(i_Id, out result))
            {
                return result;
            }
            Log.DebugLogError("IntelligentText Style not found with id: {0}", i_Id);
            return null;
        }

        public IntelligentTextImage GetImage(string i_Id)
        {
            IntelligentTextImage result;
            if (m_CurrentLocalization.Images.TryGetValue(i_Id, out result))
            {
                return result;
            }
            Log.DebugLogError("IntelligentText Image not found with id: {0}", i_Id);
            return new IntelligentTextImage();
        }

        public IntelligentTextTransform GetTransform(string i_Id)
        {
            IntelligentTextTransform result;
            if (m_CurrentLocalization.Transforms.TryGetValue(i_Id, out result))
            {
                return result;
            }
            Log.DebugLogError("IntelligentText Transform not found with id: {0}", i_Id);
            return null;
        }

        public string Localize(string i_Text)
        {
            return m_CurrentLocalization.Localize(i_Text);
        }

        public void RefreshText()
        {
            //regenerate text
            foreach(var intelligentText in m_ActiveTextRegistry.Values)
            {
                intelligentText.Refresh();
            }
        }

        public void Reload()
        {
            m_CurrentLocalization.Clear();
            if(string.IsNullOrEmpty(m_CurrentLocalizationRecord.id))
            {
                Initialise();
                return;
            }

            IntelligentTextLocalizationRecord record;
            if (m_LocalizationsRecords.TryGetValue(LOCALIZATION_ID_GLOBAL, out record))
            {
                var globalResource = ResourcesDB.GetByPath(record.path);
                if (globalResource != null)
                {
                    var currentGlobalSource = globalResource.Load<TextAsset>();
                    var globals = JsonUtility.FromJson<IntelligentTextLocalizationData>(currentGlobalSource.text);
                    m_CurrentLocalization.Append(globals);
                    globalResource.Unload();
                }
            }
            var resource = ResourcesDB.GetByPath(m_CurrentLocalizationRecord.path);
            var currentLocalizationSource = resource.Load<TextAsset>();
            var localization = JsonUtility.FromJson<IntelligentTextLocalizationData>(currentLocalizationSource.text);
            m_CurrentLocalization.Append(localization);
            resource.Unload();
        }

        public bool SetLocalization(string i_LocalizationId, bool i_Force = false)
        {
            if(m_CurrentLocalizationRecord.id != i_LocalizationId || i_Force)
            {
                IntelligentTextLocalizationRecord record;
                if (m_LocalizationsRecords.TryGetValue(i_LocalizationId, out record))
                {
                    if (!string.IsNullOrEmpty(record.id))
                    {
                        m_CurrentLocalizationRecord = record;
                        return true;
                    }
                }
            }
            return false;
        }

        private void Initialise()
        {
            var localizationsContainer = JsonUtility.FromJson<IntelligentTextLocalizationsContainer>(m_Localisations.text);
            foreach(var localizationRecord in localizationsContainer.localizationList)
            {
                m_LocalizationsRecords[localizationRecord.id] = localizationRecord;
            }
            Log.DebugLogWarningIf(m_LocalizationsRecords.Count > 0, "No LocalizationsRecords");

            string savedLocalization = PlayerPrefs.GetString(LOCALIZATION_SAVE_ID, string.Empty);
            if(string.IsNullOrEmpty(savedLocalization))
            {
                savedLocalization = System.Globalization.CultureInfo.CurrentCulture.Name;
            }
            if(!SetLocalization(savedLocalization, true))
            {//fallback
                if(savedLocalization.Length > 2)
                {
                    var culture = new System.Globalization.CultureInfo(savedLocalization);
                    savedLocalization = culture.TwoLetterISOLanguageName;
                    SetLocalization(savedLocalization, true);
                }
                if (string.IsNullOrEmpty(m_CurrentLocalizationRecord.id))
                {
                    //pick first option
                    SetLocalization(m_LocalizationsRecords.Values.GetEnumerator().Current.id, true);
                }
            }

            if (!string.IsNullOrEmpty(m_CurrentLocalizationRecord.id))
            {
                Reload();
                RefreshText();
            }
        }
    }
}
