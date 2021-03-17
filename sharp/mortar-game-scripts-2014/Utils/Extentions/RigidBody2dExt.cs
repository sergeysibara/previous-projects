using UnityEngine;

public static class Rigidbody2DExt
{
    public static void SetVelocityX(this Rigidbody2D self, float x)
    {
        self.velocity = new Vector2(x, self.velocity.y);
    }

    public static void SetVelocityY(this Rigidbody2D self, float y)
    {
        self.velocity = new Vector2(self.velocity.x, y);
    }

    public static void AddVelocityX(this Rigidbody2D self, float x)
    {
        self.velocity = new Vector2(self.velocity.x + x, self.velocity.y);
    }

    public static void AddVelocityY(this Rigidbody2D self, float y)
    {
        self.velocity = new Vector2(self.velocity.x, self.velocity.y + y);
    }
}
