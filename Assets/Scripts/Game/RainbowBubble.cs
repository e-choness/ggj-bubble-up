using UnityEngine;

namespace Game
{
    public class RainbowBubble : Bubble
    {
        public override void OnCollidedWithBubble(Bubble other)
        {
            if (firstCollision) 
            {
                Color color = other.GetColor();
                SetColor(color);
                foreach(Bubble neighbor in neighbors)
                {
                    if (neighbor.GetType() == typeof(RainbowBubble)) neighbor.SetColor(color);
                }
            }
            base.OnCollidedWithBubble(other);
        }
    }
}