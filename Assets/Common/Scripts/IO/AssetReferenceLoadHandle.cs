using AssetBundles;
using System;
using UnityEngine;

namespace Common.IO
{

    public abstract class AssetReferenceLoadHandle
    {
        public abstract bool IsDone();
        public abstract T GetAsset<T>() where T : UnityEngine.Object;
        public Action<UnityEngine.Object> LoadCallback;
    }

    public class AssetReferenceNoLoadHandle : AssetReferenceLoadHandle
    {
        private UnityEngine.Object m_Asset;

        public AssetReferenceNoLoadHandle(UnityEngine.Object i_Asset)
        {
            m_Asset = i_Asset;
            if(LoadCallback != null)
            {
                LoadCallback.Invoke(m_Asset);
            }
        }

        public override T GetAsset<T>()
        {
            return m_Asset as T;
        }

        public override bool IsDone()
        {
            return true;
        }
    }

    public class AssetReferenceAssetBundleLoadHandle : AssetReferenceLoadHandle, UpdateRunnerTask
    {
        private AssetBundleLoadAssetOperation m_AssetBundleOperation;
        private UnityEngine.Object m_Asset;

        public AssetReferenceAssetBundleLoadHandle(AssetBundleLoadAssetOperation i_PendingOperation)
        {
            m_AssetBundleOperation = i_PendingOperation;
            //set to done if nothing to wait for
            UpdateAndFinish();
            UpdateRunner.Instance.AddTask(this);
        }

        public override T GetAsset<T>()
        {
            return m_Asset as T;
        }
        public override bool IsDone()
        {
            return m_AssetBundleOperation == null;
        }

        public bool UpdateAndFinish()
        {
            if(m_AssetBundleOperation == null)
            {
                return true;
            }
            if (m_AssetBundleOperation.IsDone())
            {
                m_Asset = m_AssetBundleOperation.GetAsset<UnityEngine.Object>();
                m_AssetBundleOperation = null;
                if (LoadCallback != null)
                {
                    LoadCallback.Invoke(m_Asset);
                }
                return true;
            }
            return false;
        }
    }

    public class AssetReferenceResourceLoadHandle : AssetReferenceLoadHandle, UpdateRunnerTask
    {
        private ResourceRequest m_ResourceOperation;
        private UnityEngine.Object m_Asset;

        public AssetReferenceResourceLoadHandle(ResourceRequest i_PendingOperation)
        {
            m_ResourceOperation = i_PendingOperation;
            //set to done if nothing to wait for
            UpdateAndFinish();

            UpdateRunner.Instance.AddTask(this);
        }

        public override T GetAsset<T>()
        {
            return m_Asset as T;
        }

        public override bool IsDone()
        {
            return m_ResourceOperation == null;
        }


        public bool UpdateAndFinish()
        {
            if(m_ResourceOperation == null)
            {
                return true;
            }
            if (m_ResourceOperation.isDone)
            {
                m_Asset = m_ResourceOperation.asset;
                m_ResourceOperation = null;
                if (LoadCallback != null)
                {
                    LoadCallback.Invoke(m_Asset);
                }
                return true;
            }
            return false;
        }
    }

    public class AssetReferenceDelayedLoadHandle : AssetReferenceLoadHandle, UpdateRunnerTask
    {
        private AssetReferenceLoadHandle m_WaitingHandle;
        private AssetDataReference m_PendingData;
        private Type m_RequestedAssetType;
        private string m_RequestedAssetSubName;
        private bool m_Done = false;

        public AssetReferenceDelayedLoadHandle(AssetReferenceLoadHandle i_WaitingHandle, AssetDataReference i_PendingData, Type i_AssetType, string i_SubName)
        {
            m_WaitingHandle = i_WaitingHandle;
            m_PendingData = i_PendingData;
            m_RequestedAssetType = i_AssetType;
            m_RequestedAssetSubName = i_SubName;
            //set to done if nothing to wait for
            UpdateAndFinish();

            UpdateRunner.Instance.AddTask(this);
        }

        public override T GetAsset<T>()
        {
            if(IsDone() && m_WaitingHandle != null)
            {
                return m_WaitingHandle.GetAsset<T>();
            }
            return null;
        }

        public override bool IsDone()
        {
            return m_Done;
        }

        public bool UpdateAndFinish()
        {
            if(m_PendingData != null)
            {
                //waiting for dependent load to finish
                if (m_WaitingHandle == null || m_WaitingHandle.IsDone())
                {
                    //creating an asset load handle of interest
                    m_WaitingHandle = m_PendingData.LoadAsync(m_RequestedAssetType, m_RequestedAssetSubName);
                    m_PendingData = null;
                }
                return UpdateAndFinish();
            }
            if(m_WaitingHandle == null)
            {
                m_Done = true;
                return true;
            }
            if(m_WaitingHandle.IsDone())
            {
                if(LoadCallback != null)
                {
                    LoadCallback.Invoke(m_WaitingHandle.GetAsset<UnityEngine.Object>());
                }
                m_Done = true;
                return true;
            }
            return false;
        }
    }
}
