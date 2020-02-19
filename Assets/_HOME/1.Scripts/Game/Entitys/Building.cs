
using HOME.Data;

namespace HOME.Game {

    public class Building : Entity {
        private bool _playerShip = false;
        public static Building PlayerShip;

        public override void Awake() {
            base.Awake();
            if (gameObject.tag == "Ship") {
                PlayerShip = this;
                _playerShip = true;

                CurrHealth = DataManager.Instance.PlayerShipHealth; // set building health to ship health
                MaxHealth = DataManager.Instance.PlayerShipMaxHealth;
            }
        }

        public override void Update() {
            if (_playerShip) {
                DataManager.Instance.PlayerShipHealth = CurrHealth; // update datamanger with building health
                DataManager.Instance.PlayerShipMaxHealth = MaxHealth;

            }
            base.Update();
        }

        public override void InGameMenuUpdate() {
            base.InGameMenuUpdate();
            float healthBar = CurrHealth / MaxHealth;
            _inGameMenu.SelectionFill(
                GetName,
                hasHealth,
                CurrHealth + "/" + MaxHealth,
                GetDescription,
                GetIcon,
                player.isAi,
                healthBar,
                hasResources,
                "",
                -0f,
                 _currInventoryAmount.ToString() + "/" + MaxInventoryAmount.ToString());
        }
    }
}

