using System;

namespace CharacterState
{
    public interface ICharacterState
    {
        /// <summary>
        /// その状態の時に実行したいアクションを定義します
        /// </summary>
        Action DelAction { get; set; }

        /// <summary>
        /// 状態の種類を定義します
        /// </summary>
        StateType State { get; set; }
    }

    public  interface ICharacterAttackState: ICharacterState { }
    public interface ICharacterDeadState : ICharacterState { }
    public interface ICharacterDownState : ICharacterState { }
    public interface ICharacterRunState : ICharacterState { }
    public interface ICharacterJoyState : ICharacterState { }
    public interface ICharacterWaitState : ICharacterState { }
    public interface ICharacterTrackState : ICharacterState { }
}