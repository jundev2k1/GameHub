using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Enum;

public enum NetworkType
{
    [Display(Name = "Tron")]
    Tron = 1,

    [Display(Name = "Ethereum")]
    Ethereum = 2
}