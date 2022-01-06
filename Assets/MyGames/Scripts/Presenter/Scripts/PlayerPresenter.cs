using System.Collections;
using System.Collections.Generic;
using PlayerModel;
using UnityEngine;
using UniRx;
using Zenject;

namespace PlayerPresenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStateModel _stateModel;

        [Inject]
        public void Construct(
            IWeaponModel weapon,
            IHpModel hp,
            IScoreModel score,
            IPointModel point,
            IStateModel state
        )
        {
            _weaponModel = weapon;
            _hpModel = hp;
            _scoreModel = score;
            _pointModel = point;
            _stateModel = state;
        }
    }
}