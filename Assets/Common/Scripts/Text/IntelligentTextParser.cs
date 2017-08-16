using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using Common.Collections;

namespace Common.Text
{
    /// <summary>
    /// A collection of functionality for controlling the process of converting Intelligent Text to a Mesh.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public struct IntelligentTextParser : IDisposable
    {
        /// <summary>
        /// Name of the root xml element.
        /// </summary>
        private static readonly string ROOT_TAG = "root";

        /// <summary>
        /// The Intelligent Text insert tag for making data insertion early in the parsing process.
        /// </summary>
        public static readonly string INSERT_TAG = "insert";
        /// <summary>
        /// Tag of elements that hold data from insertion.
        /// </summary>
        public static readonly string INSERT_DONE_TAG = "_insert";
        /// <summary>
        /// The Intelligent Text group tag for grouping multiple Intelligent Text elements.
        /// </summary>
        public static readonly string GROUP_TAG = "group";
        /// <summary>
        /// The Intelligent Text image tag for inserting an image in the text.
        /// </summary>
        public static readonly string IMAGE_TAG = "image";

        /// <summary>
        /// The Intelligent Text element interactor identifier attribute.
        /// </summary>
        public static readonly string INTERACTOR_ID_ATTRIBUTE = "interactorId";
        /// <summary>
        /// The Intelligent Text element identifier attribute.
        /// </summary>
        public static readonly string ID_ATTRIBUTE = "id";
        /// <summary>
        /// The Intelligent Text image transform identifier attribute.
        /// </summary>
        public static readonly string TRANSFORM_ID_ATTRIBUTE = "transformId";
        /// <summary>
        /// The replacement space placeholder string.
        /// </summary>
        public static readonly string SPACE_PLACEHOLDER_STR = "|";

        /// <summary>
        /// The list of all Intelligent Text data nodes.
        /// </summary>
        private List<IntelligentTextDataNode> m_DataList;
        /// <summary>
        /// The text generator.
        /// </summary>
        private TextGenerator m_TextGenerator;
        /// <summary>
        /// The replacement space placeholder size per unit from the measuring test.
        /// </summary>
        private Vector2 m_SpacePlaceholderSizePerUnit;
        /// <summary>
        /// The replacement space placeholder size from the generated text.
        /// </summary>
        private Vector2 m_SpacePlaceholderSize;
        /// <summary>
        /// The Intelligent Text mesh.
        /// </summary>
        private Mesh m_Mesh;
        /// <summary>
        /// The Intelligent Text mesh materials.
        /// </summary>
        private List<Material> m_Materials;
        
        /// <summary>
        /// The text settings for text generator.
        /// </summary>
        public TextGenerationSettings TextSettings;
        /// <summary>
        /// The text generation extents rectangle.
        /// </summary>
        public Rect Rectangle;

        /// <summary>
        /// Gets the replacement space placeholder size per unit.
        /// </summary>
        /// <value>
        /// The space placeholder size per unit.
        /// </value>
        public Vector2 SpacePlaceholderSizePerUnit { get { return m_SpacePlaceholderSizePerUnit; } }
        /// <summary>
        /// Gets the estimated size of the replacement space placeholder.
        /// </summary>
        /// <value>
        /// The estimated size of the space placeholder.
        /// </value>
        public Vector2 SpacePlaceholderEstimatedSize { get { return m_SpacePlaceholderSizePerUnit * TextSettings.fontSize; } }
        /// <summary>
        /// Gets the current size of the replacement space placeholder.
        /// </summary>
        /// <value>
        /// The size of the space placeholder.
        /// </value>
        public Vector2 SpacePlaceholderSize { get { return m_SpacePlaceholderSize; } }
        /// <summary>
        /// Gets the root of Intelligent Text data nodes.
        /// </summary>
        /// <value>
        /// The Intelligent Text data node root.
        /// </value>
        public IntelligentTextDataNode DataRoot { get { return m_DataList[0]; } }
        /// <summary>
        /// Gets the mesh.
        /// </summary>
        /// <value>
        /// The mesh.
        /// </value>
        public Mesh Mesh { get { return m_Mesh; } }
        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value>
        /// The materials.
        /// </value>
        public List<Material> Materials { get { return m_Materials; } }


        /// <summary>
        /// Replaces the Intelligent Text insersts.
        /// </summary>
        /// <param name="i_Document">The text xml document.</param>
        private void ReplaceInsersts(XmlDocument i_Document)
        {
            const int maxRecursionCount = 5;
            int recursionCount = 0;
            XmlNodeList elementList = null;
            do
            {
                ++recursionCount;
                elementList = i_Document.GetElementsByTagName(INSERT_TAG);

                int size = elementList.Count;
                XmlNode element;
                for(int i = 0; i < size; ++i)
                {
                    element = elementList[i];
                    var idAttribute = element.Attributes["id"];
                    if (idAttribute != null)
                    {
                        var id = idAttribute.Value;
                        if (!string.IsNullOrEmpty(id))
                        {
                            var newElement = i_Document.CreateElement(INSERT_DONE_TAG);
                            string insertValue = IntelligentTextSettings.Instance.GetInsert(id);
                            newElement.InnerXml = insertValue;
                            element.ParentNode.ReplaceChild(newElement, element);
                        }
                    }
                }
            }
            while (elementList.Count > 0 && recursionCount < maxRecursionCount);
        }

        /// <summary>
        /// Builds the Intelligent Text data nodes.
        /// </summary>
        /// <param name="i_XmlContainer">The i XML container.</param>
        /// <param name="i_Parent">The i parent.</param>
        public void BuildTextData(XmlNode i_XmlContainer, IntelligentTextDataNode i_Parent)
        {
            foreach (XmlNode node in i_XmlContainer)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Text:
                        {
                            IntelligentTextDataNode data = new IntelligentTextDataTextNode(
                                m_DataList.Count,
                                null,
                                node.InnerText
                            );

                            int lastSiblingIndex = i_Parent.Children.Count - 1;
                            //attempt to merge sibling nodes
                            if (lastSiblingIndex < 0 || !i_Parent.Children[lastSiblingIndex].Merge(data))
                            {
                                //track a new node if merging failed
                                i_Parent.Children.Add(data);
                                m_DataList.Add(data);
                            }
                        }
                        break;
                    case XmlNodeType.Element:
                        if(node.Name == INSERT_DONE_TAG)
                        {
                            BuildTextData(node, i_Parent);
                        }
                        else if (node.Name == GROUP_TAG)
                        {
                            var interactorContainer = node.Attributes[INTERACTOR_ID_ATTRIBUTE];
                            string interactorValue = interactorContainer != null ? interactorContainer.Value : null;
                            IntelligentTextDataNode data = new IntelligentTextDataGroupNode(
                                    m_DataList.Count,
                                    interactorValue
                                );
                            i_Parent.Children.Add(data);
                            m_DataList.Add(data);
                            BuildTextData(node, data);
                        }
                        else if (node.Name == IMAGE_TAG)
                        {
                            var idContainer = node.Attributes[ID_ATTRIBUTE];
                            var transformContainer = node.Attributes[TRANSFORM_ID_ATTRIBUTE];
                            if (idContainer != null && transformContainer != null)
                            {
                                IntelligentTextImage image = IntelligentTextSettings.Instance.GetImage(idContainer.Value);
                                IntelligentTextTransform transform = IntelligentTextSettings.Instance.GetTransform(transformContainer.Value);
                                if (image.Material != null && transform != null)
                                {
                                    var interactorContainer = node.Attributes[INTERACTOR_ID_ATTRIBUTE];
                                    string interactorValue = interactorContainer != null ? interactorContainer.Value : null;
                                    IntelligentTextDataImageNode data = new IntelligentTextDataImageNode(
                                            m_DataList.Count,
                                            interactorValue,
                                            image,
                                            transform
                                        );
                                    
                                    i_Parent.Children.Add(data);
                                    m_DataList.Add(data);
                                }
                            }
                        }
                    break;
                }
            }
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        private void Setup()
        {
            if (m_TextGenerator == null)
            {
                m_DataList = new List<IntelligentTextDataNode>();
                m_TextGenerator = new TextGenerator();
            }

            TextGenerationSettings tempTextSettings = new TextGenerationSettings()
            {
                color = Color.black,
                font = TextSettings.font,
                lineSpacing = 1,
                alignByGeometry = false,
                fontStyle = FontStyle.Normal,
                generateOutOfBounds = true,
                generationExtents = new Vector2(100, 100),
                horizontalOverflow = HorizontalWrapMode.Overflow,
                pivot = new Vector2(0.5f, 0.5f),
                resizeTextForBestFit = false,
                richText = false,
                scaleFactor = 1,
                textAnchor = TextAnchor.MiddleCenter,
                updateBounds = true,
                verticalOverflow = VerticalWrapMode.Overflow
            };
            //update the spacing placeholder width for selected font
            m_TextGenerator.Populate(SPACE_PLACEHOLDER_STR, tempTextSettings);

            var tempVerts = m_TextGenerator.verts;
            var placeholderSize = tempVerts[1].position - tempVerts[3].position;
            m_SpacePlaceholderSizePerUnit = new Vector2(placeholderSize.x, placeholderSize.y) / TextSettings.font.fontSize;
        }

