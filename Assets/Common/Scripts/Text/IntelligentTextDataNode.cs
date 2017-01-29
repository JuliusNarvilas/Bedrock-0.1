using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common.Text
{
    public enum IntelligentTextDataType
    {
        None,
        Group,
        Image,
        Text
    }

    /// <summary>
    /// An interface to adjust final generated IntelligentText mesh.
    /// </summary>
    public interface IIntelligentTextMeshModifier
    {
        void ChangeMesh();
    }

    public class IntelligentTextDataNode
    {
        public readonly List<Bounds> Bounds;
        public readonly int Id;
        public readonly IntelligentTextDataType Type;
        public readonly string InteractorId;
        public readonly List<IntelligentTextDataNode> Children;
        public readonly List<IIntelligentTextMeshModifier> MeshModifier;

        public IntelligentTextDataNode(int i_Id)
        {
            Bounds = new List<Bounds>();
            Id = i_Id;
            Type = IntelligentTextDataType.None;
            InteractorId = null;
            Children = new List<IntelligentTextDataNode>();
            MeshModifier = new List<IIntelligentTextMeshModifier>();
        }

        protected IntelligentTextDataNode(int i_Id, string i_InteractorId, IntelligentTextDataType i_Type, List<IntelligentTextDataNode> i_Children)
        {
            Id = i_Id;
            Type = i_Type;
            InteractorId = i_InteractorId;
            Children = i_Children;
            MeshModifier = new List<IIntelligentTextMeshModifier>();
        }

        public virtual bool Merge(IntelligentTextDataNode i_Node)
        {
            return false;
        }

        public virtual void BuildText(StringBuilder i_TextAccumulator, ref IntelligentTextParser i_Parser)
        { }

        public virtual int BuildSubMesh(int i_StartCharIndex, List<IntelligentTextMeshData> i_MeshSets, ref IntelligentTextParser i_Parser)
        {
            int size = MeshModifier.Count;
            for (int i = 0; i < size; ++i)
            {
                MeshModifier[i].ChangeMesh();
            }

            return i_StartCharIndex;
        }
    }

    public class IntelligentTextDataTextNode : IntelligentTextDataNode
    {
        public string Text;

        public IntelligentTextDataTextNode(int i_Id, string i_InteractorId, string i_Text) :
            base(i_Id, i_InteractorId, IntelligentTextDataType.Text, null)
        {
            Text = i_Text;
        }

        public override bool Merge(IntelligentTextDataNode i_Node)
        {
            if (i_Node.Type == Type && i_Node.InteractorId == InteractorId)
            {
                var textNode = (IntelligentTextDataTextNode)i_Node;
                Text += textNode.Text;
                return true;
            }
            return false;
        }

        public override void BuildText(StringBuilder i_TextAccumulator, ref IntelligentTextParser i_Parser)
        {
            i_TextAccumulator.Append(IntelligentTextSettings.Instance.Localize(Text));
        }
        
        public override int BuildSubMesh(int i_StartCharIndex, List<IntelligentTextMeshData> i_MeshSets, ref IntelligentTextParser i_Parser)
        {
            int characterCount = Text.Length;
            var subMeshData = new IntelligentTextSubMeshData {
                Trinagles = new List<int>(characterCount * 6),
                Material = i_Parser.TextSettings.font.material
            };
            for (int i = 0; i < characterCount; ++i)
            {
                int vertIndexStart = (i_StartCharIndex + i) * 4;
                subMeshData.Trinagles.Add(vertIndexStart);
                subMeshData.Trinagles.Add(vertIndexStart + 1);
                subMeshData.Trinagles.Add(vertIndexStart + 2);
                subMeshData.Trinagles.Add(vertIndexStart);
                subMeshData.Trinagles.Add(vertIndexStart + 2);
                subMeshData.Trinagles.Add(vertIndexStart + 3);
            }

            i_MeshSets[0].SubMeshes.Add(subMeshData);

            int size = MeshModifier.Count;
            for (int i = 0; i < size; ++i)
            {
                MeshModifier[i].ChangeMesh();
            }
            return i_StartCharIndex + characterCount;
        }
    }

    public class IntelligentTextDataImageNode : IntelligentTextDataNode
    {
        private int m_PlaceholderLength;

        public IntelligentTextImage ImageData;
        public IntelligentTextTransform Transform;

        public IntelligentTextDataImageNode(int i_Id, string i_InteractorId, IntelligentTextImage i_Image, IntelligentTextTransform i_Transform) :
            base(i_Id, i_InteractorId, IntelligentTextDataType.Image, null)
        {
            ImageData = i_Image;
            Transform = i_Transform;
            //MeshModifier.Add(this);
        }

        public override void BuildText(StringBuilder i_TextAccumulator, ref IntelligentTextParser i_Parser)
        {
            Vector2 imageSize = ImageData.Sprite.rect.size;
            imageSize *= i_Parser.TextSettings.fontSize / imageSize.y * Transform.scale;
            float estimatedPlaceholderWidth = i_Parser.TextSettings.fontSize * i_Parser.SpacePlaceholderSizePerUnit.x;
            m_PlaceholderLength = (int)(imageSize.x / estimatedPlaceholderWidth + 0.5f);
            if (m_PlaceholderLength <= 0)
            {
                m_PlaceholderLength = 1;
            }
            
            for (int i = 0; i < m_PlaceholderLength; ++i)
            {
                i_TextAccumulator.Append(IntelligentTextParser.SPACE_PLACEHOLDER_STR);
            }
        }


        public override int BuildSubMesh(int i_StartCharIndex, List<IntelligentTextMeshData> i_MeshSets, ref IntelligentTextParser i_Parser)
        {
            var meshData = i_MeshSets[0];
            meshData.RemoveChars(i_StartCharIndex + 1, m_PlaceholderLength - 1);

            int startVertIndex = i_StartCharIndex * 4;
            var subMeshData = new IntelligentTextSubMeshData
            {
                Trinagles = new List<int>() {
                    startVertIndex,
                    startVertIndex + 1,
                    startVertIndex + 2,
                    startVertIndex,
                    startVertIndex + 3,
                    startVertIndex + 1
                },
                Material = ImageData.Material
            };
            
            //find line
            int lastLineIndex = meshData.Lines.Count - 1;
            var matchedLine = meshData.Lines[lastLineIndex];
            for (int i = 0; i < lastLineIndex; ++i)
            {
                if (i_StartCharIndex <= meshData.Lines[i + 1].StartCharIndex)
                {
                    matchedLine = meshData.Lines[i];
                    break;
                }
            }

            float actualFontSize = i_Parser.SpacePlaceholderSize.y * i_Parser.SpacePlaceholderSizePerUnit.y;
            Vector2 imageSize = ImageData.Sprite.rect.size;
            imageSize *= actualFontSize / imageSize.y * Transform.scale;
            Vector3 startVert = new Vector3();
            switch(Transform.pivot)
            {
                case IntelligentTextTransformAnchor.Top:
                    startVert = meshData.Verts[startVertIndex + 3];
                    startVert.y -= imageSize.y - matchedLine.Height;
                    break;
                case IntelligentTextTransformAnchor.Center:
                    startVert = meshData.Verts[startVertIndex + 3];
                    startVert.y -= (imageSize.y - matchedLine.Height) * 0.5f;
                    break;
                case IntelligentTextTransformAnchor.Bottom:
                    startVert = meshData.Verts[startVertIndex + 3];
                    break;
            }
            
            meshData.Verts[startVertIndex + 3] = startVert;
            startVert.x += imageSize.x;
            meshData.Verts[startVertIndex + 1] = startVert;
            startVert.y += imageSize.y;
            meshData.Verts[startVertIndex + 2] = startVert;
            startVert.x -= imageSize.x;
            meshData.Verts[startVertIndex] = startVert;
            

            var spriteUVs = ImageData.Sprite.uv;
            int endVertexIndex = startVertIndex + 4;
            for (int i = startVertIndex; i < endVertexIndex; ++i)
            {
                meshData.Colors[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                meshData.Uvs[i] = spriteUVs[i - startVertIndex];
            }

            /*
            meshData.Colors[startVertIndex + 0] = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
            meshData.Colors[startVertIndex + 1] = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
            meshData.Colors[startVertIndex + 2] = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
            meshData.Colors[startVertIndex + 3] = new Color32(0, 0, 0, byte.MaxValue);
            */

            meshData.SubMeshes.Add(subMeshData);

            int size = MeshModifier.Count;
            for (int i = 0; i < size; ++i)
            {
                MeshModifier[i].ChangeMesh();
            }
            return i_StartCharIndex + 1;
        }
    }

    public class IntelligentTextDataGroupNode : IntelligentTextDataNode
    {
        public IntelligentTextDataGroupNode(int i_Id, string i_InteractorId) :
            base(i_Id, i_InteractorId, IntelligentTextDataType.Group, new List<IntelligentTextDataNode>())
        { }

        public override bool Merge(IntelligentTextDataNode i_Node)
        {
            if (i_Node.Type == Type && i_Node.InteractorId == InteractorId)
            {
                var groupNode = (IntelligentTextDataGroupNode)i_Node;
                Children.AddRange(groupNode.Children);
                return true;
            }
            return false;
        }
    }
}
