namespace StageObject
{
    /// <summary>
    /// ポイントアイテム
    /// </summary>
    public interface IPointItem : IPoint, IScore
    {
        /// <summary>
        /// 削除処理
        /// </summary>
        void Destroy();
    }
}