        /// <summary>
        /// Rebuilds the INtelligent Text mesh.
        /// </summary>
        public void RebuildMesh()
        {
            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
                m_Mesh.name = "Text Mesh";
                m_Mesh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy | HideFlags.NotEditable;
            }
            m_Mesh.Clear(false);

            //build up the final text
            StringBuilder textAccumulator = new StringBuilder();
            int textDataSize = m_DataList.Count;
            for (int i = 0; i < textDataSize; ++i)
            {
                m_DataList[i].BuildText(textAccumulator, ref this);
            }

            //append the space placeholder to find out its size in current text
            textAccumulator.Append(SPACE_PLACEHOLDER_STR);
            string finalText = textAccumulator.ToString();
            m_TextGenerator.Populate(finalText, TextSettings);

            //create line data for calculating mesh adjustments for added elements like images
            var generatedLines = m_TextGenerator.lines;
            int lineCount = generatedLines.Count;
            IList<UIVertex> generatorVerts = m_TextGenerator.verts;
            int vertCount = generatorVerts.Count;
            int trimmedVertCount = vertCount - (lineCount * 4);
            IntelligentTextMeshData initialMeshData = new IntelligentTextMeshData
            {
                Order = 0,
                TextLength = finalText.Length,
                Lines = new List<IntelligentTextLineInfo>(lineCount),
                Verts = new List<Vector3>(trimmedVertCount),
                Colors = new List<Color32>(trimmedVertCount),
                Uvs = new List<Vector2>(trimmedVertCount),
                SubMeshes = new List<IntelligentTextSubMeshData>(),
                ExtentRect = Rectangle
            };

