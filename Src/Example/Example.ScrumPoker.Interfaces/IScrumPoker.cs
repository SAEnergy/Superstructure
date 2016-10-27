using Core.Interfaces.Base;
using Core.Interfaces.ServiceContracts;
using Core.Models;
using System;
using System.ServiceModel;

namespace Example.ScrumPoker.Interfaces
{
    [ServiceContract]
    public interface IScrumPokerCallback
    {
        [OperationContract(IsOneWay = true)]
        void StoryUpdated(ScrumPokerStory game);

        [OperationContract(IsOneWay = true)]
        void PlayerUpdated(ScrumPokerPlayer player);

        [OperationContract(IsOneWay = true)]
        void PlayerRemoved(ScrumPokerPlayer player);
    }

    [ServiceContract(CallbackContract = typeof(IScrumPokerCallback))]
    public interface IScrumPoker : IUserAuthentication
    {
        [OperationContract]
        ScrumPokerCard[] GetAvailableCards();

        [OperationContract]
        ScrumPokerStory GetStoryInfo();

        [OperationContract]
        ScrumPokerPlayer[] GetPlayers();

        [OperationContract]
        void Flip();

        [OperationContract]
        void ResetGame();

        [OperationContract]
        void UpdatePlayer(ScrumPokerPlayer player);

        [OperationContract]
        void UpdateStory(ScrumPokerStory story);
    }

    public class ScrumPokerPlayer : ICloneable<ScrumPokerPlayer>
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public Tuple<byte, byte, byte> CardColor { get; set; }
        public bool HasVoted { get; set; }
        public ScrumPokerCard SelectedCard { get; set; }

        public ScrumPokerPlayer Clone() { return (ScrumPokerPlayer)this.MemberwiseClone(); }
    }

    public class ScrumPokerStory : ICloneable<ScrumPokerStory>
    {
        public string StoryName { get; set; }

        public ScrumPokerStory Clone() { return (ScrumPokerStory)this.MemberwiseClone(); }
    }

    public class ScrumPokerCard
    {
        public string DisplayName { get; set; }
        public double NumericValue { get; set; }

        public static bool operator ==(ScrumPokerCard a, ScrumPokerCard b)
        {
            if (((object)a) == null && ((object)b) == null) { return true; }
            if (((object)a) != null && ((object)b) == null) { return false; }
            if (((object)a) == null && ((object)b) != null) { return false; }
            return a.DisplayName == b.DisplayName && a.NumericValue == b.NumericValue;
        }
        public static bool operator !=(ScrumPokerCard a, ScrumPokerCard b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            ScrumPokerCard other = obj as ScrumPokerCard;
            if (other == null) { return false; }
            if (other.NumericValue != this.NumericValue) { return false; }
            if (other.DisplayName != this.DisplayName) { return false; }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
