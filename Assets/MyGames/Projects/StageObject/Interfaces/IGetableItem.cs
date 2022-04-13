namespace StageObject
{
    /// <summary>
    /// 獲得可能アイテム
    /// </summary>
    public interface IGetableItem: IScore
    {
        /// <summary>
        /// 削除処理
        /// </summary>
        void Destroy();
    }
}
