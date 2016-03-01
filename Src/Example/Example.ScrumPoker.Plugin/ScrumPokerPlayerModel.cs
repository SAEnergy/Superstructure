﻿using Client.Base;
using Core.Comm.BaseClasses;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerPlayerModel : ModelBase<ScrumPokerPlayer>
    {
        public string Name
        {
            get { return OriginalObject.Name; }
            set
            {
                ModifiedObject.Name = value;
                SetDirty();
            }
        }

        public Brush BackgroundBrush
        {
            get { return new SolidColorBrush(Color.FromRgb(OriginalObject.CardColor.Item1, OriginalObject.CardColor.Item2, OriginalObject.CardColor.Item3)); }
        }

        public void SetVote(ScrumPokerCard card)
        {
            ModifiedObject.SelectedCard = card;
            ModifiedObject.HasVoted = true;
            SetDirty();
        }

        public ScrumPokerCard SelectedCard
        {
            get { return OriginalObject.SelectedCard; }
            set { SetVote(value); }
        }

        public ScrumPokerPlayerModel()
        {
            ScrumPokerPlayer player = new ScrumPokerPlayer();
            player.Name = "Player Name";
            player.ID = Guid.NewGuid();
            // leave blank to let server auto assign
            // player.CardColor = new Tuple<byte, byte, byte>((byte)(_randy.Next(127) + 128), (byte)(_randy.Next(127) + 128), (byte)(_randy.Next(127) + 128));
            UpdateFrom(player);
        }

        public ScrumPokerPlayerModel(ScrumPokerPlayer player)
        {
            UpdateFrom(player);
        }
    }
}