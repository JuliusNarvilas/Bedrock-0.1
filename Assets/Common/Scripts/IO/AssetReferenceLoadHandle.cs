using AssetBundles;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common.IO
{

    public abstract class AssetReferenceLoadHandle : IDisposable
    {
        protected class DisposalRunner : UpdateRunnerTask
        {
            private AssetReferenceLoadHandle m_WaitingHandle;

            public DisposalRunner(AssetReferenceLoadHandle i_Handle)
            {
                m_WaitingHandle = i_Handle;
            }

            public bool UpdateAndFinish()
            {
                if(m_WaitingHandle != null)
                {
                    if(m_WaitingHandle.IsDone())
                    {
                        m_WaitingHandle.FinalizeDispose();
                        m_WaitingHandle = null;
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        protected bool m_Active = true;
        protected readonly AssetReferenceInfo m_Info;
        protected UnityEngine.Object m_Asset;

        public AssetReferenceLoadHandle(AssetReferenceInfo i_Info)
        {
            m_Info = i_Info;
        }

        public abstract bool IsDone();

        protected virtual void FinalizeDispose()
        {
            switch (m_Info.ReferenceType)
            {
                case AssetReference.ReferenceType.AssetBundle:
                    AssetBundleManager.UnloadAssetBundle(m_Info.Src);
                    break;
                case AssetReference.ReferenceType.Resource:
                    Resources.UnloadAsset(m_Asset);
                    break;
            }
        }
        public virtual T GetAsset<T>() where T : UnityEngine.Object
        {
            return m_Asset as T;
        }

        public void Dispose()
        {
            if (m_Active)
            {
                m_Active = false;
                var disposalTask = new DisposalRunner(this);
                if (!disposalTask.UpdateAndFinish())
                {
                    UpdateRunner.Instance.AddTask(disposalTask);
                }
            }
        }

        private static UnityEngine.Object LoadSubAsset(AssetReferenceInfo i_Info)
        {
            switch (i_Info.ReferenceType)
            {
                case AssetReference.ReferenceType.AssetBundle:
                    {
#if UNITY_EDITOR
                        if (AssetBundleManager.SimulateAssetBundleInEditor)
                        {
                            var subAssets = AssetDatabase.LoadAllAssetsAtPath(i_Info.FilePath);
                            return subAssets.Where(o => o.name == i_Info.SubAssetName && i_Info.GetAssetType().IsAssignableFrom(o.GetType())).FirstOrDefault();
                        }
                        else
#endif
                        {
                            string error;
                            var assetBundle = AssetBundleManager.GetLoadedAssetBundle(i_Info.Src, out error);
                            var subAssetArr = assetBundle.m_AssetBundle.LoadAssetWithSubAssets(i_Info.Name, i_Info.GetAssetType());
                            int count = subAssetArr.Length;
                            for (int i = 0; i < count; ++i)
                            {
                                if (subAssetArr[i].name == i_Info.SubAssetName)
                                {
                                    return subAssetArr[i];
                                }
                            }
                        }
                        return null;
                    }
                case AssetReference.ReferenceType.Resource:
                    {
                        var subAssetArr = Resources.LoadAll(i_Info.Src, i_Info.GetAssetType());
                        int count = subAssetArr.Length;
                        for (int i = 0; i < count; ++i)
                        {
                            if (subAssetArr[i].name == i_Info.SubAssetName)
                            {
                                return subAssetArr[i];
                            }
                        }
                        return null;
                    }
            }
            return null;
        }

        public static AssetReferenceLoadHandle Create(AssetReferenceInfo i_Info)
        {
            switch (i_Info.ReferenceType)
            {
                case AssetReference.ReferenceType.AssetBundle:
                    {
                        if (string.IsNullOrEmpty(i_Info.SubAssetName))
                        {
                            var operation = AssetBundleManager.LoadAssetAsync(i_Info.Src, i_Info.Name, i_Info.GetAssetType());
                            return new AssetReferenceAssetBundleLoadHandle(operation, i_Info);
                        }
                        else
                        {
                            string error;
                            var assetBundleHandle = AssetBundleManager.GetLoadedAssetBundle(i_Info.Src, out error);
                            if (assetBundleHandle != null)
                            {
                                assetBundleHandle.m_ReferencedCount++;
                                var subAssetArr = assetBundleHandle.m_AssetBundle.LoadAssetWithSubAssets(i_Info.Name, i_Info.GetAssetType());
                                UnityEngine.Object matchedAsset = null;
                                int count = subAssetArr.Length;
                                for (int i = 0; i < count; ++i)
                                {
                                    if (subAssetArr[i].name == i_Info.SubAssetName)
                                    {
                                        matchedAsset = subAssetArr[i];
                                        break;
                                    }
                                }
                                return new AssetReferenceNoLoadHandle(matchedAsset, i_Info);
                            }
                            else
                            {
                                var operation = AssetBundleManager.LoadAssetAsync(i_Info.Src, i_Info.Name, i_Info.GetAssetType());
                                var initialLoad = new AssetReferenceAssetBundleLoadHandle(operation, i_Info);
                                return new AssetReferenceDelayedLoadHandle(initialLoad, i_Info, () => { return LoadSubAsset(i_Info); });
                            }
                        }
                    }
                case AssetReference.ReferenceType.Resource:
                    {
                        if (string.IsNullOrEmpty(i_Info.SubAssetName))
                        {
                            //async loading doesn't work in editor mode
                            if (Application.isPlaying)
                            {
                                var operation = Resources.LoadAsync(i_Info.Src, i_Info.GetAssetType());
                                return new AssetReferenceResourceLoadHandle(operation, i_Info);
                            }
                            else
                            {
                                return new AssetReferenceNoLoadHandle(Resources.Load(i_Info.Src, i_Info.GetType()), i_Info);
                            }
                        }
                        else
                        {
                            return new AssetReferenceNoLoadHandle(LoadSubAsset(i_Info), i_Info);
                        }
                    }
            }
            return null;
        }

        protected static void DisposeAsset(UnityEngine.Object i_Asset, AssetReferenceInfo i_Info)
        {
            switch(i_Info.ReferenceType)
            {
                case AssetReference.ReferenceType.AssetBundle:
                    AssetBundleManager.UnloadAssetBundle(i_Info.Src);
                    break;
                case AssetReference.ReferenceType.Resource:
                    Resources.UnloadAsset(i_Asset);
                    break;
            }
        }
    }

    public class AssetReferenceNoLoadHandle : AssetReferenceLoadHandle
    {
        public AssetReferenceNoLoadHandle(UnityEngine.Object i_Asset, AssetReferenceInfo i_Info) : base(i_Info)
        {
            m_Asset = i_Asset;
        }

        public override bool IsDone()
        {
            return true;
        }
    }

    public class AssetReferenceAssetBundleLoadHandle : AssetReferenceLoadHandle
    {
        private readonly AssetBundleLoadAssetOperation m_AssetBundleOperation;

        public AssetReferenceAssetBundleLoadHandle(AssetBundleLoadAssetOperation i_PendingOperation, AssetReferenceInfo i_Info) : base(i_Info)
        {
            m_AssetBundleOperation = i_PendingOperation;
        }

        public override T GetAsset<T>()
        {
            if(m_Asset == null)
            {
                m_Asset = m_AssetBundleOperation == null ? null : m_AssetBundleOperation.GetAsset<T>();
            }
            return base.GetAsset<T>();
        }
        public override bool IsDone()
        {
            return m_AssetBundleOperation == null || m_AssetBundleOperation.IsDone();
        }
    }

    public class AssetReferenceResourceLoadHandle : AssetReferenceLoadHandle
    {
        private readonly ResourceRequest m_ResourceOperation;

        public AssetReferenceResourceLoadHandle(ResourceRequest i_PendingOperation, AssetReferenceInfo i_Info) : base(i_Info)
        {
            m_ResourceOperation = i_PendingOperation;
        }

        public override T GetAsset<T>()
        {
            if (m_Asset == null)
            {
                m_Asset = m_ResourceOperation == null ? null : m_ResourceOperation.asset;
            }
            return base.GetAsset<T>();
        }
        public override bool IsDone()
        {
            return m_ResourceOperation == null || m_ResourceOperation.isDone;
        }
    }

    public class AssetReferenceDelayedLoadHandle : AssetReferenceLoadHandle, UpdateRunnerTask
    {
        private readonly AssetReferenceLoadHandle m_WaitingHandle;
        private Func<UnityEngine.Object> m_LoadCallback;

        public AssetReferenceDelayedLoadHandle(AssetReferenceLoadHandle i_WaitingHandle, AssetReferenceInfo i_Info, Func<UnityEngine.Object> i_Callback) : base(i_Info)
        {
            m_WaitingHandle = i_WaitingHandle;
            m_LoadCallback = i_Callback;
            //set to done if nothing to wait for
            if(!UpdateAndFinish())
            {
                UpdateRunner.Instance.AddTask(this);
            }
        }

        public override bool IsDone()
        {
            return m_WaitingHandle == null || (m_WaitingHandle.IsDone() && m_LoadCallback == null);
        }

        public bool UpdateAndFinish()
        {
            if(m_WaitingHandle == null || m_WaitingHandle.IsDone())
            {
                if(m_LoadCallback != null)
                {
                    //can skip execution if handle is being disposed
                    if (m_Active)
                    {
                        m_Asset = m_LoadCallback.Invoke();
                    }
                    m_LoadCallback = null;
                }
                return true;
            }
            return false;
        }
    }
}