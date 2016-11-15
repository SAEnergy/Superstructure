using Client.Base;
using Core.Comm;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerViewModel : ViewModelBase<IScrumPoker>, IScrumPokerCallback
    {
        public ObservableCollection<ScrumPokerCard> AvailableCards { get; private set; }
        public ObservableCollection<ScrumPokerPlayerModel> Players { get; private set; }
        public ScrumPokerStoryModel Story { get; protected set; }
        public ScrumPokerPlayerModel Me { get; protected set; }

        public SimpleCommand FlipCardsCommand { get; private set; }
        public SimpleCommand ResetGameCommand { get; private set; }

        private ScrumPokerSettings _settings;
        private PropertyChangeNotifier _notifyName;
        private PropertyChangeNotifier _notifyRed;
        private PropertyChangeNotifier _notifyGreen;
        private PropertyChangeNotifier _notifyBlue;

        public ScrumPokerViewModel(ViewBase parent) : base(parent)
        {
            _settings = ClientSettingsEngine.Instance.GetInstance<ScrumPokerSettings>();
            _notifyName = new PropertyChangeNotifier(_settings, ScrumPokerSettings.PlayerNameProperty);
            _notifyRed = new PropertyChangeNotifier(_settings, ScrumPokerSettings.ColorRedProperty);
            _notifyGreen = new PropertyChangeNotifier(_settings, ScrumPokerSettings.ColorGreenProperty);
            _notifyBlue = new PropertyChangeNotifier(_settings, ScrumPokerSettings.ColorBlueProperty);
            _notifyName.ValueChanged += SettingsValueChanged;
            _notifyRed.ValueChanged += SettingsValueChanged;
            _notifyGreen.ValueChanged += SettingsValueChanged;
            _notifyBlue.ValueChanged += SettingsValueChanged;

            FlipCardsCommand = new SimpleCommand(ExecuteFlipCardsCommand);
            ResetGameCommand = new SimpleCommand(ExecuteResetGameCommand);

            AvailableCards = new ObservableCollection<ScrumPokerCard>();
            Players = new ObservableCollection<ScrumPokerPlayerModel>();

            Story = new ScrumPokerStoryModel();
            Story.Modified += Story_Modified;

            Me = new ScrumPokerPlayerModel();
            Me.Modified += Me_Modified;
        }

        private void SettingsValueChanged(object sender, EventArgs e)
        {
            Me.ModifiedObject.CardColor = new Tuple<byte, byte, byte>(_settings.ColorRed, _settings.ColorGreen, _settings.ColorBlue);
            Me.Name = _settings.PlayerName;
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
                BeginInvoke(() => PlayerUpdated(player));
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

        public void PlayerRemoved(ScrumPokerPlayer player)
        {
            if (player==null) { return; }
            this.BeginInvoke(() =>
            {
            foreach (ScrumPokerPlayerModel p in Players.ToArray())
                {
                    if (p.OriginalObject.ID == player.ID)
                    {
                        Players.Remove(p);
                    }
                }
            });
        }

        public void PlayerUpdated(ScrumPokerPlayer player)
        {
            if (player == null) { return; }
            this.BeginInvoke(() =>
            {
                if (Me.OriginalObject.ID == player.ID) { Me.UpdateFrom(player); }

                foreach (ScrumPokerPlayerModel p in Players)
                {
                    if (p.OriginalObject.ID == player.ID)
                    {
                        p.UpdateFrom(player);
                        return;
                    }
                }
                // not found
                Players.Add(new ScrumPokerPlayerModel(player));
            });
        }

        private void ExecuteResetGameCommand()
        {
            Execute(() => Channel.ResetGame());
        }

        private void ExecuteFlipCardsCommand()
        {
            Execute(() => Channel.Flip());
        }

        public override void Dispose()
        {
            _notifyName.Dispose();
            _notifyRed.Dispose();
            _notifyGreen.Dispose();
            _notifyBlue.Dispose();
            base.Dispose();
        }
    }
}
