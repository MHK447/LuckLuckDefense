using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitActiveSkillData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private int _skill_value;
		public int skill_value
		{
			get { return _skill_value;}
			set { _skill_value = value;}
		}
		[SerializeField]
		private int _skill_cooltime;
		public int skill_cooltime
		{
			get { return _skill_cooltime;}
			set { _skill_cooltime = value;}
		}
		[SerializeField]
		private int _increase_cost;
		public int increase_cost
		{
			get { return _increase_cost;}
			set { _increase_cost = value;}
		}
		[SerializeField]
		private int _increase_value;
		public int increase_value
		{
			get { return _increase_value;}
			set { _increase_value = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}

    }

    [System.Serializable]
    public class UnitActiveSkill : Table<UnitActiveSkillData, int>
    {
    }
}

