namespace ClientSideApplication.Enums
{
    public enum States 
    {
        Attacking,
        DefendingHeavyShield,   //Absorbs 75% damage, 25% chance to hit back the same damage
        DefendingNormalShield,  //Absorbs 55% damage, 40% chance to hit back the same damage
        DefendingLightShield,   //Absorbs 45% damage, 50% chance to hit back the same damage
        Resting,
        Checking,
        Interrupted,
        Stunned,
        DoingNothing,
        Winner,
        Dead
    }
}