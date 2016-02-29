using Client.Base;
using Core.Comm;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerViewModel : ViewModelBase<IScrumPoker>, IScrumPokerCallback
    {
        public ObservableCollection<ScrumPokerCard> AvailableCards { get; private set; }
        public ObservableCollection<ScrumPokerPlayerModel> Players { get; private set; }
        public ScrumPokerStoryModel Story { get; protected set; }
        public ScrumPokerPlayerModel Me { get; protected set; }

        public SimpleCommand FlipCardsCommand { get; private set; }
        public SimpleCommand RescindVoteCommand { get; private set; }
        public SimpleCommand ResetGameCommand { get; private set; }

        public ScrumPokerViewModel(ViewBase parent) : base(parent)
        {
            FlipCardsCommand = new SimpleCommand(ExecuteFlipCardsCommand);
            RescindVoteCommand = new SimpleCommand(ExecuteRescindVoteCommand);
            ResetGameCommand = new SimpleCommand(ExecuteResetGameCommand);

            AvailableCards = new ObservableCollection<ScrumPokerCard>();
            Players = new ObservableCollection<ScrumPokerPlayerModel>();

            Story = new ScrumPokerStoryModel();
            Story.Modified += Story_Modified;

            Me = new ScrumPokerPlayerModel();
            Me.Modified += Me_Modified;
        }

        private void Me_Modified()
        {
            Execute(() => Channel.UpdatePlayer(Me.ModifiedObject));
        }

        private void Story_Modified()
        {
            Execute(() => Channel.UpdateStory(Story.ModifiedObject));
        }

        protected override void OnConnect(ISubscription source)
        {
            base.OnConnect(source);

            Channel.UpdatePlayer(Me.ModifiedObject);

            foreach (ScrumPokerCard card in Channel.GetAvailableCards())
            {
                BeginInvoke(() => AvailableCards.Add(card));
            }

            ScrumPokerStory story = Channel.GetStoryInfo();
            BeginInvoke(() => Story.UpdateFrom(story));

            foreach (ScrumPokerPlayer player in Channel.GetPlayers())
            {
                BeginInvoke(() => PlayerAdded(player));
            }
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            this.BeginInvoke(() =>
            {
                AvailableCards.Clear();
                Players.Clear();
            });
            base.OnDisconnect(source, error);
        }

        public void StoryUpdated(ScrumPokerStory story)
        {
            Story.UpdateFrom(story);
        }

        public void PlayerAdded(ScrumPokerPlayer player)
        {
            BeginInvoke(() => Players.Add(new ScrumPokerPlayerModel(player)));
        }

        public void PlayerRemoved(ScrumPokerPlayer player)
        {
        }

        public void PlayerUpdated(ScrumPokerPlayer player)
        {
            this.BeginInvoke(() =>
            {
                if (Me.OriginalObject.ID == player.ID) { Me.UpdateFrom(player); }

                foreach (ScrumPokerPlayerModel p in Players)
                {
                    if (p.OriginalObject.ID == player.ID)
                    {
                        p.UpdateFrom(player);
                    }
                }
            });
        }

        private void ExecuteResetGameCommand()
        {
            Execute(() => Channel.ResetGame());
        }

        private void ExecuteRescindVoteCommand()
        {
            Me.ResetVote();
        }

        private void ExecuteFlipCardsCommand()
        {
            Execute(() => Channel.Flip());
        }
    }
}
