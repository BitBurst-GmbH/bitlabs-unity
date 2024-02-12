[System.Serializable]
public class Leaderboard
{
    public string nextResetAt;
    public User ownUser;
    public Reward[] rewards;
    public User[] topUsers;
}
