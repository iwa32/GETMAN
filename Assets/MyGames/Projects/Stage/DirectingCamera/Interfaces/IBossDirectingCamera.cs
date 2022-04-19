using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StageDirectingCamera
{
    public interface IBossDirectingCamera
    {
        /// <summary>
        /// ボスを演出します
        /// </summary>
        /// <returns></returns>
        UniTask Direct(Transform targetBoss);
    }
}
