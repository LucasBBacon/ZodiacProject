using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] Image _healthBarFill;
    [SerializeField] Image _manaBarFill;

    Player _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        _manaBarFill.fillAmount = Utilities.MappingUtil.Map(
            _player.Stats.Mana.CurrentValue,
            0, _player.Stats.Mana.MaxValue,
            0, 1,
            true
            );
        _healthBarFill.fillAmount = Utilities.MappingUtil.Map(
            _player.Stats.Health.CurrentValue,
            0, _player.Stats.Health.MaxValue,
            0, 1,
            true
            );
    } 
}
