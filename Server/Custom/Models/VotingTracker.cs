using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Custom.Models
{
    public class VotingTracker
    {
        public VotingTracker() 
        {
        
        }

        public bool IsVoteRunning = false;

        public string VoteType = "";

        public List<string> PlayersWhoVoted = new List<string>();

        public int VotedYesCount = 0;

        public int VotedNoCount = 0;
    }
}