            //track characters to skip to avoid the generated empty characters
            var charactersToSkip = new int[lineCount];
            for (int i = 1; i < lineCount; ++i)
            {
                charactersToSkip[i - 1] = generatedLines[i].startCharIdx - 1;
            }
            //don't skip any character when you reach this point
            charactersToSkip[lineCount - 1] = -1;

            for (int i = 0; i < lineCount; ++i)
            {
                var line = new IntelligentTextLineInfo() {
                    Height = generatedLines[i].height,
                    StartCharIndex = generatedLines[i].startCharIdx
                };
                initialMeshData.Lines.Add(line);
            }

            int skipCharacterArrayIndex = 0;
            int skipCharacterIndex = charactersToSkip[skipCharacterArrayIndex++];
            //removing last 2 characters because of empty space and placeholder;
            int vertCountWithoutEnding = vertCount - (4 * 2);
            for (int i = 0; i < vertCountWithoutEnding; ++i)
            {
                if (i != skipCharacterIndex)
                {
                    initialMeshData.Verts.Add(generatorVerts[i].position);
                    initialMeshData.Colors.Add(generatorVerts[i].color);
                    initialMeshData.Uvs.Add(generatorVerts[i].uv0);
                }
                else
                {
                    skipCharacterIndex = charactersToSkip[skipCharacterArrayIndex++];
                }
            }
            List<IntelligentTextMeshData> meshDataList = new List<IntelligentTextMeshData>() { initialMeshData };


