using UnityEngine;
namespace BakerySO
{
    [CreateAssetMenu(fileName = "ProducerConfig", menuName = "Bakery/ProducerConfig", order = -999)]
    public class ProducerConfig : ScriptableObject
    {
        public RecipeData DoughRecipe;

        public float BaseShortagePenaltyModifier = 1f;
        public float BaseSurplusPenaltyModifier = 1f;
        public float BaseRedundantPenaltyModifier = 1f;
        public float BaseBakingPenaltyModifier = 1f;

        public float BasePerfectBonusModifier = 1f;
    }
}
