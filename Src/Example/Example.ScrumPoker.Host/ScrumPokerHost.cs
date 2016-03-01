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
        private Random _randy = new Random();

        private ScrumPokerPlayer _player;
        public ScrumPokerPlayer Player { get; private set; }

        private static List<Tuple<byte, byte, byte>> AvailableColors = new List<Tuple<byte, byte, byte>>()
        {
            new Tuple<byte, byte, byte>(0,0,200),
            new Tuple<byte, byte, byte>(200,0,0),
            new Tuple<byte, byte, byte>(0,200,0),
            new Tuple<byte, byte, byte>(200,200,0),
            new Tuple<byte, byte, byte>(0,200,200),
            new Tuple<byte, byte, byte>(200,0,200),
            new Tuple<byte, byte, byte>(200,100,0),
        };

        public ScrumPokerCard[] GetAvailableCards()
        {
            return new ScrumPokerCard[]
            {
                new ScrumPokerCard() { DisplayName="🚫", NumericValue=double.NaN },
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

            if (_player!=null && HaveAllPlayersVoted() && _player.SelectedCard.DisplayName != player.SelectedCard.DisplayName) { throw new InvalidOperationException("Cannot update vote after cards have been flipped!"); }

            _player = player;

            // some checks
            if (_player.SelectedCard == null || _player.SelectedCard.DisplayName == "🚫") { _player.HasVoted = false; _player.SelectedCard = null; }
            if (_player.CardColor == null || _player.CardColor.Item1 == 0 && _player.CardColor.Item2 == 0 && _player.CardColor.Item3 == 0)
            {
                SetColor(_player);
            }

            ScrumPokerPlayer pubplayer = player.Clone();
            pubplayer.SelectedCard = null;
            Player = pubplayer;

            BroadcastOther((IScrumPokerCallback c) => c.PlayerUpdated(Player));
            Post((IScrumPokerCallback c) => c.PlayerUpdated(_player));

            if (HaveAllPlayersVoted()) { Flip(); }
        }

        private void SetColor(ScrumPokerPlayer player)
        {
            lock (_instances)
            {
                var inUse = GetInstances<ScrumPokerHost>().Where(h => h._player != null).Select(h => h._player.CardColor);
                var tuple = AvailableColors.FirstOrDefault(c => !inUse.Contains(c));
                if (tuple!=null && !(tuple.Item1 == 0 && tuple.Item2 == 0 && tuple.Item3 == 0))
                {
                    player.CardColor = tuple;
                }
                else
                {
                    player.CardColor = new Tuple<byte, byte, byte>((byte)(_randy.Next(255)), (byte)(_randy.Next(255)), (byte)(_randy.Next(255)));
                }
            }
        }

        public void UpdateStory(ScrumPokerStory story)
        {
            _story = story;
            Broadcast((IScrumPokerCallback c) => c.StoryUpdated(_story));
        }

        public override void Dispose()
        {
            if (Player != null) { BroadcastOther((IScrumPokerCallback c) => c.PlayerRemoved(Player)); }
            base.Dispose();
        }
    }
}
