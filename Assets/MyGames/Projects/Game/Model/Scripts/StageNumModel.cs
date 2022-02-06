using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GameModel
{
    public class StageNumModel : IStageNumModel
    {
        IntReactiveProperty _stageNum = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> StageNum => _stageNum;

        public void IncreaseStageNum()
        {
            _stageNum.Value++;
        }
        
        public void SetStageNum(int stageNum)
        {
            _stageNum.Value = stageNum;
        }
    }
}
