using Common.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetReferenceTest : MonoBehaviour {

    public RawImage m_TextureWithSpritesTarget;
    public AssetReference m_TextureWithSpritesReference;

    public Image m_SpriteTarget;
    public AssetReference m_SpriteReference;

    public RawImage m_TextureTarget;
    public AssetReference m_TextureReference;

    // Use this for initialization
    IEnumerator Start () {

        while(!AssetBundleManagerInit.IsInitialised())
        {
            yield return null;
        }

        var assetHandle = m_TextureWithSpritesReference.GetAsync<Texture2D>();
        while(!assetHandle.IsDone())
        {
            yield return null;
        }
        m_TextureWithSpritesTarget.texture = assetHandle.GetAsset<Texture2D>();

        assetHandle = m_SpriteReference.GetAsync<Sprite>();
        while (!assetHandle.IsDone())
        {
            yield return null;
        }
        m_SpriteTarget.sprite = assetHandle.GetAsset<Sprite>();

        assetHandle = m_TextureReference.GetAsync<Texture2D>();
        while (!assetHandle.IsDone())
        {
            yield return null;
        }
        m_TextureTarget.texture = assetHandle.GetAsset<Texture2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
