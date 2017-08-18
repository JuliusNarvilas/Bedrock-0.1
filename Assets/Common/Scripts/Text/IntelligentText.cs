using UnityEngine;

namespace Common.Text
{
    /// <summary>
    /// A behaviour for introducing text with more features into a scene.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class IntelligentText : MonoBehaviour
    {
        /// <summary>
        /// The text type mode this bahaviour is using.
        /// </summary>
        protected enum ERenderMode
        {
            ConvasRenderer,
            MeshRenderer,
            Unknown,
            Invalid
        }

        /// <summary>
        /// The text to be put through intelligent text systen.
        /// </summary>
        [SerializeField]
        [TextArea]
        protected string m_Text;
        /// <summary>
        /// The font style identifier.
        /// </summary>
        [SerializeField]
        protected string m_StyleId;
        [SerializeField]
        protected bool m_GenerateOutOfBounds = false;
        [SerializeField]
        protected HorizontalWrapMode m_HorizontalOverflow = HorizontalWrapMode.Wrap;
        [SerializeField]
        protected bool m_BestFit = false;
        [SerializeField]
        protected bool m_RichText = false;
        [SerializeField]
        protected float m_ScaleFactor = 1;
        [SerializeField]
        protected TextAnchor m_Anchor = TextAnchor.MiddleCenter;


        public string Style
        {
            get { return m_StyleId; }
            set
            {
                m_StyleId = value;
                Refresh();
            }
        }

        
        protected ERenderMode m_RenderMode = ERenderMode.Unknown;
        protected IntelligentTextParser m_Parser = new IntelligentTextParser();

        public string Text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                RebuildText();
            }
        }

        /// <summary>
        /// Initialises the render mode.
        /// </summary>
        private void InitialiseRenderMode()
        {
            m_RenderMode = ERenderMode.Unknown;
            if (GetComponent<CanvasRenderer>() != null)
            {
                m_RenderMode = ERenderMode.ConvasRenderer;
            }
            else
            {
                if (GetComponent<MeshRenderer>() != null)
                {
                    m_RenderMode = ERenderMode.MeshRenderer;
                    var meshFilter = GetComponent<MeshFilter>();
                    if (meshFilter == null)
                    {
                        meshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                    meshFilter.sharedMesh = null;
                    meshFilter.hideFlags = HideFlags.None;
                }
            }

            switch (m_RenderMode)
            {
                case ERenderMode.ConvasRenderer:
                case ERenderMode.MeshRenderer:
                    break;
                case ERenderMode.Unknown:
                    m_RenderMode = ERenderMode.ConvasRenderer;
                    gameObject.AddComponent<CanvasRenderer>();
                    break;
            }
        }

        /// <summary>
        /// Rebuilds the text by parsing the initial intelligent text.
        /// </summary>
        public void RebuildText()
        {
            if (m_RenderMode == ERenderMode.Unknown || m_RenderMode == ERenderMode.Invalid)
            {
                InitialiseRenderMode();
            }

            m_Parser.Parse(m_Text);
            Display();
        }

        /// <summary>
        /// Updates the text by rebuilding the mesh.
        /// </summary>
        public void UpdateText()
        {
            if(m_RenderMode == ERenderMode.Unknown || m_RenderMode == ERenderMode.Invalid)
            {
                InitialiseRenderMode();
            }

            m_Parser.RebuildMesh();
            Display();
        }

        /// <summary>
        /// Displays text.
        /// </summary>
        private void Display()
        {
            switch (m_RenderMode)
            {
                case ERenderMode.ConvasRenderer:
                    {
                        var materials = m_Parser.Materials;
                        var canvasRenderer = GetComponent<CanvasRenderer>();
                        canvasRenderer.Clear();
                        canvasRenderer.SetMesh(m_Parser.Mesh);
                        canvasRenderer.materialCount = materials.Count;
                        for (int i = 0; i < materials.Count; ++i)
                        {
                            canvasRenderer.SetMaterial(materials[i], i);
                        }
                    }
                    break;
                case ERenderMode.MeshRenderer:
                    {
                        var meshFilter = GetComponent<MeshFilter>();
                        var meshRenderer = GetComponent<MeshRenderer>();
                        meshFilter.sharedMesh = m_Parser.Mesh;
                        meshRenderer.sharedMaterials = m_Parser.Materials.ToArray();
                    }
                    break;
            }
        }

        /// <summary>
        /// Refreshes text by reapplying the parser settings and rebuilds the text.
        /// </summary>
        /// <seealso cref="RebuildText"/>
        public void Refresh()
        {
            IntelligentTextStyle style = IntelligentTextSettings.Instance.GetStyle(m_StyleId);
            if (style != null)
            {
                var transform = GetComponent<RectTransform>();

                m_Parser.TextSettings.color = style.Color;
                m_Parser.TextSettings.font = style.Font;
                m_Parser.TextSettings.fontSize = style.FontSize;
                m_Parser.TextSettings.lineSpacing = style.LineSpacing;

                m_Parser.TextSettings.alignByGeometry = false;
                m_Parser.TextSettings.fontStyle = FontStyle.Normal;
                m_Parser.TextSettings.generateOutOfBounds = m_GenerateOutOfBounds;
                m_Parser.TextSettings.generationExtents = transform.rect.size;
                m_Parser.TextSettings.horizontalOverflow = m_HorizontalOverflow;
                m_Parser.TextSettings.pivot = new Vector2(0.5f, 0.5f);
                m_Parser.TextSettings.resizeTextForBestFit = m_BestFit;
                m_Parser.TextSettings.resizeTextMaxSize = 600;
                m_Parser.TextSettings.resizeTextMinSize = 6;
                m_Parser.TextSettings.richText = m_RichText;
                m_Parser.TextSettings.scaleFactor = m_ScaleFactor;
                m_Parser.TextSettings.textAnchor = m_Anchor;
                m_Parser.TextSettings.updateBounds = true;
                m_Parser.TextSettings.verticalOverflow = VerticalWrapMode.Overflow;

                m_Parser.Rectangle = transform.rect;

                RebuildText();
            }
            else
            {
                Log.ProductionLogError("IntelligentText Style not found for id: {0}", m_StyleId);
            }

        }

#if UNITY_EDITOR
        private bool m_ForceRefresh;

        private void OnValidate()
        {
            //defer text regeneration after the script started up, 
            //as OnValidate can trigger too early in the bootup
            m_ForceRefresh = true;
        }


        private void Update()
        {
            if(m_ForceRefresh)
            {
                m_ForceRefresh = false;
                m_RenderMode = ERenderMode.Unknown;
                Refresh();
            }
        }
#endif

        private void OnEnable()
        {
#if UNITY_EDITOR
            //prevent two text regenerations during the bootup
            //as IntelligentTextSettings.Instance.RegisterText calls Refresh
            m_ForceRefresh = false;
#endif
            IntelligentTextSettings.Instance.RegisterText(this);
        }
        
        private void OnDisable()
        {
            IntelligentTextSettings.Instance.UnregisterText(this);
        }



        private void OnDistroy()
        {
            m_Parser.Dispose();
        }

        void OnApplicationQuit()
        {
            IntelligentTextSettings.Dispose();
        }
    }
}
