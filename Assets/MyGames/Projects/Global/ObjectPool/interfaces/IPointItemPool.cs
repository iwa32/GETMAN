using System.Collections;
using System.Collections.Generic;
using StageObject;

namespace ObjectPool
{
    /// <summary>
    /// ポイントアイテム用オブジェクトプール
    /// </summary>
    public interface IPointItemPool
    {
        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceObj"></param>
        /// <param name="maxObjectCount"></param>
        void CreatePool(PointItem pointItemPrefab, int maxObjectCount);

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        PointItem GetPool();
    }
}