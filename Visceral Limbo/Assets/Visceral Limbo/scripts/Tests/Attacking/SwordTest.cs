using Visceral_Limbo.scripts.Base_Classes;

namespace Visceral_Limbo.scripts.Tests.Attacking
{
    public class SwordTest : Visceral_WeaponBase
    {
        public override void Attacking()
        {
            EnableWeaponCollision();
            StartAttack?.Invoke();
        }

        public override void StopAttacking()
        {
            DisableWeaponCollision();
            EndAttack?.Invoke();
        }

        protected override void DisableWeaponCollision()
        {

            foreach (var Item in _WeaponColliders)
            {
                Item.Activate(false);
                Item.UpdateValues(Damage, KnockBack);
            }
        }

        protected override void EnableWeaponCollision()
        {
            foreach (var Item in _WeaponColliders)
            {
                Item.Activate(true);
                Item.UpdateValues(Damage, KnockBack);
            }

        }
    }
}
