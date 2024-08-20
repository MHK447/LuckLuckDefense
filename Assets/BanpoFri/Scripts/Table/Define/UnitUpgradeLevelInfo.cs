using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitUpgradeLevelInfoData
    {
        [SerializeField]
		private int _unit_level;
		public int unit_level
		{
			get { return _unit_level;}
			set { _unit_level = value;}
		}
		[SerializeField]
		private int _need_card;
		public int need_card
		{
			get { return _need_card;}
			set { _need_card = value;}
		}

    }

    [System.Serializable]
    public class UnitUpgradeLevelInfo : Table<UnitUpgradeLevelInfoData, int>
    {
    }
}

