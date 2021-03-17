public enum UnitState
{
    Normal = 1,
    Attack = 5,
    Retreat = 10
    //TimeFreezing- отсановка времени, скорее всего будет StartFreezing и Invoke("StopFreezing",cooldown) методами в baseAI, а не отдельный стейтом.

}