            //TODO: avoid using another property like "characters"

            //calculate the space placeholder size in current text
            var generatedChars = m_TextGenerator.characters;
            float generatedSpacePlaceholderWidth = 0;
            for (int i = SPACE_PLACEHOLDER_STR.Length; i > 0; --i)
            {
                var generatedSpacePlaceholderChar = generatedChars[finalText.Length - i];
                generatedSpacePlaceholderWidth += generatedSpacePlaceholderChar.charWidth;
            }
            m_SpacePlaceholderSize = m_SpacePlaceholderSizePerUnit * (generatedSpacePlaceholderWidth / m_SpacePlaceholderSizePerUnit.x);



            //building final mesh data, applying mesh adjustments from Intelligent Text data nodes
            int currentCharIndex = 0;
            for (int i = 0; i < textDataSize; ++i)
            {
                currentCharIndex = m_DataList[i].BuildSubMesh(currentCharIndex, meshDataList, ref this);
            }
            //sorting the rendered elements if they require to appear in a specific order
            meshDataList.InsertionSort(IntelligentTextMeshData.Sorter.Ascending);

            //combining seperate mesh data
            var combinedData = meshDataList[0];
            for (int i = 1; i < meshDataList.Count; ++i)
            {
                var tempMeshData = meshDataList[i];

                //adjust the indices to work with the merged vertex data
                int combinedDataVertCount = combinedData.Verts.Count;
                int subMeshCount = tempMeshData.SubMeshes.Count;
                for(int j = 0; j < subMeshCount; ++j)
                {
                    var subMesh = tempMeshData.SubMeshes[i];
                    int indicesCount = subMesh.Trinagles.Count;
                    for (int k = 0; k < indicesCount; ++k)
                    {
                        subMesh.Trinagles[k] += combinedDataVertCount;
                    }
                }

                combinedData.Verts.AddRange(tempMeshData.Verts);
                combinedData.Colors.AddRange(tempMeshData.Colors);
                combinedData.Uvs.AddRange(tempMeshData.Uvs);
                combinedData.SubMeshes.AddRange(tempMeshData.SubMeshes);
            }

            m_Mesh.SetVertices(combinedData.Verts);
            m_Mesh.SetColors(combinedData.Colors);
            m_Mesh.SetUVs(0, combinedData.Uvs);

            int combinedSubMeshCount = combinedData.SubMeshes.Count;
            m_Mesh.subMeshCount = combinedSubMeshCount;
            if(m_Materials == null)
            {
                m_Materials = new List<Material>(combinedSubMeshCount);
            }
            else
            {
                m_Materials.Clear();
            }
            for(int i = 0; i < combinedSubMeshCount; ++i)
            {
                m_Mesh.SetTriangles(combinedData.SubMeshes[i].Trinagles, i);
                m_Materials.Add(combinedData.SubMeshes[i].Material);
            }

            //m_Mesh.RecalculateBounds();
            //TODO: update data bounds after inserted image adjustment
        }

        /// <summary>
        /// Parses the specified Intelligent Text string.
        /// </summary>
        /// <param name="i_Text">The Intelligent Text string.</param>
        public void Parse(string i_Text)
        {
            Setup();
            m_DataList.Clear();
            var document = new XmlDocument();
            var dataRoot = new IntelligentTextDataNode(0);
            m_DataList.Add(dataRoot);

            XmlElement xmlRoot = document.CreateElement(ROOT_TAG);
            document.AppendChild(xmlRoot);
            xmlRoot.InnerXml = i_Text;

            ReplaceInsersts(document);
            BuildTextData(xmlRoot, dataRoot);

            RebuildMesh();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (m_Mesh != null)
            {
                m_Mesh.Clear();
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(m_Mesh);
#else
                UnityEngine.Object.Destroy(m_Mesh);
#endif
            }
            m_Mesh = null;
            using (m_TextGenerator)
            { }
        }
    }
}
