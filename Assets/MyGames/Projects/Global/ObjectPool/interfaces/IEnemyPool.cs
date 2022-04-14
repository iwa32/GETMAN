using System.Collections;
using System.Collections.Generic;
using EP = EnemyPresenter;

//ジェネリックでオブジェクトプールの処理を共通化したかったが断念
namespace ObjectPool {
    /// <summary>
    /// エネミー用オブジェクトプール
    /// </summary>
    public interface IEnemyPool
    {
        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceObj"></param>
        /// <param name="maxObjectCount"></param>
        void CreatePool(EP.EnemyPresenter[] enemies, int maxObjectCount);

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        EP.EnemyPresenter GetPool();

        /// <summary>
        /// エネミーの作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EP.EnemyPresenter Create(EP.EnemyPresenter prefab);
    }
}
