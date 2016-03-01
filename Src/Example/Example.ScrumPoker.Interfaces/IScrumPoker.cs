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
        void PlayerAdded(ScrumPokerPlayer player);

        [OperationContract(IsOneWay = true)]
        void PlayerRemoved(ScrumPokerPlayer player);

        [OperationContract(IsOneWay = true)]
        void PlayerUpdated(ScrumPokerPlayer player);
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
        public bool HasVoted { get; set; }
        public ScrumPokerCard SelectedCard { get; set; }

        public ScrumPokerPlayer() { Name = "Player Name"; ID = Guid.NewGuid(); }
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
    }

}
