using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class SelectComponent : MonoBehaviour
{
    [SerializeField]
    private Image BulletImg;

    [SerializeField]
    private Text NameText;

    [SerializeField]
    private Text AttackDesc;

    [SerializeField]
    private Button SelectBtn;

    [SerializeField]
    private List<Image> ImageList = new List<Image>();

    [SerializeField]
    private int WeaponIdx = 0;

    private System.Action<int> ClickAction = null;

    private void Awake()
    {
        SelectBtn.onClick.AddListener(OnClickSelect);
    }

    public void Set(int weaponidx , System.Action<int> clickaction)
    {
        var td = Tables.Instance.GetTable<PlanetWeaponInfo>().GetData(weaponidx);

        if(td != null)
        {
            WeaponIdx = weaponidx;
            ClickAction = clickaction;
            BulletImg.sprite = Config.Instance.GetInGameAtlas(td.image);
            NameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);

            foreach(var starimg in ImageList)
            {
                starimg.sprite = Config.Instance.GetIconImg("Icon_star");
            }

            var findata = GameRoot.Instance.UserData.CurMode.PlayerWeapon.WeaponList.ToList().Find(x => x.WeaponIdx == weaponidx);

            if(findata != null)
            {
                var weapongachaupgradetd = Tables.Instance.GetTable<WeaponGachaUpgrade>().GetData(new KeyValuePair<int, int>(weaponidx, findata.WeaponLevel + 1));

                var weaponinfotd = Tables.Instance.GetTable<GachaUpgradeType>().GetData(findata.WeaponLevel + 1);

                if(weaponinfotd != null)
                {
                    AttackDesc.text = Tables.Instance.GetTable<Localize>().GetFormat(weaponinfotd.desc , (float)weapongachaupgradetd.upgrade_value / 100f);
                }

                for (int i = 0; i < findata.WeaponLevel + 1; ++i)
                {
                    ImageList[i].sprite = Config.Instance.GetIconImg("Icon_Star2");
                }
            }
            else
            {
                AttackDesc.text = Tables.Instance.GetTable<Localize>().GetString(td.name_desc);
            }
        }
    }

    public void OnClickSelect()
    {
        ClickAction?.Invoke(WeaponIdx);
        
    }

}
