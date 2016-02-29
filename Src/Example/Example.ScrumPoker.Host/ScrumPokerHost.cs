using System;
using System.Linq;
using Core.Comm.BaseClasses;
using Example.ScrumPoker.Interfaces;
using System.ServiceModel;
using System.Collections.Generic;

namespace Example.ScrumPoker.Host
{
    public class ScrumPokerHost : ServiceHostBase<IScrumPoker, IScrumPokerCallback>, IScrumPoker
    {
        private static object _lock = new object();
        private static ScrumPokerStory _story = new ScrumPokerStory() { StoryName = "Story Name" };

        private ScrumPokerPlayer _player;
        public ScrumPokerPlayer Player { get; private set; }

        public ScrumPokerCard[] GetAvailableCards()
        {
            return new ScrumPokerCard[]
            {
                new ScrumPokerCard() { DisplayName="?", NumericValue=double.NaN },
                new ScrumPokerCard() { DisplayName="0", NumericValue=0 },
                new ScrumPokerCard() { DisplayName="1/2", NumericValue=0.5 },
                new ScrumPokerCard() { DisplayName="1", NumericValue=1 },
                new ScrumPokerCard() { DisplayName="2", NumericValue=2 },
                new ScrumPokerCard() { DisplayName="3", NumericValue=3 },
                new ScrumPokerCard() { DisplayName="5", NumericValue=5 },
                new ScrumPokerCard() { DisplayName="8", NumericValue=8 },
                new ScrumPokerCard() { DisplayName="13", NumericValue=13 },
                new ScrumPokerCard() { DisplayName="20", NumericValue=20 },
                new ScrumPokerCard() { DisplayName="40", NumericValue=40 },
                new ScrumPokerCard() { DisplayName="100", NumericValue=100 },
                new ScrumPokerCard() { DisplayName="∞", NumericValue=double.PositiveInfinity },
                new ScrumPokerCard() { DisplayName="☕", NumericValue=double.NaN },
            };
        }

        public ScrumPokerHost()
        {
        }

        public ScrumPokerPlayer[] GetPlayers()
        {
            return GetInstances<ScrumPokerHost>().Select(s => s.Player).ToArray();
        }

        private bool HaveAllPlayersVoted()
        {
            List<ScrumPokerPlayer> players = GetInstances<ScrumPokerHost>().Where(s => s.Player != null).Select(s => s.Player).ToList();
            if (players.Count == 0) { return false; }
            return players.All(p => p.HasVoted == true);
        }

        public ScrumPokerStory GetStoryInfo()
        {
            return _story;
        }

        public void ResetGame()
        {
            lock (_instances)
            {
                foreach (ScrumPokerHost iter in GetInstances<ScrumPokerHost>())
                {
                    ScrumPokerHost host = iter;
                    if (host._player == null) { continue; }
                    host._player.HasVoted = false;
                    host._player.SelectedCard = null;
                    host.Player = host._player.Clone();
                    host.Broadcast((IScrumPokerCallback c) => c.PlayerUpdated(host.Player));
                }
            }
        }

        public void Flip()
        {
            lock (_instances)
            {
                foreach (ScrumPokerHost iter in GetInstances<ScrumPokerHost>())
                {
                    ScrumPokerHost host = iter;
                    if (host._player == null) { continue; }
                    if (host._player.HasVoted == false)
                    {
                        host._player.HasVoted = true;
                        host._player.SelectedCard = GetAvailableCards().First(c => c.DisplayName == "☕");
                    }
                    host.Player = host._player.Clone();
                    Broadcast((IScrumPokerCallback c) => c.PlayerUpdated(host.Player));
                }
            }
        }

        public void UpdatePlayer(ScrumPokerPlayer player)
        {
            bool newPlayer = _player == null;

            if (!newPlayer && HaveAllPlayersVoted()) { throw new InvalidOperationException("Cannot update vote after cards have been flipped!"); }

            _player = player;

            ScrumPokerPlayer pubplayer = player.Clone();
            pubplayer.SelectedCard = null;
            Player = pubplayer;

            if (newPlayer)
            {
                BroadcastOther((IScrumPokerCallback c) => c.PlayerAdded(Player));
            }
            else
            {
                Broadcast((IScrumPokerCallback c) => c.PlayerUpdated(Player));
            }

            if (HaveAllPlayersVoted()) { Flip(); }
        }

        public void UpdateStory(ScrumPokerStory story)
        {
            _story = story;
            Broadcast((IScrumPokerCallback c) => c.StoryUpdated(_story));
        }

        public override void Dispose()
        {
            base.Dispose();
            BroadcastOther((IScrumPokerCallback c) => c.PlayerRemoved(Player));
        }
    }
}
