
namespace HOME.Game {

    public class DoNothingAI : AIBehavior {

        public float ReturnWeight = 0.5f;

        public override float GetWeight() {
            return ReturnWeight;
        }

        public override void Execute() {
            //Debug.Log("Doing Nothing");
        }
    }
}
