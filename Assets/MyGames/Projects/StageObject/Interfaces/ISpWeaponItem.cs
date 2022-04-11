namespace StageObject
{
    /// <summary>
    /// ウェポンアイテム
    /// </summary>
    public interface ISpWeaponItem : IScore
    {
        /// <summary>
        /// 武器の種類
        /// </summary>
        SpWeaponType SpWeaponType { get; }

        /// <summary>
        /// 削除処理
        /// </summary>
        void Destroy();
    }
}
