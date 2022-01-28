namespace StageObject
{
    /// <summary>
    /// ポイントアイテム
    /// </summary>
    public interface IWeaponItem : IScore
    {
        int Power { get; }
        int Id { get; }

        /// <summary>
        /// 削除処理
        /// </summary>
        void Destroy();
    }
}
