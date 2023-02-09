namespace JPCC.Models
{
    // Keeps track of the state of a running vote
    public class VotingTracker
    {
        public VotingTracker() {}

        public bool IsVoteRunning = false;

        public bool CanStartNewVote = true;

        public string VoteType = "";

        public List<string> PlayersWhoVoted = new List<string>();

        public int VotedYesCount = 0;

        public int VotedNoCount = 0;
    }
}
